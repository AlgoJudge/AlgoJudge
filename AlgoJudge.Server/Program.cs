
using AlgoJudge.Server.Database;
using AlgoJudge.Server.Database.Models;
using AlgoJudge.Server.Services;
using AlgoJudge.Server.Utils;
using Microsoft.EntityFrameworkCore;

namespace AlgoJudge.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            {
                String? dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                //Console.WriteLine($"Connection string: {connectionString}");
                dbConnectionString ??= builder.Configuration.GetConnectionString("ApplicationContext");
                builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseNpgsql(dbConnectionString));
            }

            builder.Services.AddAuthorization();
            builder.Services.AddIdentityApiEndpoints<User>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<IActivityService, ActivityService>();

            builder.Services.AddControllers(options =>
                options.Filters.Add<HttpResponseExceptionFilter>());
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:5173").AllowAnyMethod().AllowAnyHeader();
                        policy.AllowCredentials();
                    });
            });

            var app = builder.Build();

            {
                using var scope = app.Services.CreateScope();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            }

            app.MapGroup("/identity").MapIdentityApi<User>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseCors(builder => builder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin()); // TODO

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
