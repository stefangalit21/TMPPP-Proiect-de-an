using System.Text.Json;

namespace MusicPlayer.Application.Dispatcher;

public class IpcDispatcher
{
    private readonly Dictionary<string, Func<Dictionary<string, string>, string>> _handlers = new();

    public void Register(string action, Func<Dictionary<string, string>, string> handler)
    {
        _handlers[action] = handler;
    }

    public string Dispatch(string action, Dictionary<string, string> payload)
    {
        if (_handlers.TryGetValue(action, out var handler))
            return handler(payload);

        return Serialize(new { ok = false, error = $"Actiune necunoscuta: {action}" });
    }

    public static string Serialize(object obj) =>
        JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
}