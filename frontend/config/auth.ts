export const authConfig = {
  // Backend API URL - update this to match your backend
  backendUrl: process.env.BACKEND_URL || 'https://taskflowbackend-g3anf0c9g5eac9ce.westeurope-01.azurewebsites.net',
  
  // App name
  appName: process.env.NEXT_PUBLIC_APP_NAME || 'TaskFlow',
  
  // Token storage key
  tokenKey: 'taskflow_token',
  
  // Auth endpoints
  endpoints: {
    register: '/api/auth/register',
    login: '/api/auth/login',
    verify: '/api/auth/verify',
  },
  
  // Protected routes that require authentication
  protectedRoutes: [
    '/',
    '/projects',
    '/calendar',
    '/reports',
  ],
  
  // Public routes that don't require authentication
  publicRoutes: [
    '/login',
    '/register',
  ],
};
