using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace WebAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class SmsCallbackController : BaseController
{
    private readonly ILogger _logger;

    public SmsCallbackController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpPost("callback")]
    public async Task<IActionResult> ReceiveDeliveryReport()
    {
        try
        {
            var report = await JsonSerializer.DeserializeAsync<VodafoneDeliveryReport>(
                Request.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            _logger.LogInformation("SMS delivery report received: {MessageId}, Status: {Status}",
                report?.CallbackData, report?.DeliveryStatus);

            // Here you can update your database with the delivery status
            // or trigger other actions based on the delivery status

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing SMS delivery report");
            return StatusCode(500);
        }
    }

    private class VodafoneDeliveryReport
    {
        public string CallbackData { get; set; }
        public string DeliveryStatus { get; set; }
        public string Address { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
