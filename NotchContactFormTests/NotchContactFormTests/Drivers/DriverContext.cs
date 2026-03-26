using OpenQA.Selenium;

namespace NotchContactFormTests.Drivers
{
    /// <summary>
    /// Manages the lifecycle of a single WebDriver instance per scenario.
    /// Registered as a Reqnroll dependency so each scenario gets its own isolated driver.
    /// </summary>
    public class DriverContext : IDisposable
    {
        private IWebDriver? _driver;

        // Lazy initialization — driver is created only when first accessed
        public IWebDriver Driver
        {
            get
            {
                _driver ??= WebDriverFactory.CreateDriver();
                return _driver;
            }
        }

        public void Dispose()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch
            {
                // Suppress driver shutdown errors to avoid masking test failures
            }
            finally
            {
                _driver = null;
            }
        }
    }
}