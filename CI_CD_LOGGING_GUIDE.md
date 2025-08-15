# CI/CD Logging Guide - Azure Web App Deployment

## Overview
This guide explains what to look for in your GitHub Actions workflow logs to identify exactly where deployment problems occur.

## Phase 1: Build Phase (Windows Runner)

### 1.1 Dependencies Installation
**Look for:**
```
=== Installing dependencies ===
Current directory: C:\actions-runner\_work\my-taskflow\my-taskflow\frontend
Node version: v20.x.x
npm version: x.x.x
=== Starting npm ci ===
=== Dependencies installed successfully ===
=== Verifying node_modules ===
✓ node_modules directory exists
```

**Red flags:**
- ❌ `npm error code EUSAGE` - Lock file mismatch
- ❌ `Missing: [package] from lock file` - Dependencies out of sync
- ❌ `✗ node_modules directory missing!` - Installation failed

**Fix:** Run `npm install` locally and commit the updated `package-lock.json`

### 1.2 Next.js Build
**Look for:**
```
=== Starting Next.js build ===
Current directory: C:\actions-runner\_work\my-taskflow\my-taskflow\frontend
Available scripts in package.json: [list of scripts]
=== Running npm run build ===
=== Build completed, exit code: 0 ===
=== Verifying build output ===
=== Build successful! .next directory contents: ===
[Directory listing of .next]
=== .next/static contents: ===
[Static files listing]
=== .next/server contents: ===
[Server files listing]
```

**Red flags:**
- ❌ `ERROR: Build failed with exit code 1` - Build process failed
- ❌ `ERROR: .next directory not found after build` - Build didn't create output
- ❌ `WARNING: .next/static directory missing!` - Incomplete build

**Fix:** Check for TypeScript errors, missing dependencies, or build configuration issues

### 1.3 Cleanup and Verification
**Look for:**
```
=== Cleaning up for deployment ===
=== BEFORE CLEANUP - Current directory contents: ===
[File listing]
=== BEFORE CLEANUP - .next directory size: ===
X File(s) Y bytes
=== Removing development files ===
Removing .git directory...
Removing log files...
[etc.]
=== AFTER CLEANUP - Final package contents: ===
[File listing]
=== Verifying .next still exists: ===
SUCCESS: .next directory preserved
=== .next directory contents after cleanup: ===
[Directory listing]
=== .next directory size after cleanup: ===
X File(s) Y bytes
```

**Red flags:**
- ❌ `ERROR: .next directory was removed during cleanup!` - Cleanup script is too aggressive
- ❌ Missing essential files after cleanup

**Fix:** Review cleanup script to ensure it's not removing necessary files

### 1.4 Production Dependencies
**Look for:**
```
=== Installing production dependencies ===
=== Running npm ci --only=production ===
=== Production dependencies installed ===
```

**Red flags:**
- ❌ `npm error` - Production dependency installation failed
- ❌ Missing production packages

**Fix:** Check if all production dependencies are properly listed in `package.json`

### 1.5 Script Validation
**Look for:**
```
=== Testing startup.cjs syntax ===
✓ startup.cjs syntax is valid
=== Testing server.cjs syntax ===
✓ server.cjs syntax is valid
```

**Red flags:**
- ❌ `ERROR: startup.cjs has syntax errors!` - CommonJS syntax issues
- ❌ `ERROR: server.cjs has syntax errors!` - CommonJS syntax issues

**Fix:** Check for import/require syntax mismatches or syntax errors

### 1.6 Final Package Structure
**Look for:**
```
=== Final deployment package structure: ===
=== Root directory: ===
[File listing]
=== Essential files check: ===
✓ package.json
✓ startup.js
✓ server.cjs
✓ web.config
✓ .deployment
✓ .next directory
```

**Red flags:**
- ❌ Missing essential files
- ❌ `.next` directory missing or empty

## Phase 2: Artifact Upload

### 2.1 Upload Verification
**Look for:**
```
=== Verifying artifact upload ===
=== Artifact name: node-app ===
=== Upload path: ./frontend ===
=== Frontend directory contents before upload: ===
[File listing]
=== Frontend/.next contents before upload: ===
[Directory listing]
=== .next directory size: ===
X.XM .next/
```

