using Application.Features.Logs.Commands.Create;
using Application.Features.Logs.Commands.Delete;
using Application.Features.Logs.Commands.Update;
using Application.Features.Logs.Queries.GetById;
using Application.Features.Logs.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedLogResponse>> Add([FromBody] CreateLogCommand command)
    {
        CreatedLogResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedLogResponse>> Update([FromBody] UpdateLogCommand command)
    {
        UpdatedLogResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedLogResponse>> Delete([FromRoute] Guid id)
    {
        DeleteLogCommand command = new() { Id = id };

        DeletedLogResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdLogResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdLogQuery query = new() { Id = id };

        GetByIdLogResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListLogListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListLogQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListLogListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}