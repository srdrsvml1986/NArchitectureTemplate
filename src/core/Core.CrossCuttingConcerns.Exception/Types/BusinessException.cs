using System.Collections;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;

public class BusinessException : System.Exception
{
    public BusinessException() { }

    public BusinessException(string? message)
        : base(message) { }

    public BusinessException(string? message, System.Exception? innerException)
        : base(message, innerException) { }

    public override IDictionary Data => base.Data;

    public override string Message => base.Message;
}
