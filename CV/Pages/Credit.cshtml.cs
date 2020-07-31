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
    public class CreditModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IList<WorkItem> WorkItems { get; }

        public CreditModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}