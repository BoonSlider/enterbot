using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace HouseScraperSelenium
{
    public record HouseData
    {
        public long Price { get; set; }
        public long Taxes { get; set; }
        public long ProtectedGuards { get; set; }
        public long MoonshineLimit { get; set; }
        public long RequiredFame { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            // options.AddArgument("--headless"); // Uncomment if running in headless mode

            using var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            try
            {
                // Navigate to login page
                driver.Navigate().GoToUrl("https://enternum.net/logi_sisse");

                // Click the radio button before login
                driver.FindElement(By.Id("maailm13")).Click();

                // Fill in credentials and submit the login form
                driver.FindElement(By.Name("username")).SendKeys("demo");
                driver.FindElement(By.Name("password")).SendKeys("parool");
                driver.FindElement(By.XPath("//input[@type='submit']")).Click();

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.Url.Contains("enternum.net"));

                // Navigate to the kinnisvara page
                driver.Navigate().GoToUrl("https://enternum.net/kinnisvara");
                wait.Until(d => d.FindElement(By.XPath("//div[contains(@class, 'content') and contains(@class, 'last')]/table")));

                // Get the list of dropdown values (avoiding stale element references)
                var dropdown = driver.FindElement(By.Name("vali"));
                var selectElement = new SelectElement(dropdown);
                var optionsList = new List<string>();

                foreach (var option in selectElement.Options)
                {
                    optionsList.Add(option.GetAttribute("value"));
                }

                // Iterate through each value
                foreach (var value in optionsList)
                {
                    // Re-find the dropdown each time to avoid stale elements
                    dropdown = driver.FindElement(By.Name("vali"));
                    selectElement = new SelectElement(dropdown);

                    // Select the new option
                    selectElement.SelectByValue(value);

                    // Wait for the page to update
                    System.Threading.Thread.Sleep(500);

                    // Scrape the table data
                    var table = driver.FindElement(By.XPath("//div[contains(@class, 'content') and contains(@class, 'last')]/table"));
                    var rows = table.FindElements(By.TagName("tr"));

                    long price = 0, taxes = 0, protectedGuards = 0, moonshineLimit = 0, requiredFame = 0;

                    foreach (var row in rows)
                    {
                        var cells = row.FindElements(By.TagName("td"));
                        if (cells.Count < 2)
                            continue;

                        string label = cells[0].Text.Trim();
                        string valueText = cells[1].Text.Trim();
                        long valueParsed = ParseLongFromText(valueText);

                        if (label.StartsWith("Maksumus", StringComparison.OrdinalIgnoreCase))
                            price = valueParsed;
                        else if (label.StartsWith("Kommunaalkulud", StringComparison.OrdinalIgnoreCase))
                            taxes = valueParsed;
                        else if (label.StartsWith("Kaitstud turvamehi", StringComparison.OrdinalIgnoreCase))
                            protectedGuards = valueParsed;
                        else if (label.StartsWith("Puskari limiit", StringComparison.OrdinalIgnoreCase))
                            moonshineLimit = valueParsed;
                        else if (label.StartsWith("Kuulsust vaja", StringComparison.OrdinalIgnoreCase))
                            requiredFame = valueParsed;
                    }

                    var houseData = new HouseData
                    {
                        Price = price,
                        Taxes = taxes,
                        ProtectedGuards = protectedGuards,
                        MoonshineLimit = moonshineLimit,
                        RequiredFame = requiredFame
                    };

                    // Output the scraped data as JSON
                    var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                    string json = JsonSerializer.Serialize(houseData, jsonOptions);
                    Console.WriteLine($"{value} => new HouseData");
                    Console.Write("{ ");
                    Console.Write($"Price = {houseData.Price}, ");
                    Console.Write($"Taxes = {houseData.Taxes}, ");
                    Console.Write($"ProtectedGuards = {houseData.ProtectedGuards}, ");
                    Console.Write($"MoonshineLimit = {houseData.MoonshineLimit}, ");
                    Console.Write($"RequiredFame = {houseData.RequiredFame}, ");
                    Console.WriteLine("},");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                driver.Quit();
            }
        }

        /// <summary>
        /// Parses a string like "650 000 000 EEK" into a long.
        /// </summary>
        private static long ParseLongFromText(string text)
        {
            var digits = Regex.Replace(text, @"[^\d-]", "");
            return long.TryParse(digits, out var result) ? result : 0;
        }
    }
}
