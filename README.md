# Clinical Trials API

A .NET 8 RESTful API for uploading and processing JSON metadata files related to clinical trials.

## Features

- File upload with size and type validation (max 1MB)
- JSON schema validation
- Data transformation and normalization
- SQL Server database storage with automatic creation
- Swagger/OpenAPI documentation
- Docker and Docker Compose support
- Comprehensive unit tests
- Clean Architecture implementation

## Prerequisites

- .NET 8 SDK
- Docker and Docker Compose
- Visual Studio 2022 (optional, for debugging)

## Getting Started

### Using Docker Compose (Recommended)

1. Clone the repository
2. Run the application using Docker Compose:
```bash
docker-compose up -d
```

The API will be available at:
- API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- SQL Server: localhost:1433

### Local Development

1. Clone the repository
2. Update the connection string in `ClinicalTrialsApi.Api/appsettings.Development.json` if needed
3. Run the following commands:
```bash
dotnet restore
dotnet build
dotnet run --project ClinicalTrialsApi.Api
```

## API Endpoints

### Upload Trial
```http
POST /api/clinicaltrials/upload
Content-Type: multipart/form-data
```

### Get Trial by ID
```http
GET /api/clinicaltrials/{id}
```

### Get Trial by Trial ID
```http
GET /api/clinicaltrials/trial/{trialId}
```

### Get All Trials (with optional status filter)
```http
GET /api/clinicaltrials?status={status}
```
Status can be: "NotStarted", "Ongoing", "Completed"

## JSON Schema

The API accepts JSON files following this schema:
```json
{
  "trialId": "string",
  "title": "string",
  "startDate": "date",
  "endDate": "date",
  "participants": "integer",
  "status": "Not Started|Ongoing|Completed"
}
```

Required fields:
- trialId
- title
- startDate
- status

## Business Rules

1. Duration in days is automatically calculated when endDate is provided
2. For "Ongoing" trials without an endDate, it's set to one month from startDate

## Development

### Project Structure
- `ClinicalTrialsApi.Api` - API controllers and configuration
- `ClinicalTrialsApi.Core` - Domain models and interfaces
- `ClinicalTrialsApi.Infrastructure` - Implementation of services and repositories
- `ClinicalTrialsApi.Tests` - Unit tests

### Running Tests
```bash
dotnet test
```

### Debugging with Visual Studio
1. Set the docker-compose project as the startup project
2. Press F5 to start debugging
3. The application will build and start in Docker with debugger attached

## Database

The database is automatically created on first run. It uses:
- SQL Server 2022
- Entity Framework Core for data access
- Automatic migrations
- Retry policies for reliability

## Docker Support

The solution includes:
- Multi-stage Dockerfile for optimized images
- Docker Compose configuration for both API and SQL Server
- Volume mapping for database persistence
- Network configuration for service communication 