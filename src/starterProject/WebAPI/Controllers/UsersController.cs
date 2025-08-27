using Application.Features.Users.Commands.AddUserClaims;
using Application.Features.Users.Commands.AddUserGroups;
using Application.Features.Users.Commands.Create;
using Application.Features.Users.Commands.Delete;
using Application.Features.Users.Commands.ForgotPassword;
using Application.Features.Users.Commands.ResetPassword;
using Application.Features.Users.Commands.Update;
using Application.Features.Users.Commands.UpdateFromAuth;
using Application.Features.Users.Commands.UpdatePhotoURL;
using Application.Features.Users.Commands.UpdateStatus;
using Application.Features.Users.Commands.UpdateUserClaims;
using Application.Features.Users.Commands.UpdateUserGroups;
using Application.Features.Users.Queries.GetById;
using Application.Features.Users.Queries.GetClaimsByUserId;
using Application.Features.Users.Queries.GetGroupsByUserId;
using Application.Features.Users.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;

namespace WebAPI.Controllers;


/// <summary>
/// Kullanıcı işlemlerinin yönetildiği API controller sınıfı.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UsersController : BaseController
{
    /// <summary>
    /// Kullanıcı bilgilerini almak için kullanılan API metodu.
    /// </summary>
    /// <param name="getByIdUserQuery"></param>
    /// <returns></returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdUserQuery getByIdUserQuery)
    {
        GetByIdUserResponse result = await Mediator.Send(getByIdUserQuery);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı listesini almak için kullanılan API metodu.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserQuery getListUserQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserListItemDto> result = await Mediator.Send(getListUserQuery);
        return Ok(result);
    }
    /// <summary>
    /// Yeni bir kullanıcı oluşturmak için kullanılan API metodu.
    /// </summary>
    /// <param name="createUserCommand"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserCommand createUserCommand)
    {
        CreatedUserResponse result = await Mediator.Send(createUserCommand);
        return Created(uri: "", result);
    }
    /// <summary>
    /// Kullanıcı bilgilerini güncellemek için kullanılan API metodu.
    /// </summary>
    /// <param name="updateUserCommand"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand updateUserCommand)
    {
        UpdatedUserResponse result = await Mediator.Send(updateUserCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı bilgilerini silmek için kullanılan API metodu.
    /// </summary>
    /// <param name="deleteUserCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand deleteUserCommand)
    {
        DeletedUserResponse result = await Mediator.Send(deleteUserCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı bilgilerini almak için kullanılan API metodu.
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = getUserIdFromRequest();
        var query = new GetByIdUserQuery { Id = userId };
        var result = await Mediator.Send(query);
        return Ok(result);
    }


    /// <summary>
    /// Kullanıcının durumunu güncellemek için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        [FromRoute] Guid userId,
        [FromBody] UpdateUserStatusCommand command)
    {
        command.Id = userId;
        var result = await Mediator.Send(command);
        return Ok(result);
    }


    /// <summary>
    /// Kullanıcının kimlik doğrulama bilgileri üzerinden şifre güncellemesi yapmak için kullanılan API metodu.
    /// </summary>
    /// <param name="updateUserFromAuthCommand">Şifre güncelleme komutunu içeren model.</param>
    /// <returns>Güncellenen kullanıcı bilgilerini içeren yanıt.</returns> 

    [HttpPut("UpdatePasswordFromAuth")]
    public async Task<IActionResult> UpdatePasswordFromAuth([FromBody] UpdateUserPasswordFromAuthCommand updateUserFromAuthCommand)
    {
        updateUserFromAuthCommand.Id = getUserIdFromRequest();
        UpdatedUserFromAuthResponse result = await Mediator.Send(updateUserFromAuthCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcının fotoğraf URL'sini güncellemek için kullanılan API metodu.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatePhotoUrlCommand"></param>
    /// <returns></returns>
    [HttpPut("{userId}/photo-url")]
    public async Task<IActionResult> UpdatePhotoUrl([FromRoute] Guid id, [FromBody] UpdatePhotoUrlCommand updatePhotoUrlCommand)
    {
        updatePhotoUrlCommand.Id = id;
        UpdatedUserResponse result = await Mediator.Send(updatePhotoUrlCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcının şifresini unuttuğunda şifre sıfırlama bağlantısı göndermek için kullanılan API metodu.
    /// </summary>
    /// <param name="forgotPasswordCommand"></param>
    /// <returns></returns>
    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand forgotPasswordCommand)
    {
        ForgotPasswordResponse result = await Mediator.Send(forgotPasswordCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcının şifresini sıfırlamak için kullanılan API metodu.
    /// </summary>
    /// <param name="resetPasswordCommand"></param>
    /// <returns></returns>
    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand)
    {
        ResetPasswordResponse result = await Mediator.Send(resetPasswordCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcının claim'lerini almak için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("{userId}/claims")]
    public async Task<IActionResult> GetUserClaims([FromRoute] Guid userId)
    {
        GetClaimsByUserIdQuery getClaimsByUserIdQuery = new() { UserId = userId };
        GetClaimsByUserIdResponse result = await Mediator.Send(getClaimsByUserIdQuery);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcının claim'lerini eklemek için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="claimIds"></param>
    /// <returns></returns>
    [HttpPost("{userId}/claims")]
    public async Task<IActionResult> AddUserClaims([FromRoute] Guid userId, [FromBody] IList<int> claimIds)
    {
        AddUserClaimsCommand addUserClaimsCommand = new() { UserId = userId, ClaimIds = claimIds };
        AddUserClaimsResponse result = await Mediator.Send(addUserClaimsCommand);
        return Created(uri: "", result);
    }
    /// <summary>
    /// Kullanıcının claim'lerini güncellemek için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="claimIds"></param>
    /// <returns></returns>
    [HttpPut("{userId}/claims")]
    public async Task<IActionResult> UpdateUserClaims([FromRoute] Guid userId, [FromBody] IList<int> claimIds)
    {
        UpdateUserClaimsCommand updateUserClaimsCommand = new() { UserId = userId, ClaimIds = claimIds };
        UpdateUserClaimsResponse result = await Mediator.Send(updateUserClaimsCommand);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının gruplarını almak için kullanılan API metodu.
    /// </summary>
    /// <param name="userId">Grupları alınacak kullanıcının ID'si.</param>
    /// <returns>Kullanıcının gruplarını içeren yanıt.</returns>
    [HttpGet("{userId}/groups")]
    public async Task<IActionResult> GetUserGroups([FromRoute] Guid userId)
    {
        GetGroupsByUserIdQuery getGroupsByUserIdQuery = new() { UserId = userId };
        GetGroupsByUserIdResponse result = await Mediator.Send(getGroupsByUserIdQuery);
        return Ok(result);
    }

    /// <summary>
    /// Kullanıcının gruplarını eklemek için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupIds"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Kullanıcının gruplarını güncellemek için kullanılan API metodu.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupIds"></param>
    /// <returns></returns>
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
