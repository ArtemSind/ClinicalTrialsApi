# Clinical Trials API

A .NET 8 RESTful API for uploading and processing JSON metadata files related to clinical trials.

## Features

- File upload with size and type validation
- JSON schema validation
- Data transformation and normalization
- SQL Server database storage
- Swagger/OpenAPI documentation
- Docker support
- Unit tests

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)
- Docker and Docker Compose (optional)

## Getting Started

1. Clone the repository
2. Update the connection string in `ClinicalTrialsApi.Api/appsettings.json` if needed
3. Run the following commands from the solution directory:

```bash
dotnet restore
dotnet build
dotnet run --project ClinicalTrialsApi.Api
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

## Docker Support

### Using Docker Compose (Recommended)

The easiest way to run the application is using Docker Compose, which will set up both the API and SQL Server:

```bash
docker-compose up -d
```

The API will be available at:
- HTTP: http://localhost:8080
- HTTPS: http://localhost:8081

SQL Server will be available at:
- Host: localhost
- Port: 1433
- User: sa
- Password: YourStrong!Passw0rd

### Using Docker Only

To build and run only the API container:

```bash
docker build -t clinical-trials-api .
docker run -p 8080:80 clinical-trials-api
```

## API Endpoints

- POST /api/clinicaltrials/upload - Upload JSON file
- GET /api/clinicaltrials/{id} - Get trial by ID
- GET /api/clinicaltrials/trial/{trialId} - Get trial by trial ID
- GET /api/clinicaltrials?status={status} - Get all trials with optional status filter

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

Required fields: trialId, title, startDate, status

## Running Tests

```bash
dotnet test
```

## Business Rules

1. Duration is automatically calculated when endDate is provided
2. For "Ongoing" trials without an endDate, it's set to one month from startDate 