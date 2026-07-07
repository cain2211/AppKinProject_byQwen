import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Project, ProjectDetail, CreateProjectDto, UpdateProjectDto, GanttData } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private readonly apiUrl = `${environment.apiUrl}/projects`;

  constructor(private http: HttpClient) {}

  getProjects(status?: string, includeMembers: boolean = false): Observable<Project[]> {
    let params = new HttpParams().set('includeMembers', includeMembers.toString());
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<Project[]>(this.apiUrl, { params });
  }

  getProject(id: string, includeTasks: boolean = false, includeMembers: boolean = false): Observable<ProjectDetail> {
    let params = new HttpParams()
      .set('includeTasks', includeTasks.toString())
      .set('includeMembers', includeMembers.toString());
    return this.http.get<ProjectDetail>(`${this.apiUrl}/${id}`, { params });
  }

  createProject(project: CreateProjectDto): Observable<ProjectDetail> {
    return this.http.post<ProjectDetail>(this.apiUrl, project);
  }

  updateProject(id: string, project: UpdateProjectDto): Observable<ProjectDetail> {
    return this.http.put<ProjectDetail>(`${this.apiUrl}/${id}`, project);
  }

  deleteProject(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  addMember(projectId: string, userId: string, role: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${projectId}/members`, { userId, role });
  }

  removeMember(projectId: string, userId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${projectId}/members/${userId}`);
  }

  getGanttData(id: string): Observable<GanttData> {
    return this.http.get<GanttData>(`${this.apiUrl}/${id}/gantt-data`);
  }
}
