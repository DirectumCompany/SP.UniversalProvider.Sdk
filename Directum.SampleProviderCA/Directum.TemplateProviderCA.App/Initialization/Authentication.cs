using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Методы расширения IoC-контейнера для Authentication.
/// </summary>
internal static class Authentication
{
  /// <summary>
  /// Добавляет аутентификацию.
  /// </summary>
  /// <param name="services">IoC-контейнер.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  /// <returns>Тот же IoC-контейнер, что и на входе.</returns>
  internal static IServiceCollection AddAuthentication(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var authSettings = configuration.GetSection(key: "Authentication").Get<AuthenticationSettings>();

    services
      .AddAuthentication()
      .AddTrustedIssuers(authSettings.Audience, authSettings.TrustedIssuers);

    services.AddAuthorization(options =>
    {
      options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes(authSettings.TrustedIssuers
          .Select((issuer, i) => GetAuthenticationSchemeName(issuer.Issuer, i + 1))
          .ToArray())
        .Build();
    });

    return services;
  }

  /// <summary>
  /// Добавляет доверенных издателей в настройки аутентификации приложения.
  /// </summary>
  /// <param name="builder">Конфигуратор аутентификации.</param>
  /// <param name="audience">Потребитель токена.</param>
  /// <param name="trustedIssuers">Настройки доверенных издателей токенов.</param>
  /// <returns>Конфигуратор аутентификации.</returns>
  private static AuthenticationBuilder AddTrustedIssuers(
    this AuthenticationBuilder builder,
    string audience,
    IEnumerable<TrustedIssuerSettings> trustedIssuers)
  {
    var i = 1;

    foreach (var trustedIssuer in trustedIssuers)
    {
      var securityKey = GetSecurityKey(trustedIssuer);

      builder.AddJwtBearer(
        GetAuthenticationSchemeName(trustedIssuer.Issuer, i),
        options =>
        {
          options.RequireHttpsMetadata = false;
          options.TokenValidationParameters =
            new TokenValidationParameters
            {
              ValidateIssuer = true,
              ValidIssuer = trustedIssuer.Issuer,
              ValidateAudience = true,
              ValidAudience = audience,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = securityKey,
              SaveSigninToken = true,
            };
        });

      i++;
    }

    return builder;
  }

  /// <summary>
  /// Возвращает имя схемы аутентификации.
  /// </summary>
  /// <param name="issuer">Издатель.</param>
  /// <param name="index">Порядковый номер схемы.</param>
  /// <returns>Имя схемы аутентификации.</returns>
  private static string GetAuthenticationSchemeName(string issuer, int index)
  {
    return $"Authentication scheme {index} (issuer: {issuer})";
  }

  /// <summary>
  /// Возвращает ключ безопасности на основе сертификата.
  /// </summary>
  /// <param name="trustedIssuerSettings">Настройки доверенного издателя токенов.</param>
  /// <returns>Ключ безопасности.</returns>
  private static SecurityKey GetSecurityKey(TrustedIssuerSettings trustedIssuerSettings)
  {
    if (!string.IsNullOrEmpty(trustedIssuerSettings.SigningCertificatePath))
    {
      var certificate = new X509Certificate2(trustedIssuerSettings.SigningCertificatePath);

      return new X509SecurityKey(certificate);
    }

    if (string.IsNullOrEmpty(trustedIssuerSettings.SigningCertificateThumbprint))
    {
      return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(trustedIssuerSettings.EncryptionKey));
    }

    var storeLocation = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
      ? StoreLocation.CurrentUser
      : StoreLocation.LocalMachine;

    using var store = new X509Store(StoreName.My, storeLocation);
    store.Open(OpenFlags.ReadOnly);

    var certificates = store.Certificates.Find(
      X509FindType.FindByThumbprint,
      trustedIssuerSettings.SigningCertificateThumbprint,
      validOnly: true);

    if (certificates.Count == 0)
    {
      throw new InvalidOperationException(
        message: $"Сертификат с отпечатком '{trustedIssuerSettings.SigningCertificateThumbprint}' не найден среди " +
        (storeLocation == StoreLocation.LocalMachine
          ? "сертификатов локального компьютера."
          : "пользовательских сертификатов."));
    }

    store.Close();

    return new X509SecurityKey(certificates[0]);
  }
}
