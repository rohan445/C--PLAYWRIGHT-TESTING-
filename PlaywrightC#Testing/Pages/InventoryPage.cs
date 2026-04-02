using Microsoft.Playwright;
using System.Threading.Tasks;

public class InventoryPage
{
    private readonly IPage _page;
    
    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public async Task VerifyInventoryLoaded() /* check items are loaded 
    by waiting for the inventory item selector to appear */
    {
        await _page.WaitForSelectorAsync(".inventory_item");
    }

    public async Task AddItemToCart()
    {
        await _page.ClickAsync("text=Add to cart");
    }

    public async Task<string> GetCartCount() /*returns the number of items 
    in the cart*/
    {
        return await _page.InnerTextAsync(".shopping_cart_badge");
    }

    public async Task GoToCart()
    {
        await _page.ClickAsync(".shopping_cart_link");
    }
}