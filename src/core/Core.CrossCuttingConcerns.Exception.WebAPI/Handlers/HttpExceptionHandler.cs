using Microsoft.AspNetCore.Http;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Handlers;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Extensions;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.HttpProblemDetails;

namespace NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.WebApi.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    public HttpResponse Response
    {
#pragma warning disable S112 // General or reserved exceptions should never be thrown
        get => _response ?? throw new NullReferenceException(nameof(_response));
#pragma warning restore S112 // General or reserved exceptions should never be thrown
        set => _response = value;
    }

    private HttpResponse? _response;

    public override Task HandleException(BusinessException businessException)
    {
        // HttpContext'in geçerliliğini kontrol et
        if (Response.HasStarted || _response?.HttpContext == null)
        {
            return Task.CompletedTask;
        }
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new BusinessProblemDetails(businessException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(ValidationException validationException)
    {
        // HttpContext'in geçerliliğini kontrol et
        if (Response.HasStarted || _response?.HttpContext == null)
        {
            return Task.CompletedTask;
        }
        Response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new ValidationProblemDetails(validationException.Errors).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(AuthorizationException authorizationException)
    {
        // HttpContext'in geçerliliğini kontrol et
        if (Response.HasStarted || _response?.HttpContext == null)
        {
            return Task.CompletedTask;
        }
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        string details = new AuthorizationProblemDetails(authorizationException.Message).ToJson();
        return Response.WriteAsync(details);
    }


    public override Task HandleException(NotFoundException notFoundException)
    {
        // HttpContext'in geçerliliğini kontrol et
        if (Response.HasStarted || _response?.HttpContext == null)
        {
            return Task.CompletedTask;
        }
        Response.StatusCode = StatusCodes.Status404NotFound;
        string details = new NotFoundProblemDetails(notFoundException.Message).ToJson();
        return Response.WriteAsync(details);
    }

    public override Task HandleException(System.Exception exception)
    {
        // HttpContext'in geçerliliğini kontrol et
        if (Response.HasStarted || _response?.HttpContext == null)
        {
            return Task.CompletedTask;
        }
        Response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new InternalServerErrorProblemDetails(exception.Message).ToJson();
        return Response.WriteAsync(details);
    }
}
