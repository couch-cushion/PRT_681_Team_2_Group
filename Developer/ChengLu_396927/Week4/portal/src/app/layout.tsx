import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import "@progress/kendo-theme-default/dist/all.css";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Task Management Portal",
  description: "Week 4: Next.js + Kendo UI management portal backed by the Week 3 ASP.NET Core API.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en" className={`${geistSans.variable} ${geistMono.variable}`}>
      <body>{children}</body>
    </html>
  );
}
