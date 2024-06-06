
var build = Host.CreateDefaultBuilder()
  .ConfigureWebHostDefaults(webHost => webHost.UseStartup<Startup>())
  .Build();

build.Run();
