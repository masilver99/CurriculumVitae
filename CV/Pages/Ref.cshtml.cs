using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CV.Pages
{
    public class RefModel : PageModel
    {
        private readonly ILogger<RefModel> _logger;

        public RefModel(ILogger<RefModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}