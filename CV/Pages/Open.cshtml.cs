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
    public class OpenModel : PageModel
    {
        private readonly ILogger<OpenModel> _logger;

        public IList<OpenItem> OpenItems { get; }

        public OpenModel(ILogger<OpenModel> logger)
        {
            _logger = logger;
            this.OpenItems = JsonSerializer.Deserialize<List<OpenItem>>(System.IO.File.ReadAllText("data/open.json"));
        }

        public void OnGet()
        {

        }
    }
}