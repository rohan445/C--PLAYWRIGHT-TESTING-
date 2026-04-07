using System;
using System.Net.Http;
using System.Text;
using System.Text.Json; 
using System.Threading.Tasks;
using HtmlAgilityPack; 

class Program
{
    static async Task Main(string[] args)
    {
        // to scrape url and generate test cases 
        Console.Write("Enter URL: ");
        string url = Console.ReadLine();
        var html = await ScrapeHtml(url);
        var aiResponse = await GenerateTestWithAI(html, url);
        Console.WriteLine(aiResponse);
    }
    
    // scrape html content from the given URL
    static async Task<string> ScrapeHtml(string url)
    {
        var client = new HttpClient();
        return await client.GetStringAsync(url);
    }

    // generate test cases code 
    static async Task<string> GenerateTestWithAI(string html, string url)
    {
        string apiKey = "";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "");

        // token limit for prompt 
        if (html.Length > 8000)
            html = html.Substring(0, 8000);
        string prompt = $@"
Generate a Playwright C# test using xUnit.

Website: {url}
HTML snippet: {html}
Requirements:
- Use async/await
- Use Playwright .NET syntax
- Include navigation, interaction, and assertions
- Use good selectors (role/text if possible)

Return only code.
";

        var requestBody = new
        {
            model = "claude",
            max_tokens = 1000,
            messages = new[]
            {
                new {
                    role = "user",
                    content = prompt
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(requestBody), 
            Encoding.UTF8,
            "application/json"
        );
        // send post request 
        var response = await client.PostAsync(
            "https://api.anthropic.com/v1/messages",
            content
        );

        var json = await response.Content.ReadAsStringAsync();

        // Extract text response (basic parsing)
        using var doc = JsonDocument.Parse(json); // parse the JSON response

        // to get the generated code from the response
        var result = doc.RootElement 
                        .GetProperty("content")[0]
                        .GetProperty("text")
                        .GetString();

        return result;
    }
}