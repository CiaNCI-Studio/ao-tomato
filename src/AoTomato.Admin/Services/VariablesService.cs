namespace AoTomato.Admin.Services;

using AoTomato.Admin.Abstractions;
using AoTomato.Domain.Variables.Dtos;
using Cordel.Client.Services;

public class VariablesService : ServiceBase<VariableDto>, IVariablesService
{
    public VariablesService(HttpClient httpClient) : base(httpClient, "/v1/Variable")
    {
    }
}