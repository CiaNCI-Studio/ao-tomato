namespace AoTomato.Services.Variables;

using AoTomato.Domain.Common.Models;
using AoTomato.Domain.Variables.Abstractions.Repositories;
using AoTomato.Domain.Variables.Abstractions.Services;
using AoTomato.Domain.Variables.Dtos;
using AoTomato.Domain.Variables.Models;
using AutoMapper;
using Serilog;

public class VariableService : ServiceBase<VariableDto, Variable>, IVariablesService
{
    private readonly IVariablessRepository variableRepository;

    public VariableService(IVariablessRepository variableRepository, ILogger logger, IMapper mapper) : base(variableRepository, logger, mapper)
    {
        this.variableRepository = variableRepository;
    }

    public async Task<VariableDto?> GetByKeyAsync(string key)
    {
        var variable = await variableRepository.GetByKeyAsync(key);
        return mapper.Map<VariableDto>(variable);
    }

    public async Task<bool> SetVariableAsync(VariableDto variableDto)
    {
        var currentVariable = await variableRepository.GetByKeyAsync(variableDto.Key);
        var newVariable = mapper.Map<Variable>(variableDto);
        if(currentVariable != null)
        {
            if (!currentVariable.ReadOnly)
            {
                currentVariable.Value = newVariable.Value;
                await variableRepository.UpdateAsync(currentVariable, currentVariable.CreatedBy);
                return true;
            }
            else
            {
                return false;
            }
        }
        newVariable.ReadOnly = false;
        await variableRepository.InsertAsync(newVariable, "function");
        return true;
    }
}
