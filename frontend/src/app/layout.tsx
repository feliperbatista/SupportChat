import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "Support Chat",
  description: "WhatsApp Support Panel",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
    >
      <body className="bg-wa-bg text-wa-text h-screen overflow-hidden">{children}</body>
    </html>
  );
}