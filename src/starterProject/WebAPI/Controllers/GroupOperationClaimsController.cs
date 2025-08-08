using Application.Features.GroupOperationClaims.Commands.Create;
using Application.Features.GroupOperationClaims.Commands.Delete;
using Application.Features.GroupOperationClaims.Commands.Update;
using Application.Features.GroupOperationClaims.Queries.GetById;
using Application.Features.GroupOperationClaims.Queries.GetList;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

/// <summary>
/// GroupClaimsController s�n�f�, bir ASP.NET Core Web API kontrolc�s�d�r
/// ve grup yetkilendirme (Group Claims) i�lemleri i�in CRUD (Create, Read, Update, Delete)
/// operasyonlar�n� sa�lar. Bu kontrolc�, BaseController s�n�f�ndan t�retilmi�tir 
/// ve IMediator arac�l���yla CQRS (Command Query Responsibility Segregation) desenini kullan�r.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class GroupOperationClaimsController : BaseController
{
    /// <summary>
    /// Add metodu, yeni bir grup yetkilendirme (Group Claim) olu�turmak i�in kullan�l�r.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CreatedGroupOperationClaimResponse>> Add([FromBody] CreateGroupOperationClaimCommand command)
    {
        CreatedGroupOperationClaimResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }
    /// <summary>
    /// Update metodu, mevcut bir grup yetkilendirme (Group Claim) g�ncellemek i�in kullan�l�r.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<UpdatedGroupOperationClaimResponse>> Update([FromBody] UpdateGroupOperationClaimCommand command)
    {
        UpdatedGroupOperationClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// Delete metodu, mevcut bir grup yetkilendirme (Group Claim) silmek i�in kullan�l�r.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedGroupOperationClaimResponse>> Delete([FromRoute] int id)
    {
        DeleteGroupOperationClaimCommand command = new() { Id = id };

        DeletedGroupOperationClaimResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// GetById metodu, belirli bir grup yetkilendirme (Group Claim) nesnesini
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdGroupOperationClaimResponse>> GetById([FromRoute] int id)
    {
        GetByIdGroupOperationClaimQuery query = new() { Id = id };

        GetByIdGroupOperationClaimResponse response = await Mediator.Send(query);

        return Ok(response);
    }
    /// <summary>
    /// GetList metodu, grup yetkilendirme (Group Claim) nesnelerinin
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListGroupOperationClaimListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListGroupOperationClaimQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListGroupOperationClaimListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}