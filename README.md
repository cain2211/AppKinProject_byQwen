# ProjectFlow - Aplicación de Gestión de Proyectos

## Descripción
Aplicación web estilo Trello/ClickUp para el análisis y gestión de proyectos. Permite la creación de proyectos con tareas específicas, tiempos definidos, visualización en múltiples formatos (Gantt, tabla, tarjetas) y soporte multi-usuario con diferentes niveles de seguridad.

## Stack Tecnológico

### Frontend
- **Angular 17** - Framework principal
- **TypeScript** - Lenguaje de programación
- **RxJS** - Programación reactiva
- **Angular Material** - Componentes UI
- **Ngx-Gantt** - Gráficas de Gantt interactivas

### Backend
- **.NET Core 8** - Framework backend
- **Entity Framework Core 8** - ORM
- **SQL Server** - Base de datos
- **ASP.NET Core Identity** - Autenticación y autorización
- **JWT** - Tokens de autenticación

## Estructura del Proyecto

```
/workspace
├── frontend/          # Aplicación Angular 17
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/           # Servicios singleton, guards, interceptors
│   │   │   ├── shared/         # Componentes compartidos, pipes, directivas
│   │   │   ├── features/       # Módulos de funcionalidad
│   │   │   │   ├── auth/       # Autenticación y autorización
│   │   │   │   ├── projects/   # Gestión de proyectos
│   │   │   │   ├── tasks/      # Gestión de tareas
│   │   │   │   ├── users/      # Gestión de usuarios
│   │   │   │   └── dashboard/  # Panel principal
│   │   │   └── layouts/        # Layouts de la aplicación
│   │   ├── assets/             # Recursos estáticos
│   │   ├── environments/       # Configuraciones por entorno
│   │   └── styles/             # Estilos globales
│   ├── angular.json
│   ├── package.json
│   └── tsconfig.json
├── backend/           # API .NET Core 8
│   ├── Controllers/      # Controladores API
│   ├── Models/           # Modelos de dominio
│   ├── DTOs/             # Data Transfer Objects
│   ├── Services/         # Servicios de negocio
│   ├── Repositories/     # Repositorios de acceso a datos
│   ├── Data/             # Contexto de base de datos y migraciones
│   ├── Security/         # Autenticación, autorización, JWT
│   ├── Mappings/         # Configuración de AutoMapper
│   └── Properties/       # Configuración de la aplicación
└── README.md
```

## Características Principales

### Multi-usuario con Roles
- **Administrador**: Acceso completo al sistema
- **Manager**: Crear/editar proyectos, asignar tareas
- **Member**: Ver y editar tareas asignadas
- **Viewer**: Solo lectura

### Gestión de Proyectos
- Creación y edición de proyectos
- Asignación de miembros al proyecto
- Definición de fechas de inicio y fin
- Seguimiento de progreso

### Gestión de Tareas
- Creación de tareas con subtareas
- Asignación de responsables
- Prioridades y etiquetas
- Fechas de vencimiento
- Adjuntar archivos y comentarios

### Visualizaciones
- **Vista Kanban**: Tarjetas arrastrables por columnas
- **Vista Gantt**: Línea de tiempo interactiva con arrastre de fechas
- **Vista Tabla**: Lista detallada con filtros y ordenamiento
- **Vista Calendario**: Tareas por fecha

### Funcionalidades Avanzadas
- Arrastrar y soltar en vista Gantt para ajustar fechas
- Filtros personalizados por usuario, prioridad, estado
- Notificaciones en tiempo real
- Historial de cambios
- Exportación a PDF/Excel

## Instalación y Configuración

### Prerrequisitos
- Node.js 18+ y npm
- .NET SDK 8
- SQL Server 2019+ o Azure SQL
- Angular CLI 17

### Backend (.NET Core 8)

1. Navegar al directorio backend:
```bash
cd backend
```

2. Configurar la cadena de conexión en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ProjectFlow;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

3. Ejecutar migraciones para crear la base de datos:
```bash
dotnet ef database update
```

4. Iniciar la API:
```bash
dotnet run
```

La API estará disponible en `https://localhost:5001` y `http://localhost:5000`

### Frontend (Angular 17)

1. Navegar al directorio frontend:
```bash
cd frontend
```

2. Instalar dependencias:
```bash
npm install
```

3. Configurar el entorno en `src/environments/environment.ts`:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api'
};
```

4. Iniciar la aplicación:
```bash
ng serve
```

La aplicación estará disponible en `http://localhost:4200`

## Desarrollo

### Comandos Útiles

#### Backend
```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Actualizar base de datos
dotnet ef database update

# Ejecutar tests
dotnet test

# Build de producción
dotnet publish -c Release
```

#### Frontend
```bash
# Generar nuevo componente
ng generate component nombre-componente

# Generar nuevo servicio
ng generate service nombre-servicio

# Ejecutar tests
ng test

# Build de producción
ng build --configuration production

# Ejecutar linter
ng lint
```

## Modelo de Datos Principal

### Entidades
- **User**: Usuarios del sistema
- **Role**: Roles de seguridad
- **Project**: Proyectos
- **Task**: Tareas
- **TaskAssignment**: Asignaciones de tareas
- **Comment**: Comentarios en tareas
- **Attachment**: Archivos adjuntos
- **ProjectMember**: Miembros de proyecto
- **AuditLog**: Registro de auditoría

## Seguridad

- Autenticación basada en JWT
- Autorización basada en roles (RBAC)
- Password hashing con BCrypt
- CORS configurado para el frontend
- Validación de entrada de datos
- Protección contra ataques comunes (XSS, CSRF, SQL Injection)

## API Endpoints Principales

### Autenticación
- `POST /api/auth/register` - Registro de usuario
- `POST /api/auth/login` - Inicio de sesión
- `POST /api/auth/refresh` - Refrescar token

### Usuarios
- `GET /api/users` - Listar usuarios
- `GET /api/users/{id}` - Obtener usuario
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### Proyectos
- `GET /api/projects` - Listar proyectos
- `POST /api/projects` - Crear proyecto
- `GET /api/projects/{id}` - Obtener proyecto
- `PUT /api/projects/{id}` - Actualizar proyecto
- `DELETE /api/projects/{id}` - Eliminar proyecto

### Tareas
- `GET /api/tasks` - Listar tareas
- `POST /api/tasks` - Crear tarea
- `GET /api/tasks/{id}` - Obtener tarea
- `PUT /api/tasks/{id}` - Actualizar tarea
- `DELETE /api/tasks/{id}` - Eliminar tarea
- `PATCH /api/tasks/{id}/move` - Mover tarea (Kanban)
- `PATCH /api/tasks/{id}/dates` - Actualizar fechas (Gantt)

## Contribución

1. Fork el repositorio
2. Crear rama para feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit cambios (`git commit -m 'Añadir nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Abrir Pull Request

## Licencia

Este proyecto está bajo la licencia MIT.

## Contacto

Para preguntas o soporte, contactar al equipo de desarrollo.
