using Microsoft.AspNetCore.Mvc;
using NArchitectureTemplate.Core.Notification.Services;
using ILogger = NArchitectureTemplate.Core.CrossCuttingConcerns.Logging.Abstraction.ILogger;

namespace WebAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class NotificationsController : BaseController
{
    private readonly IPushNotificationService _pushNotificationService;
    private readonly ILogger _logger;

    public NotificationsController(IPushNotificationService pushNotificationService, ILogger logger)
    {
        _pushNotificationService = pushNotificationService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] PushNotificationRequest request)
    {
        var notification = new PushNotification
        {
            Title = request.Title,
            Body = request.Body,
            DeviceTokens = request.DeviceTokens,
            Data = request.Data,
            Platform = PushPlatform.All
        };

        var result = await _pushNotificationService.SendAsync(notification);

        _logger.Information($"Push notification send attempt. Success: {result.IsSuccess}, MessageId: {result.MessageId}, ErrorMessage: {result.ErrorMessage}");

        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}
public class PushNotificationRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public List<string> DeviceTokens { get; set; }
    public Dictionary<string, string> Data { get; set; }
}