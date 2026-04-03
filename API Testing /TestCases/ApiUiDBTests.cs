using Xunit;
using Microsoft.Playwright;
using System.Threading.Tasks;
using System;

[Collection("ParallelTests")]
public class ApiUiDbTests
{
    [Fact]
    public async Task Full_E2E_Test_1()
    {
        string testUser = "user_" + Guid.NewGuid();

        using var playwright = await Playwright.CreateAsync();
        var requestContext = await playwright.APIRequest.NewContextAsync();

        var apiResponse = await requestContext.PostAsync(
            "https://reqres.in/api/users",
            new()
            {
                DataObject = new { name = testUser, job = "qa" }
            });

        Assert.True(apiResponse.Ok);
        var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://reqres.in/");

        var title = await page.TitleAsync();
        Assert.Contains("ReqRes", title);

        await browser.CloseAsync();
        var db = new DbHelper();

        var dbUser = await db.GetUserByName(testUser);

        Assert.NotNull(dbUser);
    }

    [Fact]
    public async Task Full_E2E_Test_2()
    {
        string testUser = "user_" + Guid.NewGuid();
        using var playwright = await Playwright.CreateAsync();
        var requestContext = await playwright.APIRequest.NewContextAsync();

        var apiResponse = await requestContext.PostAsync(
            "https://reqres.in/api/users",
            new()
            {
                DataObject = new { name = testUser, job = "dev" }
            });

        Assert.True(apiResponse.Ok);
        var browser = await playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions { Headless = true });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("https://reqres.in/");
        Assert.Contains("ReqRes", await page.TitleAsync());

        await browser.CloseAsync();
        var db = new DbHelper();

        var dbUser = await db.GetUserByName(testUser);

        Assert.NotNull(dbUser);
    }
}