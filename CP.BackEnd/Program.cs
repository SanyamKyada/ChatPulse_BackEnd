
using CP.Data.Domain;
using CP.Data.Repositories.Implementations;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Services.Implementations;
using CP.Services.Interfaces;
using CP.SignalR.DataService;
using CP.SignalR.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CP.BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var key = Encoding.ASCII.GetBytes("This is my security key:qwe123@@");
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:7003",
                        ValidAudience = "http://localhost:5173",
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            context.Token = accessToken;
                            return  Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSignalR();

            builder.Services.AddControllers();

            builder.Services.AddDbContext<CPDatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("CPConnectionString"))
                    .UseSqlServer(opt => opt.EnableRetryOnFailure()));

            // For Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CPDatabaseContext>()
                 .AddDefaultTokenProviders();

            // Add custom services
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IRefereshTokenRepository, RefereshTokenRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IRefereshTokenService, RefereshTokenService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //set up the cors policy
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: "reactApp", configurePolicy: builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddSingleton<SharedDb>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("reactApp"); // registered above defined policy

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>(pattern: "/Chat"); // This is the endpoint("/chat") to hub

            app.Run();
        }
    }
}
