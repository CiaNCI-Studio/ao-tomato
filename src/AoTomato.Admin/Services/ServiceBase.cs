using System.IO.Pipelines;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Common.Dtos;
using AoTomato.Domain.Common.Enums;
using AoTomato.Domain.Exceptions;

namespace Cordel.Client.Services;

public abstract class ServiceBase<T> : IServiceBase<T>
{

    protected HttpClient httpClient;
    
    protected readonly string baseUrl;

    protected ServiceBase(HttpClient httpClient, string baseUrl)
    {
        this.httpClient = httpClient;
        this.baseUrl = baseUrl;
    }

    public virtual async Task<T?> Save(T dto, bool create, string token)
    {
       httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
        var response = create ? await httpClient.PostAsync(baseUrl, content) : await httpClient.PutAsync(baseUrl, content);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<T>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return result.Payload;
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new DuplicatedException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<T>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            throw new InvalidPayloadException(result?.Message.Split(";").ToList() ?? []);
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public virtual async Task<bool> Delete(string id, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.DeleteAsync($"{baseUrl}/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<bool>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result?.Code == ResponseCode.OK;
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
        {
            throw new DuplicatedException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public virtual async Task<List<T>> GetAll(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.GetAsync($"{baseUrl}s");
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<List<T>>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return result.Payload ?? [];
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

    public virtual async Task<T?> GetById(string id, string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
        var response = await httpClient.GetAsync($"{baseUrl}/{id}");
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Response<T>>(resultString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (result?.Code == ResponseCode.OK)
            {
                return result.Payload;
            }
            else
            {
                throw new HttpRequestException(result?.Message);
            }
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedAccessException();
        }
        else
        {
            throw new HttpRequestException(response.ReasonPhrase);
        }
    }

}
