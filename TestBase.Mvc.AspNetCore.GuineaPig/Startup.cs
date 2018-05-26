﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestBase.Mvc.AspNetCore.GuineaPig
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Add(ServiceDescriptor.Singleton<ILoggerFactory,LoggerFactory>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if   (env.IsDevelopment()){ app.UseDeveloperExceptionPage(); }
            else { app.UseExceptionHandler("/Home/Error"); }
            
            app.UseStaticFiles();

            app.UseMvc(routes =>
                       {
                           routes.MapRoute(
                                           name: "default",
                                           template: "{controller=Home}/{action=Index}/{id?}");
                       });
        }
    }
}