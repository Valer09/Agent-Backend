# AI Agent Backend

REST backend built with **.NET 9 / ASP.NET Core**, integrated with **Azure OpenAI / Azure AI Foundry**.  
Exposes endpoints for conversational interaction (`ask`) and structured summarization (`summarize`) of technical content.

***

## Architecture

```
Client
  │
  ▼
AgentController  (HTTP REST, input validation, HTTP error handling)
  │
  ▼
AgentService     (LLM logic, structured logging, execution metrics)
  │
  ▼
Azure OpenAI / AI Foundry  (GPT-4o or other deployed model)
```

***

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (optional)
- An active deployment on **Azure OpenAI** or **Azure AI Foundry**

***

## Configuration

### Required variables

| Variable | Description |
|---|---|
| `AzureOpenAI__Endpoint` | Azure OpenAI resource URL, e.g. `https://xxx.openai.azure.com/` |
| `AzureOpenAI__ApiKey` | Resource API Key |
| `AzureOpenAI__DeploymentName` | Deployment name (e.g. `gpt-4o`) |

### Local development

In `appsettings.Development.json`:
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://<your-resource>.openai.azure.com/",
    "ApiKey": "<your-api-key>",
    "DeploymentName": "<your-deployment>"
  }
}
```

> **Note:** `appsettings.Development.json` is listed in `.gitignore`. Never commit real credentials.

Alternatively, use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) to store the API key outside the project directory:
```bash
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"
```

***

## Run locally

```bash
dotnet restore
dotnet run
```

API available at `http://localhost:5149` — Swagger UI: `http://localhost:5149/swagger`

***

## Run with Docker

```bash
# Build image
docker build -t ai-agent-backend .

# Run with environment variables
docker run -p 5149:8080 \
  -e AzureOpenAI__Endpoint="https://<your-resource>.openai.azure.com/" \
  -e AzureOpenAI__ApiKey="<your-api-key>" \
  -e AzureOpenAI__DeploymentName="<your-deployment>" \
  ai-agent-backend
```

Or with Docker Compose:
```bash
cp .env.example .env  # fill in your values
docker compose up
```

***

## Endpoints

### `GET /agent/health`

```json
{ "status": "OK", "timestampUtc": "2026-06-26T10:00:00Z" }
```

### `POST /agent/ask`

Sends a question to the AI model.

**Request:**
```json
{ "input": "What is Azure AI Foundry?" }
```

**Response 200:**
```json
{
  "output": "Azure AI Foundry is a unified Microsoft platform...",
  "operation": "ask",
  "model": "gpt-4o",
  "requestId": "a3f2b1c0",
  "timestampUtc": "2026-06-26T10:00:00.123Z",
  "elapsedMs": 1240
}
```

### `POST /agent/summarize`

Summarizes a text into 5 key points. Same response schema as `/ask`, with `"operation": "summarize"`.

### Error codes

| Code | Cause |
|---|---|
| 400 | Missing or invalid input |
| 429 | Azure OpenAI rate limit exceeded |
| 502 | Error returned by Azure OpenAI |
| 500 | Unexpected internal error |

***

## Testing the endpoints

Use the [`AgentApi.http`](./AgentApi.http) file in Visual Studio, Rider, or VS Code ([REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension).

Or with curl:
```bash
# Health
curl http://localhost:5149/agent/health

# Ask
curl -X POST http://localhost:5149/agent/ask \
  -H "Content-Type: application/json" \
  -d '{"input": "What is Azure AI Foundry?"}'

# Summarize
curl -X POST http://localhost:5149/agent/summarize \
  -H "Content-Type: application/json" \
  -d '{"input": "Text to summarize..."}'
```

***

## Project structure

```
AI-Agent-Backend/
├── Configuration/
│   └── AzureOpenAIOptions.cs   # Options pattern with startup validation
├── Controllers/
│   └── AgentController.cs      # REST endpoints, HTTP error handling
├── Model/
│   ├── AgentRequest.cs         # Input with DataAnnotations validation
│   └── AgentResponse.cs        # Enriched output (operation, model, requestId, elapsed)
├── Service/
│   ├── IAgentService.cs
│   └── AgentService.cs         # Azure OpenAI integration, structured logging
├── AgentApi.http               # HTTP request examples for IDE
├── Dockerfile
├── docker-compose.yaml
└── README.md
```

***

## Future improvements

- Authentication via Azure Managed Identity (secretless approach)
- Server-Sent Events (SSE) streaming for real-time responses
- Client-configurable system prompt
- Application-level rate limiting with `AspNetCoreRateLimit`
- Azure Monitor / Application Insights integration