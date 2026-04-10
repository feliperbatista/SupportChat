import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  /* config options here */
  reactCompiler: true,
  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'csstoragefelipe.blob.core.windows.net'
      }
    ]
  }
};

export default nextConfig;
