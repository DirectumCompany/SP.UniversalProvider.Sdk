using System.Net;
using Directum.Core.UniversalProvider.WebApiClient.Certificates;
using Directum.Core.UniversalProvider.WebApiClient.Sign;
using Directum.Core.UniversalProvider.WebApiModels;
using FluentAssertions.Execution;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Refit;

namespace Directum.Core.UniversalProvider.FunctionalTests;

public class FunctionalTestBase
{
  protected const string ServiceToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOlsic2VydmljZSJdLCJuYmYiOjE3MTc0MjM3OTcsImV4cCI6MjAxNzQyNDY5NywiaXNzIjoiSHJQcm9JZCIsImF1ZCI6IlNhbXBsZVByb3ZpZGVyQ0EifQ.BMZDnh3VjSvVoaEw-ita12h1HVxjH1hIkm6JNTZrc7I";

  protected IHost _testHost;
  protected TestServer _testServer;

  protected ICertificateIssueClient _certificateIssueClient;
  protected ISignClient _signClient;

  #region TestHostUp

  [OneTimeSetUp]
  public async Task TestHostUp()
  {
    _testHost = await new HostBuilder()
     .ConfigureWebHost(webBuilder =>
     {
       webBuilder
           .UseTestServer()
           .ConfigureServices(services => // Запускается до Startup
           {
             services.AddHttpContextAccessor();
           })
           .UseStartup<Startup>()
           .ConfigureServices(services => // Запускается после Startup
           {

           })
           .ConfigureAppConfiguration(configureBuilder =>
           {
             configureBuilder.Add(new JsonStreamConfigurationSource() { Stream = File.OpenRead("./appsettings.json") });
             configureBuilder.Add(new JsonStreamConfigurationSource() { Stream = File.OpenRead("./appsettings.Development.json") });
           });
     })
     .StartAsync();

    _testServer = _testHost.GetTestServer();

    var address = _testServer.CreateClient().BaseAddress.ToString();
    var handler = _testServer.CreateHandler();

    var refitSettings = new RefitSettings
    {
      AuthorizationHeaderValueGetter = async (request, ct) => ServiceToken,
      HttpMessageHandlerFactory = () => handler,
    };

    _certificateIssueClient = RestService.For<ICertificateIssueClient>(address, refitSettings);
    _signClient = RestService.For<ISignClient>(address, refitSettings);
  }

  #endregion

  public async Task AssertApiException(Func<Task> action, HttpStatusCode expectedHttpStatus, string expectedErrorCode)
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
      }
    }
  }
}
