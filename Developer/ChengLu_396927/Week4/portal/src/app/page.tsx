import Link from "next/link";

export default function Home() {
  return (
    <main className="page landing">
      <p className="eyebrow">WEEK 4 · NEXT.JS + KENDO UI</p>
      <h1>Enterprise task management portal</h1>
      <p className="subtitle">
        A responsive management portal built with Next.js (App Router) and Kendo UI for React,
        backed by the ASP.NET Core Web API from Week 3.
      </p>
      <div className="landing-links">
        <Link className="landing-link primary" href="/tasks">
          Open the portal →
        </Link>
        <Link className="landing-link" href="/about">
          Read the notes
        </Link>
      </div>
    </main>
  );
}
