using System.Net;
using Directum.Core.UniversalProvider.WebApiModels;

namespace Directum.TemplateProviderCA.App.Exceptions;

/// <summary>
/// Базовый класс исключения для штатных ошибок домена.
/// </summary>
public class DomainException : Exception
{
  /// <summary>
  /// HTTP статус код ошибки.
  /// </summary>
  public HttpStatusCode HttpStatusCode { get; set; }

  /// <summary>
  /// Код ошибки.
  /// </summary>
  public string Code { get; set; }

  /// <summary>
  /// Дополнительные данные.
  /// </summary>
  public IEnumerable<Error> Details { get; set; }

  /// <summary>
  /// Инициализирует класс <see cref="DomainException"/>.
  /// </summary>
  /// <param name="message">Сообщение об ошибке.</param>
  /// <param name="code">Код ошибки.</param>
  /// <param name="innerException">Внутренняя ошибка.</param>
  public DomainException(string message, string code, HttpStatusCode httpStatusCode, List<Error> details = null, Exception innerException = null)
    : base(message, innerException)
  {
    Code = code;
    HttpStatusCode = httpStatusCode;
    Details = details;
  }
}
