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
    public class EdModel : PageModel
    {
        private readonly ILogger<EdModel> _logger;

        public IList<WorkItem> WorkItems { get; }

        public EdModel(ILogger<EdModel> logger)
        {
            _logger = logger;
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            this.WorkItems = JsonSerializer.Deserialize<List<WorkItem>>(System.IO.File.ReadAllText("data/work.json"), options);
        }

        public void OnGet()
        {

        }
    }
}