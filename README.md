# Community Events - Task Management Web App

A community-based task management system for planning events. Members join an event, break its issues into tasks, and share the work fairly.

## Tech Stack
- **Backend**: ASP.NET Core 8 with Blazor Server
- **Database**: Entity Framework Core + SQLite
- **Auth**: ASP.NET Core Identity
- **Deployment**: Docker + Azure App Service

## Features
- User registration and event-based login
- Role-based permissions (Organizer vs Member)
- Issue tracking with task breakdown
- Status board view (Not Started, Working On, Completed)
- Task comments with threading
- Activity feed and member directory
- Fair work distribution algorithm ("Auto-Split Tasks")
- Responsive UI for desktop and mobile

## Getting Started (Local Development)

### Prerequisites
- .NET 8 SDK or Docker

### Run Locally
```bash
cd CommunityEvents
dotnet build
dotnet run
```

The app will be available at `https://localhost:7000`

### Create Database
```bash
dotnet ef database update
```

## Development with Codespaces
Open this repo in GitHub Codespaces - the dev container will have .NET 8 pre-installed.

```bash
cd CommunityEvents
dotnet build
dotnet run
```

## Project Structure
```
CommunityEvents/
├── Models/              # Entity models
├── Data/                # DbContext and migrations
├── Services/            # Business logic and authorization
├── Components/          # Blazor components
├── wwwroot/            # Static assets
├── Pages/              # Auth pages (Razor Pages)
└── Program.cs
```

## Database Schema
- **Event**: name, description, date, budget
- **Member**: name, email, skills, availability, role
- **Issue**: event, title, description, status
- **Task**: issue, title, description, effort, money value, assigned member, status
- **Comment**: task, author, text, timestamp
- **EventSettings**: effort weight, money weight (for auto-split)
- **AspNetUsers**: Identity users

## Roles & Permissions
- **Organizer**: Full event control (create, edit, delete events/issues/tasks; assign tasks; run auto-split)
- **Member**: View all tasks, edit own profile and assigned tasks, comment on tasks

Permissions are enforced server-side in the service layer.

## Running Tests
```bash
dotnet test
```

## Deployment

### Docker
```bash
docker build -t community-events .
docker run -p 8080:8080 -e "ASPNETCORE_ENVIRONMENT=Production" community-events
```

### Azure App Service
See DEPLOYMENT.md for full instructions.

---

**Status**: Initial scaffold
**Last Updated**: 2026-11-04
