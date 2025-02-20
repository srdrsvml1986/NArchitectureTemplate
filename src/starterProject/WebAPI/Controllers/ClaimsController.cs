using Application.Features.Claims.Commands.Create;
using Application.Features.Claims.Commands.Delete;
using Application.Features.Claims.Commands.Update;
using Application.Features.Claims.Queries.GetById;
using Application.Features.Claims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClaimsController : BaseController
{
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdClaimQuery getByIdOperationClaimQuery)
    {
        GetByIdClaimResponse result = await Mediator.Send(getByIdOperationClaimQuery);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListClaimQuery getListClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListClaimListItemDto> result = await Mediator.Send(getListClaimQuery);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateClaimCommand createClaimCommand)
    {
        CreatedClaimResponse result = await Mediator.Send(createClaimCommand);
        return Created(uri: "", result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateClaimCommand updateClaimCommand)
    {
        UpdatedClaimResponse result = await Mediator.Send(updateClaimCommand);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteClaimCommand deleteClaimCommand)
    {
        DeletedClaimResponse result = await Mediator.Send(deleteClaimCommand);
        return Ok(result);
    }
}
