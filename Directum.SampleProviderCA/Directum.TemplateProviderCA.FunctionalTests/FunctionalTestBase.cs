using System.Net;
using Directum.Core.UniversalProvider.WebApiClient.Certificates;
using Directum.Core.UniversalProvider.WebApiClient.Sign;
using Directum.Core.UniversalProvider.WebApiModels;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;

namespace Directum.Core.UniversalProvider.FunctionalTests;

public class FunctionalTestBase
{
  protected const string ServiceToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE3MTc0MjM3OTcsImV4cCI6MjAxNzQyNDY5NywiaXNzIjoiSHJQcm9JZCIsImF1ZCI6IlNhbXBsZVByb3ZpZGVyQ0EifQ.erZsKN2CS4BXDNUXJX-AB7pm1f1H8YKm0vcA4Pol4ZU";

  protected ICertificateIssueClient _certificateIssueClient;
  protected ISignClient _signClient;

  #region SetUp

  [OneTimeSetUp]
  public async Task SetUp()
  {
    // Поднятие тествого хоста.
    await TestHostUp();

    // Подключение к реальному сервису.
    //RealHost("http://localhost:8825");
  }

  public void RealHost(string address)
  {
    var refitSettings = new RefitSettings
    {
      AuthorizationHeaderValueGetter = async (request, ct) => ServiceToken,
    };
    CreateRefitClients(address, refitSettings);
  }

  public async Task TestHostUp()
  {
    var testHost = await new HostBuilder()
     .ConfigureWebHost(webBuilder =>
     {
       webBuilder
           .UseTestServer()
           .UseStartup<Startup>()
           .ConfigureAppConfiguration(configureBuilder =>
           {
             configureBuilder.Add(new JsonStreamConfigurationSource() { Stream = File.OpenRead("./appsettings.json") });
             configureBuilder.Add(new JsonStreamConfigurationSource() { Stream = File.OpenRead("./appsettings.Development.json") });
           });
     })
     .StartAsync();

    var testServer = testHost.GetTestServer();

    var address = testServer.CreateClient().BaseAddress.ToString();
    var handler = testServer.CreateHandler();

    var refitSettings = new RefitSettings
    {
      AuthorizationHeaderValueGetter = async (request, ct) => ServiceToken,
      HttpMessageHandlerFactory = () => handler,
    };

    CreateRefitClients(address, refitSettings);
  }

  public void CreateRefitClients(string address, RefitSettings refitSettings)
  {
    _certificateIssueClient = RestService.For<ICertificateIssueClient>(address, refitSettings);
    _signClient = RestService.For<ISignClient>(address, refitSettings);
  }

  #endregion

  public async Task AssertApiException(Func<Task> action, HttpStatusCode expectedHttpStatus, string expectedErrorCode, string expectedErrorMessageWildcard = null)
  {
    try
    {
      await action();
    }
    catch (ApiException ex)
    {
      var errorResponse = JsonConvert.DeserializeObject<Error>(ex.Content);

      using (new AssertionScope())
      {
        ex.StatusCode.Should()
          .Be(expectedHttpStatus);

        errorResponse.Code.Should()
          .BeEquivalentTo(expectedErrorCode);

        if (expectedErrorMessageWildcard != null)
        {
          errorResponse.Message.Should()
            .Match(expectedErrorMessageWildcard);
        }
      }
    }
  }
}
