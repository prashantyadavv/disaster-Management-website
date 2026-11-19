# DisasterReady India 🛡️

**Smart India Hackathon 2024 — Problem Statement SIH1462**  
**Ministry of Home Affairs, Government of India**  
*Be Prepared. Stay Safe. Save Lives.*

---

## 📋 Prerequisites

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 10.0.x | https://dotnet.microsoft.com/download |
| SQL Server | 2019+ or LocalDB | https://www.microsoft.com/en-in/sql-server/sql-server-downloads |
| Visual Studio / VS Code | Latest | https://visualstudio.microsoft.com |

---

## 🚀 Setup & Run

### 1. Clone / Extract the project
```
c:\Users\prash\OneDrive\Desktop\DisasterManagement Project\DisasterReadyIndia\
```

### 2. Database

The development configuration is ready to run: it uses a self-contained SQLite
database (`disasterready-dev.db`) and automatically creates and seeds it on the
first launch. No database server or migration command is needed for the demo.

For a SQL Server deployment, edit `appsettings.json` or an environment-specific
configuration to set `DatabaseProvider` to `SqlServer`, then use a SQL Server
connection string such as:

**SQL Server Express**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=DisasterReadyIndia;Trusted_Connection=True;TrustServerCertificate=True;"
```

**LocalDB**
```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DisasterReadyIndia;Trusted_Connection=True;TrustServerCertificate=True;"
```

**Full SQL Server**
```json
"DefaultConnection": "Server=localhost;Database=DisasterReadyIndia;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 3. Apply SQL Server Migrations (SQL Server only)

```powershell
cd "c:\Users\prash\OneDrive\Desktop\DisasterManagement Project\DisasterReadyIndia\DisasterReadyIndia"
dotnet ef database update
```

For SQL Server deployments, this will:
- Create the `DisasterReadyIndia` database
- Create all tables
- Seed demo users, disaster guides, sample alerts and resources

### 4. Run the Application

```powershell
dotnet run
```

Open your browser at: **http://localhost:5234**

---

## 👥 Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| **Admin** | admin@disasterready.in | Admin@DisasterReady2024! |
| **Officer** | officer@disasterready.in | Officer@DisasterReady2024! |
| **Citizen** | citizen@disasterready.in | Citizen@DisasterReady2024! |

> ⚠️ These are demo credentials for SIH evaluation only. Change all passwords in production.

---

## 🗺️ Application Features

### Citizen Portal (`/`)
- 🏠 **Homepage** — Active alerts, disaster categories, quick actions
- 📚 **Disaster Guides** (`/Prepared`) — 8 disaster types with full safety content
- 🚨 **Report Incident** (`/Incident/Report`) — Submit geo-tagged incident reports
- 🔍 **Track Incident** (`/Incident/Track`) — Track your report by Incident ID
- 🔔 **Active Alerts** (`/Alerts`) — View live disaster alerts
- ✅ **Preparedness Checklist** (`/Checklist`) — Interactive readiness tracker
- 👨‍👩‍👧 **Family Emergency Plan** (`/FamilyPlan`) — Printable household plan
- 🏥 **Emergency Resources** (`/Prepared/Resources`) — Map-based resource finder
- 🤖 **Safety Assistant** (`/Assistant`) — Curated disaster Q&A

### Disaster Operations Center (`/Operations`) — Officers & Admins only
- 📊 **Dashboard** — Real-time incident stats from database
- 📋 **Incident Management** — Verify, reject, assign, track incidents
- 🎯 **Triage Engine** — Transparent rule-based incident prioritization
- 🔔 **Alert Management** — Issue and manage disaster alerts
- 📈 **Analytics** — Charts from live DB data (Chart.js)
- 🏥 **Resource Management** — Add and monitor emergency resources

---

## 🛠️ Technology Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core MVC 10.0 |
| ORM | Entity Framework Core 10 |
| Auth | ASP.NET Core Identity |
| Real-time | SignalR |
| Database | Microsoft SQL Server |
| Frontend | Bootstrap 5, Razor Views |
| Charts | Chart.js |
| Maps | Leaflet.js + OpenStreetMap |
| Logging | Serilog |

---

## 📁 Project Structure

```
DisasterReadyIndia/
├── Controllers/          # MVC Controllers for all modules
├── Data/
│   ├── ApplicationDbContext.cs
│   └── DatabaseSeeder.cs
├── Hubs/                 # SignalR hub
├── Middleware/           # Global exception handler
├── Migrations/           # EF Core migrations
├── Models/Entities/      # Domain models + Enums
├── Services/
│   ├── Interfaces/       # Service contracts + DTOs
│   └── *.cs              # Service implementations
├── Views/                # Razor views (citizen + ops)
└── wwwroot/              # Static files (CSS, JS, uploads)
```

---

## 📝 Important Notes

- All **sample data** (alerts, resources, guides) is clearly labelled as DEMONSTRATION DATA
- The **triage engine** is rule-based and transparent — not a scientific prediction model
- The **assistant** is a curated keyword-matching system — not an actual AI
- **File uploads** are limited to 5MB, JPG/PNG/GIF/PDF only
- **Maps** use OpenStreetMap (no API key required)
- Geolocation requires user permission and HTTPS in production

---

## 🏆 SIH Demo Workflow

1. Open homepage → view active alerts → click a disaster guide
2. Register as a citizen → submit an incident report → note the Incident ID
3. Track the incident using the Incident ID
4. Log in as officer → go to Operations Center
5. Verify the incident → assign to an officer → update status
6. Create a new disaster alert
7. Log in as citizen → view the alert
8. Use the Preparedness Checklist → create a Family Emergency Plan → print it
9. Visit the Emergency Resources map
10. Try the Safety Assistant

---

*DisasterReady India — SIH1462 | Ministry of Home Affairs, Government of India*
