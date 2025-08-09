using Application.Features.Roles.Commands.Create;
using Application.Features.Roles.Commands.Delete;
using Application.Features.Roles.Commands.Update;
using Application.Features.Roles.Queries.GetById;
using Application.Features.Roles.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedRoleResponse>> Add([FromBody] CreateRoleCommand command)
    {
        CreatedRoleResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedRoleResponse>> Update([FromBody] UpdateRoleCommand command)
    {
        UpdatedRoleResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedRoleResponse>> Delete([FromRoute] int id)
    {
        DeleteRoleCommand command = new() { Id = id };

        DeletedRoleResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdRoleResponse>> GetById([FromRoute] int id)
    {
        GetByIdRoleQuery query = new() { Id = id };

        GetByIdRoleResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListRoleListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListRoleQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListRoleListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}