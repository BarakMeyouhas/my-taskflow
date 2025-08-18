import { NextRequest, NextResponse } from 'next/server';
import { authConfig } from '@/config/auth';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    const { username, password } = body;

    // Validate input!
    if (!username || !password) {
      return NextResponse.json(
        { message: 'Username and password are required' },
        { status: 400 }
      );
    }

    // Call the backend API
    const response = await fetch(`${authConfig.backendUrl}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, password }),
    });

    if (response.ok) {
      const data = await response.json();
      return NextResponse.json(
        { 
          message: 'Login successful',
          token: data.token,
          user: data.user
        },
        { status: 200 }
      );
    } else {
      const errorData = await response.json();
      return NextResponse.json(
        { message: errorData.message || 'Login failed' },
        { status: response.status }
      );
    }
  } catch (error) {
    console.error('Login error:', error);
    return NextResponse.json(
      { message: 'Internal server error' },
      { status: 500 }
    );
  }
}
