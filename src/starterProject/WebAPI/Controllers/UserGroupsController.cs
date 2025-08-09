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
/// Kullanýcý gruplarý yönetimi için HTTP endpoint'leri saðlayan API controller sýnýfý.
/// Add: Yeni kullanýcý grubu oluþturur.
/// Update: Mevcut kullanýcý grubunu günceller.
/// Delete: Kullanýcý grubunu siler.
/// GetById: ID'ye göre kullanýcý grubu getirir.
/// GetList: Sayfalanmýþ kullanýcý gruplarý listesini getirir.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserGroupsController : BaseController
{
    /// <summary>
    /// Kullanýcý grubu oluþturma iþlemi için HTTP POST endpoint'i.
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
    /// Kullanýcý grubu güncelleme iþlemi için HTTP PUT endpoint'i.
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
    /// Kullanýcý grubu silme iþlemi için HTTP DELETE endpoint'i.
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
    /// Belirtilen ID'ye sahip kullanýcý grubunu getiren HTTP GET endpoint'i.
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
    /// Sayfalanmýþ kullanýcý gruplarý listesini getiren HTTP GET endpoint'i.
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