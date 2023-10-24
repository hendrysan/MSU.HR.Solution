using Microsoft.IdentityModel.Logging;
using MSU.HR.WebApi;
using MSU.HR.WebApi.Extensions;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddWebAPIServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
//var host = builder.Build();
//_ = new JwtPayload();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureSwagger v1"));
//}

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

