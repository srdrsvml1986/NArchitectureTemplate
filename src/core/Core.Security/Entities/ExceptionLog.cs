namespace NArchitectureTemplate.Core.Security.Entities;
public class ExceptionLog<TId> : Log<TId>
{
    public string ExceptionType { get; set; }
    public string StackTrace { get; set; }
}