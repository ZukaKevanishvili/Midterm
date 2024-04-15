using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reddit;
using Reddit.Mapper;
using System.Text.Json.Serialization;
using Reddit.Filters;
using Reddit.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; 
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


builder.Services.AddControllers(options => options.Filters.Add<ModelValidationActionFilter>());






// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





builder.Services.AddDbContext<ApplcationDBContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDb")));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
               builder => builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader());
});
builder.Services.AddSingleton<IMapper, Mapper>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}




app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Type", "application/json");
    await next();
});


app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapGet("/throws", (context) => throw new Exception("your fault"));




app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
