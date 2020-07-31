using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace CV.Pages
{
    public class TechModel : PageModel
    {
        private readonly ILogger<TechModel> _logger;

        public IList<TechCategory> TechCategories { get; }

        public TechModel(ILogger<TechModel> logger)
        {
            _logger = logger;
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            this.TechCategories = JsonSerializer.Deserialize<List<TechCategory>>(System.IO.File.ReadAllText("data/tech.json"), options);
        }

        public void OnGet()
        {

        }
    }
}