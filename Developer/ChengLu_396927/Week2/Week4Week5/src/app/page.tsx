"use client";

import { useEffect, useState, type FormEvent } from "react";
import { Grid, GridColumn as Column, type GridCustomCellProps, type GridRowClickEvent } from "@progress/kendo-react-grid";

type Order = {
  id: string; customer: string; email: string; product: string; quantity: number;
  unitPrice: number; total: number; status: string; createdAt: string; emailWorkflowStarted: boolean;
};
type OrderForm = Omit<Order, "id" | "total" | "createdAt" | "emailWorkflowStarted">;

const apiUrl = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5206";
const blankOrder: OrderForm = { customer: "", email: "", product: "", quantity: 1, unitPrice: 0, status: "Pending" };
const money = new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 });

function MoneyCell(props: GridCustomCellProps) {
  return <td {...props.tdProps}>{money.format(Number(props.dataItem.total))}</td>;
}

function StatusCell(props: GridCustomCellProps) {
  const status = String(props.dataItem.status);
  return <td {...props.tdProps}><span className={`status status-${status}`}>{status}</span></td>;
}

function DateCell(props: GridCustomCellProps) {
  return <td {...props.tdProps}>{new Date(String(props.dataItem.createdAt)).toLocaleDateString("en-US")}</td>;
}

