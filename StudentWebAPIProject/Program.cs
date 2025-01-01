using StudentWebAPIProject.Logging;
using Serilog;
using StudentWebAPIProject.DBSets;
using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Configurations;

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
builder.Services.AddSwaggerGen();

//To add Automapper configuration
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

//Dependency Injection
//Creates 1 object per request ['n' object instance for 'n' requests]
builder.Services.AddScoped<StudentWebAPIProject.Logging.ILogger, LogToDB>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
