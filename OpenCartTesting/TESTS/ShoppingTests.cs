using Microsoft.Playwright;
using Xunit;
using System.Threading.Tasks;
using Allure.Xunit;
using Allure.Xunit.Attributes;
using static Microsoft.Playwright.Assertions;
using Allure.Net.Commons;

[AllureSuite("OpenCart Tests")]
[AllureFeature("Shopping")]
public class ShoppingTests
{
    private async Task<(IPage page, IBrowser browser)> Setup()
    {
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false
        });

        var page = await browser.NewPageAsync();
        return (page, browser);
    }

    [Fact]
    [AllureStory("Search and Add to Cart")]
    public async Task Search_Add_To_Cart_Flow()
    {
        var (page, browser) = await Setup();

        var home = new HomePage(page);
        var product = new ProductPage(page);
        var cart = new CartPage(page);

        try
        {
            AllureApi.Step("Navigate to Home Page");
            await home.Navigate("https://demo.opencart.com/");

            AllureApi.Step("Search for iPhone");
            await home.SearchProduct("iPhone");

            AllureApi.Step("Select product");
            await product.SelectFirstProduct();

            AllureApi.Step("Add product to cart");
            await product.AddToCart();

            AllureApi.Step("Open cart");
            await cart.OpenCart();

            var productName = await cart.GetFirstProductName();

            AllureApi.Step("Verify product in cart");
            Assert.Contains("iPhone", productName);
        }
        catch
        {
            var screenshot = await page.ScreenshotAsync();
            AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshot);
            throw;
        }

        await browser.CloseAsync();
    }

    [Fact]
    [AllureStory("Remove Product from Cart")]
    public async Task Remove_Product_From_Cart()
    {
        var (page, browser) = await Setup();

        var home = new HomePage(page);
        var product = new ProductPage(page);
        var cart = new CartPage(page);

        try
        {
            await home.Navigate("https://demo.opencart.com/");
            await home.SearchProduct("iPhone");
            await product.SelectFirstProduct();
            await product.AddToCart();

            await cart.OpenCart();
            await cart.RemoveFirstProduct();

            var products = await cart.GetAllProductNames();

            Assert.DoesNotContain("iPhone", products);
        }
        catch
        {
            var screenshot = await page.ScreenshotAsync();
            AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshot);
            throw;
        }

        await browser.CloseAsync();
    }
}
