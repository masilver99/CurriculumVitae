using System.Collections.Generic;
using CV.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

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