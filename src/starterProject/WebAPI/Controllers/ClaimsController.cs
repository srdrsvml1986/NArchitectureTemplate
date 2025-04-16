using Application.Features.Claims.Commands.Create;
using Application.Features.Claims.Commands.Delete;
using Application.Features.Claims.Commands.Update;
using Application.Features.Claims.Queries.GetById;
using Application.Features.Claims.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

/// <summary>
/// Controller for managing claims.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ClaimsController : BaseController
{
    /// <summary>
    /// Belirtilen Id'ye sahip yetki bilgisini getirir.
    /// </summary>
    /// <param name="getByIdOperationClaimQuery">yetki bilgilerini getirmek için kullanılan sorgu.</param>
    /// <returns>yetki bilgilerini içeren yanıt nesnesi.</returns>
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetById([FromRoute] GetByIdClaimQuery getByIdOperationClaimQuery)
    {
        GetByIdClaimResponse result = await Mediator.Send(getByIdOperationClaimQuery);
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
        GetListClaimQuery getListClaimQuery = new() { PageRequest = pageRequest };
        GetListResponse<GetListClaimListItemDto> result = await Mediator.Send(getListClaimQuery);
        return Ok(result);
    }

    /// <summary>
    /// Yeni bir yetki oluşturur.
    /// </summary>
    /// <param name="createClaimCommand"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateClaimCommand createClaimCommand)
    {
        CreatedClaimResponse result = await Mediator.Send(createClaimCommand);
        return Created(uri: "", result);
    }

    /// <summary>
    /// Güncellenmiş yetki bilgilerini içeren bir yanıt döndürür.
    /// </summary>
    /// <param name="updateClaimCommand"></param>
    /// <returns></returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateClaimCommand updateClaimCommand)
    {
        UpdatedClaimResponse result = await Mediator.Send(updateClaimCommand);
        return Ok(result);
    }

    /// <summary>
    /// Silinmiş yetki bilgilerini içeren bir yanıt döndürür.
    /// </summary>
    /// <param name="deleteClaimCommand"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteClaimCommand deleteClaimCommand)
    {
        DeletedClaimResponse result = await Mediator.Send(deleteClaimCommand);
        return Ok(result);
    }
}
