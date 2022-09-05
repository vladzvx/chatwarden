using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();


var tnt = Environment.GetEnvironmentVariable("TARANTOOL_CNNSTR");
if (!string.IsNullOrEmpty(tnt))
{
    var box = new Box(new ClientOptions(tnt));
    box.Connect().Wait();
    services.AddSingleton(box);
}




var app = builder.Build();

app.MapControllers();

app.Run();
