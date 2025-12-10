using Serilog;
using Serilog.Exceptions;
using ProductsMicroService.API.Middleware;
using ProductsMicroService.API.Filter;
using BusinessLogicLayer;
using DataAccessLayer;
using BusinessLogicLayer.Validators;
using FluentValidation;
using ProductsMicroService.API.APIEndpoints;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


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

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});



//Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});



var app = builder.Build();


// ------------ Middleware Pipeline ----------
// logs every API call
app.UseExceptionHandlingMiddleware(); // custom global exception handler
app.UseRouting();

//CORS
app.UseCors();

//Swagger
app.UseSwagger();
app.UseSwaggerUI();


//Auth
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ------------ Endpoints --------------------
app.MapControllers();
app.MapProductAPIEndpoints();

app.Run();
