using OpenQA.Selenium;
using NotchContactFormTests.Helpers;

namespace NotchContactFormTests.Pages
{
    /// <summary>
    /// Base class for all page objects. Provides reusable Selenium interactions
    /// built on top of explicit waits — no implicit waits are used anywhere.
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WaitHelper Wait;

        protected BasePage(IWebDriver driver, WaitHelper waitHelper)
        {
            Driver = driver;
            Wait = waitHelper;
        }

        protected void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
            Wait.WaitForPageLoad();
            DismissCookieBannerIfPresent();
        }

        protected void TypeInto(By locator, string value)
        {
            var el = Wait.WaitForElementVisible(locator);
            Wait.ScrollToElement(el);
            el.Clear();
            el.SendKeys(value);
        }

        protected void ClickOn(By locator)
        {
            var el = Wait.WaitForElementClickable(locator);
            Wait.ScrollToElement(el);
            el.Click();
        }

        // Returns empty string instead of throwing if the element is not found within the timeout
        protected string GetText(By locator, int timeoutSeconds = 10)
        {
            try
            {
                var el = Wait.WaitForElementVisible(locator, timeoutSeconds);
                return el.Text;
            }
            catch (WebDriverTimeoutException)
            {
                return string.Empty;
            }
        }

        // Clicks the element only if it is not already checked — safe for both checkboxes and radio buttons
        protected void EnsureChecked(By locator)
        {
            var el = Wait.WaitForElementVisible(locator);
            Wait.ScrollToElement(el);
            if (!el.Selected)
                el.Click();
        }

     
        protected void SelectFromChosenDropdown(By chosenContainerLocator, string optionText)
        {
            var container = Wait.WaitForElementClickable(chosenContainerLocator);
            Wait.ScrollToElement(container);
            container.Click();

            // XPath can't use single quotes inside a single-quoted string — fall back to last active result
            var optionLocator = optionText.Contains("'")
                ? By.XPath("//ul[@class='chosen-results']//li[contains(@class,'active-result')][last()]")
                : By.XPath($"//ul[@class='chosen-results']//li[contains(@class,'active-result') and normalize-space(.)='{optionText}']");

            var option = Wait.WaitForElementClickable(optionLocator);
            option.Click();
        }

        // Attempts to dismiss the cookie consent banner on page load — silently ignored if not present
        private void DismissCookieBannerIfPresent()
        {
            try
            {
                var btn = Wait.WaitForElementClickable(
                    By.XPath("//div[@class='cky-consent-container cky-popup-center']//button[contains(.,'Accept All')]"),
                    timeoutSeconds: 5);
                btn.Click();
                Wait.WaitForElementNotVisible(
                    By.XPath("//div[@class='cky-consent-container cky-popup-center']"),
                    timeoutSeconds: 5);
            }
            catch (WebDriverTimeoutException) { /* Banner may not be present on every page load — not an error */ }
        }
    }
}