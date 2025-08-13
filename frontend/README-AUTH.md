# Authentication Setup for TaskFlow

This document explains how to set up and use the authentication system in TaskFlow.

## Features

- **User Registration**: Create new accounts with username, email, and password
- **User Login**: Sign in with existing credentials
- **Form Validation**: Client-side and server-side validation
- **Error Handling**: Comprehensive error messages and user feedback
- **Responsive Design**: Mobile-friendly authentication forms

## Pages

### `/register` - User Registration
- Username (minimum 3 characters)
- Email address (with validation)
- Password (minimum 6 characters)
- Password confirmation
- Form validation with real-time error clearing
- Success redirect to login page

### `/login` - User Authentication
- Username/email field
- Password field
- Remember me checkbox
- Forgot password link
- Success message display from registration
- JWT token storage in localStorage

## API Routes

### `/api/auth/register`
- **Method**: POST
- **Body**: `{ username, email, password }`
- **Response**: Success message or error details

### `/api/auth/login`
- **Method**: POST
- **Body**: `{ username, password }`
- **Response**: JWT token, user data, and success message

## Environment Configuration

Create a `.env.local` file in the frontend directory:

```bash
# Backend API URL
BACKEND_URL=http://localhost:5225

# Next.js configuration
NEXT_PUBLIC_APP_NAME=TaskFlow
```

## Backend Integration

The authentication system is designed to work with your existing .NET backend:

1. **Register Endpoint**: `POST /api/auth/register`
2. **Login Endpoint**: `POST /api/auth/login`
3. **JWT Authentication**: Token-based authentication system

## Styling

The authentication pages use the same design system as the main application:
- Tailwind CSS for styling
- Blue to purple gradient theme
- Consistent spacing and typography
- Responsive design patterns
- Form validation styling

## Security Features

- Password confirmation validation
- Input sanitization
- Secure token storage
- Form validation on both client and server
- Error handling without exposing sensitive information

## Usage

1. Users can navigate to `/register` to create new accounts
2. After successful registration, they're redirected to `/login`
3. Users can sign in with their credentials
4. Successful login stores the JWT token and redirects to dashboard
5. The token can be used for authenticated API requests

## Customization

To customize the authentication system:

1. Modify the form validation rules in the page components
2. Update the API endpoints in the route handlers
3. Customize the styling by modifying the Tailwind classes
4. Add additional fields or validation as needed

## Testing

The authentication system includes:
- Form validation testing
- API endpoint testing
- Error handling testing
- Responsive design testing

## Future Enhancements

- Password strength indicator
- Two-factor authentication
- Social login integration
- Password reset functionality
- Account verification emails
- Session management
- Remember me functionality
