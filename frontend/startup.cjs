#!/usr/bin/env node

const { spawn } = require('child_process');
const path = require('path');
const fs = require('fs');

console.log('Starting TaskFlow application...');
console.log('Current directory:', process.cwd());
console.log('Node version:', process.version);
console.log('NODE_ENV:', process.env.NODE_ENV);

// Check if .next directory exists
const nextDir = path.join(process.cwd(), '.next');

if (!fs.existsSync(nextDir)) {
  console.error('ERROR: .next directory not found!');
  console.error('This means the Next.js application was not built properly.');
  console.error('Available files in current directory:');
  try {
    const files = fs.readdirSync(process.cwd());
    console.error(files);
  } catch (err) {
    console.error('Could not read directory:', err.message);
  }
  process.exit(1);
}

console.log('✓ .next directory found');

// Check if dependencies are installed
const nodeModulesDir = path.join(process.cwd(), 'node_modules');
const nextPackageDir = path.join(nodeModulesDir, 'next');

if (!fs.existsSync(nextPackageDir)) {
  console.log('⚠️  Next.js dependencies not found, installing production dependencies...');
  
  try {
    // Install production dependencies
    const npmInstall = spawn('npm', ['ci', '--omit=dev', '--no-audit', '--no-fund'], {
      stdio: 'inherit',
      cwd: process.cwd()
    });
    
    npmInstall.on('exit', (code) => {
      if (code === 0) {
        console.log('✓ Production dependencies installed successfully');
        startServer();
      } else {
        console.error(`✗ Failed to install dependencies (exit code: ${code})`);
        process.exit(1);
      }
    });
    
    npmInstall.on('error', (err) => {
      console.error('Failed to run npm install:', err);
      process.exit(1);
    });
  } catch (err) {
    console.error('Failed to start dependency installation:', err);
    process.exit(1);
  }
} else {
  console.log('✓ Dependencies already installed');
  startServer();
}

function startServer() {
  console.log('✓ Starting Next.js server...');
  
  // Start the server
  const server = spawn('node', ['server.cjs'], {
    stdio: 'inherit',
    env: Object.assign({}, process.env, {
      NODE_ENV: 'production',
      PORT: process.env.PORT || 8080
    })
  });

  server.on('error', (err) => {
    console.error('Failed to start server:', err);
    process.exit(1);
  });

  server.on('exit', (code) => {
    console.log(`Server process exited with code ${code}`);
    process.exit(code);
  });

  // Handle process termination
  process.on('SIGTERM', () => {
    console.log('Received SIGTERM, shutting down gracefully...');
    server.kill('SIGTERM');
  });

  process.on('SIGINT', () => {
    console.log('Received SIGINT, shutting down gracefully...');
    server.kill('SIGINT');
  });
}
