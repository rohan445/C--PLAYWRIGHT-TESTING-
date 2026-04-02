using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

class TestCaseProgram : PageTest
{
    [Test]
    public async Task Testcase()
    {
        // Navigate to site
        await Page.GotoAsync("https://www.saucedemo.com/");

        // Enter credentials to Login
        await Page.FillAsync("#user-name", "standard_user");
        await Page.FillAsync("#password", "secret_sauce");
        await Page.ClickAsync("#login-button");

        TestContext.WriteLine("Logged in successfully");
        
        // Assertion check if login successful by checking URL
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/inventory.html");
        
        // Assertion if login successful by checking inventory page is loaded
        await Expect(Page.Locator(".inventory_item")).ToBeVisibleAsync();
        
        // Verify that inventory items are displayed
        var items = await Page.Locator(".inventory_item").CountAsync();
        Assert.That(items, Is.GreaterThan(0), "No inventory items found");

        TestContext.WriteLine("Inventory items displayed");

        // Add product to cart (add first product)
        await Page.ClickAsync(".btn_inventory");
        
        // Verify cart badge
        await Expect(Page.Locator(".shopping_cart_badge")).ToHaveTextAsync("1");

        TestContext.WriteLine("Item added to cart");

        // Go to cart to verify item is added
        await Page.ClickAsync(".shopping_cart_link");
        
        // Assertion if cart page is loaded
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/cart.html");
        await Expect(Page.Locator(".cart_item")).ToBeVisibleAsync();

        TestContext.WriteLine("Cart verified");

        // Checkout
        await Page.ClickAsync("#checkout");

        // Fill details
        await Page.FillAsync("#first-name", "John");
        await Page.FillAsync("#last-name", "Doe");
        await Page.FillAsync("#postal-code", "110001");

        await Page.ClickAsync("#continue");

        // Wait for step two page
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/checkout-step-two.html");
        await Expect(Page.Locator(".summary_info")).ToBeVisibleAsync();

        TestContext.WriteLine("Checkout info entered");

        // Finish order
        await Page.ClickAsync("#finish");

        // Assertion if order is completed by checking URL
        await Expect(Page).ToHaveURLAsync("https://www.saucedemo.com/checkout-complete.html");
        
        // Assertion order completion
        await Expect(Page.Locator(".complete-header")).ToHaveTextAsync("Thank you for your order!");

        TestContext.WriteLine("Order completed successfully");
    }
}