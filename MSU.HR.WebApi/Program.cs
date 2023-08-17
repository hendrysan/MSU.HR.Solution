using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MSU.HR.Contexts;
using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;
using MSU.HR.Services.Interfaces;
using MSU.HR.Services.Repositories;
using MSU.HR.WebApi.Extensions;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
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


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
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

builder.Services
    .AddIdentityCore<AspNetUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<DatabaseContext>();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

builder.Services.AddDbContext<DatabaseContext>(options =>
{
});

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // enables immediate logout, after updating the user's stat.
    options.ValidationInterval = TimeSpan.Zero;
});

builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IResponse, ResponseRepository>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IToken, TokenRepository>();
builder.Services.AddScoped<ILogError, LogErrorRepository>();
builder.Services.AddScoped<ICorporate, CorporateRepository>();
builder.Services.AddScoped<IBank, BankRepository>();
builder.Services.AddScoped<IDepartment, DepartmentRepository>();
builder.Services.AddScoped<IEducation, EducationRepository>();
builder.Services.AddScoped<IGrade, GradeRepository>();
builder.Services.AddScoped<IJob, JobRepository>();
builder.Services.AddScoped<IReason, ReasonRepository>();
builder.Services.AddScoped<ISection, SectionRepository>();
builder.Services.AddScoped<ITypeEmployee, TypeEmployeeRepository>();
builder.Services.AddScoped<IPTKP, PTKPRepository>();
builder.Services.AddScoped<IRole, RoleRepository>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();

builder.Services.AddScoped<ITimeOff, TimeOffRepository>();



var app = builder.Build();

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

