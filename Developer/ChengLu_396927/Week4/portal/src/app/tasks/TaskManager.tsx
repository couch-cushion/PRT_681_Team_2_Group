"use client";

import { useState } from "react";
import { Grid, GridColumn, GridToolbar, GridCellProps } from "@progress/kendo-react-grid";
import { Dialog } from "@progress/kendo-react-dialogs";
import { Form, Field, FormElement, FieldRenderProps } from "@progress/kendo-react-form";
import { Input, TextArea, Checkbox } from "@progress/kendo-react-inputs";
import { Button } from "@progress/kendo-react-buttons";
import { Loader } from "@progress/kendo-react-indicators";
import { createTask, deleteTask, updateTask } from "@/lib/api";
import type { TaskInput, TaskItem } from "@/lib/types";

interface TaskManagerProps {
  initialTasks: TaskItem[];
}

function validateTitle(value: string) {
  if (!value || !value.trim()) return "Title is required.";
  if (value.trim().length > 200) return "Title must be 200 characters or fewer.";
  return "";
}

function validateDescription(value: string) {
  if (value && value.length > 1000) return "Description must be 1000 characters or fewer.";
  return "";
}

// Kendo Field expects components that accept FieldRenderProps (value/onChange/validationMessage).
function TitleField(props: FieldRenderProps) {
  const { validationMessage, visited, label, ...rest } = props;
  return (
    <div className="k-form-field">
      <label className="k-label">{label}</label>
      <Input {...rest} />
      {visited && validationMessage && <div className="k-form-error">{validationMessage}</div>}
    </div>
  );
}

function DescriptionField(props: FieldRenderProps) {
  const { validationMessage, visited, label, ...rest } = props;
  return (
    <div className="k-form-field">
      <label className="k-label">{label}</label>
      <TextArea {...rest} rows={3} />
      {visited && validationMessage && <div className="k-form-error">{validationMessage}</div>}
    </div>
  );
}

function CompletedField(props: FieldRenderProps) {
  const { value, onChange, label, id } = props;
  return (
    <div className="k-form-field k-form-field--checkbox">
      <Checkbox id={id} checked={Boolean(value)} onChange={(event) => onChange({ value: event.value })} label={label} />
    </div>
  );
}

export default function TaskManager({ initialTasks }: TaskManagerProps) {
  const [tasks, setTasks] = useState<TaskItem[]>(initialTasks);
  const [editingTask, setEditingTask] = useState<TaskItem | null>(null);
  const [isDialogOpen, setDialogOpen] = useState(false);
  const [taskPendingDelete, setTaskPendingDelete] = useState<TaskItem | null>(null);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  function openCreateDialog() {
    setEditingTask(null);
    setError("");
    setDialogOpen(true);
  }

  function openEditDialog(task: TaskItem) {
    setEditingTask(task);
    setError("");
    setDialogOpen(true);
  }

  function closeDialog() {
    setDialogOpen(false);
    setEditingTask(null);
  }

  async function handleSubmit(dataItem: { [name: string]: unknown }) {
    const input: TaskInput = {
      title: String(dataItem.title ?? "").trim(),
      description: dataItem.description ? String(dataItem.description).trim() : "",
      isCompleted: Boolean(dataItem.isCompleted),
    };

    setSaving(true);
    setError("");
    try {
      if (editingTask) {
        await updateTask(editingTask.id, input);
        setTasks((current) =>
          current.map((task) => (task.id === editingTask.id ? { ...task, ...input } : task))
        );
      } else {
        const created = await createTask(input);
        setTasks((current) => [created, ...current]);
      }
      closeDialog();
    } catch {
      setError(editingTask ? "The task could not be updated." : "The task could not be added.");
    } finally {
      setSaving(false);
    }
  }

  async function confirmDelete() {
    if (!taskPendingDelete) return;
    setSaving(true);
    setError("");
    try {
      await deleteTask(taskPendingDelete.id);
      setTasks((current) => current.filter((task) => task.id !== taskPendingDelete.id));
      setTaskPendingDelete(null);
    } catch {
      setError("The task could not be deleted.");
    } finally {
      setSaving(false);
    }
  }

  function StatusCell(props: GridCellProps) {
    const task = props.dataItem as TaskItem;
    return (
      <td>
        <span className={`status-pill ${task.isCompleted ? "status-done" : "status-open"}`}>
          {task.isCompleted ? "Completed" : "Open"}
        </span>
      </td>
    );
  }

  function ActionsCell(props: GridCellProps) {
    const task = props.dataItem as TaskItem;
    return (
      <td>
        <Button size="small" onClick={() => openEditDialog(task)}>
          Edit
        </Button>{" "}
        <Button size="small" themeColor="error" fillMode="outline" onClick={() => setTaskPendingDelete(task)}>
          Delete
        </Button>
      </td>
    );
  }

  return (
    <div className="task-manager">
      {error && <div className="error-banner">{error}</div>}

      <Grid data={tasks} style={{ height: "auto" }}>
        <GridToolbar>
          <Button themeColor="primary" onClick={openCreateDialog}>
            + Add task
          </Button>
        </GridToolbar>
        <GridColumn field="title" title="Title" />
        <GridColumn field="description" title="Description" />
        <GridColumn field="isCompleted" title="Status" cells={{ data: StatusCell }} width={140} />
        <GridColumn field="createdAt" title="Created" width={200} />
        <GridColumn title="Actions" cells={{ data: ActionsCell }} width={180} sortable={false} filterable={false} />
      </Grid>

      {isDialogOpen && (
        <Dialog title={editingTask ? "Edit task" : "Add task"} onClose={closeDialog} width={480}>
          <Form
            initialValues={{
              title: editingTask?.title ?? "",
              description: editingTask?.description ?? "",
              isCompleted: editingTask?.isCompleted ?? false,
            }}
            onSubmit={handleSubmit}
            render={(formRenderProps) => (
              <FormElement>
                <Field
                  name="title"
                  label="Title"
                  component={TitleField}
                  validator={validateTitle}
                />
                <Field
                  name="description"
                  label="Description (optional)"
                  component={DescriptionField}
                  validator={validateDescription}
                />
                <Field name="isCompleted" label="Completed" component={CompletedField} />

                <div className="dialog-actions">
                  <Button type="button" onClick={closeDialog} disabled={saving}>
                    Cancel
                  </Button>
                  <Button themeColor="primary" type="submit" disabled={!formRenderProps.allowSubmit || saving}>
                    {saving ? <Loader size="small" type="pulsing" /> : "Save"}
                  </Button>
                </div>
              </FormElement>
            )}
          />
        </Dialog>
      )}

      {taskPendingDelete && (
        <Dialog title="Delete task" onClose={() => setTaskPendingDelete(null)} width={400}>
          <p>
            Are you sure you want to delete <strong>{taskPendingDelete.title}</strong>?
          </p>
          <div className="dialog-actions">
            <Button type="button" onClick={() => setTaskPendingDelete(null)} disabled={saving}>
              Cancel
            </Button>
            <Button themeColor="error" onClick={confirmDelete} disabled={saving}>
              {saving ? <Loader size="small" type="pulsing" /> : "Delete"}
            </Button>
          </div>
        </Dialog>
      )}
    </div>
  );
}
