using ChatWarden.CoreLib.Extentions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddSwaggerGen(c =>
{
    string dir = AppContext.BaseDirectory;
    var files = Directory.GetFiles(dir).Where(fn => fn.EndsWith(".xml")).ToArray();
    foreach (var file in files)
    {
        c.IncludeXmlComments(file);
    }
    c.UseInlineDefinitionsForEnums();
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ChatWarden", Version = "v1"});
}).AddSwaggerGenNewtonsoftSupport();

var tnt = Environment.GetEnvironmentVariable("TARANTOOL_CNNSTR");
var token = Environment.GetEnvironmentVariable("TOKEN");

if (!string.IsNullOrEmpty(tnt) && !string.IsNullOrEmpty(token))
{
    services.AddHandler(tnt, token);
}

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.ShowCommonExtensions();
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "ChatWarden");
});
app.Run();
