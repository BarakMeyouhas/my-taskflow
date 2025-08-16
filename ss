[33mb122e77[m[33m ([m[1;36mHEAD -> [m[1;32mmain[m[33m)[m Remove Azure Storage connection string from appsettings.json for security reasons
[33md466701[m Add Azure Functions deployment workflow and configure VSCode settings for development
[33mfe81a19[m[33m ([m[1;31morigin/main[m[33m)[m Update backend URL in auth configuration to new Azure endpoint
[33m851196c[m Add detailed logging to AuthController for login attempts and errors; introduce a new database test endpoint
[33meb5f43b[m Update Program.cs to switch from in-memory database to SQL Server connection for production readiness
[33m00ea319[m Refactor authentication logic in AuthController to improve error handling and validation; update Program.cs for in-memory database testing and code formatting.
[33mf856012[m Add CORS support and basic health check endpoints in Program.cs
[33m4db30e3[m Remove unnecessary comment in Program.cs to improve code clarity
[33m26a5764[m Refactor Azure deployment workflows for frontend and backend by removing explicit login steps and updating to use publish profiles for deployment.
[33m55d87ff[m Enhance backend deployment workflow by adding Azure login step and updating deployment action name
[33m9e76e22[m Remove test comment in Program.cs for code clarity
[33mc6d3e98[m Refactor Azure deployment workflow to simplify backend deployment step by removing explicit login and renaming the deployment action.
[33m32bd15a[m Add test comment in Program.cs for clarity
[33mcd506b2[m Update backend deployment workflow to include changes in the workflow file path
[33mf56e49d[m Add frontend deployment workflow for Azure integration
[33md7d69af[m Enhance Azure deployment workflows by adding login step for authentication
[33mef899dd[m Fix: Update frontend to use Azure backend, add backend deployment workflow
[33m46ebd5e[m Fix: Complete Azure deployment solution - include production dependencies, enhanced startup script
[33m1981c14[m Fix: Resolve 404 error - remove ES module conflict, enhance Azure startup
[33m8447262[m Fix: Convert startup.js to CommonJS (startup.cjs) for Azure compatibility
[33m40fcc15[m Fix: Update Azure configuration to run startup.js instead of bypassing it
[33m6390fcb[m Enhance startup script to auto-install production dependencies if missing
[33mf7835bb[m Add comprehensive debugging for dependency installation and deployment package verification
[33m635eb57[m Fix: Use pre-built zip package for Azure deployment to avoid permission issues
[33m3ae9265[m Fix: Include package-lock.json for npm ci to work properly
[33m3c5c0d8[m Fix: Handle permission issues gracefully in verification step
[33mb53e87e[m Fix: Ignore unzip warnings to continue with deployment
[33m259e26e[m Optimize: Create smaller deployment package and reinstall dependencies on Azure
[33m4b73ae8[m Fix: Use zip archive to preserve .next directory during artifact transfer
[33mfa7701a[m Add artifact download verification to identify where .next directory is lost
[33m4b5ba1b[m Add artifact verification step to catch when .next directory is lost
[33m138a7be[m Enhanced deployment verification to ensure .next directory is preserved
[33m10ade9a[m Fix: Install production dependencies before deployment to resolve missing 'next' module error
[33m6d061c8[m Add Azure deployment to working build workflow
[33m780ea3b[m Fix shell syntax by explicitly using cmd for verification steps
[33m70c0075[m Add working build workflow based on successful environment test
[33m1f97d3e[m Add environment test workflow to diagnose GitHub Actions issues
[33m9bf8405[m Add new Next.js build artifacts and update GitHub Actions workflow
[33m6ea6596[m Add build error debugging workflow
[33me2d0cfd[m Add debug build workflow for Next.js application
[33m0301200[m Enhance logging in main_taskflow_robust.yml for package.json checks
[33m7321bc3[m Replace main_taskflow.yml with main_taskflow_robust.yml for enhanced Azure deployment workflow
[33mb069a41[m Add GitHub Actions workflow for Node.js app deployment to Azure
[33m320639f[m Enhance GitHub Actions workflows for build and testing
[33m67a2d81[m Refactor Azure deployment process and enhance logging
[33mcfd3328[m Add Azure Web App deployment fixes and enhancements
[33m51c68b8[m Update package.json and add tatus file for project rebranding and dependency upgrades
[33m4271fcd[m Update package.json to trigger workflow
[33m451a060[m Refactor GitHub Actions workflow for Next.js build process
[33m5a4aded[m Refactor Next.js configuration to use ES6 module syntax
[33m57b0d07[m Add Next.js configuration and enhance server.cjs logging
[33m66b1857[m Refactor server.cjs to use CommonJS syntax for module imports
[33m7d26623[m Update web.config to reference server.cjs instead of server.js for improved compatibility with ES6 module syntax
[33m3038cf2[m Refactor Next.js server setup by introducing server.cjs and changing package.json module type to 'module'
[33mb6c3e18[m Change module type in package.json from ES6 to CommonJS for compatibility with existing codebase
[33mf648190[m Refactor server.js to use ES6 module syntax
[33mba051b9[m Fix typo in GitHub Actions workflow log message for clarity
[33ma5f4a52[m Enhance GitHub Actions workflow for Azure deployment with detailed build verification and cleanup steps
[33m3c9c5d8[m Resolve merge conflict - keep local Azure deployment config
[33m3f5658f[m Add Azure deployment configuration and Next.js startup files
[33m94900c7[m Add or update the Azure App Service build and deployment workflow config
[33m04f1dd4[m Enhance GitHub Actions workflow for Azure deployment with Kudu fallback
[33m937ad32[m Add server and ecosystem configuration for Next.js application
[33m5f8c882[m Add startup script for Next.js application and update web.config for Azure deployment
[33m08d6d01[m Enhance GitHub Actions workflow for Azure deployment by refining dependency management
[33md6a2637[m Update web.config for improved Azure deployment handling
[33ma6542e3[m Add web.config for Azure deployment and enhance GitHub Actions logging
[33me04598c[m Update GitHub Actions workflow to correct artifact upload and comment out debug steps
[33m5ffcbb6[m Enhance GitHub Actions workflow with additional logging and artifact upload correction
[33mfa9dbdd[m Enhance GitHub Actions workflow with improved cleanup verification and error handling
[33ma61a8eb[m Enhance GitHub Actions workflow with detailed cleanup and verification steps
[33m777f42a[m Enhance GitHub Actions workflow with verbose build output and improved error handling
[33mf1f65b1[m Enhance GitHub Actions workflow with improved build failure messaging and verification
[33m38cf6db[m Enhance GitHub Actions workflow with detailed logging and build verification steps
[33mad98b93[m Improve GitHub Actions workflow with enhanced build error handling and output verification
[33m748ca23[m Enhance GitHub Actions workflow with TypeScript compilation check and improved build output logging
[33m6a7a73e[m Refactor GitHub Actions workflow for improved build process and output verification
[33m0eb48e7[m Refactor login page component and enhance GitHub Actions workflow
[33m9efea44[m Enhance GitHub Actions workflow for frontend build and verification
[33m3b6e365[m Update Node.js version and refine cleanup process in GitHub Actions workflow
[33m71a2de7[m Add build output verification step in GitHub Actions workflow
[33ma84e300[m Add debug step to GitHub Actions workflow for deployment package
[33m4ac31a5[m Update cleanup step in GitHub Actions workflow to use cmd shell
[33m24518e9[m Refactor deployment cleanup process in GitHub Actions workflow
[33m5e046d8[m Refactor deployment package structure in GitHub Actions workflow
[33m8ebf99b[m Enhance GitHub Actions workflow for frontend deployment
[33m05dc48f[m Update GitHub Actions workflow for frontend deployment
[33m37c66ac[m Enhance GitHub Actions workflow for frontend deployment
[33mc05e79d[m Update deployment package path in GitHub Actions workflow
[33m494946d[m Update GitHub Actions workflow for frontend deplo