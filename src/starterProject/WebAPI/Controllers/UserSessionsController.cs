using Application.Features.UserSessions.Commands.Create;
using Application.Features.UserSessions.Commands.Delete;
using Application.Features.UserSessions.Commands.Update;
using Application.Features.UserSessions.Queries.GetById;
using Application.Features.UserSessions.Queries.GetList;
using Application.Features.UserSessions.Commands.CheckSuspiciousSessions;
using Application.Features.UserSessions.Commands.RevokeAllOtherSessions;
using Application.Features.UserSessions.Commands.RevokeMySession;
using Application.Features.UserSessions.Commands.RevokeUserSession;
using Application.Features.UserSessions.Queries.GetActiveSessionCount;
using Application.Features.UserSessions.Queries.GetActiveSessions;
using Application.Features.UserSessions.Queries.GetMyActiveSessionCount;
using Application.Features.UserSessions.Queries.GetMySessions;
using Application.Features.UserSessions.Queries.GetUserSessions;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserSessionsController : BaseController
{
    #region Mevcut Endpoint'ler

    /// <summary>
    /// Yeni bir kullan�c� oturumu olu�turur
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreatedUserSessionResponse>> Add([FromBody] CreateUserSessionCommand command)
    {
        CreatedUserSessionResponse response = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    /// <summary>
    /// Mevcut bir kullan�c� oturumunu g�nceller
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<UpdatedUserSessionResponse>> Update([FromBody] UpdateUserSessionCommand command)
    {
        UpdatedUserSessionResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip kullan�c� oturumunu siler
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedUserSessionResponse>> Delete([FromRoute] Guid id)
    {
        DeleteUserSessionCommand command = new() { Id = id };
        DeletedUserSessionResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip kullan�c� oturumunu getirir
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdUserSessionResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdUserSessionQuery query = new() { Id = id };
        GetByIdUserSessionResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Kullan�c� oturumlar�n� sayfal� �ekilde listeler
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListUserSessionListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListUserSessionQuery query = new() { PageRequest = pageRequest };
        GetListResponse<GetListUserSessionListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    #endregion

    #region Yeni Eklenen Endpoint'ler - Command'ler

    /// <summary>
    /// Kullan�c�n�n ��pheli oturumlar�n� kontrol eder
    /// </summary>
    [HttpPost("check-suspicious")]
    public async Task<ActionResult<CheckSuspiciousSessionsResponse>> CheckSuspiciousSessions([FromBody] CheckSuspiciousSessionsCommand command)
    {
        CheckSuspiciousSessionsResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Mevcut oturum d���ndaki t�m oturumlar� sonland�r�r
    /// </summary>
    [HttpPost("revoke-all-other")]
    public async Task<ActionResult<RevokeAllOtherSessionsResponse>> RevokeAllOtherSessions([FromBody] RevokeAllOtherSessionsCommand command)
    {
        RevokeAllOtherSessionsResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Kullan�c�n�n kendi oturumunu sonland�r�r
    /// </summary>
    [HttpPost("revoke-my-session")]
    public async Task<ActionResult<RevokeMySessionResponse>> RevokeMySession([FromBody] RevokeMySessionCommand command)
    {
        RevokeMySessionResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    /// <summary>
    /// Belirtilen kullan�c� oturumunu sonland�r�r
    /// </summary>
    [HttpPost("revoke-session")]
    public async Task<ActionResult<RevokeMySessionResponse>> RevokeUserSession([FromBody] RevokeUserSessionCommand command)
    {
        RevokeMySessionResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    #endregion

    #region Yeni Eklenen Endpoint'ler - Query'ler

    /// <summary>
    /// Aktif oturum say�s�n� getirir
    /// </summary>
    [HttpGet("active-count")]
    public async Task<ActionResult<GetMyActiveSessionCountResponse>> GetActiveSessionCount([FromQuery] GetActiveSessionCountQuery query)
    {
        GetMyActiveSessionCountResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Aktif oturumlar� listeler
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<GetListResponse<GetListUserSessionListItemDto>>> GetActiveSessions([FromQuery] GetActiveSessionsQuery query)
    {
        GetListResponse<GetListUserSessionListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Kullan�c�n�n aktif oturum say�s�n� getirir
    /// </summary>
    [HttpGet("my-active-count")]
    public async Task<ActionResult<GetMyActiveSessionCountResponse>> GetMyActiveSessionCount()
    {
        GetMyActiveSessionCountQuery query = new() { UserId = getUserIdFromRequest() };
        GetMyActiveSessionCountResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Kullan�c�n�n kendi oturumlar�n� listeler
    /// </summary>
    [HttpGet("my-sessions")]
    public async Task<ActionResult<GetListResponse<GetListUserSessionListItemDto>>> GetMySessions()
    {
        GetMySessionsQuery query = new() { UserId = getUserIdFromRequest() };
        GetListResponse<GetListUserSessionListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// Belirtilen kullan�c�n�n oturumlar�n� listeler
    /// </summary>
    [HttpGet("user-sessions/{userId}")]
    public async Task<ActionResult<GetListResponse<GetListUserSessionListItemDto>>> GetUserSessions([FromRoute] Guid userId)
    {
        GetUserSessionsQuery query = new() { UserId = userId };
        GetListResponse<GetListUserSessionListItemDto> response = await Mediator.Send(query);
        return Ok(response);
    }

    #endregion
}
