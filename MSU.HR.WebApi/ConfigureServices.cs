﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;
using System.Text;
using System.Text.Json.Serialization;

namespace MSU.HR.WebApi
{
    public static class ConfigureServices
    {
        public static IConfiguration? Configuration { get; }
        public static IServiceCollection AddWebAPIServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    //var message = context

                    var result = new ErrorModel()
                    {
                        IsSuccess = false,
                        ErrorCode = 400,
                        Message = "Bad Request",
                        Data = context.ModelState.Values.SelectMany(x => x.Errors.Select(p => p.ErrorMessage)).ToList()
                    };
                    return new BadRequestObjectResult(result);
                };
            });

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Payroll AppService API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddIdentityCore<AspNetUser>(options =>
                    {
                        options.SignIn.RequireConfirmedAccount = false;
                        options.User.RequireUniqueEmail = true;
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 6;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireLowercase = false;
                    }).AddEntityFrameworkStores<DatabaseContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "apiWithAuthBackend",
                        ValidAudience = "apiWithAuthBackend",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes("*&^!@#$%@!SomethingSecret!@%$#@!^&*")
                        ),
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = async (context) =>
                        {
                            Console.WriteLine("Printing in the delegate OnAuthFailed");
                        },
                        OnChallenge = async (context) =>
                        {
                            Console.WriteLine("Printing in the delegate OnChallenge");

                            // this is a default method
                            // the response statusCode and headers are set here
                            context.HandleResponse();

                            // AuthenticateFailure property contains 
                            // the details about why the authentication has failed
                            if (context.AuthenticateFailure != null)
                            {
                                context.Response.StatusCode = 401;

                                ErrorModel response = new ErrorModel()
                                {
                                    IsSuccess = false,
                                    ErrorCode = 401,
                                    Message = "Token Validation Has Failed. Request Access Denied"

                                };
                                // we can write our own custom response content here
                                //await context.HttpContext.Response.WriteAsync("Token Validation Has Failed. Request Access Denied");
                                await context.Response.WriteAsJsonAsync(response);
                            }
                        }
                    };
                });

            services.AddDbContext<DatabaseContext>(options =>
            {
            });

            services.Configure<SecurityStampValidatorOptions>(options =>
            {
                // enables immediate logout, after updating the user's stat.
                options.ValidationInterval = TimeSpan.Zero;
            });

            services.AddHttpContextAccessor();
            services.AddInterfaceServices(configuration);

            return services;
        }
    }
}
