using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Набор методов-расширений для проверки работоспособности сервиса.
/// </summary>
public static class HealthCheck
{
  /// <summary>
  /// Инициализация конечной точки проверки работоспособности сервиса.
  /// <param name="endpoints">Контракт определения конечной точки.</param>
  /// </summary>
  public static IEndpointRouteBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints)
  {
    endpoints.MapHealthChecks("/health", new HealthCheckOptions()
    {
      AllowCachingResponses = false,
      Predicate = (check) => check.Tags.Contains("SYSTEM"),
      ResultStatusCodes =
      {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status500InternalServerError,
      },
      ResponseWriter = WriteResponse,
    });

    return endpoints;
  }

  /// <summary>
  /// Записывает результат работы проверки работоспособности сервиса.
  /// </summary>
  /// <param name="context">HTTP контекст.</param>
  /// <param name="result">Результат работы проверки работоспособности сервиса.</param>
  /// <returns>Представление асинхронной операции записи.</returns>
  private static Task WriteResponse(HttpContext context, HealthReport result)
  {
    return context.Response.WriteAsJsonAsync(result, new JsonSerializerOptions
    {
      Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
      Converters = { new JsonStringEnumConverter() },
      WriteIndented = true,
    });
  }
}
