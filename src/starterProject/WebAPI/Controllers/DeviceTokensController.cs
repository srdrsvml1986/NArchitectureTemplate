using Application.Features.DeviceTokens.Commands.Create;
using Application.Features.DeviceTokens.Commands.Delete;
using Application.Features.DeviceTokens.Commands.Update;
using Application.Features.DeviceTokens.Queries.GetById;
using Application.Features.DeviceTokens.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeviceTokensController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedDeviceTokenResponse>> Add([FromBody] CreateDeviceTokenCommand command)
    {
        CreatedDeviceTokenResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedDeviceTokenResponse>> Update([FromBody] UpdateDeviceTokenCommand command)
    {
        UpdatedDeviceTokenResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedDeviceTokenResponse>> Delete([FromRoute] Guid id)
    {
        DeleteDeviceTokenCommand command = new() { Id = id };

        DeletedDeviceTokenResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdDeviceTokenResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdDeviceTokenQuery query = new() { Id = id };

        GetByIdDeviceTokenResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListDeviceTokenListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListDeviceTokenQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListDeviceTokenListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}