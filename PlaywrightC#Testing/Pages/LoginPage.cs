using Microsoft.Playwright;
using System.Threading.Tasks;

public class LoginPage
{
    private readonly IPage _page; 
    public LoginPage(IPage page)
    {
        _page = page; // private field to hold the page object
    }

    public async Task Navigate() // method to navigate to the login page
    {
        await _page.GotoAsync("https://www.saucedemo.com/");
    }

    public async Task Login(string username, string password)
    {
        await _page.FillAsync("#user-name", username);
        await _page.FillAsync("#password", password);
        await _page.ClickAsync("#login-button");
    }
}