**Red flags:**
- ❌ `.next` directory missing before upload
- ❌ Empty or incomplete `.next` directory

## Phase 3: Deployment Phase (Ubuntu Runner)

### 3.1 Artifact Download
**Look for:**
```
Downloading artifact 'node-app' to '/home/runner/work/my-taskflow/my-taskflow'
```

**Red flags:**
- ❌ Download failures
- ❌ Missing files after download

### 3.2 Azure Deployment
**Look for:**
```
Deploying to Azure Web App...
Deployment successful
```

**Red flags:**
- ❌ Deployment failures
- ❌ Authentication errors

### 3.3 Deployment Verification
**Look for:**
```
=== Verifying deployed files ===
=== Current working directory: /home/runner/work/my-taskflow/my-taskflow ===
=== Essential files check: ===
✓ package.json
✓ startup.cjs
✓ server.cjs
✓ web.config
✓ .deployment
✓ .next directory
=== File sizes and details: ===
package.json: X lines, X.XK
startup.cjs: X lines, X.XK
server.cjs: X lines, X.XK
.deployment: [config] command = npm start
=== .next directory size: ===
X.XM .next/
=== Final verification summary: ===
✓ All essential files present
✓ .next directory exists with X files
✓ Ready for Azure deployment
```

**Red flags:**
- ❌ Missing essential files
- ❌ `.next` directory missing or empty
- ❌ File size discrepancies

## Phase 4: Runtime Phase (Azure Web App)

### 4.1 Azure Startup Logs
**Use:** `az webapp log tail --name TaskFlow --resource-group barak_m`

**Look for:**
```
Starting TaskFlow application...
Current directory: /home/site/wwwroot
Node version: v20.x.x
NODE_ENV: production
✓ .next directory found
✓ Starting Next.js server...
Starting Next.js server...
Environment: production
Port: 8080
Hostname: 0.0.0.0
Next.js app prepared successfully
> Ready on http://0.0.0.0:8080
```

**Red flags:**
- ❌ `ERROR: .next directory not found!` - Build artifacts didn't reach Azure
- ❌ `Failed to prepare Next.js app` - Missing or corrupted build files
- ❌ `Could not find a production build in the '.next' directory` - Build missing

## Common Failure Points and Solutions

### 1. **Lock File Mismatch**
**Symptoms:** `npm ci` fails with missing packages
**Solution:** Run `npm install` locally, commit `package-lock.json`

### 2. **Build Failure**
**Symptoms:** `.next` directory missing after build
**Solution:** Check TypeScript errors, dependencies, build configuration

### 3. **Cleanup Too Aggressive**
**Symptoms:** `.next` directory removed during cleanup
**Solution:** Review cleanup script, ensure it preserves build artifacts

### 4. **File Upload Issues**
**Symptoms:** Files missing in deployment verification
**Solution:** Check artifact upload step, file paths

### 5. **Azure Configuration**
**Symptoms:** App starts but can't find `.next` directory
**Solution:** Verify `.deployment` file, startup command

### 6. **Module System Mismatch**
**Symptoms:** Syntax errors in startup scripts
**Solution:** Ensure ES modules vs CommonJS consistency

## Debugging Commands

### Local Testing
```bash
# Test build locally
cd frontend
npm run build

# Test startup script
node startup.cjs

# Test server
node server.cjs
```

### Azure Debugging
```bash
# View real-time logs
az webapp log tail --name TaskFlow --resource-group barak_m

# Download logs
az webapp log download --name TaskFlow --resource-group barak_m

# Check app settings
az webapp config show --name TaskFlow --resource-group barak_m
```

## Key Success Indicators

✅ **Build Phase:** `.next` directory created with static and server files
✅ **Cleanup:** Essential files preserved, development files removed
✅ **Upload:** All files successfully uploaded as artifact
✅ **Deployment:** Files verified on Azure runner
✅ **Runtime:** Startup script finds `.next` directory and starts server

## Next Steps

1. **Push these enhanced logging changes**
2. **Monitor the workflow run** - look for each phase's success indicators
3. **Identify the exact failure point** using the logging guide
4. **Apply targeted fixes** based on the specific failure
5. **Verify the fix** by running the workflow again

This enhanced logging will give you pinpoint accuracy on where and why the deployment is failing!
