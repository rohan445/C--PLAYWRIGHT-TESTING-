using Microsoft.Playwright;
using System.Threading.Tasks;

public class APITests
{
    public async Task CreateUser_API_Test()
    {
        using var playwright = await Playwright.CreateAsync();

        var request = await playwright.APIRequest.NewContextAsync();

        var response = await request.PostAsync(
            "https://reqres.in/api/users",
            new APIRequestContextOptions
            {
                DataObject = new
                {
                    name = "testuser",
                    job = "qa"
                }
            });

        Assert.True(response.Ok);

        var body = await response.JsonAsync();
        Assert.NotNull(body);
    }
}