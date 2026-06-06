namespace AoTomato.Domain.Variables.Abstractions.Services;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Variables.Models;
using AoTomato.Domain.Variables.Dtos;

public interface IVariablesService : IServiceBase< VariableDto,Variable>
{
    Task<VariableDto?> GetByKeyAsync(string key);
    Task<bool> SetVariableAsync(VariableDto variableDto);
}