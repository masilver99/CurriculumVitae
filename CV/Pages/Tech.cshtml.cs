using System.Collections.Generic;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CV.data;

namespace CV.Pages
{
    public class TechModel : PageModel
    {
        private readonly ILogger<TechModel> _logger;
        private Repository _repository;

        public IEnumerable<TechCategory> TechCategories { get; private set; }

        public TechModel(ILogger<TechModel> logger, Repository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnGet()
        {
            TechCategories = await _repository.GetTechCategories();
        }
    }
}