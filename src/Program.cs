using System.Reflection;
using Asp.Versioning;
using FinanceControl.Application;
using FinanceControl.Infra;
using FinanceControl.Infra.Auth;
using FinanceControl.Infra.HostedServices;
using FinanceControl.Infra.Middlewares;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");

// (builder.Configuration as IConfigurationBuilder).Add(new DatabaseConfigurationSource(connectionString!));

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("bankAccountLogs.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(new Uri(builder.Configuration.GetConnectionString("LogElasticSource")!))
        {
            MinimumLogEventLevel = LogEventLevel.Information,
            IndexFormat = "finance-control-{0:yyyy.MM.dd}"
        })
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApiVersioning(opts =>
{
    opts.DefaultApiVersion = new ApiVersion(1);
    opts.ReportApiVersions = true;
    opts.AssumeDefaultVersionWhenUnspecified = true;
    opts.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("x-version"),
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader()
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = builder.Configuration.GetConnectionString("Redis");
    opts.InstanceName = "FinanceControl";
});

builder.Services.AddScoped<CacheProvider>();
builder.Services.ConfigureAuthorizationAndAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddScoped<IdentityUserService>();
builder.Services.AddScoped<IMessageBroker, MessageBrokerService>();
builder.Services.AddScoped<ExchangeService>();

builder.Services.AddHostedService<CreateAdminUserHostedService>();

builder.Services.AddDbContext<FinanceControlDbContext>(opts =>
    opts.UseSqlServer(connectionString)
);

builder.Services.AddDbContext<IdentityDbContext>(opts =>
    opts.UseSqlServer(connectionString)
);

builder.Services.Configure<ExchangeServiceOptions>(builder.Configuration.GetSection("ExchangeService"));

builder.Services
    .AddHealthChecks()
    .AddSqlServer(connectionString!, tags:["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = registration => !registration.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LogScopedMiddleware>();

app.MapControllers();

app.Services.CreateScope().ServiceProvider.GetRequiredService<FinanceControlDbContext>().Database.Migrate();
app.Services.CreateScope().ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();

app.Run();

public partial class Program
{
}
