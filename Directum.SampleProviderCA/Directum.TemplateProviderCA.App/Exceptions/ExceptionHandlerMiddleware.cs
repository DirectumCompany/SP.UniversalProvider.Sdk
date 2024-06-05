using Directum.Core.UniversalProvider.WebApiModels;

namespace Directum.TemplateProviderCA.App.Exceptions;

public class ExceptionHandlerMiddleware
{
  private readonly RequestDelegate _next;

  /// <summary>
  /// Инициализирует класс <see cref="ExceptionHandlerMiddleware"/>.
  /// </summary>
  /// <param name="next">Делегат вызова следующего Middleware.</param>
  public ExceptionHandlerMiddleware(
    RequestDelegate next)
  {
    _next = next;
  }

  /// <summary>
  /// Вызов Middleware.
  /// </summary>
  /// <param name="httpContext">Http контекст запроса.</param>
  public async Task InvokeAsync(HttpContext httpContext)
  {
    try
    {
      await _next(httpContext);
    }
    catch (Exception exception)
    {
      httpContext.Response.ContentType = "application/json";
      httpContext.Response.StatusCode = exception switch
      {
        DomainException domainException => (int) domainException.HttpStatusCode,

        _ => StatusCodes.Status500InternalServerError,
      };

      var errorModel = exception switch
      {
        DomainException domainException => new Error
        {
          Code = domainException.Code,
          Message = domainException.Message,
          Details = domainException.Details,
        },

        _ => new Error
        {
          Code = "InternalServerError",
          Message = "Произошла неожиданная ошибка сервиса. Пожалуйста, обратитесь к администратору.",
        },
      };

      await httpContext.Response.WriteAsJsonAsync(errorModel);
    }
  }
}
