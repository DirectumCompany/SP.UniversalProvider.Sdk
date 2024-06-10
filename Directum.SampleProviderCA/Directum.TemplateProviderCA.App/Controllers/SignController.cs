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
        OperationId = "1",
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
              Data = "Link",
            },
            new ConfirmationData
            {
              Type = ConfirmationDataType.QrCode,
              Data = "QrCode",
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
            Signature = "Signature1",
          },
          new()
          {
            DocumentName = "DocumentName2",
            Signature = "Signature1",
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
