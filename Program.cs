
using Gimapi.Data;
using Gimapi.Services;
using Microsoft.EntityFrameworkCore;

namespace Gimapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //inicios
            // Obtener la cadena de conexión del archivo appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // Registrar el Contexto con la base de datos SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //
            builder.Services.AddScoped<IUsuarioService, UsuarioServicio>();
            builder.Services.AddScoped<IMembresiaService, MembresiaService>();

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
