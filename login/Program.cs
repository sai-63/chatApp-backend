using login.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using Repository;
using Service;

namespace login
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;
            var mongoConnectionString = configuration.GetConnectionString("MongoDB");
            var databaseName = "chat_app";

            builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConnectionString));
            builder.Services.AddScoped<IUserRepository, UserRepository>(_ => new UserRepository(_.GetRequiredService<IMongoClient>(), databaseName, "Users"));
            builder.Services.AddScoped<IChatRepository, ChatRepository>(_ => new ChatRepository(_.GetRequiredService<IMongoClient>(), databaseName, "Chats"));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            // Add SignalR service
            builder.Services.AddSignalR();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure CORS
            // Configure CORS
            app.UseCors(options =>
            {
                options.WithOrigins("http://localhost:3000") // Specify allowed origin
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials(); // Allow credentials
            });


            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
