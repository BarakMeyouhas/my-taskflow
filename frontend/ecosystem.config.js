module.exports = {
  apps: [
    {
      name: 'taskflow',
      script: 'npm',
      args: 'start',
      cwd: './',
      instances: 1,
      autorestart: true,
      watch: false,
      max_memory_restart: '1G',
      env: {
        NODE_ENV: 'production',
        PORT: process.env.PORT || 8080,
        HOSTNAME: '0.0.0.0'
      }
    }
  ]
};
