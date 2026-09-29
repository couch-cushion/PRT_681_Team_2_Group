import type { Metadata } from "next";
import "@progress/kendo-theme-default/dist/all.css";
import "./globals.css";

export const metadata: Metadata = {
  title: "Northstar | Order overview",
  description: "Northstar enterprise order operations portal",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return <html lang="en"><body>{children}</body></html>;
}
