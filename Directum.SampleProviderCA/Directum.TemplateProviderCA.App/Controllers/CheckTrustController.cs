using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Контроллер для проверки доверия.
/// </summary>
[ApiController]
public class CheckTrustController : ControllerBase
{
  /// <summary>
  /// Авторизованный метод для проверки на доверие.
  /// </summary>
  /// <returns>ОК, если проверка на доверие прошла.</returns>
  [HttpGet("/checktrust")]
  [Authorize]
  public ActionResult CheckTrust()
  {
    return Ok();
  }
}
