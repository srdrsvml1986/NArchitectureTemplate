﻿using Application.Features.SecurityClaims.Commands.Create;
using Application.Features.SecurityClaims.Commands.Delete;
using Application.Features.SecurityClaims.Commands.Update;
using Application.Features.SecurityClaims.Queries.GetById;
using Application.Features.SecurityClaims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

/// <summary>
/// Controller for managing claims.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SecurityClaimsController : BaseController
{
    /// <summary>
    /// Belirtilen Id'ye sahip yetki bilgisini getirir.
    /// </summary>
    /// <param name="getByIdSecurityClaimQuery">yetki bilgilerini getirmek için kullanılan sorgu.</param>
    /// <returns>yetki bilgilerini içeren yanıt nesnesi.</returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdSecurityClaimQuery getByIdSecurityClaimQuery)
    {
        GetByIdSecurityClaimResponse result = await Mediator.Send(getByIdSecurityClaimQuery);
        return Ok(result);
    }

    /// <summary>
    /// Belirtilen sayfa isteğine göre yetki listesini getirir.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListSecurityClaimQuery getListClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListSecurityClaimListItemDto> result = await Mediator.Send(getListClaimQuery);
        return Ok(result);
    }

    /// <summary>
    /// Yeni bir yetki oluşturur.
    /// </summary>
    /// <param name="createSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateSecurityClaimCommand createSecurityClaimCommand)
    {
        CreatedSecurityClaimResponse result = await Mediator.Send(createSecurityClaimCommand);
        return Created(uri: "", result);
    }

    /// <summary>
    /// Güncellenmiş yetki bilgilerini içeren bir yanıt döndürür.
    /// </summary>
    /// <param name="updateSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSecurityClaimCommand updateSecurityClaimCommand)
    {
        UpdateSecurityClaimResponse result = await Mediator.Send(updateSecurityClaimCommand);
        return Ok(result);
    }

    /// <summary>
    /// Silinmiş yetki bilgilerini içeren bir yanıt döndürür.
    /// </summary>
    /// <param name="deleteSecurityClaimCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteSecurityClaimCommand deleteSecurityClaimCommand)
    {
        DeleteSecurityClaimResponse result = await Mediator.Send(deleteSecurityClaimCommand);
        return Ok(result);
    }
}
