using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Utf8Json;
using CV.Models;

namespace CV
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
            services
                .AddRazorPages()
                .AddRazorRuntimeCompilation()
                .AddRazorOptions(options =>
                {
                    options.PageViewLocationFormats.Add("/Pages/Partials/{0}.cshtml");
                });

            // Cache file.  They aren't that large, so it shouldn't affect memory.
            services.AddSingleton(JsonSerializer.Deserialize<List<TechCategory>>(System.IO.File.ReadAllText(@"data/tech.json")));
            services.AddSingleton(JsonSerializer.Deserialize<List<EdItem>>(System.IO.File.ReadAllText(@"data/ed.json")));
            services.AddSingleton(JsonSerializer.Deserialize<List<WorkItem>>(System.IO.File.ReadAllText(@"data/work.json")));
            services.AddSingleton(JsonSerializer.Deserialize<List<ProjectItem>>(System.IO.File.ReadAllText(@"data/projects.json")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
