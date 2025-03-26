#region Serilog

using System.Reflection;
using System.Text;
using AtividadeBimestral.Repository;
using AtividadeBimestral;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using AtividadeBimestral.Service;


var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
Directory.CreateDirectory(logFolder);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(new CompactJsonFormatter(),
           Path.Combine(logFolder, ".json"),
            retainedFileCountLimit: 1,
            rollingInterval: RollingInterval.Month)
    .WriteTo.File(Path.Combine(logFolder, ".log"),
            retainedFileCountLimit: 1,
            rollingInterval: RollingInterval.Day)
    .CreateLogger();

#endregion


try
{

    var builder = WebApplication.CreateBuilder(args);

    #region Lendo as configurações do projeto

    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    string pathAppSettings = "appsettings.json";

    if (env == "Development")
    {
        pathAppSettings = "appsettings.Development.json";
    }

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(pathAppSettings)
        .Build();

    var appSettings = config.Get<AppSettings>();

    //Registra a instância como Singleton
    builder.Services.AddSingleton(appSettings);

    #endregion

    //***Adicionar o Middleware do Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Gerenciamento da API...",
            Version = "v1",
            Description = $@"<h3>Título <b>da API</b></h3>
                            <p>
                                Alguma descrição....
                            </p>",
            Contact = new OpenApiContact
            {
                Name = "Suporte Unoeste",
                Email = string.Empty,
                Url = new Uri("https://www.unoeste.br"),
            },
        });

        // Set the comments path for the Swagger JSON and UI.
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    }); ;


    //Habilitar o uso do serilog.
    builder.Host.UseSerilog();






    builder.Services.AddHttpContextAccessor();


    // Add services to the container.

    builder.Services.AddControllers();


    builder.Services.AddScoped<CartaoService>();
    builder.Services.AddScoped<CartaoRepositorio>();
    builder.Services.AddScoped<TransacaoService>();
    builder.Services.AddScoped<TransacaoRepositorio>();
    builder.Services.AddScoped<MySqlDbContext>();




    var app = builder.Build();

    // *** Usa o Middleware do Swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        c.RoutePrefix = ""; //habilitar a página inicial da API ser a doc.
        c.DocumentTitle = "Gerenciamento de Produtos - API V1";
    });


    // *** Usa o Middleware de autenticação e autorização
    app.UseAuthorization();
    app.UseAuthentication();

    app.MapControllers();

    //int.Parse("fatallll");
    app.Run();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Erro fatal na aplicação");
}