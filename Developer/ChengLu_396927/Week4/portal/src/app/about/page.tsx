// No data fetching here, so Next.js statically generates this page at build time (SSG).
export default function AboutPage() {
  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">WEEK 4 · KEY CONCEPTS</p>
        <h1>Notes on this portal</h1>
      </header>

      <section className="notes">
        <article>
          <h2>SSR vs. client-side hydration</h2>
          <p>
            The <code>/tasks</code> route is a Next.js Server Component with{" "}
            <code>export const dynamic = &quot;force-dynamic&quot;</code>. On every request the
            server calls the ASP.NET Core API and renders the initial HTML with the task list
            already in it. The <code>TaskManager</code> client component then hydrates in the
            browser, taking over interactivity (adding, editing, deleting) without a full page
            reload. This page, <code>/about</code>, fetches nothing, so Next.js renders it once at
            build time (SSG) and serves the same static HTML to every visitor.
          </p>
        </article>

        <article>
          <h2>Enterprise component libraries</h2>
          <p>
            The management portal uses <strong>Progress Telerik Kendo UI for React</strong> (
            <code>@progress/kendo-react-grid</code>, <code>kendo-react-form</code>,{" "}
            <code>kendo-react-dialogs</code>) to get a data grid, validated forms, and dialogs
            without hand-rolling them. DevExpress offers an equivalent React/Blazor suite
            (<code>DevExtreme DataGrid</code>, <code>dx-react-form</code>) with a very similar
            API shape&mdash;grids, schedulers, and pivot tables as drop-in components instead of
            custom UI.
          </p>
        </article>

        <article>
          <h2>&quot;Vibe coding&quot; with AI assistants</h2>
          <p>
            Most of this portal (the API client in <code>src/lib/api.ts</code>, the Kendo Grid
            wiring, and the Kendo Form validators in <code>TaskManager.tsx</code>) was scaffolded
            through prompt-driven, AI-assisted development with GitHub Copilot: describe the CRUD
            screen and validation rules in natural language, let the assistant generate the
            boilerplate against the existing Week 3 API contract, then review and refine the
            generated code by hand.
          </p>
        </article>
      </section>
    </main>
  );
}
