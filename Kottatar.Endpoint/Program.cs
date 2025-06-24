using Kottatar.Data;
using Kottatar.Entities.Helpers;
using Kottatar.Logic.Helpers;
using Kottatar.Logic.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kottatar.Endpoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            
            builder.Services.AddTransient(typeof(Repository<>));
            builder.Services.AddTransient<DtoProvider>();
            builder.Services.AddTransient<MusicLogic>();
            builder.Services.AddTransient<InstrumentLogic>();

            builder.Services.AddDbContext<KottatarContext>(options =>
            {
                options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=KottatarDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True");
                options.UseLazyLoadingProxies();
            });

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            // Add global exception handling filters
            builder.Services.AddControllers(options => 
            {
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<ValidationFilterAttribute>();
            });

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

            // Global exception handling middleware (optional, as we're already using filters)
            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
