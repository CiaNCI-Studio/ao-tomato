using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using AoTomato.Admin.Abstractions;

namespace AoTomato.Admin.Pages;

public abstract class PageBase : ComponentBase
{
    [Inject]
    protected IAdminService AdminService { get; set; } = default!;

    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    protected IDialogService DialogService { get; set; } = default!;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = default!;

    protected bool IsInitialized { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await CheckAuthAsync();
        if (!IsAuthenticated) return;

        try
        {
            await LoadDataAsync();
        }
        catch (UnauthorizedAccessException)
        {
            await HandleUnauthorizedAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsInitialized = true;
        }
    }

    protected virtual async Task CheckAuthAsync()
    {
        IsAuthenticated = await AdminService.CheckLoginAsync();
        if (!IsAuthenticated)
        {
            NavigationManager.NavigateTo("/login");
        }
    }

    protected bool IsAuthenticated { get; set; }

    protected virtual Task LoadDataAsync() => Task.CompletedTask;

    protected async Task HandleUnauthorizedAsync()
    {
        await AdminService.LogoutAsync();
        NavigationManager.NavigateTo("/login");
    }

    protected void ShowError(string message)
    {
        Snackbar.Add(message, Severity.Error, config => config.VisibleStateDuration = 3000);
    }

    protected void ShowSuccess(string message)
    {
        Snackbar.Add(message, Severity.Success, config => config.VisibleStateDuration = 3000);
    }

    protected async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (UnauthorizedAccessException)
        {
            await HandleUnauthorizedAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }
}
