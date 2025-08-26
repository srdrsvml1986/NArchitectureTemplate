using Application.Features.UserNotificationSettings.Commands.Create;
using Application.Features.UserNotificationSettings.Commands.Delete;
using Application.Features.UserNotificationSettings.Commands.Update;
using Application.Features.UserNotificationSettings.Queries.GetById;
using Application.Features.UserNotificationSettings.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserNotificationSettingsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedUserNotificationSettingResponse>> Add([FromBody] CreateUserNotificationSettingCommand command)
    {
        CreatedUserNotificationSettingResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedUserNotificationSettingResponse>> Update([FromBody] UpdateUserNotificationSettingCommand command)
    {
        UpdatedUserNotificationSettingResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedUserNotificationSettingResponse>> Delete([FromRoute] Guid id)
    {
        DeleteUserNotificationSettingCommand command = new() { Id = id };

        DeletedUserNotificationSettingResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdUserNotificationSettingResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdUserNotificationSettingQuery query = new() { Id = id };

        GetByIdUserNotificationSettingResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListUserNotificationSettingListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserNotificationSettingQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListUserNotificationSettingListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}