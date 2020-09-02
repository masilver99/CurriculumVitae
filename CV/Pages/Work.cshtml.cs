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
    public class WorkModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private Repository _repository;
        public IEnumerable<WorkItem> WorkItems { get; private set; }


        public WorkModel(ILogger<IndexModel> logger, Repository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnGet()
        {
            this.WorkItems = await _repository.GetWorkItems();
        }
    }
}