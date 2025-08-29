# TeamSync

A modern web-based team collaboration and project management platform built with ASP.NET Core.

## Overview

TeamSync is a comprehensive project management solution that enables teams to organize projects, manage tasks, and collaborate effectively. The application provides an intuitive interface for tracking project progress and team activities.

## Features

- **Project Management**: Create and manage multiple projects with team collaboration
- **Task Tracking**: Assign tasks with status tracking (To Do, In Progress, Done)
- **User Management**: Role-based authentication and user profiles
- **Activity Logging**: Track project and task activities
- **Comments System**: Team communication on tasks and projects

## Tech Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap, jQuery

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB/Express)
- Visual Studio 2022 or VS Code

## Quick Start

1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run database migrations:
   ```
   Update-Database
   ```
4. Start the application

## Project Architecture

- **TeamCollabTool.Web**: MVC web application
- **TeamCollabTool.Data**: Data models and DbContext
- **TeamCollabTool.Services**: Business logic layer
- **TeamCollabTool.Repositories**: Data access patterns
