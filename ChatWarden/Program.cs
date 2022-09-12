using ChatWarden.CoreLib.Extentions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();


var tnt = Environment.GetEnvironmentVariable("TARANTOOL_CNNSTR");
var token = Environment.GetEnvironmentVariable("TOKEN");

if (!string.IsNullOrEmpty(tnt) && !string.IsNullOrEmpty(token))
{
    services.AddHandler(tnt, token);
}




var app = builder.Build();

app.MapControllers();

app.Run();
