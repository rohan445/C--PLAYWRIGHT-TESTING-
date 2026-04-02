using Microsoft.Playwright;
using System.Threading.Tasks;

public class CheckoutPage
{
    private readonly IPage _page;

    public CheckoutPage(IPage page)
    {
        _page = page;
    }

    public async Task EnterDetails(string first, string last, string zip)
    {
        await _page.FillAsync("#first-name", first);
        await _page.FillAsync("#last-name", last);
        await _page.FillAsync("#postal-code", zip);
        await _page.ClickAsync("#continue");
    }

    public async Task FinishOrder()
    {
        // Click the finish button to complete the order process
        await _page.ClickAsync("#finish");
    }

    public async Task<string> GetConfirmation()
    // Retrieve the confirmation message after finishing the order
    // This method waits for the element with class "complete-header" 
    // returns its inner text, which contains the confirmation message.
    {
        return await _page.InnerTextAsync(".complete-header");
    }
}