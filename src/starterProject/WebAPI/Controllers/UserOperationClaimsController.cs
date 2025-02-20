using Application.Features.UserClaims.Commands.Create;
using Application.Features.UserClaims.Commands.Delete;
using Application.Features.UserClaims.Commands.Update;
using Application.Features.UserClaims.Queries.GetById;
using Application.Features.UserClaims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserOperationClaimsController : BaseController
{
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdUserClaimQuery getByIdUserOperationClaimQuery)
    {
        GetByIdUserClaimResponse result = await Mediator.Send(getByIdUserOperationClaimQuery);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserClaimQuery getListUserOperationClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserClaimListItemDto> result = await Mediator.Send(getListUserOperationClaimQuery);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserClaimCommand createUserOperationClaimCommand)
    {
        CreatedUserClaimResponse result = await Mediator.Send(createUserOperationClaimCommand);
        return Created(uri: "", result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserClaimCommand updateUserOperationClaimCommand)
    {
        UpdatedUserClaimResponse result = await Mediator.Send(updateUserOperationClaimCommand);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserClaimCommand deleteUserOperationClaimCommand)
    {
        DeletedUserClaimResponse result = await Mediator.Send(deleteUserOperationClaimCommand);
        return Ok(result);
    }
}
