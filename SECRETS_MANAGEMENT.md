# API Key Security - Implementation Guide

## Overview
The API key has been secured using multiple methods to prevent exposure in source code and configuration files.

## Methods Implemented

### 1. **User Secrets (Development)**
For local development, use .NET User Secrets to store the API key securely:

```bash
dotnet user-secrets set "OpenRouter:ApiKey" "your-api-key-here"
```

This stores the key in your user profile folder outside of the project directory and source control.

### 2. **Environment Variables (All Environments)**
Set the API key as an environment variable:

**Windows:**
```powershell
$env:OpenRouter__ApiKey = "your-api-key-here"
```

**Linux/macOS:**
```bash
export OpenRouter__ApiKey="your-api-key-here"
```

**Docker:**
```yaml
environment:
  - OpenRouter__ApiKey=your-api-key-here
```

### 3. **Azure Key Vault (Production)**
For production deployments, Azure Key Vault provides enterprise-grade secret management.

#### Setup Steps:

1. **Create an Azure Key Vault:**
   ```bash
   az keyvault create --name your-keyvault-name --resource-group your-rg --location eastus
   ```

2. **Add the API key as a secret:**
   ```bash
   az keyvault secret set --vault-name your-keyvault-name --name "OpenRouter--ApiKey" --value "your-api-key-here"
   ```
   
   Note: Use double dashes (--) in the secret name to represent configuration hierarchy (OpenRouter:ApiKey).

3. **Grant your application access:**
   
   **For Managed Identity (recommended for Azure hosting):**
   ```bash
   az keyvault set-policy --name your-keyvault-name --object-id <your-app-managed-identity-id> --secret-permissions get list
   ```

   **For Service Principal:**
   ```bash
   az keyvault set-policy --name your-keyvault-name --spn <your-app-client-id> --secret-permissions get list
   ```

4. **Configure your application:**
   
   Update `appsettings.Production.json` or `appsettings.json`:
   ```json
   {
     "KeyVault": {
       "Enabled": true,
       "VaultUri": "https://your-keyvault-name.vault.azure.net/"
     }
   }
   ```

5. **Authentication Methods:**
   
   The application uses `DefaultAzureCredential` which automatically tries these methods in order:
   - Environment variables (AZURE_CLIENT_ID, AZURE_TENANT_ID, AZURE_CLIENT_SECRET)
   - Managed Identity (when deployed to Azure)
   - Visual Studio credentials (local development)
   - Azure CLI credentials (local development)
   - Azure PowerShell credentials (local development)

#### Local Testing with Azure Key Vault:
```bash
# Login with Azure CLI
az login

# The application will automatically use your Azure CLI credentials
```

## Configuration Priority
The .NET configuration system uses this priority (last wins):
1. appsettings.json
2. appsettings.{Environment}.json
3. User Secrets (Development only)
4. Environment Variables
5. Azure Key Vault (if enabled)
6. Command-line arguments

## Security Best Practices

### ✅ DO:
- Use User Secrets for local development
- Use Azure Key Vault for production
- Use Managed Identity when hosting in Azure
- Rotate keys regularly
- Use different keys for different environments

### ❌ DON'T:
- Commit API keys to source control
- Store keys in appsettings.json
- Share keys via email or chat
- Use production keys in development
- Log API keys (even partially)

## Docker Deployment

### Using Environment Variables:
```yaml
# docker-compose.yml
services:
  cv:
    image: your-image
    environment:
      - OpenRouter__ApiKey=${OPENROUTER_API_KEY}
```

Then set the host environment variable before running:
```bash
export OPENROUTER_API_KEY="your-api-key-here"
docker-compose up
```

### Using Azure Key Vault with Docker:
```yaml
# docker-compose.yml
services:
  cv:
    image: your-image
    environment:
      - KeyVault__Enabled=true
      - KeyVault__VaultUri=https://your-keyvault-name.vault.azure.net/
      - AZURE_CLIENT_ID=${AZURE_CLIENT_ID}
      - AZURE_TENANT_ID=${AZURE_TENANT_ID}
      - AZURE_CLIENT_SECRET=${AZURE_CLIENT_SECRET}
```

## Verifying Configuration

The application will log an error if the API key is not configured:
```
OpenRouter API key is not configured. Please set it using user secrets or environment variables.
```

Check the startup logs to verify Key Vault integration:
```
Azure Key Vault integration enabled. Vault URI: https://your-keyvault-name.vault.azure.net/
```

## Migration Checklist

- [x] Removed hardcoded API key from appsettings.json
- [x] Added Azure Key Vault NuGet packages
- [x] Implemented Key Vault configuration in Program.cs
- [ ] Set up User Secrets for local development
- [ ] Create Azure Key Vault (if using)
- [ ] Add secret to Key Vault (if using)
- [ ] Configure Managed Identity or Service Principal
- [ ] Update deployment scripts/pipelines
- [ ] Test in all environments
- [ ] Rotate the exposed API key

## Troubleshooting

### "OpenRouter API key is not configured"
- Verify the key is set in one of the configuration sources
- Check the configuration key uses the correct format: `OpenRouter:ApiKey` (environment: `OpenRouter__ApiKey`)

### "Azure Key Vault authentication failed"
- Verify the VaultUri is correct
- Ensure your application has the correct permissions
- Check that authentication credentials are available (Managed Identity, CLI, etc.)
- Review application logs for detailed error messages

### "Access denied" from Key Vault
- Verify the access policy includes your application's identity
- Ensure "Get" and "List" permissions are granted for secrets

