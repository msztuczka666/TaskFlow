# TaskFlow

TaskFlow is a comprehensive task and employee management system built with ASP.NET Core 8.0 MVC. It helps organizations manage tasks, track employee absences, and monitor certification expiration dates.

## Features

### Core Functionality
- **Employee Management (Pracownicy)**: Track employee information including main company, badge company, and SEP certification expiration
- **Absence Tracking (Nieobecnosci)**: Manage employee absences with support for multiple absence types (vacation, sick leave, training, etc.)
- **Task Management (Zlecenia)**: Create and track tasks with working day calculations
- **Role-Based Access Control**: Four user roles (Admin, Kierownik, Specjalista, Pracownik) with appropriate permissions

### Advanced Features
- **Dashboard**: Overview of key metrics including total employees, active tasks, and upcoming SEP expirations
- **Calendar View**: Visualize employee absences on a calendar
- **Reports**: 
  - Monthly absence summaries
  - SEP expiration warnings
  - Absence reports by type
- **Excel Import**: Import employee and absence data from Excel files
- **Automatic Calculations**: 
  - Working days calculation (Monday-Friday)
  - Automatic day reservation for specific absence types (e.g., 2 days for blood donation)

## Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQLite with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **UI**: Bootstrap 5
- **Excel Processing**: EPPlus

## Project Structure

```
TaskFlow/
├── Controllers/         # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── PracownicyController.cs
│   ├── NieobecnosciController.cs
│   ├── ZleceniaController.cs
│   ├── ReportsController.cs
│   └── ImportController.cs
├── Models/             # Entity models
│   ├── Pracownik.cs
│   ├── Nieobecnosc.cs
│   ├── Zlecenie.cs
│   └── ApplicationUser.cs
├── Data/              # Database context and seeding
│   ├── ApplicationDbContext.cs
│   └── DbSeeder.cs
├── Views/             # Razor views (to be created)
└── ViewModels/        # View models for forms
```

## Database Schema

### Pracownicy (Employees)
- Id, Imie, Nazwisko, FirmaGlowna, FirmaIdentyfikatorowa, DataWaznosciSEP

### Users (Application Users)
- Extends IdentityUser with Imie, Nazwisko

### Nieobecnosci (Absences)
- Id, PracownikId, DataOd, DataDo, TypNieobecnosci, Uwagi, LiczbaDni

### Zlecenia (Tasks)
- Id, Nazwa, Opis, DataRozpoczecia, DataZakonczenia, LiczbaDniRoboczych, Status

## Setup Instructions

1. **Prerequisites**:
   - .NET 8.0 SDK
   - Any IDE (Visual Studio, VS Code, or Rider)

2. **Clone the repository**:
   ```bash
   git clone https://github.com/msztuczka666/TaskFlow.git
   cd TaskFlow
   ```

3. **Restore packages**:
   ```bash
   dotnet restore
   ```

4. **Run migrations**:
   ```bash
   cd TaskFlow
   dotnet ef database update
   ```

5. **Run the application**:
   ```bash
   dotnet run
   ```

6. **Default Admin Credentials**:
   - Email: admin@taskflow.com
   - Password: Admin123!

## User Roles

- **Admin**: Full access to all features including user management
- **Kierownik**: Can manage employees, absences, and tasks
- **Specjalista**: Can manage absences and update task status
- **Pracownik**: View-only access to relevant information

## Business Rules

- Working days are calculated as Monday-Friday
- Certain absence types (e.g., blood donation) have automatic day reservations
- SEP certification warnings are shown 30 days before expiration
- Cache invalidation occurs upon updates to absences or tasks

## Future Enhancements

- Gantt chart visualization for tasks
- Dynamic exports to PDF and Excel
- Email notifications for SEP expirations
- Mobile-responsive UI improvements
- Integration with external HR systems

