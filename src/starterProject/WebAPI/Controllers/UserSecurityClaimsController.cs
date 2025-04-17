using Application.Features.UserSecurityClaims.Commands.Create;
using Application.Features.UserSecurityClaims.Commands.Delete;
using Application.Features.UserSecurityClaims.Commands.Update;
using Application.Features.UserSecurityClaims.Queries.GetById;
using Application.Features.UserSecurityClaims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

/// <summary>
/// Kullanıcı yetkilendirme işlemlerini yönetmek için kullanılan kontrolcü.
/// CRUD işlemleri için uç noktalar sağlar.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserSecurityClaimsController : BaseController
{
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini almak için kullanılan uç nokta.
    /// </summary>
    /// <param name="getByIdUserSecurityClaimQuery"></param>
    /// <returns></returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdUserSecurityClaimQuery getByIdUserSecurityClaimQuery)
    {
        GetByIdUserSecurityClaimResponse result = await Mediator.Send(getByIdUserSecurityClaimQuery);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini listelemek için kullanılan uç nokta.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserSecurityClaimQuery getListUserClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserSecurityClaimListItemDto> result = await Mediator.Send(getListUserClaimQuery);
        return Ok(result);
    }
    /// <summary>
    /// Yeni bir kullanıcı yetkilendirme bilgisi eklemek için kullanılan uç nokta.
    /// </summary>
    /// <param name="createUserSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserSecurityClaimCommand createUserSecurityClaimCommand)
    {
        CreatedUserSecurityClaimResponse result = await Mediator.Send(createUserSecurityClaimCommand);
        return Created(uri: "", result);
    }
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini güncellemek için kullanılan uç nokta.
    /// </summary>
    /// <param name="updateUserSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserSecurityClaimCommand updateUserSecurityClaimCommand)
    {
        UpdatedUserSecurityClaimResponse result = await Mediator.Send(updateUserSecurityClaimCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini silmek için kullanılan uç nokta.
    /// </summary>
    /// <param name="deleteUserSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserSecurityClaimCommand deleteUserSecurityClaimCommand)
    {
        DeletedUserSecurityClaimResponse result = await Mediator.Send(deleteUserSecurityClaimCommand);
        return Ok(result);
    }
}
