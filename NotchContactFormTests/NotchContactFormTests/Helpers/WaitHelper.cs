using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using NotchContactFormTests.Config;
using SeleniumExtras.WaitHelpers;

namespace NotchContactFormTests.Helpers
{
    public class WaitHelper
    {
        private readonly IWebDriver _driver;
        private readonly int _defaultTimeoutSeconds;

        public WaitHelper(IWebDriver driver)
        {
            _driver = driver;
            _defaultTimeoutSeconds = ConfigReader.DefaultTimeout;
        }

        public IWebElement WaitForElementVisible(By locator, int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public IWebElement WaitForElementClickable(By locator, int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds);
            return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }

        public bool WaitForElementNotVisible(By locator, int? timeoutSeconds = null)
        {
            try
            {
                var wait = CreateWait(timeoutSeconds);
                return wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public void WaitForPageLoad(int? timeoutSeconds = null)
        {
            var wait = CreateWait(timeoutSeconds ?? ConfigReader.DefaultTimeout);
            wait.Until(driver => ((IJavaScriptExecutor)driver)
                .ExecuteScript("return document.readyState").ToString() == "complete");
        }

        public void ScrollToElement(IWebElement element)
        {
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);
        }

        private WebDriverWait CreateWait(int? timeoutSeconds)
        {
            return new WebDriverWait(_driver, TimeSpan.FromSeconds(timeoutSeconds ?? _defaultTimeoutSeconds));
        }

        public void WaitForCondition(Func<IWebDriver, bool> condition, int timeoutSeconds = 10)
        {
            var wait = CreateWait(timeoutSeconds);
            wait.Until(condition);
        }
    }
}
