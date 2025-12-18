#nullable enable
using System.Threading.Tasks;
using CV.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CV.Pages
{
    public class MarkdownModel : PageModel
    {
        private readonly Repository _repository;

        public MarkdownModel(Repository repository)
        {
            _repository = repository;
        }

        public string? HtmlContent { get; private set; }

        public async Task<IActionResult> OnGet([FromQuery] bool compact = false)
        {
            var markdown = await _repository.GenerateLlmFriendlyMarkdown(compact);
            HtmlContent = markdown;
            return Page();
        }
    }
}
