using Microsoft.Playwright;
using Xunit;
using Allure.Xunit;
using Allure.Xunit.Attributes;
using Allure.Net.Commons;
using OpenCartTests.Pages;
using System;
using System.Threading.Tasks;

namespace OpenCartTests
{
    [AllureSuite("OpenCart Authentication Tests")]
    [AllureFeature("Login Functionality")]
    [AllureEpic("User Authentication")]
    public class LoginTests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;
        private LoginPage _loginPage;
        private const string ValidEmail = "test@test.com";
        private const string ValidPassword = "password123";
        private const string InvalidEmail = "wrong@test.com";
        private const string InvalidPassword = "wrongpass";
        
        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 100 // Slow down operations for better visibility
            });
            
            _page = await _browser.NewPageAsync();
            _loginPage = new LoginPage(_page);
        }
        
        public async Task DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
            _playwright?.Dispose();
        }
        
        [Fact]
        [AllureStory("Valid Login")]
        [AllureSeverity(SeverityLevel.critical)]
        [Trait("Category", "Smoke")]
        public async Task Login_With_ValidCredentials_ShouldSucceed()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step($"Login with credentials: {ValidEmail}");
                await _loginPage.LoginAsync(ValidEmail, ValidPassword);
                
                AllureApi.Step("Verify successful login");
                var isLoggedIn = await _loginPage.IsLoggedInAsync();
                Assert.True(isLoggedIn, "User should be logged in successfully");
                
                // Optional: Take screenshot on success
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Login Success", "image/png", screenshot);
            }
            catch (Exception ex)
            {
                // Take screenshot on failure
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
        
        [Fact]
        [AllureStory("Invalid Login")]
        [AllureSeverity(SeverityLevel.normal)]
        [Trait("Category", "Regression")]
        public async Task Login_With_InvalidCredentials_ShouldShowError()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step($"Login with invalid credentials: {InvalidEmail}");
                await _loginPage.LoginAsync(InvalidEmail, InvalidPassword);
                
                AllureApi.Step("Verify error message is displayed");
                var errorVisible = await _loginPage.IsErrorMessageVisibleAsync();
                Assert.True(errorVisible, "Error message should be displayed for invalid credentials");
                
                var errorText = await _loginPage.GetErrorMessageTextAsync();
                AllureApi.AddAttachment("Error Message", "text/plain", errorText);
                Assert.Contains("Warning", errorText);
            }
            catch (Exception ex)
            {
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
        
        [Fact]
        [AllureStory("Empty Credentials")]
        [AllureSeverity(SeverityLevel.minor)]
        [Trait("Category", "Edge Cases")]
        public async Task Login_With_EmptyCredentials_ShouldShowError()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step("Attempt login with empty credentials");
                await _loginPage.LoginAsync("", "");
                
                AllureApi.Step("Verify error message is displayed");
                var errorVisible = await _loginPage.IsErrorMessageVisibleAsync();
                Assert.True(errorVisible, "Error message should be displayed for empty credentials");
            }
            catch (Exception ex)
            {
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
        
        [Fact]
        [AllureStory("Password Field")]
        [AllureSeverity(SeverityLevel.normal)]
        [Trait("Category", "Security")]
        public async Task Login_PasswordField_ShouldBeMasked()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step("Check password field type");
                var passwordField = _page.Locator("#input-password");
                var type = await passwordField.GetAttributeAsync("type");
                
                AllureApi.Step("Verify password field is masked");
                Assert.Equal("password", type);
            }
            catch (Exception ex)
            {
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
        
        [Fact]
        [AllureStory("Remember Me")]
        [AllureSeverity(SeverityLevel.minor)]
        [Trait("Category", "UI")]
        public async Task Login_RememberMeCheckbox_ShouldBePresent()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step("Check for remember me checkbox");
                var rememberCheckbox = _page.Locator("input[name='remember']");
                var isPresent = await rememberCheckbox.CountAsync() > 0;
                
                AllureApi.Step("Verify remember me checkbox exists");
                Assert.True(isPresent, "Remember me checkbox should be present");
            }
            catch (Exception ex)
            {
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
        
        [Fact]
        [AllureStory("Navigation")]
        [AllureSeverity(SeverityLevel.normal)]
        [Trait("Category", "Navigation")]
        public async Task Login_ForgotPasswordLink_ShouldNavigate()
        {
            try
            {
                AllureApi.Step("Navigate to Login Page");
                await _loginPage.NavigateAsync();
                
                AllureApi.Step("Click Forgot Password link");
                await _page.ClickAsync("text=Forgotten Password");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                AllureApi.Step("Verify navigation to forgot password page");
                Assert.Contains("route=account/forgotten", _page.Url);
            }
            catch (Exception ex)
            {
                var screenshot = await _page.ScreenshotAsync();
                AllureApi.AddAttachment("Test Failure Screenshot", "image/png", screenshot);
                throw new Exception($"Test failed: {ex.Message}", ex);
            }
        }
    }
}
