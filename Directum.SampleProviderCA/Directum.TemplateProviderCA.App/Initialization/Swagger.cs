using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

/// <summary>
/// Методы расширения IoC-контейнера для Swagger.
/// </summary>
internal static class Swagger
{
  /// <summary>
  /// Добавить Swagger в IoC-контейнер.
  /// </summary>
  /// <param name="services">IoC-контейнер.</param>
  /// <param name="configuration">Конфигурация приложения.</param>
  /// <returns>Тот же IoC-контейнер, что и на входе.</returns>
  internal static IServiceCollection AddSwagger(
    this IServiceCollection services,
    IConfiguration configuration)
  {
    var authenticationSettings = configuration
        .GetSection(key: "Authentication")
        .Get<AuthenticationSettings>();

    services.AddSwaggerGen(c =>
    {
      var authenticationSettings = configuration
        .GetSection(key: "Authentication")
        .Get<AuthenticationSettings>();

      c.SwaggerDoc(
        name: "v1",
        info: new OpenApiInfo
        {
          Title = "Universal Provider",
          Version = "1.0",
          Description = "Web API Universal Provider.",
        });

      c.EnableAnnotations();

      var trustedIssuers = authenticationSettings.
        TrustedIssuers.
        Select(trustedIssuer => trustedIssuer.Issuer).
        Distinct();

      c.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,
        new OpenApiSecurityScheme
        {
          Description = $@"Авторизация с помощью Jwt токена с использованием схемы Bearer.<br/>
              Необходимо указать Jwt токен с указанием схемы **'Bearer <токен>'**.<br/>
              Пример: `Bearer eyJhbGciOiJIUzI1NiJ9.e30.ZRrHA1JJJW8opsbCGfG_HACGpVUMN_a9IV7pAx_Zmeo`<br/>
              В составе токена обязательны claims:<br/>
                - iss: '**{string.Join("**', '**", trustedIssuers)}**'<br/>
                - aud: '**{authenticationSettings.Audience}**'",
          Name = HeaderNames.Authorization,
          In = ParameterLocation.Header,
          Type = SecuritySchemeType.ApiKey,
          Scheme = JwtBearerDefaults.AuthenticationScheme,
          BearerFormat = JwtConstants.TokenType,
        });

      c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme {
            Reference =
              new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = JwtBearerDefaults.AuthenticationScheme,
              },
            Scheme = "oauth2",
            Name = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header,
          },
          new List<string>()
        },
      });

      // Сбор XML документации для Swagger
      var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
      foreach (var xmlFile in xmlFiles)
      {
        c.IncludeXmlComments(xmlFile);
      }
    });

    return services;
  }
}
