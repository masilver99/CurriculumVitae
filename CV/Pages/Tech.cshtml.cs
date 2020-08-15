using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.IO;
using Utf8Json;

namespace CV.Pages
{
    public class TechModel : PageModel
    {
        private readonly ILogger<TechModel> _logger;

        public IList<TechCategory> TechCategories { get; }

        public TechModel(ILogger<TechModel> logger, List<TechCategory> techCategories)
        {
            _logger = logger;
            TechCategories = techCategories;
        }

        public void OnGet()
        {

        }
    }
}