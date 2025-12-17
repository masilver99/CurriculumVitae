using System.Collections.Generic;
using System.Threading.Tasks;
using CV.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using CV.data;

namespace CV.Pages
{
    public class CertificationsModel : PageModel
    {
        private readonly ILogger<CertificationsModel> _logger;
        private readonly Repository _repository;

        public IEnumerable<CertificationItem> CertificationItems { get; private set; }

        public CertificationsModel(ILogger<CertificationsModel> logger, Repository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task OnGet()
        {
            CertificationItems = await _repository.GetCertificationItems();
        }
    }
}
