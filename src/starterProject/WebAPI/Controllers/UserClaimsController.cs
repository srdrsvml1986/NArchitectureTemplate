﻿using Application.Features.UserClaims.Commands.Create;
using Application.Features.UserClaims.Commands.Delete;
using Application.Features.UserClaims.Commands.Update;
using Application.Features.UserClaims.Queries.GetById;
using Application.Features.UserClaims.Queries.GetList;
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
public class UserClaimsController : BaseController
{
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini almak için kullanılan uç nokta.
    /// </summary>
    /// <param name="getByIdUserOperationClaimQuery"></param>
    /// <returns></returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdUserClaimQuery getByIdUserOperationClaimQuery)
    {
        GetByIdUserClaimResponse result = await Mediator.Send(getByIdUserOperationClaimQuery);
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
        GetListUserClaimQuery getListUserClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserClaimListItemDto> result = await Mediator.Send(getListUserClaimQuery);
        return Ok(result);
    }
    /// <summary>
    /// Yeni bir kullanıcı yetkilendirme bilgisi eklemek için kullanılan uç nokta.
    /// </summary>
    /// <param name="createUserClaimCommand"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserClaimCommand createUserClaimCommand)
    {
        CreatedUserClaimResponse result = await Mediator.Send(createUserClaimCommand);
        return Created(uri: "", result);
    }
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini güncellemek için kullanılan uç nokta.
    /// </summary>
    /// <param name="updateUserClaimCommand"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserClaimCommand updateUserClaimCommand)
    {
        UpdatedUserClaimResponse result = await Mediator.Send(updateUserClaimCommand);
        return Ok(result);
    }
    /// <summary>
    /// Kullanıcı yetkilendirme bilgilerini silmek için kullanılan uç nokta.
    /// </summary>
    /// <param name="deleteUserClaimCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserClaimCommand deleteUserClaimCommand)
    {
        DeletedUserClaimResponse result = await Mediator.Send(deleteUserClaimCommand);
        return Ok(result);
    }
}
