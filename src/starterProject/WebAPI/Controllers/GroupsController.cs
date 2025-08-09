using Application.Features.Groups.Commands.Create;
using Application.Features.Groups.Commands.Delete;
using Application.Features.Groups.Commands.Update;
using Application.Features.Groups.Queries.GetById;
using Application.Features.Groups.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Groups.Commands.AddClaimsToGroup;
using Application.Features.Groups.Commands.UpdateClaimsInGroup;
using Application.Features.Groups.Queries.GetClaimsByGroupId;

namespace WebAPI.Controllers;

/// <summary>
/// Gruplar ile ilgili CRUD iþlemleri ve grup taleplerini yönetmek için kullanýlan API denetleyicisidir.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class GroupsController : BaseController
{
    /// <summary>
    /// Gruplarý listelemek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CreatedGroupResponse>> Add([FromBody] CreateGroupCommand command)
    {
        CreatedGroupResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }
    /// <summary>
    /// Gruplarý güncellemek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<UpdatedGroupResponse>> Update([FromBody] UpdateGroupCommand command)
    {
        UpdatedGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// Gruplarý silmek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedGroupResponse>> Delete([FromRoute] int id)
    {
        DeleteGroupCommand command = new() { Id = id };

        DeletedGroupResponse response = await Mediator.Send(command);

        return Ok(response);
    }
    /// <summary>
    /// Belirli bir grubu ID'sine göre almak için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdGroupResponse>> GetById([FromRoute] int id)
    {
        GetByIdGroupQuery query = new() { Id = id };

        GetByIdGroupResponse response = await Mediator.Send(query);

        return Ok(response);
    }
    /// <summary>
    /// Gruplarý listelemek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="pageRequest"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListGroupListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListGroupQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListGroupListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
    /// <summary>
    /// Grup yetkilerini almak için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns></returns>
    [HttpGet("{groupId}/claims")]
    public async Task<IActionResult> GetGroupClaims([FromRoute] int groupId)
    {
        GetClaimsByGroupIdGroupQuery getClaimsByGroupIdQuery = new() { Id = groupId };
        GetClaimsByGroupIdGroupResponse result = await Mediator.Send(getClaimsByGroupIdQuery);
        return Ok(result);
    }
    /// <summary>
    /// Gruba yetki eklemek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="claimIds"></param>
    /// <returns></returns>
    [HttpPost("{groupId}/claims")]
    public async Task<IActionResult> AddClaimsToGroup([FromRoute] int groupId, [FromBody] IList<int> claimIds)
    {
        AddClaimsToGroupCommand addClaimsToGroupCommand = new()
        {
            GroupId = groupId,
            ClaimIds = claimIds
        };
        AddClaimsToGroupResponse result = await Mediator.Send(addClaimsToGroupCommand);
        return Created(uri: "", result);
    }
    /// <summary>
    /// Grup yetkilerini güncellemek için kullanýlan API uç noktasýdýr.
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="claimIds"></param>
    /// <returns></returns>
    [HttpPut("{groupId}/claims")]
    public async Task<IActionResult> UpdateGroupClaims([FromRoute] int groupId, [FromBody] IList<int> claimIds)
    {
        UpdateClaimsInGroupCommand updateClaimsInGroupCommand = new()
        {
            GroupId = groupId,
            ClaimIds = claimIds
        };
        UpdateClaimsInGroupResponse result = await Mediator.Send(updateClaimsInGroupCommand);
        return Ok(result);
    }
}