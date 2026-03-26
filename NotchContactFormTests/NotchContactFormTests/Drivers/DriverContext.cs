using OpenQA.Selenium;

namespace NotchContactFormTests.Drivers
{
    // Manages the lifecycle of a single IWebDriver instance for the duration of one test scenario.
    // Implements IDisposable so that the browser is properly closed after each scenario.
    public class DriverContext : IDisposable
    {
        // Backing field for the lazily-created driver; null until the first access.
        private IWebDriver? _driver;

        // Returns the active WebDriver instance, creating it on first access (lazy initialization).
        public IWebDriver Driver
        {
            get
            {
                // Create the driver only when first requested; reuse on subsequent accesses.
                _driver ??= WebDriverFactory.CreateDriver();
                return _driver;
            }
        }

        // Quits and disposes the WebDriver, then resets the backing field to null.
        // Errors during shutdown are swallowed so they never mask a test failure.
        public void Dispose()
        {
            try
            {
                _driver?.Quit();
                _driver?.Dispose();
            }
            catch
            {
                // Intentionally ignored — browser cleanup errors must not hide test results.
            }
            finally
            {
                // Ensure the reference is released even if an exception occurred above.
                _driver = null;
            }
        }
    }
}