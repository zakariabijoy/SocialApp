var builder = WebApplication.CreateBuilder(args);

//add services to the container
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
builder.Services.AddCors();       
builder.Services.AddIndentityServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddDbContext<DataContext>(options =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                    string connString;

                    // Depending on if in development or production, use either Heroku-provided
                    // connection string, or development connection string from env var.
                    if (env == "Development")
                    {
                        // Use connection string from file.
                        connString = builder.Configuration.GetConnectionString("DefaultConnection");
                    }
                    else
                    {
                        // Use connection string provided at runtime by FlyIO.
                        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                        // Parse connection URL to connection string for Npgsql
                        connUrl = connUrl.Replace("postgres://", string.Empty);
                        var pgUserPass = connUrl.Split("@")[0];
                        var pgHostPortDb = connUrl.Split("@")[1];
                        var pgHostPort = pgHostPortDb.Split("/")[0];
                        var pgDb = pgHostPortDb.Split("/")[1];
                        var pgUser = pgUserPass.Split(":")[0];
                        var pgPass = pgUserPass.Split(":")[1];
                        var pgHost = pgHostPort.Split(":")[0];
                        var pgPort = pgHostPort.Split(":")[1];
                        var updatedHost = pgHost.Replace("flycast", "internal");

                        connString = $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
                    }

                    // Whether the connection string came from the local development configuration file
                    // or from the environment variable from Heroku, use it to set up your DbContext.
                    options.UseNpgsql(connString);
                });

//configure the HTTP request pipeline
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("https://localhost:4200"));

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence");
app.MapHub<MessageHub>("hubs/message");
app.MapFallbackToController("Index", "Fallback");

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<DataContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    await context.Database.MigrateAsync();
    await Seed.ClearConnections(context);
    await Seed.SeedUsers(userManager, roleManager);
}
catch (System.Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during migration");

}

await app.RunAsync();