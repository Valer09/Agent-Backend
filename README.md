# AI Agent Backend

Backend REST in **.NET 9 / ASP.NET Core** con integrazione **Azure OpenAI / Azure AI Foundry**.  
Espone endpoint per interazione conversazionale (`ask`) e riepilogo strutturato (`summarize`) di testi tecnici.

***

## Architettura

```
Client
  │
  ▼
AgentController  (HTTP REST, validazione input, gestione errori HTTP)
  │
  ▼
AgentService     (logica LLM, logging strutturato, metriche di esecuzione)
  │
  ▼
Azure OpenAI / AI Foundry  (deployment GPT-4o o altro modello)
```

***

## Prerequisiti

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (opzionale)
- Un deployment attivo su **Azure OpenAI** o **Azure AI Foundry**

***

## Configurazione

### Variabili richieste

| Variabile | Descrizione |
|---|---|
| `AzureOpenAI__Endpoint` | URL del resource Azure OpenAI, es. `https://xxx.openai.azure.com/` |
| `AzureOpenAI__ApiKey` | API Key del resource |
| `AzureOpenAI__DeploymentName` | Nome del deployment (es. `gpt-4o`) |

### Configurazione locale (sviluppo)

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

> **Nota:** `appsettings.Development.json` è in `.gitignore`. Non committare chiavi reali.

***

## Avvio locale

```bash
dotnet restore
dotnet run
```

API disponibile su `http://localhost:5149` — Swagger UI: `http://localhost:5149/swagger`

***

## Avvio con Docker

```bash
docker build -t ai-agent-backend .

docker run -p 5149:8080 \
  -e AzureOpenAI__Endpoint="https://<your-resource>.openai.azure.com/" \
  -e AzureOpenAI__ApiKey="<your-api-key>" \
  -e AzureOpenAI__DeploymentName="<your-deployment>" \
  ai-agent-backend
```

Oppure con Docker Compose:
```bash
cp .env.example .env  # compila con i tuoi valori
docker compose up
```

***

## Endpoint

### `GET /agent/health`
```json
{ "status": "OK", "timestampUtc": "2026-06-26T10:00:00Z" }
```

### `POST /agent/ask`
**Request:** `{ "input": "Cos'è Azure AI Foundry?" }`

**Response 200:**
```json
{
  "output": "Azure AI Foundry è...",
  "operation": "ask",
  "model": "gpt-4o",
  "requestId": "a3f2b1c0",
  "timestampUtc": "2026-06-26T10:00:00.123Z",
  "elapsedMs": 1240
}
```

### `POST /agent/summarize`
Stesso schema di `/ask`, con `"operation": "summarize"`.

### Codici di errore

| Codice | Causa |
|---|---|
| 400 | Input mancante o non valido |
| 429 | Rate limit Azure superato |
| 502 | Errore restituito da Azure OpenAI |
| 500 | Errore interno non atteso |

***

## Test con curl

```bash
# Health
curl http://localhost:5149/agent/health

# Ask
curl -X POST http://localhost:5149/agent/ask \
  -H "Content-Type: application/json" \
  -d '{"input": "Cos'''è Azure AI Foundry?"}'

# Summarize
curl -X POST http://localhost:5149/agent/summarize \
  -H "Content-Type: application/json" \
  -d '{"input": "Testo da riassumere..."}'
```

Oppure usa [`AgentApi.http`](./AgentApi.http) in Visual Studio / Rider / VS Code (REST Client).

***

## Struttura del progetto

```
AI-Agent-Backend/
├── Configuration/
│   └── AzureOpenAIOptions.cs   # Options pattern con validazione startup
├── Controllers/
│   └── AgentController.cs      # Endpoint REST, gestione errori HTTP
├── Model/
│   ├── AgentRequest.cs         # Input con validazione DataAnnotations
│   └── AgentResponse.cs        # Output arricchito (operation, model, requestId, elapsed)
├── Service/
│   ├── IAgentService.cs
│   └── AgentService.cs         # Integrazione Azure OpenAI, logging strutturato
├── AgentApi.http               # Esempi chiamate HTTP per IDE
├── Dockerfile
├── docker-compose.yaml
└── README.md
```

***

## Sviluppi futuri

- Autenticazione tramite Azure Managed Identity (secretless)
- Streaming SSE per risposte in tempo reale
- System prompt personalizzabile dal client
- Rate limiting applicativo con `AspNetCoreRateLimit`
- Integrazione Azure Monitor / Application Insights