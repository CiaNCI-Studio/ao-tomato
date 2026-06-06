namespace AoTomato.Services.Helpers;

using NLua;
using System.Text;
using System.Text.Json;

public static class LuaHelpers
{
    public const string JsonLibrary = @"
local json = {}

local function escape(s)
    return string.gsub(s, '[\""\\]', '\\%0')
end

local function encode(v)
    local t = type(v)
    if t == 'string' then
        return string.format('%q', v)
    elseif t == 'number' then
        return string.format('%g', v)
    elseif t == 'boolean' then
        return tostring(v)
    elseif t == 'table' then
        local parts = {}
        local isArray = true
        local maxIdx = 0
        for k in pairs(v) do
            if type(k) ~= 'number' or k < 1 or math.floor(k) ~= k then
                isArray = false
            else
                maxIdx = math.max(maxIdx, k)
            end
        end
        if isArray and maxIdx > 0 then
            for i = 1, maxIdx do
                parts[i] = encode(v[i])
            end
            return '[' .. table.concat(parts, ',') .. ']'
        else
            for k, val in pairs(v) do
                parts[#parts + 1] = encode(k) .. ':' .. encode(val)
            end
            return '{' .. table.concat(parts, ',') .. '}'
        end
    elseif t == 'nil' then
        return 'null'
    end
    return 'null'
end

function json.encode(v)
    return encode(v)
end

function json.decode(s)
    if s == nil or s == '' then return nil end
    local ok, result = pcall(function()
        local f = load('return ' .. s)
        if f then return f() end
    end)
    if ok then return result end
    return nil
end
";

    public static void LoadJsonLibrary(Lua lua)
    {
        lua.DoString(JsonLibrary);
    }

    public static LuaTable NewLuaTable(Lua lua)
    {
        return (LuaTable)lua.DoString("return {}")[0]!;
    }

    public static LuaTable BuildLuaTableFromJson(Lua lua, string json)
    {
        return (LuaTable)lua.DoString($"return json.decode({JsonSerializer.Serialize(json)})")[0]!;
    }

    public static LuaTable JsonElementToLuaTable(Lua lua, JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => BuildObjectTable(lua, element),
            JsonValueKind.Array => BuildArrayTable(lua, element),
            _ => throw new InvalidOperationException("Root JSON element must be an object or array")
        };
    }

    public static LuaTable BuildObjectTable(Lua lua, JsonElement element)
    {
        var table = NewLuaTable(lua);
        foreach (var prop in element.EnumerateObject())
            table[prop.Name] = JsonValueToLuaValue(lua, prop.Value);
        return table;
    }

    public static LuaTable BuildArrayTable(Lua lua, JsonElement element)
    {
        var table = NewLuaTable(lua);
        int index = 1;
        foreach (var item in element.EnumerateArray())
            table[index++] = JsonValueToLuaValue(lua, item);
        return table;
    }

    public static object? JsonValueToLuaValue(Lua lua, JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => BuildObjectTable(lua, element),
            JsonValueKind.Array => BuildArrayTable(lua, element),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var intVal) ? intVal : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null or _ => null
        };
    }

    public static JsonDocument LuaTableToJsonDocument(LuaTable table)
    {
        var json = LuaValueToJson(table);
        return JsonDocument.Parse(json);
    }

    public static string LuaValueToJson(object? value)
    {
        return value switch
        {
            null => "null",
            LuaTable table => LuaTableToJsonString(table),
            string s => JsonSerializer.Serialize(s),
            double d => JsonSerializer.Serialize(d),
            long l => JsonSerializer.Serialize(l),
            int i => JsonSerializer.Serialize(i),
            bool b => b ? "true" : "false",
            _ => "null"
        };
    }

    public static string LuaTableToJsonString(LuaTable table)
    {
        var isArray = IsArrayTable(table);
        var sb = new StringBuilder();

        if (isArray)
        {
            sb.Append('[');
            var parts = new List<string>();
            for (int i = 1; ; i++)
            {
                var val = table[i];
                if (val == null) break;
                parts.Add(LuaValueToJson(val));
            }
            sb.Append(string.Join(',', parts));
            sb.Append(']');
        }
        else
        {
            sb.Append('{');
            var parts = new List<string>();
            foreach (KeyValuePair<object, object> kvp in table)
            {
                var key = JsonSerializer.Serialize(kvp.Key.ToString());
                parts.Add($"{key}:{LuaValueToJson(kvp.Value)}");
            }
            sb.Append(string.Join(',', parts));
            sb.Append('}');
        }

        return sb.ToString();
    }

    public static bool IsArrayTable(LuaTable table)
    {
        if (table.Keys.Count == 0) return false;
        var i = 1;
        foreach (var key in table.Keys)
        {
            if (key is long k && k == i)
            {
                i++;
                continue;
            }
            return false;
        }
        return true;
    }
}
