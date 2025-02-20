using Application.Features.GroupClaims.Commands.Create;
using Application.Features.GroupClaims.Commands.Delete;
using Application.Features.GroupClaims.Commands.Update;
using Application.Features.GroupClaims.Queries.GetById;
using Application.Features.GroupClaims.Queries.GetList;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupClaimsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedGroupClaimResponse>> Add([FromBody] CreateGroupClaimCommand command)
    {
        CreatedGroupClaimResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedGroupClaimResponse>> Update([FromBody] UpdateGroupClaimCommand command)
    {
        UpdatedGroupClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedGroupClaimResponse>> Delete([FromRoute] int id)
    {
        DeleteGroupClaimCommand command = new() { Id = id };

        DeletedGroupClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdGroupClaimResponse>> GetById([FromRoute] int id)
    {
        GetByIdGroupClaimQuery query = new() { Id = id };

        GetByIdGroupClaimResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListGroupClaimListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListGroupClaimQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListGroupClaimListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}