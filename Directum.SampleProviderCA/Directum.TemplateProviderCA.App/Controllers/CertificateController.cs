using Directum.Core.UniversalProvider.WebApiModels.Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер для работы с сертификатами.
/// </summary>
[Authorize]
[ApiController]
[Route("certificate")]
public class CertificateController : ControllerBase
{
  /// <summary>
  /// Возвращает информацию о сертификате.
  /// </summary>
  /// <param name="id">Идентификатор сертификата.</param>
  /// <returns>Информация о сертификате.</returns>
  [HttpGet("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<CertificateInfo>> GetAsync(int id)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Отзывает сертификат.
  /// </summary>
  /// <param name="revocationRequest">Запрос на отзыв сертификата.</param>
  [HttpPost("revoke")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult> Revoke(RevocationRequest revocationRequest)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }
}
