using Serilog;
using Serilog.Exceptions;
using ProductsMicroService.API.Middleware;
using ProductsMicroService.API.Filter;
using BusinessLogicLayer;
using DataAccessLayer;
using BusinessLogicLayer.Validators;
using FluentValidation;
using ProductsMicroService.API.APIEndpoints;

var builder = WebApplication.CreateBuilder(args);

// ------------ Serilog Logging ------------
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithExceptionDetails()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ------------ DAL + BLL -------------------
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

// ------------ Controllers + Validation -----
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>(); // global validation filter
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;   // optional: keeps PascalCase response
});

// Register validators automatically
builder.Services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();

// Enables automatic FluentValidation execution (important!)


var app = builder.Build();

// ------------ Middleware Pipeline ----------
app.UseSerilogRequestLogging();       // logs every API call
app.UseExceptionHandlingMiddleware(); // custom global exception handler

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ------------ Endpoints --------------------
app.MapControllers();
app.MapProductAPIEndpoints();

app.Run();
