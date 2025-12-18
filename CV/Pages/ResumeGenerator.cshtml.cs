#nullable enable
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CV.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace CV.Pages
{
    public class ResumeGeneratorModel : PageModel
    {
        private readonly Repository _repository;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ResumeGeneratorModel(Repository repository, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string? JobDescription { get; set; }

        [BindProperty]
        public string? ResumeStyle { get; set; }

        [BindProperty]
        public string? OutputFormat { get; set; }

        public string? GeneratedResume { get; set; }
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Initialize defaults
            ResumeStyle = "ats";
            OutputFormat = "text";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(JobDescription))
            {
                ErrorMessage = "Please provide a job description.";
                return Page();
            }

            try
            {
                // Get the CV markdown
                var cvMarkdown = await _repository.GenerateLlmFriendlyMarkdown(compact: false);

                // Get API configuration
                var apiKey = _configuration["OpenRouter:ApiKey"];
                var apiUrl = _configuration["OpenRouter:ApiUrl"] ?? "https://openrouter.ai/api/v1/chat/completions";
                var model = _configuration["OpenRouter:Model"] ?? "anthropic/claude-3.5-sonnet";

                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    ErrorMessage = "OpenRouter API key is not configured. Please add it to appsettings.json or user secrets.";
                    return Page();
                }

                // Build the prompt based on style
                var styleInstructions = ResumeStyle == "executive"
                    ? "Create an executive-style resume that is detailed, achievement-focused, and uses a narrative approach. Emphasize leadership, strategic impact, and quantifiable results. Use rich, descriptive language."
                    : "Create an ATS-optimized resume that is keyword-focused and formatted for applicant tracking systems. Use clear section headers, bullet points, and industry-standard terminology. Optimize for keyword matching.";

                var formatInstructions = OutputFormat switch
                {
                    "html" => "Format the output as clean, semantic HTML without <html>, <head>, or <body> tags. Use appropriate HTML tags like <h2>, <h3>, <p>, <ul>, <li>, etc.",
                    "markdown" => "Format the output in Markdown format with proper headers, lists, and formatting.",
                    "pdf" => "Format the output in a way that would be suitable for PDF conversion. Use clear sections and formatting.",
                    _ => "Format the output as plain text with clear section headers and structure."
                };

                var prompt = $@"You are an expert resume writer. Based on the following curriculum vitae and job description, create a tailored resume.

{styleInstructions}

{formatInstructions}

**Curriculum Vitae:**
{cvMarkdown}

**Job Description:**
{JobDescription}

**Instructions:**
1. Analyze the job description to identify key requirements, skills, and qualifications
2. Select and highlight the most relevant experiences, skills, and achievements from the CV
3. Tailor the language and emphasis to match the job requirements
4. Include only the most relevant information - this should be a focused resume, not a complete CV
5. Ensure the resume is professional, concise, and impactful
6. Do not include any preamble or explanation - output only the resume content

Generate the resume now:";

                // Call OpenRouter API
                var httpClient = _httpClientFactory.CreateClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/masilver99/CurriculumVitae");

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Log.Information("Calling OpenRouter API with model {Model}", model);
                var response = await httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Log.Error("OpenRouter API error: {StatusCode} - {Error}", response.StatusCode, errorContent);
                    ErrorMessage = $"API Error: {response.StatusCode}. Please check your API key and configuration.";
                    return Page();
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent);

                if (apiResponse?.Choices != null && apiResponse.Choices.Length > 0)
                {
                    GeneratedResume = apiResponse.Choices[0].Message?.Content ?? string.Empty;

                    // For PDF, we would need to implement actual PDF generation
                    // For now, we'll just provide the content
                    if (OutputFormat == "pdf")
                    {
                        // In a real implementation, you would use a library like:
                        // - PuppeteerSharp
                        // - IronPdf
                        // - QuestPDF
                        // For now, just return the formatted text
                        ErrorMessage = "Note: PDF generation requires additional setup. The content is available as text below.";
                        OutputFormat = "text";
                    }
                }
                else
                {
                    ErrorMessage = "No response received from the API.";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating resume");
                ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return Page();
        }

        // DTOs for OpenRouter API
        private class OpenRouterResponse
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("choices")]
            public Choice[]? Choices { get; set; }

            [JsonPropertyName("usage")]
            public Usage? Usage { get; set; }
        }

        private class Choice
        {
            [JsonPropertyName("message")]
            public Message? Message { get; set; }

            [JsonPropertyName("finish_reason")]
            public string? FinishReason { get; set; }
        }

        private class Message
        {
            [JsonPropertyName("role")]
            public string? Role { get; set; }

            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        private class Usage
        {
            [JsonPropertyName("prompt_tokens")]
            public int PromptTokens { get; set; }

            [JsonPropertyName("completion_tokens")]
            public int CompletionTokens { get; set; }

            [JsonPropertyName("total_tokens")]
            public int TotalTokens { get; set; }
        }
    }
}
