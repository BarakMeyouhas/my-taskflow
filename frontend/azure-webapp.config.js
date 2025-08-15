// Azure Web App Configuration
// This file provides additional configuration for Azure Web App deployment

module.exports = {
  // Node.js version
  nodeVersion: '20.x',
  
  // Startup command
  startupCommand: 'node startup.js',
  
  // Environment variables
  environmentVariables: {
    NODE_ENV: 'production',
    PORT: 8080,
    HOSTNAME: '0.0.0.0'
  },
  
  // Build configuration
  build: {
    command: 'npm run build',
    outputDirectory: '.next'
  },
  
  // Runtime configuration
  runtime: {
    type: 'node',
    version: '20.x'
  }
};
