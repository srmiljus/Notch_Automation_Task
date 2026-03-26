using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NotchContactFormTests.Config;
using SeleniumExtras.WaitHelpers;

namespace NotchContactFormTests.Helpers
{
    // Centralises all explicit-wait logic so that page objects don't have to
    // instantiate WebDriverWait directly. Timeout defaults come from configuration.
    public class WaitHelper
    {
        private readonly IWebDriver _driver;
        // Cached default timeout read once from config to avoid repeated parsing.
        private readonly int _defaultTimeoutSeconds;

        // Initialises the helper with the active driver and the configured default timeout.
        public WaitHelper(IWebDriver driver)
        {
            _driver = driver;
            _defaultTimeoutSeconds = ConfigReader.DefaultTimeout;
        }

        // Waits until the element matching the locator is visible in the DOM, then returns it.
        public IWebElement WaitForElementVisible(By locator, int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        // Waits until the element matching the locator is both visible and enabled (clickable).
        public IWebElement WaitForElementClickable(By locator, int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        // Waits until the element is no longer visible. Returns false if the timeout expires
        // before the element disappears, instead of throwing an exception.
        public bool WaitForElementNotVisible(By locator, int? timeoutSeconds = null)
        {
            try
            {
                var wait = CreateWait(timeoutSeconds);
                return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            }
            catch (WebDriverTimeoutException)
            {
                // Element is still visible after the timeout — return false rather than failing.
                return false;
            }
        }

        // Polls the browser's document.readyState until it equals "complete",
        // indicating that the page and all synchronous resources have loaded.
        public void WaitForPageLoad(int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds ?? ConfigReader.DefaultTimeout);
            wait.Until(driver => ((IJavaScriptExecutor)driver)
                .ExecuteScript("return document.readyState").ToString() == "complete");
        }

        // Scrolls the given element into the centre of the viewport via JavaScript
        // so that subsequent interactions are not blocked by sticky headers or footers.
        public void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
        }

        // Creates a WebDriverWait instance using the supplied timeout, or the default if null.
        private WebDriverWait CreateWait(int? timeoutSeconds)
        {
            return new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds ?? _defaultTimeoutSeconds));
        }

        // Waits until the supplied predicate returns true, using a custom timeout.
        // Useful for non-standard conditions that ExpectedConditions does not cover.
        public void WaitForCondition(Func<IWebDriver, bool> condition, int timeoutSeconds = 10)
        {
            var wait = CreateWait(timeoutSeconds);
            wait.Until(condition);
        }
    }
}
