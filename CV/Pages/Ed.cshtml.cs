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
using CV.data;

namespace CV.Pages
{
    public class EdModel : PageModel
    {
        private readonly ILogger<EdModel> _logger;
        private Repository _repository;
        public IEnumerable<EdItem> EdItems { get; private set; }

        public EdModel(ILogger<EdModel> logger, Repository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnGet()
        {
            EdItems = await _repository.GetEdItems();
        }
    }
}