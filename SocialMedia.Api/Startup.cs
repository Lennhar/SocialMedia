using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.Services;
using SocialMedia.Infraestructure.Data;
using SocialMedia.Infraestructure.Filters;
using SocialMedia.Infraestructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Newtonsoftjson, ignorar referencia circular
            services.AddControllers(options=>
            {
                options.Filters.Add<ValidationFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            //Validaciones manuales con un filter y no validar dentro del controller con modelstate
            .ConfigureApiBehaviorOptions(options =>
            {
                //Usaremos la validación de ApiController
                //options.SuppressModelStateInvalidFilter = true;
            });

            //Configurar automapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //Cadena de conexión pertenece al dbcontext
            services.AddDbContext<SocialMediaContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("LocalDb")));

            //Dependencia de interfaz de service y IService
            services.AddTransient<IPostService, PostService>();

            //Inyección de dependencias, es trabajar con abstraciones Lennhar Ortega 17/03
            // services.AddTransient<IPostRepository, PostRepository>();
            //services.AddTransient<IUserRepository, UserRepository>();
            //Este reemplaza los repositorios post y user
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Usamos el validationFilter de infraestructure como filtro global, ya que quitamos la validación automatica, así se usaba antes
            services.AddMvc(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
                //Se agrego fluentValidator para usar las validaciones en el proyecto infraestructure
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
