using Application.Features.Users.Commands.AddUserClaims;
using Application.Features.Users.Commands.AddUserGroups;
using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.ForgotPassword;
using Application.Features.Users.Commands.ResetPassword;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.Commands.UpdateFromAuth;
using Application.Features.Users.Commands.UpdatePhotoURL;
using Application.Features.Users.Commands.UpdateUserClaims;
using Application.Features.Users.Commands.UpdateUserGroups;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.GetClaimsByUserId;
using Application.Features.Users.Queries.GetGroupsByUserId;
using Application.Features.Users.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseController
{
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdUserQuery getByIdUserQuery)
    {
        GetByIdUserResponse result = await Mediator.Send(getByIdUserQuery);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserQuery getListUserQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserListItemDto> result = await Mediator.Send(getListUserQuery);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserCommand createUserCommand)
    {
        CreatedUserResponse result = await Mediator.Send(createUserCommand);
        return Created(uri: "", result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand updateUserCommand)
    {
        UpdatedUserResponse result = await Mediator.Send(updateUserCommand);
        return Ok(result);
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand deleteUserCommand)
    {
        DeletedUserResponse result = await Mediator.Send(deleteUserCommand);
        return Ok(result);
    }

    [HttpGet("GetFromAuth")]
    public async Task<IActionResult> GetFromAuth()
    {
        GetByIdUserQuery getByIdUserQuery = new() { Id = getUserIdFromRequest() };
        GetByIdUserResponse result = await Mediator.Send(getByIdUserQuery);
        return Ok(result);
    }

    [HttpPut("UpdatePasswordFromAuth")]
    public async Task<IActionResult> UpdatePasswordFromAuth([FromBody] UpdateUserPasswordFromAuthCommand updateUserFromAuthCommand)
    {
        updateUserFromAuthCommand.Id = getUserIdFromRequest();
        UpdatedUserFromAuthResponse result = await Mediator.Send(updateUserFromAuthCommand);
        return Ok(result);
    }

    [HttpPut("{userId}/photo-url")]
    public async Task<IActionResult> UpdatePhotoUrl([FromRoute] Guid id, [FromBody] UpdatePhotoUrlCommand updatePhotoUrlCommand)
    {
        updatePhotoUrlCommand.Id = id;
        UpdatedUserResponse result = await Mediator.Send(updatePhotoUrlCommand);
        return Ok(result);
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand forgotPasswordCommand)
    {
        ForgotPasswordResponse result = await Mediator.Send(forgotPasswordCommand);
        return Ok(result);
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand)
    {
        ResetPasswordResponse result = await Mediator.Send(resetPasswordCommand);
        return Ok(result);
    }
    // UsersController'a eklenecek metodlar
    [HttpGet("{userId}/claims")]
    public async Task<IActionResult> GetUserClaims([FromRoute] Guid userId)
    {
        GetClaimsByUserIdQuery getClaimsByUserIdQuery = new() { UserId = userId };
        GetClaimsByUserIdResponse result = await Mediator.Send(getClaimsByUserIdQuery);
        return Ok(result);
    }

    [HttpPost("{userId}/claims")]
    public async Task<IActionResult> AddUserClaims([FromRoute] Guid userId, [FromBody] IList<int> claimIds)
    {
        AddUserClaimsCommand addUserClaimsCommand = new() { UserId = userId, ClaimIds = claimIds };
        AddUserClaimsResponse result = await Mediator.Send(addUserClaimsCommand);
        return Created(uri: "", result);
    }

    [HttpPut("{userId}/claims")]
    public async Task<IActionResult> UpdateUserClaims([FromRoute] Guid userId, [FromBody] IList<int> claimIds)
    {
        UpdateUserClaimsCommand updateUserClaimsCommand = new() { UserId = userId, ClaimIds = claimIds };
        UpdateUserClaimsResponse result = await Mediator.Send(updateUserClaimsCommand);
        return Ok(result);
    }
    [HttpGet("{userId}/groups")]
    public async Task<IActionResult> GetUserGroups([FromRoute] Guid userId)
    {
        GetGroupsByUserIdQuery getGroupsByUserIdQuery = new() { UserId = userId };
        GetGroupsByUserIdResponse result = await Mediator.Send(getGroupsByUserIdQuery);
        return Ok(result);
    }

    [HttpPost("{userId}/groups")]
    public async Task<IActionResult> AddUserGroups([FromRoute] Guid userId, [FromBody] IList<int> groupIds)
    {
        AddUserGroupsCommand addUserGroupsCommand = new()
        {
            UserId = userId,
            GroupIds = groupIds
        };
        AddUserGroupsResponse result = await Mediator.Send(addUserGroupsCommand);
        return Created(uri: "", result);
    }

    [HttpPut("{userId}/groups")]
    public async Task<IActionResult> UpdateUserGroups([FromRoute] Guid userId, [FromBody] IList<int> groupIds)
    {
        UpdateUserGroupsCommand updateUserGroupsCommand = new()
        {
            UserId = userId,
            GroupIds = groupIds
        };
        UpdateUserGroupsResponse result = await Mediator.Send(updateUserGroupsCommand);
        return Ok(result);
    }

}
