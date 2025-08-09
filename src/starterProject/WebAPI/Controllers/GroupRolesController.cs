using Application.Features.GroupRoles.Commands.Create;
using Application.Features.GroupRoles.Commands.Delete;
using Application.Features.GroupRoles.Commands.Update;
using Application.Features.GroupRoles.Queries.GetById;
using Application.Features.GroupRoles.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupRolesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedGroupRoleResponse>> Add([FromBody] CreateGroupRoleCommand command)
    {
        CreatedGroupRoleResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedGroupRoleResponse>> Update([FromBody] UpdateGroupRoleCommand command)
    {
        UpdatedGroupRoleResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedGroupRoleResponse>> Delete([FromRoute] int id)
    {
        DeleteGroupRoleCommand command = new() { Id = id };

        DeletedGroupRoleResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdGroupRoleResponse>> GetById([FromRoute] int id)
    {
        GetByIdGroupRoleQuery query = new() { Id = id };

        GetByIdGroupRoleResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListGroupRoleListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListGroupRoleQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListGroupRoleListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}