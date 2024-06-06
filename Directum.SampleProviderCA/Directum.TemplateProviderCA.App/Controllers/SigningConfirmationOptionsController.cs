using System.Net.Mime;
using Directum.Core.UniversalProvider.WebApiModels;
using Directum.Core.UniversalProvider.WebApiModels.Sign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер для работы с опциями подтверждения подписания.
/// </summary>
[Authorize]
[ApiController]
[Route("signing-confirmation-options")]
public class SigningConfirmationOptionsController : ControllerBase
{
  /// <summary>
  /// Возвращает опции подтверждения подписания для тенанта.
  /// </summary>
  /// <returns>Опции подтверждения подписания для тенанта.</returns>
  [HttpGet]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public ActionResult<ConfirmationOptionV3> GetAllForTenant()
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Устанавливает опцию подтверждения подписания для тенанта.
  /// </summary>
  /// <param name="confirmationOption">Опция подтверждения подписания.</param>
  [HttpPost]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult> SetForTenant([FromBody] ConfirmationOptionV3 confirmationOption)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Устанавливает опцию подтверждения подписания для пользователя.
  /// </summary>
  /// <param name="login">Логин пользователя.</param>
  /// <param name="confirmationOption">Опция подтверждения операции подписания.</param>
  [HttpPost("{login}")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
  public async Task<ActionResult> SetForUser([FromRoute] string login, [FromBody] ConfirmationOptionV3 confirmationOption)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Возвращает опцию подтверждения подписания установленную по умолчанию для тенанта.
  /// </summary>
  /// <returns>Опция подтверждения подписания установленная по умолчанию для тенанта.</returns>
  [HttpGet("default")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<ActionResult<ConfirmationOptionV3>> GetDefaultForTenant()
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }

  /// <summary>
  /// Возвращает опцию подтверждения подписания установленную по умолчанию для пользователя.
  /// </summary>
  /// <returns>Опция подтверждения подписания установленная по умолчанию для пользователя.</returns>
  [HttpGet("{login}/default")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ConfirmationOptionV3>> GetDefaultForUser([FromRoute] string login)
  {
    return StatusCode(StatusCodes.Status405MethodNotAllowed);
  }
}
