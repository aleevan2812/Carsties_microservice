/** @type {import('next').NextConfig} */
const nextConfig = {
  experimental: {
    serverActions: true,
  },

  images: {
    domains: ["cdn.pixabay.com", "vinfastnewway.com.vn"],
  },
};

export default nextConfig;
