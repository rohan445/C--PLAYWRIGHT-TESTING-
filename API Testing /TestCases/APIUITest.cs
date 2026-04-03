using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ApiUiTests : IAsyncLifetime
    {
        private IPlaywright playwright;
        private IBrowser browser;
        private IAPIRequestContext api;
        private IPage page;

        public async Task InitializeAsync()
        {
            playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            api = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions { BaseURL = "https://reqres.in/api" });
            page = await browser.NewPageAsync();
        }

        [Fact]
        public async Task CreateUser_API_Then_VerifyOnUI()
        {
            var response = await api.PostAsync("/users", new APIRequestContextOptions
            {
                DataObject = new { name = "morpheus", job = "leader" }
            });

            Assert.Equal(201, response.Status);
            var body = await response.JsonAsync();
            Assert.NotNull(body);

            await page.GotoAsync("https://reqres.in");
            Assert.Contains("ReqRes", await page.TitleAsync());
            
            await page.Locator("text=Users").First.ClickAsync();
            await page.WaitForSelectorAsync(".user");
            Assert.True(await page.Locator(".user:has-text('morpheus')").IsVisibleAsync());
        }

        [Fact]
        public async Task UpdateUser_API()
        {
            var create = await api.PostAsync("/users", new APIRequestContextOptions
            {
                DataObject = new { name = "morpheus", job = "leader" }
            });
            var userId = (await create.JsonAsync()).GetProperty("id").GetInt32();

            var update = await api.PutAsync($"/users/{userId}", new APIRequestContextOptions
            {
                DataObject = new { name = "morpheus", job = "captain" }
            });

            Assert.Equal(200, update.Status);
            var job = (await update.JsonAsync()).GetProperty("job").GetString();
            Assert.Equal("captain", job);
        }

        [Fact]
        public async Task DeleteUser_API()
        {
            var create = await api.PostAsync("/users", new APIRequestContextOptions
            {
                DataObject = new { name = "tester", job = "temp" }
            });
            var userId = (await create.JsonAsync()).GetProperty("id").GetInt32();  

            var delete = await api.DeleteAsync($"/users/{userId}");
            Assert.Equal(204, delete.Status);

            var get = await api.GetAsync($"/users/{userId}");
            Assert.Equal(404, get.Status);
        }

        [Fact]
        public async Task Compare_API_And_UI_Users()
        {
            var apiResponse = await api.GetAsync("/users?page=2");
            var apiUsers = (await apiResponse.JsonAsync()).GetProperty("data").EnumerateArray().ToList();
            var firstApiUser = apiUsers[0];
            var apiEmail = firstApiUser.GetProperty("email").GetString();
            var apiName = $"{firstApiUser.GetProperty("first_name")} {firstApiUser.GetProperty("last_name")}";

            await page.GotoAsync("https://reqres.in/users?page=2");
            await page.WaitForSelectorAsync(".user");
            
            var uiName = await page.Locator(".user").First.Locator(".user-name").TextContentAsync();
            var uiEmail = await page.Locator(".user").First.Locator(".user-email").TextContentAsync();

            Assert.Equal(apiName, uiName?.Trim());
            Assert.Equal(apiEmail, uiEmail?.Trim());
        }

        [Theory]
        [InlineData("john", "engineer")]
        [InlineData("jane", "designer")]
        [InlineData("bob", "manager")]
        public async Task CreateMultipleUsers(string name, string job)
        {
            var response = await api.PostAsync("/users", new APIRequestContextOptions
            {
                DataObject = new { name, job }
            });
            Assert.Equal(201, response.Status);
        }

        public async Task DisposeAsync()
        {
            await page.CloseAsync();
            await api.DisposeAsync();
            await browser.CloseAsync();
            playwright.Dispose();
        }
    }
}