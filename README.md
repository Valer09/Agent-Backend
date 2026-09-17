# ASP.NET Core REST Backend

REST backend built with **.NET 10 / ASP.NET Core**, integrating **Azure OpenAI** for text Q&A and summarization.

The project focuses on backend concerns such as API design, configuration, dependency injection, external-service integration, structured logging, error handling, containerization, and automated testing.

## Architecture

```text
Client
  |
  v
AgentController   HTTP endpoints, validation, error mapping
  |
  v
AgentService      Azure OpenAI integration, logging, execution metrics
  |
  v
Azure OpenAI      deployed language model
```

## Tech Stack

- C# / .NET 10
- ASP.NET Core
- Azure OpenAI
- Azure Key Vault integration for production configuration
- Docker / Docker Compose
- GitHub Actions
- Swagger / OpenAPI
- xUnit / WebApplicationFactory

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (optional)
- An Azure OpenAI deployment

## Configuration

Required settings:

| Variable | Description |
|---|---|
| `AzureOpenAI__Endpoint` | Azure OpenAI resource endpoint |
| `AzureOpenAI__ApiKey` | Azure OpenAI API key |
| `AzureOpenAI__DeploymentName` | Model deployment name |

For local development, use `appsettings.Development.json` or .NET User Secrets. Do not commit real credentials.

```bash
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"
```

## Run locally

```bash
dotnet restore
dotnet run
```

By default, the API is available at `http://localhost:5149` and Swagger UI at:
`http://localhost:5149/swagger`

## Run with Docker

```bash
docker build -t aspnetcore-rest-backend .

docker run -p 5149:8080 \
  -e AzureOpenAI__Endpoint="https://<your-resource>.openai.azure.com/" \
  -e AzureOpenAI__ApiKey="<your-api-key>" \
  -e AzureOpenAI__DeploymentName="<your-deployment>" \
  aspnetcore-rest-backend
```

Or with Docker Compose:

```bash
cp .env.example .env
docker compose up
```

## API

### `GET /agent/health`

Returns the application health status.

### `POST /agent/ask`

Sends a question to the configured language model.

```json
{ "input": "What is Azure AI Foundry?" }
```

### `POST /agent/summarize`

Summarizes the provided text into key points.

### Error handling

The API maps validation errors and upstream Azure OpenAI failures to explicit HTTP responses, including `400`, `429`, `502`, and `500`.

## Testing

The test project uses `WebApplicationFactory` to exercise the HTTP API pipeline with a fake service implementation, covering health checks, valid requests, validation failures, and error handling without calling Azure OpenAI.

Run tests with:

```bash
dotnet test
```

## Project structure

```text
ASP.NET Core REST Backend/
├── Configuration/       # Azure OpenAI options and validation
├── Controllers/         # REST endpoints
├── Model/               # Request/response models
├── Service/             # Business logic and Azure OpenAI integration
├── AgentApi.http        # Example HTTP requests
├── Dockerfile
├── docker-compose.yaml
└── README.md
```

## Deployment

The application can be containerized and deployed to **Azure Container Apps**. Production configuration also supports loading secrets from **Azure Key Vault** via Azure identity credentials.
