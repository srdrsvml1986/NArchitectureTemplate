using Application.Features.RoleOperationClaims.Commands.Create;
using Application.Features.RoleOperationClaims.Commands.Delete;
using Application.Features.RoleOperationClaims.Commands.Update;
using Application.Features.RoleOperationClaims.Queries.GetById;
using Application.Features.RoleOperationClaims.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleOperationClaimsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedRoleOperationClaimResponse>> Add([FromBody] CreateRoleOperationClaimCommand command)
    {
        CreatedRoleOperationClaimResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedRoleOperationClaimResponse>> Update([FromBody] UpdateRoleOperationClaimCommand command)
    {
        UpdatedRoleOperationClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedRoleOperationClaimResponse>> Delete([FromRoute] int id)
    {
        DeleteRoleOperationClaimCommand command = new() { Id = id };

        DeletedRoleOperationClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdRoleOperationClaimResponse>> GetById([FromRoute] int id)
    {
        GetByIdRoleOperationClaimQuery query = new() { Id = id };

        GetByIdRoleOperationClaimResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListRoleOperationClaimListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListRoleClaimQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListRoleOperationClaimListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}