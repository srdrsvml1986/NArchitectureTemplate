using Application.Features.RoleClaims.Commands.Create;
using Application.Features.RoleClaims.Commands.Delete;
using Application.Features.RoleClaims.Commands.Update;
using Application.Features.RoleClaims.Queries.GetById;
using Application.Features.RoleClaims.Queries.GetList;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleClaimsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedRoleClaimResponse>> Add([FromBody] CreateRoleClaimCommand command)
    {
        CreatedRoleClaimResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedRoleClaimResponse>> Update([FromBody] UpdateRoleClaimCommand command)
    {
        UpdatedRoleClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedRoleClaimResponse>> Delete([FromRoute] int id)
    {
        DeleteRoleClaimCommand command = new() { Id = id };

        DeletedRoleClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdRoleClaimResponse>> GetById([FromRoute] int id)
    {
        GetByIdRoleClaimQuery query = new() { Id = id };

        GetByIdRoleClaimResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListRoleClaimListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListRoleClaimQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListRoleClaimListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}