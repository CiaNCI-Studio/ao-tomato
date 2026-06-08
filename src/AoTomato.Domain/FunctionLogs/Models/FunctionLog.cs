using AoTomato.Domain.Common.Models;

namespace AoTomato.Domain.FunctionLogs.Models;

public class FunctionLog : EntityBase
{
    public string FunctionId { get; set; } = string.Empty;
    public string ExecutionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Headers { get; set; } = string.Empty;
    public string QueryParameters { get; set; } = string.Empty;       
    public string Result { get; set; } = string.Empty;       
}