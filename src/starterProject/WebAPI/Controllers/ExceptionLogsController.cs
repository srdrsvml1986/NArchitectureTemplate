using Application.Features.ExceptionLogs.Commands.Create;
using Application.Features.ExceptionLogs.Commands.Delete;
using Application.Features.ExceptionLogs.Commands.Update;
using Application.Features.ExceptionLogs.Queries.GetById;
using Application.Features.ExceptionLogs.Queries.GetList;
using NArchitectureTemplate.Core.Application.Requests;
using NArchitectureTemplate.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExceptionLogsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedExceptionLogResponse>> Add([FromBody] CreateExceptionLogCommand command)
    {
        CreatedExceptionLogResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedExceptionLogResponse>> Update([FromBody] UpdateExceptionLogCommand command)
    {
        UpdatedExceptionLogResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedExceptionLogResponse>> Delete([FromRoute] Guid id)
    {
        DeleteExceptionLogCommand command = new() { Id = id };

        DeletedExceptionLogResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdExceptionLogResponse>> GetById([FromRoute] Guid id)
    {
        GetByIdExceptionLogQuery query = new() { Id = id };

        GetByIdExceptionLogResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListExceptionLogListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListExceptionLogQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListExceptionLogListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}