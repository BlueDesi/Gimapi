
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
            //
            var env = builder.Environment;

            // 2. Elegir la cadena de conexión según el entorno
            string connectionString;

            if (env.IsDevelopment())
            {
                // Usa la local si estás en Visual Studio
                connectionString = builder.Configuration.GetConnectionString("LocalConnection");
            }
            else
            {
                // Usa la del servidor si ya está publicado
                connectionString = builder.Configuration.GetConnectionString("RemoteConnection");
            }

            // 3. Registrar el DbContext con la variable dinámica
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //
            // Obtener la cadena de conexión del archivo appsettings.json
            // Registrar el Contexto con la base de datos SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            //
            builder.Services.AddScoped<IUsuarioService, UsuarioServicio>();
            builder.Services.AddScoped<IMembresiaService, MembresiaService>();
            builder.Services.AddScoped<IRolService, RolService>(); // Registro del nuevo servicio de Roles
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gimapi v1");
                   // c.RoutePrefix = string.Empty; // Esto hace que Swagger cargue al entrar a la URL principal
                });
            }
            //app.UseSwagger();
           // app.UseSwaggerUI();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gimapi v1");
                c.RoutePrefix = string.Empty; // Esto hace que Swagger cargue al entrar a la URL principal
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
