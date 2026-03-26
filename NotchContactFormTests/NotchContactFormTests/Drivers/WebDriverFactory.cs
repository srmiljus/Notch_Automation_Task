using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Drivers
{
    public class WebDriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            return ConfigReader.Browser.ToLower() switch
            {
                "chrome" => CreateChromeDriver(),
                "edge" => CreateEdgeDriver(),
                "firefox" => CreateFirefoxDriver(),
                _ => throw new NotSupportedException(
                    $"Browser '{ConfigReader.Browser}' is not supported. Use 'Chrome', 'Edge' or 'Firefox'.")
            };
        }

        private static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();

            if (ConfigReader.Headless)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
            }
            else
            {
                options.AddArgument("--start-maximized");
            }

            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            var driver = new ChromeDriver(options);
            ApplyTimeouts(driver);
            return driver;
        }

        private static IWebDriver CreateEdgeDriver()
        {
            var options = new EdgeOptions();

            if (ConfigReader.Headless)
            {
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
            }
            else
            {
                options.AddArgument("--start-maximized");
            }

            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            var driver = new EdgeDriver(options);
            ApplyTimeouts(driver);
            return driver;
        }

        private static IWebDriver CreateFirefoxDriver()
        {
            var options = new FirefoxOptions();

            if (ConfigReader.Headless)
            {
                options.AddArgument("--headless");
                options.AddArgument("--width=1920");
                options.AddArgument("--height=1080");
            }

            options.SetPreference("dom.webnotifications.enabled", false);
            options.SetPreference("dom.push.enabled", false);

            var driver = new FirefoxDriver(options);

            if (!ConfigReader.Headless)
                driver.Manage().Window.Maximize();

            ApplyTimeouts(driver);
            return driver;
        }

        private static void ApplyTimeouts(IWebDriver driver)
        {
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(ConfigReader.PageLoadTimeoutSeconds);
        }
    }
}