<p align="center">
  <img src="docs/logo_full_white.png" alt="Ao-Tomato" width="500">
</p>

---

**Ao-Tomato** is a simple, lightweight, free, and open-source automation server built on .NET. It uses **Lua** as its scripting language to define HTTP-triggered functions, and ships as a single self-contained container. No external databases, no complex dependencies -- just run and go.

### Why "Ao-Tomato"?

"Ao" (あお) means "blue" in Japanese, and "tomato" refers to the iconic round fruit. The name plays on the contrast between Ao (blue) and Tomato (red), I just made this up, in fact, it's just a pun with the word automato.

---

## Features

- **Lua-powered functions** -- Write automation logic in Lua. Each function is mapped to an HTTP route and method (GET/POST/PUT/PATCH/DELETE).
- **HTTP library** -- Call external APIs from within your Lua functions using the built-in `http` library.
- **Variables store** -- Persist key-value variables accessible from Lua via `variables.get` / `variables.set`.
- **API Key protection** -- Optionally protect functions with API keys (header `X-API-Key` or query param `api_key`).
- **Blazor Admin UI** -- Manage functions, variables, users, and settings through a responsive web interface with a built-in Monaco code editor.
- **JWT Authentication** -- Secure admin API with JWT tokens. Auto-generates keys on first run if not configured.
- **API documentation** -- Built-in Scalar OpenAPI docs at `/docs`.
- **Self-contained** -- Embedded LiteDB database, no external database required. Single volume for persistence.
- **Docker support** -- Ready-to-use Dockerfile, docker-compose.yml, and GitHub Actions CI/CD pipeline.

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (for building from source)
- [Docker](https://www.docker.com/) (for containerized deployment)

---

## Quick Start (Docker)

```bash
docker compose up -d
```

The server will be available at `http://localhost:7071`.

- **Admin UI**: `http://localhost:7071`
- **API Docs**: `http://localhost:7071/docs`

On first run, visit the admin UI and create your initial admin user.

---

## Quick Start (Manual)

```bash
cd src
dotnet restore AoTomato.slnx
dotnet run --project AoTomato.API/AoTomato.API.csproj
```

The server starts at `http://localhost:5142` in development mode (with Blazor WASM debugging enabled).

---

## Configuration

| Environment Variable | Default | Description |
|---|---|---|
| `ASPNETCORE_URLS` | `http://+:7071` | Server listen address |
| `DbSettings__ConnectionString` | `aotomato.db` | LiteDB file path. Use `/data/aotomato.db` in Docker. |
| `ASPNETCORE_ENVIRONMENT` | `Production` | ASP.NET environment (`Development` or `Production`) |

### Settings managed via Admin UI

JWT configuration (SecretKey, ClientKey, Audience, Issuer) is auto-generated on first login if not explicitly set. You can manage these and other settings through the admin panel at `/settings`.

---

## API Reference

### Public Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `GET` | `ao/{routeKey}` | API Key (optional) | Execute a Lua function via GET |
| `POST` | `ao/{routeKey}` | API Key (optional) | Execute a Lua function via POST |
| `PUT` | `ao/{routeKey}` | API Key (optional) | Execute a Lua function via PUT |
| `PATCH` | `ao/{routeKey}` | API Key (optional) | Execute a Lua function via PATCH |
| `DELETE` | `ao/{routeKey}` | API Key (optional) | Execute a Lua function via DELETE |

**API Key** can be passed as:
- Header: `X-API-Key: your-api-key`
- Query parameter: `?api_key=your-api-key`

The function URL can be copied directly from the admin UI (copy button on function cards and edit page). The format is `{baseUrl}/ao/{route}?api_key={key}`.

### Auth Endpoints

| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| `POST` | `v1/login` | Anonymous | Authenticate user, returns JWT |
| `GET` | `v1/login/initialCheck` | Anonymous | Check if fresh install |
| `POST` | `v1/login/initialSetup` | Anonymous | Create first admin user |

### Admin Endpoints (JWT required)

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `v1/functions` | List all functions |
| `GET/POST/PUT/DELETE` | `v1/function[/{id}]` | CRUD function |
| `GET` | `v1/variables` | List all variables |
| `GET/POST/PUT/DELETE` | `v1/variable[/{id}]` | CRUD variable |
| `GET` | `v1/users` | List all users |
| `GET/POST/PUT/DELETE` | `v1/user[/{id}]` | CRUD user |
| `GET` | `v1/settings` | List all settings |
| `GET/POST/PUT/DELETE` | `v1/setting[/{id}]` | CRUD setting |

### Lua Runtime Environment

When a function executes, the following globals are available:

| Global | Type | Description |
|--------|------|-------------|
| `ctx` | table | Request context (`ctx.method` = `"GET"`, `"POST"`, etc.) |
| `headers` | table | Request headers (key-value pairs) |
| `queryParameters` | table | URL query parameters (key-value pairs) |
| `body` | table | Request body (parsed JSON as Lua table), or `nil` |
| `response` | table | **Output table** -- set `response.status`, `response.body`, `response.headers` |

### Lua Libraries

#### `http.request(options)`

Make HTTP requests from within Lua functions.

```lua
local res = http.request({
    method = "GET",
    url = "https://api.example.com/data",
    headers = { ["Authorization"] = "Bearer token" }
})
response.body = res.body
response.status = res.status
response.headers = res.headers
```

| Option | Type | Description |
|--------|------|-------------|
| `method` | string | HTTP method (GET, POST, PUT, etc.) |
| `url` | string | Request URL |
| `headers` | table | Request headers |
| `body` | string | Request body |

Returns a table with `status`, `body`, and `headers`.

#### `variables.get(key)`

Retrieve a stored variable by key.

```lua
local apiKey = variables.get("my_api_key")
```

Returns the variable value as a string, or `nil` if not found.

#### `variables.set({key = key, value = value})`

Store a variable by key.

```lua
variables.set({ key = "my_api_key", value = "abc123" })
```

Returns `true` on success, `false` on failure.

#### `json.encode(value)` / `json.decode(string)`

Encode/decode JSON from Lua tables.

```lua
local payload = json.decode(body)
local result = json.encode({ status = 200, data = payload })
response.body = result
```

### Example: Webhook Receiver

```lua
-- Route: POST /ao/webhook
-- Stores the incoming payload and forwards to another service

local payload = json.encode(body)
variables.set({ key = "last_webhook", value = payload })

local forward = http.request({
    method = "POST",
    url = "https://api.example.com/process",
    headers = { ["Content-Type"] = "application/json" },
    body = payload
})

response.status = forward.status
response.body = forward.body
```

### Example: API Proxy

```lua
-- Route: GET /ao/proxy
-- Forwards all query params to a target API

local apiKey = variables.get("external_api_key")
local targetUrl = "https://api.example.com/data"

local res = http.request({
    method = ctx.method,
    url = targetUrl .. "?" .. (queryParameters and queryParameters or ""),
    headers = {
        ["Authorization"] = "Bearer " .. (apiKey or ""),
        ["Content-Type"] = "application/json"
    }
})

response.status = res.status
response.body = res.body
response.headers["X-Proxied-By"] = "ao-tomato"
```

---

## Project Structure

```
ao-tomato/
├── src/
│   ├── AoTomato.slnx                  # Solution file (.NET 10 XML format)
│   ├── AoTomato.API/                  # ASP.NET Core Web API + Blazor host
│   │   ├── Endpoints/                 # REST API endpoints
│   │   │   ├── Ao/                    # Public function execution (ao/{routeKey})
│   │   │   ├── Functions/             # Admin function CRUD
│   │   │   ├── Variables/             # Admin variable CRUD
│   │   │   ├── Users/                 # Admin user CRUD
│   │   │   ├── Settings/              # Admin settings CRUD
│   │   │   └── Login/                 # Authentication endpoints
│   │   └── Middlewares/               # Error handling, permissions
│   ├── AoTomato.Admin/                # Blazor WebAssembly admin UI
│   │   └── Pages/                     # Login, Functions, Variables, Users, Settings
│   ├── AoTomato.Services/             # Business logic, Lua execution engine
│   │   ├── Functions/                 # Function execution with NLua
│   │   ├── Helpers/                   # Lua-C# interop utilities
│   │   └── Login/                     # JWT authentication service
│   ├── AoTomato.Domain/               # Models, DTOs, interfaces, enums
│   └── AoTomato.Repositories/         # LiteDB data access layer
├── Dockerfile                         # Multi-stage Docker build
├── docker-compose.yml                 # Docker Compose with persistent volume
├── .dockerignore                      # Docker build exclusions
├── .github/workflows/
│   └── docker-publish.yml             # CI/CD: build & push to Docker Hub
└── docs/
    └── logo_full.png                  # Project logo
```

---

## Development

### Prerequisites

- .NET 10 SDK
- Visual Studio Code (recommended) with C# Dev Kit

The workspace includes VS Code configuration for debugging and building (`.vscode/launch.json`, `tasks.json`, `settings.json`).

### Build

```bash
cd src
dotnet build AoTomato.slnx
```

### Run

```bash
cd src
dotnet run --project AoTomato.API/AoTomato.API.csproj
```

### Docker Build

```bash
docker build -t aotomato .
```
