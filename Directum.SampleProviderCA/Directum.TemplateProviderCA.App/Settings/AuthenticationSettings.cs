/// <summary>
/// Настройки аутентификации приложения.
/// </summary>
public class AuthenticationSettings
{
  /// <summary>
  /// Потребитель токена.
  /// </summary>
  public string Audience { get; set; }

  /// <summary>
  /// Доверенные издатели.
  /// </summary>
  public TrustedIssuerSettings[] TrustedIssuers { get; set; }
}
