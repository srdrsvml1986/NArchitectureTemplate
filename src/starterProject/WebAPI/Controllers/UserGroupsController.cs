using Application.Features.UserGroups.Commands.Create;
using Application.Features.UserGroups.Commands.Delete;
using Application.Features.UserGroups.Commands.Update;
using Application.Features.UserGroups.Queries.GetById;
using Application.Features.UserGroups.Queries.GetList;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserGroupsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedUserGroupResponse>> Add([FromBody] CreateUserGroupCommand command)
    {
        CreatedUserGroupResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedUserGroupResponse>> Update([FromBody] UpdateUserGroupCommand command)
    {
        UpdatedUserGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedUserGroupResponse>> Delete([FromRoute] int id)
    {
        DeleteUserGroupCommand command = new() { Id = id };

        DeletedUserGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdUserGroupResponse>> GetById([FromRoute] int id)
    {
        GetByIdUserGroupQuery query = new() { Id = id };

        GetByIdUserGroupResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListUserGroupListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserGroupQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListUserGroupListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}