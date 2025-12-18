# Quick Start - Using the Secured API Key

## ✅ What Has Been Done

1. **Removed hardcoded API key** from `appsettings.json`
2. **Added Azure Key Vault support** to the project
3. **Stored API key in User Secrets** for your local development
4. **Created comprehensive documentation** in `SECRETS_MANAGEMENT.md`

## 🚀 For Local Development (RIGHT NOW)

Your API key is now stored securely in User Secrets and will work immediately. No action required!

**To verify:**
```bash
dotnet user-secrets list
```

## 🔐 For Production Deployment

### Option 1: Environment Variables (Simplest)

Set this environment variable on your production server:
```bash
# Linux/macOS
export OpenRouter__ApiKey="your-api-key-here"

# Windows PowerShell
$env:OpenRouter__ApiKey = "your-api-key-here"
```

For Docker:
```yaml
environment:
  - OpenRouter__ApiKey=your-api-key-here
```

### Option 2: Azure Key Vault (Most Secure)

1. **Create Key Vault:**
   ```bash
   az keyvault create --name cv-keyvault --resource-group your-rg --location eastus
   ```

2. **Add the secret:**
   ```bash
   az keyvault secret set --vault-name cv-keyvault --name "OpenRouter--ApiKey" --value "your-api-key-here"
   ```

3. **Update production configuration:**
   ```json
   {
     "KeyVault": {
       "Enabled": true,
       "VaultUri": "https://cv-keyvault.vault.azure.net/"
     }
   }
   ```

4. **Grant access to your app:**
   ```bash
   # For Managed Identity (recommended)
   az keyvault set-policy --name cv-keyvault --object-id <managed-identity-id> --secret-permissions get list
   ```

## 🔒 Security Improvements

### Before:
- ❌ API key hardcoded in `appsettings.json`
- ❌ Committed to source control
- ❌ Visible to anyone with repo access
- ❌ Difficult to rotate

### After:
- ✅ No secrets in source code
- ✅ User Secrets for local development
- ✅ Environment variables supported
- ✅ Azure Key Vault ready
- ✅ Easy to rotate keys

## 📋 Next Steps

### Immediate (Required):
1. **Rotate the exposed API key** - The old key from `appsettings.json` should be considered compromised
2. **Commit the changes** - The updated files are safe to commit now
3. **Update deployment** - Configure your production environment with one of the methods above

### Optional (Recommended):
1. Set up Azure Key Vault for production
2. Configure Managed Identity for your Azure resources
3. Implement key rotation schedule
4. Add monitoring for secret access

## 🧪 Testing

Your local development environment is already configured and ready to use. The application will automatically use the User Secrets.

To test with Key Vault locally:
```bash
# Login to Azure
az login

# The app will use your Azure CLI credentials to access Key Vault
```

## 📖 Full Documentation

See `SECRETS_MANAGEMENT.md` for complete details on all configuration methods, troubleshooting, and best practices.

## ⚠️ Important Security Note

**ROTATE YOUR API KEY IMMEDIATELY!**

The key that was in `appsettings.json` has been exposed and should be rotated. Contact your OpenRouter account to generate a new key:
- Old key: `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`
- This key should be deactivated and replaced with a new one

Once you have a new key, update it using:
```bash
# Local development
dotnet user-secrets set "OpenRouter:ApiKey" "new-key-here"

# Production (environment variable)
export OpenRouter__ApiKey="new-key-here"

# Production (Key Vault)
az keyvault secret set --vault-name cv-keyvault --name "OpenRouter--ApiKey" --value "new-key-here"
```

