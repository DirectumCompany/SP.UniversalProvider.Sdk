using System.Diagnostics.CodeAnalysis;
using Directum.Core.UniversalProvider.WebApiModels.Sign;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер для работы с подписанием.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class SignController : ControllerBase
{
  /// <summary>
  /// Начинает процесс подписания пакета документов.
  /// </summary>
  /// <param name="acceptLanguage">Язык и локаль пользователя.</param>
  /// <param name="request">Запрос на старт подписания пакета документов.</param>
  /// <returns>Информация о статусе процесса подписания.</returns>
  [HttpPost("start")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public async Task<ActionResult<SigningStatusInfo>> Start(
    [FromBody] SigningRequest request)
  {
    var signingStatusInfo = request.Login switch
    {
      _ => new SigningStatusInfo()
      {
        Status = SigningStatus.InProgress,
        OperationId = "1",
      },
    };

    return Ok(signingStatusInfo);
  }

  /// <summary>
  /// Возвращает статус процесса подписания.
  /// </summary>
  /// <param name="operationId">Идентификатор операции подписания.</param>
  /// <returns>Информация о статусе процесса подписания.</returns>
  [HttpGet("{operationId}/status")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SigningStatusInfo>> GetStatus([FromRoute] string operationId)
  {
    var signingStatusInfo = operationId switch
    {
      _ => new SigningStatusInfo
      {
        Status = SigningStatus.NeedConfirm,
        OperationId = operationId,
      },
    };
    return Ok(signingStatusInfo);
  }

  /// <summary>
  /// Создает запрос на подтверждение подписания.
  /// </summary>
  /// <param name="operationId">Идентификатор операции подписания.</param>
  /// <returns>Ответ на запрос создания подтверждения подписания.</returns>
  [HttpPost("{operationId}/confirmation-request")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<ConfirmationInfo>> CreateConfirmationRequest(
    [FromRoute] string operationId)
  {
    var confirmationInfo = operationId switch
    {
      _ => new ConfirmationInfo
      {
        ConfirmationType = ConfirmationType.MobileApp,
        ConfirmationData = new[]
        {
          new ConfirmationData
          {
            Type = ConfirmationDataType.Link,
            Data = "Link",
          },
          new ConfirmationData
          {
            Type = ConfirmationDataType.QrCode,
            Data = "QrCode",
          },
        },
      }
    };
    return Ok(confirmationInfo);
  }

  /// <summary>
  /// Подтверждает подписание документа.
  /// </summary>
  /// <param name="operationId">Идентификатор операции подписания.</param>
  /// <param name="confirmationCode">Код подтверждения.</param>
  [HttpPost("{operationId}/confirm")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<IActionResult> Confirm(
    [FromRoute] string operationId,
    [FromQuery][MaybeNull] string confirmationCode)
  {
    return Ok();
  }

  /// <summary>
  /// Возвращает готовые подписи.
  /// </summary>
  /// <param name="operationId">Идентификатор операции подписания.</param>
  /// <returns>Список подписей в формате строки base64.</returns>
  [HttpGet("{operationId}/signs")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<SigningResult[]>> GetSigns([FromRoute] string operationId)
  {
    var signs = new SigningResult[]
    {
      new SigningResult
      {
        DocumentName = "DocumentName1",
        Signature = "Signature1",
      },
      new SigningResult
      {
        DocumentName = "DocumentName2",
        Signature = "Signature1",
      },
    };
    return Ok(signs);
  }
}