# Ao-Tomato: AI Context for Lua Function Implementation

This document provides all context needed by an AI agent to implement Lua functions for the **Ao-Tomato** automation server.

---

## 1. Project Overview

**Ao-Tomato** is a lightweight, self-contained automation server built on .NET. It exposes HTTP endpoints that execute user-defined **Lua** scripts. Each function is mapped to a unique **route + HTTP method** combination.

- **Lua Engine**: NLua v1.7.9 (C# binding for Lua 5.4, standard Lua 5.4 syntax and features apply)
- **Database**: LiteDB (embedded NoSQL, file-based)
- **Deployment**: Single Docker container, no external dependencies

### Architecture Flow

```
HTTP Request (GET ao/myRoute)
  → ASP.NET Core Endpoint
    → FunctionsService.ExecuteFunctionAsync()
      → Creates sandboxed Lua state
      → Injects context globals (ctx, headers, queryParameters, body, response)
      → Registers custom libraries (json, http, variables, log)
      → Executes lua.DoString(function.Code)
      → Extracts response table
      → Returns HTTP response
```

---

## 2. Function Model

Each function is defined by these properties:

| Property    | Type   | Description                                                      |
|------------ |--------|------------------------------------------------------------------|
| `name`      | string | Human-readable name                                              |
| `description` | string | Optional description                                             |
| `route`     | string | URL route key (alphanumeric, hyphens). Triggered at `ao/{route}` |
| `method`    | enum   | HTTP method: `"GET"`, `"POST"`, `"PUT"`, `"DELETE"`, `"PATCH"`  |
| `code`      | string | Lua source code (your implementation)                            |
| `apiKey`    | string | Optional API key for access control                              |
| `cron`      | string | Optional cron expression (future feature)                        |

### URL Format

```
{baseUrl}/ao/{route}?api_key={optionalKey}
```

### API Key Protection

If `apiKey` is set on the function, requests must include it:
- Header: `X-API-Key: your-api-key`
- Query param: `?api_key=your-api-key`

Requests with missing/invalid API key receive a **401 Unauthorized**.

---

## 3. Lua Execution Environment

### 3.1 Runtime Characteristics

- **NLua 1.7.9** using **Lua 5.4**
- **Sandboxed**: Each execution gets a fresh Lua state
- **Standard libraries**: `base`, `string`, `table`, `math`, `coroutine`, `package`, `io`, `os`, `debug` are loaded via `lua.openlibs()`
- **No persistence between executions** (except through `variables` library)
- **UTF-8 encoding**
- Execution has a timeout (ASP.NET request timeout)

### 3.2 Request Globals (Injected BEFORE your code runs)

These are available as global variables in your Lua script:

#### `ctx` (table)
Request context information.

| Field          | Type   | Description                           |
|--------------- |--------|---------------------------------------|
| `ctx.method`   | string | HTTP method: `"GET"`, `"POST"`, etc. |

#### `headers` (table)
Request headers as key-value pairs. Keys are case-sensitive as sent by the client.

```lua
local auth = headers["Authorization"]
local contentType = headers["Content-Type"]
local userAgent = headers["User-Agent"]
```

#### `queryParameters` (table)
URL query string parameters as key-value pairs.

```lua
local page = queryParameters["page"]
local limit = queryParameters["limit"]
local filter = queryParameters["filter"]
```

#### `body` (table or nil)
Parsed request body.
- If the request has a JSON body, it's parsed into a Lua table (objects become dictionaries, arrays become array tables)
- If there is no body (e.g., GET requests), `body` is `nil`

```lua
if body ~= nil then
    local name = body["name"]          -- object field access
    local items = body["items"]        -- array access
    local firstItem = items[1]         -- Lua arrays are 1-indexed!
end
```

### 3.3 Response Global (Output)

#### `response` (table)
**This is the output table.** It is pre-created with defaults before your code runs. Set its fields to define the HTTP response.

| Field                    | Type   | Default    | Description                                      |
|--------------------------|--------|------------|--------------------------------------------------|
| `response.status`        | number | `200`      | HTTP status code                                 |
| `response.body`          | string | `""`       | Response body (should be a string, typically JSON) |
| `response.headers`       | table  | `{}` (empty) | Response headers (key-value table)               |

```lua
response.status = 201
response.body = '{"message": "Created", "id": 42}'
response.headers["Content-Type"] = "application/json"
response.headers["X-Custom-Header"] = "some-value"
```

**Important**: `response.body` must be a **string**. Use `json.encode()` to convert Lua tables to JSON strings before assigning.

---

## 4. Built-in Lua Libraries (Custom APIs)

### 4.1 `json` Library

Provides JSON encoding/decoding within Lua. Implemented in pure Lua and injected at runtime.

#### `json.encode(value)`
Converts a Lua value (table, string, number, boolean, nil) to a JSON string.

```lua
local result = json.encode({ name = "John", age = 30 })
-- result = '{"name":"John","age":30}'

local arr = json.encode({ "a", "b", "c" })
-- arr = '["a","b","c"]'
```

**Behavior:**
- Lua tables with sequential integer keys starting at 1 become JSON arrays
- Lua tables with string or non-sequential keys become JSON objects
- `nil` values become `null` in JSON
- Strings are properly escaped

#### `json.decode(string)`
Parses a JSON string into a Lua table.

```lua
local data = json.decode('{"name": "John", "items": [1, 2, 3]}')
-- data["name"] == "John"
-- data["items"][1] == 1
-- data["items"][2] == 2
```

Returns `nil` if the string is empty or invalid JSON.

### 4.2 `http` Library

Makes outbound HTTP requests from within Lua functions.

#### `http.request(options)`

| Parameter           | Type   | Required | Description                                 |
|---------------------|--------|----------|---------------------------------------------|
| `options.method`    | string | No       | HTTP method (default: `"GET"`)              |
| `options.url`       | string | Yes      | Target URL                                  |
| `options.headers`   | table  | No       | Request headers (key-value table)           |
| `options.body`      | string | No       | Request body (string, typically JSON-encoded) |

**Returns a table:**
| Field    | Type   | Description                    |
|----------|--------|--------------------------------|
| `status` | number | HTTP status code (0 on error)  |
| `body`   | string | Response body                  |
| `headers`| table  | Response headers (key-value)   |

```lua
local res = http.request({
    method = "POST",
    url = "https://httpbin.org/post",
    headers = {
        ["Content-Type"] = "application/json",
        ["Authorization"] = "Bearer abc123"
    },
    body = json.encode({ message = "Hello" })
})

response.status = res.status
response.body = res.body
```

**Error handling**: If the HTTP request fails (network error, DNS failure, etc.), the returned table has `status = 0` and empty body/headers.

### 4.3 `variables` Library

Persistent key-value storage across function executions. Variables are stored in LiteDB and survive server restarts.

#### `variables.get(key)`

Retrieves a stored variable by key.

```lua
local apiKey = variables.get("weather_api_key")
-- Returns the value as a string, or nil if not found
```

#### `variables.set({key = key, value = value})`

Stores (or updates) a variable. If the variable already exists and is not `readOnly`, its value is updated. If it doesn't exist, it's created.

```lua
local success = variables.set({ key = "last_processed_id", value = "12345" })
-- Returns true on success, false on failure
```

**Important notes:**
- All values are stored as **strings**. Convert numbers/booleans with `tostring()` when storing and `tonumber()` when retrieving.
- If a variable is marked `readOnly` in the admin panel, `variables.set` will silently return `false`.
- Variable keys are case-sensitive.

### 4.4 `log` Library

Writes custom log entries during function execution. Logs are persisted in the database and viewable from the admin UI.

#### `log.write({message = message, body = body})`

```lua
log.write({ message = "Processing started", body = json.encode({ userId = 123 }) })
log.write({ message = "Successfully updated record" })
```

**Note:** The system automatically logs:
- Function execution start (with request body, headers, query params)
- Function execution finish (with response body)
- Function execution errors (with error message)

Custom `log.write` entries appear alongside these automatic logs.

---

## 5. Complete Example Patterns

### 5.1 Simple Webhook Receiver

```lua
-- POST ao/webhook
-- Stores incoming payload and logs it

if body == nil then
    response.status = 400
    response.body = '{"error": "No body provided"}'
    response.headers["Content-Type"] = "application/json"
    return
end

local payload = json.encode(body)
variables.set({ key = "last_webhook", value = payload })

log.write({ message = "Webhook received", body = payload })

response.status = 200
response.body = json.encode({ received = true })
response.headers["Content-Type"] = "application/json"
```

### 5.2 API Proxy with Caching

```lua
-- GET ao/proxy-data
-- Fetches external API, caches result

local cacheKey = "cached_response"
local cacheTtl = 300  -- 5 minutes in seconds
local currentTime = os.time()

local cachedTime = variables.get(cacheKey .. "_time")
local cachedData = variables.get(cacheKey)

if cachedData ~= nil and cachedTime ~= nil then
    local age = currentTime - tonumber(cachedTime)
    if age < cacheTtl then
        response.status = 200
        response.body = cachedData
        response.headers["Content-Type"] = "application/json"
        response.headers["X-Cache"] = "HIT"
        return
    end
end

local res = http.request({
    method = "GET",
    url = "https://jsonplaceholder.typicode.com/posts/1"
})

variables.set({ key = cacheKey, value = res.body })
variables.set({ key = cacheKey .. "_time", value = tostring(currentTime) })

response.status = res.status
response.body = res.body
response.headers["Content-Type"] = "application/json"
response.headers["X-Cache"] = "MISS"
```

### 5.3 Data Transformation Pipeline

```lua
-- POST ao/transform
-- Transforms incoming data by calling external services

if body == nil then
    response.status = 400
    response.body = '{"error": "Body required"}'
    response.headers["Content-Type"] = "application/json"
    return
end

local enriched = {}
enriched["original"] = body
enriched["processedAt"] = os.date("%Y-%m-%dT%H:%M:%SZ")

local geoRes = http.request({
    method = "GET",
    url = "https://api.example.com/geo/" .. (body["ip"] or "")
})
enriched["geo"] = json.decode(geoRes.body)

local finalJson = json.encode(enriched)

variables.set({ key = "last_transform", value = finalJson })
log.write({ message = "Data transformed", body = finalJson })

response.status = 200
response.body = finalJson
response.headers["Content-Type"] = "application/json"
```

### 5.4 Authentication Middleware Pattern

```lua
-- GET ao/secure-data
-- Validates a bearer token before returning data

local token = headers["Authorization"]
local expectedKey = variables.get("auth_api_key")

if expectedKey == nil or token ~= "Bearer " .. expectedKey then
    response.status = 401
    response.body = '{"error": "Unauthorized"}'
    response.headers["Content-Type"] = "application/json"
    response.headers["WWW-Authenticate"] = "Bearer"
    return
end

response.status = 200
response.body = json.encode({
    data = "secret information",
    timestamp = os.time()
})
response.headers["Content-Type"] = "application/json"
```

### 5.5 Conditional Routing

```lua
-- POST ao/router
-- Routes to different backends based on request body field

if body == nil or body["type"] == nil then
    response.status = 400
    response.body = '{"error": "Missing type field"}'
    response.headers["Content-Type"] = "application/json"
    return
end

local targetUrl = nil
if body["type"] == "order" then
    targetUrl = "https://api.example.com/orders"
elseif body["type"] == "user" then
    targetUrl = "https://api.example.com/users"
else
    response.status = 400
    response.body = '{"error": "Unknown type: ' .. body["type"] .. '"}'
    response.headers["Content-Type"] = "application/json"
    return
end

local res = http.request({
    method = "POST",
    url = targetUrl,
    headers = { ["Content-Type"] = "application/json" },
    body = json.encode(body["payload"])
})

response.status = res.status
response.body = res.body
response.headers["Content-Type"] = "application/json"
```

---

## 6. Important Notes and Constraints

### 6.1 Lua 5.4 Language Specifics

- **1-indexed arrays**: `table[1]` is the first element, not `table[0]`
- **String concatenation**: Use `..` operator: `"Hello " .. name`
- **Length operator**: `#table` works only for sequences (contiguous integer keys from 1)
- **nil vs null**: Lua's `nil` maps to JSON `null`. An unset table key returns `nil`
- **Table iteration**: Use `pairs()` for key-value, `ipairs()` for sequential integer keys
- **Global scope**: All globals are injected before your code runs. Avoid re-declaring library names (`json`, `http`, `variables`, `log`, `ctx`, `headers`, `queryParameters`, `body`, `response`)

### 6.2 Response Rules

- **`response.body` must be a string**, not a Lua table. Always use `json.encode()` before assignment
- **`response.status` should be a number**. Default is 200
- **`response.headers`** is a table. Set individual headers as key-value pairs
- The function returns whatever is in `response` table at the end of execution

### 6.3 HTTP Library Constraints

- The `http.request` call is **synchronous** from Lua's perspective (blocking from the C# side via `Task.Run().Result`)
- Only one HTTP request can be in-flight at a time per execution (no async/parallel in Lua)
- Failed requests return `status = 0`, not an exception

### 6.4 Variables Constraints

- All variable values are stored and retrieved as **strings**
- Use `tostring()` for numeric values when storing, `tonumber()` when retrieving
- Variable keys are unique and case-sensitive
- Read-only variables cannot be modified via `variables.set`

### 6.5 Error Handling

If your Lua code throws an error (syntax error or runtime error), the system:
- Catches the exception
- Returns a **500 Internal Server Error**
- Logs the error message to the function logs
- The error message includes the Lua stack trace

### 6.6 Safe Coding Practices

- **Always check `body ~= nil`** before accessing fields in POST/PUT/PATCH/PATCH functions
- **Always validate input** before processing
- **Use `pairs()` for safe iteration** of tables that may have string keys
- **Set `response.headers["Content-Type"]`** explicitly when returning JSON
- **Return early** after setting error responses to prevent further processing

---

## 7. Debugging

### Viewing Logs

Logs for each function execution are available in the admin UI:
1. Navigate to the function edit page
2. Click the **Logs** button
3. View execution history, including request/response data and custom log entries

### Local Lua Testing

For quick testing, you can test Lua snippets locally using the standalone Lua 5.4 interpreter:

```bash
lua -e '
local data = {name = "test", value = 42}
print(type(data["name"]))  -- string
print(#data)                -- 0 (not an array)
'
```

Note: The custom libraries (`http`, `variables`, `log`, `json`) are only available within ao-tomato's execution environment.

### Common Gotchas

| Symptom                               | Likely Cause                                                 |
|---------------------------------------|--------------------------------------------------------------|
| Response body is empty                | Forgot to set `response.body` or assigned a table instead of string |
| Content-Type not working              | Must set `response.headers["Content-Type"]` (note: hyphen in key) |
| `body` access throws error            | Didn't check `body ~= nil` for GET requests |
| External HTTP call fails              | Network connectivity issue. Returns status 0, not exception |
| Table iteration skips elements        | Using `ipairs` on a dictionary (use `pairs` instead)         |
| Numbers lost precision after variable read | Variables store strings. Use `tonumber()` when reading |

---

## 8. Template / Starter Skeleton

Use this as a base for new functions:

```lua
-- Paste response headers first for early failure safety
response.headers["Content-Type"] = "application/json"

-- For GET: access query parameters
local q = queryParameters["q"] or ""

-- For POST/PUT/PATCH: validate body
-- if body == nil then
--     response.status = 400
--     response.body = json.encode({ error = "Request body required" })
--     return
-- end

-- Read a stored variable
local config = variables.get("some_config")

-- Call an external API
-- local apiResult = http.request({
--     method = "GET",
--     url = "https://api.example.com/data"
-- })

-- Parse and process
-- local data = json.decode(apiResult.body)

-- Store result
-- variables.set({ key = "last_result", value = apiResult.body })

-- Log processing
-- log.write({ message = "Processing completed" })

-- Build response
response.status = 200
response.body = json.encode({
    message = "Hello from ao-tomato",
    timestamp = os.time()
})
```
