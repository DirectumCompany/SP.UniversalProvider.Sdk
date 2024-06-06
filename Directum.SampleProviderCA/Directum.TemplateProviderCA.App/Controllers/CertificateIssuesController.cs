using System.Net.Mime;
using Directum.Core.UniversalProvider.WebApiModels.Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер для работы с выпуском сертификата.
/// </summary>
[Authorize]
[ApiController]
[Route("certificate-issues")]
public class CertificateIssuesController : ControllerBase
{
  /// <summary>
  /// Создает запрос на выпуск сертификата пользователю.
  /// </summary>
  /// <param name="request">Запрос на выпуск сертификата.</param>
  /// <returns>Ответ на запрос выпуска сертификата.</returns>
  [HttpPost("create")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<CertificateIssueResponse>> Create(
    [FromBody] CertificateIssueRequest request)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Возвращает статус запроса на выпуск сертификата.
  /// </summary>
  /// <param name="requestId">Идентификатор запроса на выпуск сертификата.</param>
  /// <returns>Ответ на запрос выпуска сертификата.</returns>
  [HttpGet("{requestId:int}/status")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CertificateIssueResponse>> GetStatus(
    int requestId)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Возвращает заявление на выпуск сертификата. 
  /// </summary>
  /// <param name="requestId">Идентификатор запроса на выпуск сертификата.</param>
  /// <returns>Заявление на выпуск сертификата.</returns>
  [HttpGet("{requestId:int}/statement")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<HttpContent>> GetStatement(
    int requestId)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Создает запрос подтверждения на выпуск сертификата.
  /// </summary>
  /// <param name="requestId">Идентификатор запроса на выпуск сертификата.</param>
  [Authorize]
  [HttpPost("{requestId:int}/confirmation-request")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult> CreateConfirmationRequest(
    int requestId)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Подтверждает выпуск сертификата пользователю.
  /// </summary>
  /// <param name="requestId">Идентификатор запроса на выпуск сертификата.</param>
  /// <param name="confirmationCode">Код подтверждения.</param>
  [HttpPost("{requestId:int}/confirm")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Confirm(int requestId, [FromQuery] string confirmationCode = null)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Отменяет процесс выпуска сертификата.
  /// </summary>
  /// <param name="requestId">
  /// Идентификатор запроса на выпуск сертификата.
  /// </param>
  [HttpPost("{requestId:int}/cancel")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Cancel(
    [FromRoute] int requestId)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }
}
