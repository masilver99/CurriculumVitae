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
    public class EdModel : PageModel
    {
        private readonly ILogger<EdModel> _logger;

        public List<EdItem> EdItems { get; }

        public EdModel(ILogger<EdModel> logger, List<EdItem> edItems)
        {
            _logger = logger;
            EdItems = edItems;
        }

        public void OnGet()
        {

        }
    }
}