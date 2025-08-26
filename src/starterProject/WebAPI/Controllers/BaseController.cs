using MediatR;
using Microsoft.AspNetCore.Mvc;
using NArchitectureTemplate.Core.Security.Extensions;

namespace WebAPI.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()
            ?? throw new InvalidOperationException("IMediator cannot be retrieved from request services.");

    private IMediator? _mediator;

    protected string getIpAddress()
    {
        string ipAddress = Request.Headers.ContainsKey("X-Forwarded-For")
            ? Request.Headers["X-Forwarded-For"].ToString()
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                ?? throw new InvalidOperationException("IP address cannot be retrieved from request.");
        return ipAddress;
    }

    protected Guid getUserIdFromRequest() //todo authentication behavior?
    {
        var userIdClaim = HttpContext.User?.GetIdClaim();
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new UnauthorizedAccessException("Kullanıcı kimliği bulunamadı.");
        }
        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            throw new FormatException("Kullanıcı kimliği geçerli bir GUID formatında değil.");
        }
        return userId;
    }
}
