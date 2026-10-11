using HtmlAgilityPack;
using Microsoft.Playwright.MSTest;
using Microsoft.Playwright;
using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;


public class Test
{
    public static async Task Main()
    {
        var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new() {Headless = true});
        var context = await browser.NewContextAsync();
        var cookies = context.AddCookiesAsync(new[]
        {
            new Cookie
            {
                Name = "Store",
                Value = "%7B%22CreationDate%22%3A%222026-08-24T02%3A15%3A43.7423017-04%3A00%22%2C%22StoreName%22%3A%22Publix%20Super%20Market%22%2C%22StoreNumber%22%3A91%2C%22Option%22%3A%22C%2CF%2CN%2CO%2CR%22%2C%22ShortStoreName%22%3A%22Dade%20Blvd%22%7D",
                Domain = "www.publix.com",
                Path = "/"
            }
        }
        );
        var page = await context.NewPageAsync();
        await page.GotoAsync("https://www.publix.com/menu-subs-and-wraps#popular", new PageGotoOptions
        {
            WaitUntil = WaitUntilState.DOMContentLoaded
        });
        await page.Locator("div.p-grid-item .title").First.WaitForAsync(new LocatorWaitForOptions { Timeout = 20000 });

        var previousHeight = 0;
        var position = 1;
        while (position < 21)
        {
            
            await page.EvaluateAsync("(pos) => window.scrollTo(0, (document.body.scrollHeight * pos/ 20 ))", position);
            await page.WaitForTimeoutAsync(5000);
            position++;
        }

        await page.ScreenshotAsync(new() 
{ 
            Path = "screenshot2.png" 
        });

        
        // await page.Locator("div.p-grid-item").First.WaitForAsync(new() { Timeout = 60000 });
        var thing =  page.Locator("div.categories-wrapper");
        // Gets the sections of the menu
        var MenuSections = await thing.Locator("> div.menu-product-grid").AllAsync();

        // Will go into each section that has all the items listed out
        foreach (var Section in MenuSections)
        {
            

            var attribute = await Section.GetAttributeAsync("id");
            var Items =  await Section.Locator("div.p-grid-item:visible").AllAsync();

            // var ItemText = await Items.AllTextContentsAsync();
            // foreach (var text in ItemText)
            // {
            //     Console.WriteLine("---");
            //     Console.WriteLine(text);
            // }
            // var ItemNumber = await Items.CountAsync();
            // Console.WriteLine(ItemNumber);
            foreach (var Item in Items)
            {
                var TopSection = Item.Locator("div.top-section");
                var BottomSection = Item.Locator("div.bottom-section");

                var Titles = await TopSection.Locator("div.title-wrapper").AllInnerTextsAsync();
                var Prices = await BottomSection.Locator("div.p-price-badge__wrapper").AllInnerTextsAsync();
                Console.WriteLine(Titles[0]);
                Console.WriteLine(Prices[0]);
                Console.WriteLine("---");
            }
        }
        

        // var html = await page.ContentAsync();
        // await File.WriteAllTextAsync("publix page", html);


       
        // foreach (var item in things)
        // {
        //     Console.WriteLine(item);
        // }

        // foreach (var cookie in cookies)
        // {
        //     if (cookie.Name == "current_store")
        //     {
        //         cookie.Value = "91";
        //     }
        //     Console.WriteLine(cookie.Name);
        // }
        // await page.EvaluateAsync("() => window.pblxDataLayer.user.current_store = '105';");
        // var stores = page.Locator("current_store");
        // await stores.EvaluateAsync("() => 105");
        // var htmlContent = await page.ContentAsync();
        // var htmlCode = new HtmlDocument();
        // htmlCode.LoadHtml(htmlContent);



        

       

        
    }
}



        // var website = "https://www.publix.com/pd/publix-chicken-tender-platter-large-served-fresh-chilled/BMO-PLT-117484?origin=search12";cd
        // var pubWeb = new HtmlWeb();
        // var Doc = pubWeb.Load(website);
        // var text = Doc.DocumentNode.OuterHtml;