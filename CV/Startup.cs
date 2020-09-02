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
using CV.data;

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
            //TechCategories techCats = new TechCategories(JsonSerializer.Deserialize<List<TechCategory>>(System.IO.File.ReadAllText(@"data/tech.json")));
            //var workItems = JsonSerializer.Deserialize<List<WorkItem>>(System.IO.File.ReadAllText(@"data/work.json"));
            //var projectItems = JsonSerializer.Deserialize<List<ProjectItem>>(System.IO.File.ReadAllText(@"data/projects.json"));
            //BuildWorkXRefs(workItems, techCats);
            //BuildProjectXRefs(projectItems, techCats);
            //services.AddSingleton(techCats);
            //services.AddSingleton(JsonSerializer.Deserialize<List<EdItem>>(System.IO.File.ReadAllText(@"data/ed.json")));
            //services.AddSingleton(workItems);
            //services.AddSingleton(projectItems);

            services.AddSingleton(new Repository());
        }
        /*
        private void BuildProjectXRefs(List<ProjectItem> projectItems, TechCategories techCats)
        {
            foreach (var projectItem in projectItems)
            {
                foreach (var techXref in projectItem.TechXref)
                {
                    var techItem = techCats.GetTechItemByName(techXref);
                    if (techItem != null)
                    {
                        projectItem.TechItems.Add(techItem);
                    }
                }
            }
        }

        private void BuildWorkXRefs(List<WorkItem> workItems, TechCategories techCats)
        {
            foreach (var workItem in workItems)
            {
                foreach (var techXref in workItem.TechXref)
                {
                    var techItem = techCats.GetTechItemByName(techXref);
                    if (techItem != null)
                    {
                        workItem.TechItems.Add(techItem);
                    }
                }
            }
        }
        */
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
