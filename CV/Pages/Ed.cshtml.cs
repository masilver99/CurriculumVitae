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

        public IList<EdItem> EdItems { get; }

        public EdModel(ILogger<EdModel> logger)
        {
            _logger = logger;
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            this.EdItems = JsonSerializer.Deserialize<List<EdItem>>(System.IO.File.ReadAllText("data/ed.json"), options);
        }

        public void OnGet()
        {

        }
    }
}