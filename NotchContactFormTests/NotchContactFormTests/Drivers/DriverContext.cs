using OpenQA.Selenium;

namespace NotchContactFormTests.Drivers
{
    public class DriverContext : IDisposable
    {
        private IWebDriver? _driver;

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

            }
            finally
            {
                _driver = null;
            }
        }
    }
}