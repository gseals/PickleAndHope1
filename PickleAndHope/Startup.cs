using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PickleAndHope.DataAccess;

namespace PickleAndHope
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
            // all of these things are performing some sort of magic. making it so that you can inject this into any other class that we want
            // 
            services.AddControllers();
            //transient says a class gets create every time somebody asks for it. the default when requesting a service;
            //saying it is transiemt: when someone asks for a copy of asp.net core, give them a new one

            // name of the policy here doesn't matter
            services.AddCors(options =>
                options.AddPolicy("ItsAllGood",
                    builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin())
                );

            services.AddTransient<PickleRepository>();
            //when someone asks for a copy of this classm give them one, but only give them one for each API request
            //we may not use AddScoped very often
            //services.AddScoped<>();

            services.AddSingleton<IConfiguration>(Configuration); //only create one instance and share it always
            // when to use these items: add singleton says "no matter how many requests come into my api, only use one created instance of this thing"
            // singleton: you have to restart to initiate a change; if you are assigning stateful properties inside of your class
            // singleton's use cases are very specific to its use cases

            // transient or scoped do not leak data across requests; these destroy the old instance and create a new instance every time

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

            app.UseCors("ItsAllGood");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
