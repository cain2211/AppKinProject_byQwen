export interface User {
  id: string;
  email: string;
  firstName?: string;
  lastName?: string;
  roles: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
  user: {
    userId: string;
    email: string;
    roles: string[];
  };
}

export interface Project {
  id: string;
  name: string;
  description?: string;
  status: ProjectStatus;
  startDate: Date;
  endDate: Date;
  createdDate: Date;
  createdBy?: string;
  tasksCount?: number;
  completedTasksCount?: number;
}

export type ProjectStatus = 'Planning' | 'InProgress' | 'OnHold' | 'Completed' | 'Cancelled';

export interface ProjectDetail extends Project {
  tasks?: Task[];
  members?: ProjectMember[];
}

export interface ProjectMember {
  id: string;
  userId: string;
  userName: string;
  email: string;
  role: MemberRole;
}

export type MemberRole = 'Viewer' | 'Member' | 'Manager' | 'Admin';

export interface Task {
  id: string;
  title: string;
  description?: string;
  status: TaskStatus;
  priority: Priority;
  startDate: Date;
  endDate: Date;
  estimatedHours?: number;
  actualHours?: number;
  order: number;
  projectId: string;
  parentTaskId?: string;
  createdBy?: string;
  createdDate: Date;
  assignedUsers?: string[];
  subTasksCount?: number;
  commentsCount?: number;
}

export type TaskStatus = 'ToDo' | 'InProgress' | 'InReview' | 'Testing' | 'Done' | 'Blocked';

export type Priority = 'Low' | 'Medium' | 'High' | 'Critical';

export interface GanttTask {
  id: string;
  title: string;
  startDate: Date;
  endDate: Date;
  status: TaskStatus;
  priority: Priority;
  parentTaskId?: string;
  progress: number;
  assignedUsers?: string[];
}

export interface GanttData {
  projectId: string;
  projectName: string;
  startDate: Date;
  endDate: Date;
  tasks: GanttTask[];
}

export interface CreateProjectDto {
  name: string;
  description?: string;
  startDate: Date;
  endDate: Date;
}

export interface UpdateProjectDto {
  name?: string;
  description?: string;
  status?: ProjectStatus;
  startDate?: Date;
  endDate?: Date;
}

export interface CreateTaskDto {
  title: string;
  description?: string;
  priority: Priority;
  startDate: Date;
  endDate: Date;
  estimatedHours?: number;
  projectId: string;
  parentTaskId?: string;
  assignedUserIds?: string[];
}

export interface UpdateTaskDto {
  title?: string;
  description?: string;
  status?: TaskStatus;
  priority?: Priority;
  startDate?: Date;
  endDate?: Date;
  estimatedHours?: number;
  actualHours?: number;
}

export interface MoveTaskDto {
  status: TaskStatus;
  order: number;
  newProjectId?: string;
}
