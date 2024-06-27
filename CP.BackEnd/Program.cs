using CP.Data.Domain;
using CP.Data.Repositories.Implementations;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Models.Models;
using CP.Services.Implementations;
using CP.Services.Interfaces;
using CP.SignalR.DataService;
using CP.SignalR.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace CP.BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            //var accessToken = context.Request.Query["access_token"];
                            //context.Token = accessToken;

                            var authorizationHeader = context.Request.Headers["Authorization"].ToString();

                            if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                var accessToken = authorizationHeader.Substring("Bearer ".Length).Trim();
                                context.Token = accessToken;
                            }
                            else if (!string.IsNullOrEmpty(context.Request.Query["access_token"]))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        },
                        //Return 401 instead of redirecting to Login When authorization fails
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                error = "Unauthorized"
                            }));
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
            builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            builder.Services.AddScoped<IFriendRequestMessageRepository, FriendRequestMessageRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IRefereshTokenService, RefereshTokenService>();
            builder.Services.AddScoped<IFriendRequestService, FriendRequestService>();
            builder.Services.AddScoped<IEncryptionService, EncryptionService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //set up the cors policy
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: "reactApp", configurePolicy: builder =>
                {
                    builder.WithOrigins("http://localhost:5173")
                    //builder.WithOrigins("https://chatpulse-omega.vercel.app")
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

            app.UseStaticFiles();

            app.UseCors("reactApp"); // registered above defined policy

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>(pattern: "/Chat"); // This is the endpoint("/chat") to hub

            app.Run();
        }
    }
}
