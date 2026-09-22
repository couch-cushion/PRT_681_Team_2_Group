export interface TaskItem {
  id: number;
  title: string;
  description?: string | null;
  isCompleted: boolean;
  createdAt: string;
}

export interface TaskInput {
  title: string;
  description?: string | null;
  isCompleted: boolean;
}
