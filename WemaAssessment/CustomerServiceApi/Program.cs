using CustomerServiceApi.Core.Application.Interfaces;
using CustomerServiceApi.Core.Domain;
using CustomerServiceApi.Infrastructure.Implementation;
using CustomerServiceApi.Infrastructure.Persistence;
using CustomerServiceApi.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
try
{
    // Add services to the container.

    Log.Information("starting server.");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();

    builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)));

    builder.Services.AddIdentity<Customer, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = false;           
        options.Password.RequiredLength = 6;             
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();


    builder.Services.AddHttpClient("banks", (serviceProvider, client) =>
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        client.BaseAddress = new Uri(Constants.BANKBASEURL);
    });

    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

    builder.Services.AddTransient<ICustomerService, CustomerService>();
    builder.Services.AddTransient<IOtpService, OtpService>();
    builder.Services.AddTransient<IBankServcie, BankService>();
    builder.Services.AddTransient<IGeolocation, GeolocationService>();

    builder.Services.AddAuthentication();
    builder.Services.AddAuthorization();

    builder.AddServiceDefaults();
 
    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    //app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "server terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
