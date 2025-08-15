# Azure Web App Deployment Fix

## Problem Summary
Your Azure Web App was successfully deploying but failing to run because it couldn't find the `.next` directory (Next.js build output) at runtime.

## What We Fixed

### 1. **Startup Configuration**
- Created `.deployment` file to tell Azure how to start your app
- Updated `package.json` start script to use a custom startup script
- Created `startup.cjs` with proper error handling and verification

### 2. **Build Process Improvements**
- Enhanced GitHub Actions workflow to preserve all necessary files
- Added verification steps to ensure `.next` directory is built and preserved
- Added production dependency installation
- Added comprehensive file verification before deployment

### 3. **Azure Web App Configuration**
- Created `.deployment` file for Azure startup command
- Updated `web.config` references
- Simplified startup process without external dependencies

## Files Created/Modified

### New Files:
- `.deployment` - Azure deployment configuration
- `startup.cjs` - Custom startup script with error handling
- `AZURE_DEPLOYMENT_FIX.md` - This guide

### Modified Files:
- `.github/workflows/main_taskflow.yml` - Enhanced build and deployment workflow
- `frontend/package.json` - Updated start script

## Current Architecture

The deployment now uses a simplified approach:
1. **GitHub Actions** builds the Next.js app and preserves the `.next` directory
2. **Azure Web App** uses the `.deployment` file to run `npm start`
3. **npm start** runs `node startup.cjs`
4. **startup.cjs** verifies the `.next` directory exists and starts the server
5. **server.cjs** runs the actual Next.js application

## Next Steps

### 1. **Commit and Push Changes**
```bash
git add .
git commit -m "Fix Azure Web App deployment - add startup scripts and build verification"
git push origin main
```

### 2. **Monitor GitHub Actions**
- Watch the workflow run in GitHub Actions
- Verify that all files are properly built and deployed
- Check that `.next` directory is preserved

### 3. **Verify Azure Deployment**
- Check Azure Web App logs after deployment
- Look for the startup script output
- Verify the app is running at your domain

### 4. **If Issues Persist**
- Check Azure Web App Configuration in Azure Portal
- Ensure Node.js 20.x is selected as runtime
- Verify startup command is set to `npm start`

## Expected Behavior

After these fixes, your Azure Web App should:
1. ✅ Successfully build the Next.js application
2. ✅ Preserve the `.next` directory during deployment
3. ✅ Use the custom startup script to verify files exist
4. ✅ Start the Next.js server properly
5. ✅ Serve your application at the Azure domain

## Troubleshooting

### If `.next` directory is still missing:
- Check GitHub Actions build logs for build errors
- Verify the build step completes successfully
- Check that cleanup doesn't remove essential files

### If startup script fails:
- Check Azure Web App logs for detailed error messages
- Verify all files are present in the deployment
- Check Node.js version compatibility

### If app still won't start:
- Check Azure Web App Configuration settings
- Verify startup command is correct
- Check for any Azure-specific environment variables

## Key Benefits of These Changes

1. **Better Error Handling**: Startup script provides clear error messages
2. **Build Verification**: Workflow verifies build output before deployment
3. **File Preservation**: Ensures all necessary files reach Azure
4. **Azure Integration**: Proper Azure Web App configuration files
5. **Process Management**: Custom startup script for better error handling

## Support

If you continue to have issues after implementing these fixes:
1. Check the GitHub Actions workflow logs
2. Review Azure Web App logs using `az webapp log tail`
3. Verify all configuration files are present
4. Check Azure Web App Configuration in the Azure Portal