export default function Home() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [selected, setSelected] = useState<Order | null>(null);
  const [form, setForm] = useState<OrderForm>(blankOrder);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [notice, setNotice] = useState("");
  const [error, setError] = useState("");

  async function loadOrders() {
    setLoading(true);
    setError("");
    try {
      const response = await fetch(`${apiUrl}/api/orders/`, { cache: "no-store" });
      if (!response.ok) throw new Error(`API returned ${response.status}`);
      setOrders(await response.json() as Order[]);
    } catch {
      setError("Cannot connect to the Orders API. Start the backend and retry.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    let active = true;
    void fetch(`${apiUrl}/api/orders/`, { cache: "no-store" })
      .then(async (response) => {
        if (!response.ok) throw new Error(`API returned ${response.status}`);
        return await response.json() as Order[];
      })
      .then((data) => { if (active) setOrders(data); })
      .catch(() => { if (active) setError("Cannot connect to the Orders API. Start the backend and retry."); })
      .finally(() => { if (active) setLoading(false); });
    return () => { active = false; };
  }, []);

  const totalValue = orders.reduce((sum, order) => sum + order.total, 0);
  const openOrders = orders.filter((order) => order.status !== "Confirmed").length;

  function openCreate() {
    setEditingId(null); setForm(blankOrder); setError(""); setDialogOpen(true);
  }

  function openEdit() {
    if (!selected) return;
    setEditingId(selected.id);
    setForm({ customer: selected.customer, email: selected.email, product: selected.product,
      quantity: selected.quantity, unitPrice: selected.unitPrice, status: selected.status });
    setError(""); setDialogOpen(true);
  }

  async function saveOrder(event: FormEvent<HTMLFormElement>) {
    event.preventDefault(); setSaving(true); setError("");
    try {
      const response = await fetch(`${apiUrl}/api/orders/${editingId ?? ""}`, {
        method: editingId ? "PUT" : "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(form),
      });
      if (!response.ok) {
        const problem = await response.json().catch(() => null) as { errors?: Record<string, string[]> } | null;
        throw new Error(Object.values(problem?.errors ?? {}).flat().join("; ") || `Save failed (${response.status})`);
      }
      const result = await response.json() as Order;
      setDialogOpen(false); setSelected(null);
      setNotice(editingId ? "Order updated" : result.emailWorkflowStarted
        ? "Order created. The confirmation email workflow has started."
        : "Order created, but Temporal is unavailable; the email workflow was not started.");
      await loadOrders();
    } catch (saveError) {
      setError(saveError instanceof Error ? saveError.message : "Save failed. Please retry.");
    } finally { setSaving(false); }
  }

  async function deleteOrder() {
    if (!selected || !window.confirm(`Delete the order for ${selected.product}?`)) return;
    const response = await fetch(`${apiUrl}/api/orders/${selected.id}`, { method: "DELETE" });
    if (!response.ok) { setError("Delete failed. Refresh and retry."); return; }
    setNotice("Order deleted"); setSelected(null); await loadOrders();
  }

  return (
    <main className="workspace">
      <aside className="sidebar">
        <a className="brand" href="#orders" aria-label="Northstar home"><span className="brand-mark">N</span><span>northstar<span className="brand-dot">.</span></span></a>
        <div className="side-label">Workspace</div>
        <a className="nav-item active" href="#orders"><span className="nav-glyph">▦</span>Orders</a>
        <a className="nav-item" href="#overview"><span className="nav-glyph">◷</span>Workflows</a>
        <div className="sidebar-bottom"><span className="avatar">NS</span><span className="profile"><strong>Northstar Studio</strong><small>Administrator</small></span><span className="more">···</span></div>
      </aside>

      <section className="main-panel" id="orders">
        <header className="topbar"><div className="breadcrumbs"><span>Operations</span><span className="crumb-slash">/</span><strong>Orders</strong></div>
          <div className="top-actions"><span className="live-indicator"><i />All systems operational</span><button className="icon-button" title="Notifications" aria-label="Notifications">♧<b /></button></div>
        </header>
        <div className="content">
          <div className="page-heading"><div><div className="eyebrow">{new Date().toLocaleDateString("en-US", { weekday: "long", month: "long", day: "numeric" }).toUpperCase()}</div><h1>Order overview</h1><p>Manage orders, track fulfilment, and monitor confirmation workflows.</p></div>
            <button className="primary-button" onClick={openCreate}><span>＋</span> New order</button>
          </div>
          <div className="metrics" id="overview">
            <article className="metric"><div className="metric-top"><span>Order value</span><span className="metric-icon green">↗</span></div><strong>{money.format(totalValue)}</strong><small>Combined value of all orders</small></article>
            <article className="metric"><div className="metric-top"><span>Total orders</span><span className="metric-icon blue">▤</span></div><strong>{orders.length.toString().padStart(2, "0")}</strong><small>Across all order statuses</small></article>
            <article className="metric"><div className="metric-top"><span>Needs attention</span><span className="metric-icon coral">◷</span></div><strong>{openOrders.toString().padStart(2, "0")}</strong><small>Awaiting confirmation</small></article>
          </div>

          <section className="orders-section">
            <div className="section-head"><div><h2>All orders</h2><span className="count-pill">{orders.length}</span></div>
              <div className="table-actions"><button className="secondary-button" onClick={() => void loadOrders()} title="Refresh orders">↻ <span>Refresh</span></button><button className="secondary-button" disabled={!selected} onClick={openEdit}>Edit</button><button className="danger-button" disabled={!selected} onClick={() => void deleteOrder()}>Delete</button></div>
            </div>
            {(notice || error) && <div className={error ? "notice error-notice" : "notice"} role="status">{error || notice}<button onClick={() => { setNotice(""); setError(""); }} aria-label="Dismiss message">×</button></div>}
            <div className="grid-wrap">{loading ? <div className="loading-state"><span className="spinner" />Loading orders…</div> : (
              <Grid data={orders} dataItemKey="id" onRowClick={(event: GridRowClickEvent) => setSelected(event.dataItem as Order)}>
                <Column field="customer" title="CUSTOMER" width="165px" />
                <Column field="product" title="PRODUCT / SERVICE" />
                <Column field="quantity" title="QTY" width="75px" />
                <Column field="total" title="ORDER VALUE" width="135px" cells={{ data: MoneyCell }} />
                <Column field="status" title="STATUS" width="120px" cells={{ data: StatusCell }} />
                <Column field="createdAt" title="CREATED" width="125px" cells={{ data: DateCell }} />
              </Grid>
            )}</div>
            <div className="table-footer"><span>Showing {orders.length} orders</span><span>Select a row to edit or delete</span></div>
          </section>
          <footer className="page-footer"><span>Northstar Operations</span><span>API <code>localhost:5206</code> <i className={error ? "health-dot offline" : "health-dot"} /></span></footer>
        </div>
      </section>

      {dialogOpen && <div className="dialog-backdrop" role="presentation" onMouseDown={(event) => { if (event.target === event.currentTarget) setDialogOpen(false); }}>
        <section className="order-dialog" role="dialog" aria-modal="true" aria-labelledby="dialog-title">
          <div className="dialog-heading"><div><div className="eyebrow">ORDER WORKSPACE</div><h2 id="dialog-title">{editingId ? "Edit order" : "Create order"}</h2></div><button className="close-button" onClick={() => setDialogOpen(false)} aria-label="Close">×</button></div>
          <form onSubmit={saveOrder}>
            <label>Customer name<input required minLength={2} maxLength={100} value={form.customer} onChange={(event) => setForm({ ...form, customer: event.target.value })} placeholder="e.g. Jordan Lee" /></label>
            <label>Email address<input required type="email" maxLength={254} value={form.email} onChange={(event) => setForm({ ...form, email: event.target.value })} placeholder="name@company.com" /></label>
            <label>Product or service<input required minLength={2} maxLength={160} value={form.product} onChange={(event) => setForm({ ...form, product: event.target.value })} placeholder="Product name" /></label>
            <div className="form-row"><label>Quantity<input required type="number" min={1} max={10000} value={form.quantity} onChange={(event) => setForm({ ...form, quantity: Number(event.target.value) })} /></label><label>Unit price (USD)<input required type="number" min={0.01} max={1000000} step="0.01" value={form.unitPrice || ""} onChange={(event) => setForm({ ...form, unitPrice: Number(event.target.value) })} placeholder="0.00" /></label></div>
            <label>Order status<select value={form.status} onChange={(event) => setForm({ ...form, status: event.target.value })}><option>Pending</option><option>Processing</option><option>Confirmed</option></select></label>
            {error && <p className="form-error" role="alert">{error}</p>}
            {!editingId && <p className="workflow-hint">Creating this order starts a Temporal email confirmation workflow.</p>}
            <div className="dialog-actions"><button type="button" className="secondary-button" onClick={() => setDialogOpen(false)}>Cancel</button><button className="primary-button" disabled={saving}>{saving ? "Saving…" : editingId ? "Save changes" : "Create order"}</button></div>
          </form>
        </section>
      </div>}
    </main>
  );
}
