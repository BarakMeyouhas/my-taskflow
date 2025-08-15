import { NextRequest, NextResponse } from 'next/server';
import { authConfig } from '@/config/auth';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { username, email, password } = body;

    // Validate input
    if (!username || !email || !password) {
      return NextResponse.json(
        { message: 'Username, email, and password are required' },
        { status: 400 }
      );
    }

    // Call the backend API
    const response = await fetch(`${authConfig.backendUrl}/api/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, email, password }),
    });

    if (response.ok) {
      return NextResponse.json(
        { message: 'User registered successfully' },
        { status: 201 }
      );
    } else {
      const errorData = await response.json();
      return NextResponse.json(
        { message: errorData.message || 'Registration failed' },
        { status: response.status }
      );
    }
  } catch (error) {
    console.error('Registration error:', error);
    return NextResponse.json(
      { message: 'Internal server error' },
      { status: 500 }
    );
  }
}
