using Application.Features.UserGroups.Commands.Create;
using Application.Features.UserGroups.Commands.Delete;
using Application.Features.UserGroups.Commands.Update;
using Application.Features.UserGroups.Queries.GetById;
using Application.Features.UserGroups.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


/// <summary>
/// Kullan�c� gruplar� y�netimi i�in HTTP endpoint'leri sa�layan API controller s�n�f�.
/// Add: Yeni kullan�c� grubu olu�turur.
/// Update: Mevcut kullan�c� grubunu g�nceller.
/// Delete: Kullan�c� grubunu siler.
/// GetById: ID'ye g�re kullan�c� grubu getirir.
/// GetList: Sayfalanm�� kullan�c� gruplar� listesini getirir.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserGroupsController : BaseController
{
    /// <summary>
    /// Kullan�c� grubu olu�turma i�lemi i�in HTTP POST endpoint'i.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CreatedUserGroupResponse>> Add([FromBody] CreateUserGroupCommand command)
    {
        CreatedUserGroupResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }
    /// <summary>
    /// Kullan�c� grubu g�ncelleme i�lemi i�in HTTP PUT endpoint'i.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<UpdatedUserGroupResponse>> Update([FromBody] UpdateUserGroupCommand command)
    {
        UpdatedUserGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// Kullan�c� grubu silme i�lemi i�in HTTP DELETE endpoint'i.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedUserGroupResponse>> Delete([FromRoute] int id)
    {
        DeleteUserGroupCommand command = new() { Id = id };

        DeletedUserGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// Belirtilen ID'ye sahip kullan�c� grubunu getiren HTTP GET endpoint'i.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdUserGroupResponse>> GetById([FromRoute] int id)
    {
        GetByIdUserGroupQuery query = new() { Id = id };

        GetByIdUserGroupResponse response = await Mediator.Send(query);

        return Ok(response);
    }
    /// <summary>
    /// Sayfalanm�� kullan�c� gruplar� listesini getiren HTTP GET endpoint'i.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListUserGroupListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserGroupQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListUserGroupListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}