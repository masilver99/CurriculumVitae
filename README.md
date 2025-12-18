# Curriculum Vitae Web Application

A professional CV/Resume website built with ASP.NET Core Razor Pages.

## Features

- **Work History**: Detailed work experience with technology tags
- **Technology Skills**: Comprehensive list of technical skills organized by category
- **Projects**: Portfolio of projects with descriptions and links
- **Certifications**: Professional certifications and credentials
- **Education**: Educational background
- **References**: Professional references
- **AI-Powered Resume Generator**: Generate tailored resumes based on job descriptions

## Resume Generator

The Resume Generator feature uses AI to create customized resumes based on your CV and a specific job description.

### Features

- **AI-Powered**: Uses OpenRouter API to generate intelligent, tailored resumes
- **Multiple Formats**: Output in Text, Markdown, HTML, or PDF formats
- **Resume Styles**: 
  - **ATS-Optimized**: Keyword-focused format optimized for Applicant Tracking Systems
  - **Executive Style**: Detailed, achievement-focused narrative format
- **Smart Matching**: Analyzes job descriptions to highlight relevant experience and skills

### Configuration

To use the Resume Generator, you need to configure an OpenRouter API key:

#### Option 1: User Secrets (Recommended for Development)

```bash
cd CV
dotnet user-secrets set "OpenRouter:ApiKey" "your-api-key-here"
```

#### Option 2: Environment Variables

```bash
export OpenRouter__ApiKey="your-api-key-here"
```

#### Option 3: appsettings.json (Not Recommended)

Edit `CV/appsettings.json` and add your API key:

```json
{
  "OpenRouter": {
    "ApiKey": "your-api-key-here",
    "ApiUrl": "https://openrouter.ai/api/v1/chat/completions",
    "Model": "anthropic/claude-3.5-sonnet"
  }
}
```

**Note**: Never commit API keys to source control. Use user secrets or environment variables instead.

### Getting an OpenRouter API Key

1. Visit [OpenRouter.ai](https://openrouter.ai)
2. Sign up for an account
3. Navigate to the API Keys section
4. Generate a new API key
5. Configure it using one of the methods above

### Supported Models

The default model is `anthropic/claude-3.5-sonnet`, but you can configure any OpenRouter-supported model:

- `anthropic/claude-3.5-sonnet` (default)
- `openai/gpt-4`
- `openai/gpt-3.5-turbo`
- `google/gemini-pro`
- And many more...

## Building and Running

### Prerequisites

- .NET 10.0 SDK or later

### Build

```bash
cd CV
dotnet build
```

### Run

```bash
cd CV
dotnet run
```

The application will start and be available at `http://localhost:5000` (or as configured in `launchSettings.json`).

## Project Structure

- `/CV/Pages/` - Razor Pages
- `/CV/Models/` - Data models
- `/CV/data/` - JSON data files and Repository class
- `/CV/wwwroot/` - Static files (CSS, JS, images)

## Technologies Used

- ASP.NET Core 10.0
- Razor Pages
- Bootstrap 5
- Font Awesome
- Serilog for logging
