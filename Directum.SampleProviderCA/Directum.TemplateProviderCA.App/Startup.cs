using System.Reflection;
using System.Text.Json.Serialization;

/// <summary>
/// Класс, определяющий процедуру запуска приложения.
/// </summary>
public class Startup
{
  /// <summary>
  /// Версия сервиса.
  /// </summary>
  public string ServiceVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

  /// <summary>
  /// Конфигурация приложения.
  /// </summary>
  private IConfiguration Configuration { get; }

  /// <summary>
  /// Инициализирует класс <see cref="Startup"/>.
  /// </summary>
  /// <param name="configuration">Конфигурация приложения.</param>
  public Startup(IConfiguration configuration)
  {
    Configuration = configuration;
  }

  /// <summary>
  /// Настраивает сервисы приложения.
  /// </summary>
  /// <param name="services">Сервисы приложения.</param>
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHealthChecks();

    services
     .AddControllers()
     .AddJsonOptions(options =>
     {
       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

    services.AddAuthentication(Configuration);

    services.AddSwagger(Configuration);
  }

  /// <summary>
  /// Настраивает конфигурацию приложения.
  /// </summary>
  /// <param name="app">Сборщик приложения.</param>
  /// <param name="env">Окружение хоста приложения.</param>
  public void Configure(
      IApplicationBuilder app,
      IWebHostEnvironment env)
  {
    if (env.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
      endpoints.MapHealthCheck();
      endpoints.MapControllers();
      endpoints.MapGet("/version", async context =>
      {
        await context.Response.WriteAsJsonAsync(new { serviceVersion = ServiceVersion });
      });
    });
  }
}
