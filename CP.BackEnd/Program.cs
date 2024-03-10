
using CP.Data.Domain;
using CP.Models.Entities;
using CP.Services.Implementations;
using CP.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CP.BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

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

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
        }
    }
}
