import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  experimental: {
    esmExternals: true,
  },
  eslint: {
    ignoreDuringBuilds: true,
  },
  /* config options here */
};

export default nextConfig;
