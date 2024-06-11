using System.Diagnostics.CodeAnalysis;
using System.Net;
using Directum.Core.UniversalProvider.WebApiModels;
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
    return request.Login switch
    {
      null => BadRequest(new Error
      {
        Message = "Ошибка валидации.",
        Code = "ValidationError",
        Details = new List<Error>
        {
          new()
          {
            Field = "Login",
            Message = "'Login' должно быть заполнено.",
          },
        }
      }),

      _ => Ok(new SigningStatusInfo
      {
        Status = SigningStatus.InProgress,
        OperationId = "ca3a9302-7992-464a-8fc7-0bdf875df00d",
      }),
    };
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
    return 
      TryGetActionResultByOperationId(operationId) ??
      operationId switch
      {
        "SuccessStatus" => Ok(new SigningStatusInfo
        {
          Status = SigningStatus.Success,
          OperationId = operationId,
        }),
        _ => Ok(new SigningStatusInfo
        {
          Status = SigningStatus.NeedConfirm,
          OperationId = operationId,
        }),
      };
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
    return 
      TryGetActionResultByOperationId(operationId) ??
      operationId switch
      {
        _ => Ok(new ConfirmationInfo
        {
          ConfirmationType = ConfirmationType.MobileApp,
          ConfirmationData = new[]
          {
            new ConfirmationData
            {
              Type = ConfirmationDataType.Link,
              Data = "https://placekitten.com/200/300",
            },
            new ConfirmationData
            {
              Type = ConfirmationDataType.QrCode,
              Data = "data:image/png;base64,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",
            },
          },
        }),
      };
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
    return 
      TryGetActionResultByOperationId(operationId) ??
      operationId switch
      {
        "UserNotSignDocumentInExternalApp" => BadRequest(new Error
        {
          Message = "Документы еще не подписаны. Попробуйте позже.",
          Code = "UnconfirmedSigningStatusError"
        }),

        _ => Ok()
      };
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
    return 
      TryGetActionResultByOperationId(operationId) ??
      Ok(new SigningResult[]
        {
          new()
          {
            DocumentName = "DocumentName1",
            Signature = "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",
          },
        });
  }

  /// <summary>
  /// Генерирует ошибку по ключевой строке.
  /// </summary>
  /// <param name="operationId">Переданный идентификатор подписания.</param>
  private ActionResult? TryGetActionResultByOperationId(string operationId)
  {
    return operationId switch
    {
      "OperationIdNotExitst" => NotFound(new Error
      {
        Message = "Поток подписания не найден.",
        Code = "NotFoundError",
      }),

      "UnexpectedServerError" => StatusCode(
        (int)HttpStatusCode.InternalServerError, 
        new Error 
        { 
          Message =  "Произошла неожиданная ошибка сервера, которая не была обработана",
          Code = "InternalServerError",
        }),

      _ => null,
    };
  }
}
