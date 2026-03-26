using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using NotchContactFormTests.Config;

namespace NotchContactFormTests.Drivers
{
    // Factory responsible for creating a configured IWebDriver instance.
    // The browser type and headless mode are driven by values in ConfigReader.
    public class WebDriverFactory
    {
        // Creates and returns a WebDriver for the browser specified in configuration.
        // Throws NotSupportedException when an unknown browser name is provided.
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

        // Creates a ChromeDriver with arguments that suppress automation-detection banners,
        // notifications, and pop-ups. Headless mode sets a fixed 1920×1080 viewport.
        private static IWebDriver CreateChromeDriver()
        {
            var options = new ChromeOptions();

            if (ConfigReader.Headless)
            {
                // Use the modern headless implementation and set an explicit resolution.
                options.AddArgument("--headless=new");
                options.AddArgument("--window-size=1920,1080");
            }
            else
            {
                // Open the browser maximized when running in visible mode.
                options.AddArgument("--start-maximized");
            }

            // Stability flags recommended for running Chrome inside CI/Docker containers.
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            // Hide the "Chrome is being controlled by automated software" infobars.
            options.AddExcludedArgument("enable-automation");
            options.AddAdditionalOption("useAutomationExtension", false);

            var driver = new ChromeDriver(options);
            ApplyTimeouts(driver);
            return driver;
        }

        // Creates an EdgeDriver with the same set of options used for Chrome
        // (Edge is Chromium-based and accepts identical arguments).
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

        // Creates a FirefoxDriver with web-notification preferences disabled.
        // The window is maximized programmatically when not running headless.
        private static IWebDriver CreateFirefoxDriver()
        {
            var options = new FirefoxOptions();

            if (ConfigReader.Headless)
            {
                options.AddArgument("--headless");
                options.AddArgument("--width=1920");
                options.AddArgument("--height=1080");
            }

            // Disable browser-level push/web notifications to prevent popups during tests.
            options.SetPreference("dom.webnotifications.enabled", false);
            options.SetPreference("dom.push.enabled", false);

            var driver = new FirefoxDriver(options);

            // Firefox does not support --start-maximized, so maximize the window via the API.
            if (!ConfigReader.Headless)
                driver.Manage().Window.Maximize();

            ApplyTimeouts(driver);
            return driver;
        }

        // Sets the page-load timeout on the given driver using the value from configuration.
        private static void ApplyTimeouts(IWebDriver driver)
        {
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(ConfigReader.PageLoadTimeoutSeconds);
        }
    }
}