using Microsoft.Playwright;

class Program
{
    public static async Task Main()
    {
        using var playwright = await Playwright.CreateAsync();

        var browser = await playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 500
        });

        var page = await browser.NewPageAsync();

        var login = new LoginPage(page);
        var inventory = new InventoryPage(page);
        var cart = new CartPage(page);
        var checkout = new CheckoutPage(page);
        await login.Navigate();
        await login.Login("standard_user", "secret_sauce");
        Console.WriteLine("Logged in");

        await inventory.VerifyInventoryLoaded();
        await inventory.AddItemToCart();

        string count = await inventory.GetCartCount();
        if (count != "1")
            throw new Exception("Item not added");

        Console.WriteLine("Item added")
        await inventory.GoToCart();
        await cart.VerifyCartLoaded();
        await cart.Checkout();

        await checkout.EnterDetails("John", "Doe", "110001");
        await checkout.FinishOrder();

        string confirmation = await checkout.GetConfirmation();
        if (!confirmation.Contains("Thank you"))
            throw new Exception("Order failed");

        Console.WriteLine("Order completed successfully");

        await browser.CloseAsync();
    }
}