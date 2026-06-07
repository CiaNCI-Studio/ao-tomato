namespace AoTomato.Admin.Services;

using AoTomato.Admin.Abstractions;
using AoTomato.domain.Functions.Dtos;
using Cordel.Client.Services;

public class FunctionsService : ServiceBase<FunctionDto>, IFunctionsService
{
    public FunctionsService(HttpClient httpClient) : base(httpClient, "/v1/Function")
    {
    }
}