using StudentWebAPIProject.Logging;
using Serilog;
using StudentWebAPIProject.DBSets;
using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Configurations;
using StudentWebAPIProject.DBSets.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using StudentWebAPIProject.Services;

var builder = WebApplication.CreateBuilder(args);

#region In-Build Logging
//Clearing all inbuit logging
builder.Logging.ClearProviders();
//add only inbuild debug and console logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
#endregion

#region Log4Net
//builder.Logging.AddLog4Net();
#endregion

#region Serilog
//Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Log/log.txt", rollingInterval : RollingInterval.Hour)
    .CreateLogger();

//to enable only Serilog
//builder.Host.UseSerilog();

//to add serilog along with in build logging
//builder.Logging.AddSerilog();
#endregion

#region DBConnection
builder.Services.AddDbContext<CollegeDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeDBConnection")));
#endregion

// Add services to the container.
/*
options in AddControllers = to return 406 Not Acceptable error for other formats asked in request headers
AddXmlDataContractSerializerFormatters = to accept xml format to be returned
 */
builder.Services.AddControllers(options => options.ReturnHttpNotAcceptable = false).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//To add Automapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//Dependency Injection
//Creates 1 object per request ['n' object instance for 'n' requests]
builder.Services.AddScoped<StudentWebAPIProject.Logging.ILogger, LogToDB>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped(typeof(ICollegeRepository<>), typeof(CollegeRepository<>));
builder.Services.AddScoped<IUserService, UserService>();

/*
Dependency Injection
Creates 1 object thoughtout the lifetime ['1' object instance for 'n' requests]
*/
//builder.Services.AddSingleton<StudentWebAPIProject.Logging.ILogger, LogToFile>();

/*
Dependency Injection
Creates 1 object per service call ['n*n' object instance for 'n' service calls & 'n' requests]
*/
//builder.Services.AddTransient<StudentWebAPIProject.Logging.ILogger, LogToMemory>();

#region Adding CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        //Allow All origins, methods and headers
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
    options.AddPolicy("AllowAll", builder =>
    {
        //Allow All origins, methods and headers
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
    options.AddPolicy("AllowFewMethods", builder =>
    {
        //Allow only specific methods
        builder.WithMethods("GET", "POST", "PUT", "DELETE");
    });
    options.AddPolicy("AllowLocalHost", builder =>
    {
        //Allow only specific origins
        builder.WithOrigins("https://localhost:5000", "https://localhost:5001");
    });
});
#endregion

#region Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"]))
    };
}).AddJwtBearer("PolicyForLocalUsers", options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["LocalIssuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["LocalAudience"],

        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKeyForLocalUsers"]))
    };
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Add before Cors
app.UseRouting();

//Use CORS (Add after Routing and before Authorization)
app.UseCors();  //Use default policy instead of named policy

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapControllers()
    //.RequireCors("AllowAll");

    endpoints.MapGet("api/TestEndpoint", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    }).RequireCors("AllowLocalHost");

    endpoints.MapGet("api/TestEndpoint2", async context =>
    {
        await context.Response.WriteAsync("Hello World 2!");
    }); //Uses default CORS policy
});

//mapping controller inside endpoints
app.MapControllers().RequireCors("AllowAll");

app.Run();
