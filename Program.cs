
using Gimapi.Data;
using Gimapi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
            // builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Gimapi", Version = "v1" });

                // Configurar el botón "Authorize"
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
            });
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
           
            //
            builder.Services.AddScoped<IUsuarioService, UsuarioServicio>();
            builder.Services.AddScoped<IMembresiaService, MembresiaService>();
            builder.Services.AddScoped<IRolService, RolService>(); // Registro del nuevo servicio de Roles
            builder.Services.AddScoped<ITokenService, TokenService>(); //Registro del servicio de Tokens

            //

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

            // Habilitar autorización
            builder.Services.AddAuthorization();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowPythonFront", policy =>
                {
                    policy.AllowAnyOrigin()   // En producción, reemplace por la URL del front
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
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
            app.UseCors("AllowPythonFront");
            // 1. Identifica quién es el usuario (Lee el Token)
            app.UseAuthentication();

            // 2. Verifica a qué tiene permiso (Roles)
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
