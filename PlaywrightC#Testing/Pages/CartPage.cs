using Microsoft.Playwright;
using System.Threading.Tasks;

public class CartPage
{
    private readonly IPage _page; // Store the Playwright page instance

    public CartPage(IPage page)
    {
        _page = page;
    }

    public async Task VerifyCartLoaded() /* Method to verify that 
    the cart page has loaded by checking for cart items*/
    {
        await _page.WaitForSelectorAsync(".cart_item");
    }

    public async Task Checkout()
    {
        await _page.ClickAsync("#checkout");
    }
}