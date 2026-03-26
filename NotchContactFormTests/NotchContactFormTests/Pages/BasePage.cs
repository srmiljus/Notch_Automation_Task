using OpenQA.Selenium;
using NotchContactFormTests.Helpers;

namespace NotchContactFormTests.Pages
{
    // Abstract base class for all page objects.
    // Provides shared driver access and a set of reusable low-level interaction helpers
    // so concrete page objects stay focused on page-specific selectors and actions.
    public abstract class BasePage
    {
        // The active WebDriver instance shared across all pages in a scenario.
        protected readonly IWebDriver Driver;
        // Explicit-wait helper used by all interaction methods.
        protected readonly WaitHelper Wait;

        // Receives the driver and wait helper via constructor injection from the DI container.
        protected BasePage(IWebDriver driver, WaitHelper waitHelper)
        {
            Driver = driver;
            Wait = waitHelper;
        }


        // Navigates the browser to the given URL, waits for the page to load,
        // and dismisses any cookie/consent banner that may be present.
        protected void GoToUrl(string url)
        {
            Driver.Navigate().GoToUrl(url);
            Wait.WaitForPageLoad();
            DismissCookieBannerIfPresent();
        }


        // Clears the input field matching the locator, scrolls it into view,
        // and types the supplied value.
        protected void TypeInto(By locator, string value)
        {
            var el = Wait.WaitForElementVisible(locator);
            Wait.ScrollToElement(el);
            el.Clear();
            el.SendKeys(value);
        }

        // Scrolls the element matching the locator into view and clicks it,
        // waiting for it to be clickable first to avoid race conditions.
        protected void ClickOn(By locator)
        {
            var el = Wait.WaitForElementClickable(locator);
            Wait.ScrollToElement(el);
            el.Click();
        }

        // Returns the visible text of the element matching the locator.
        // Returns an empty string if the element is not found within the timeout,
        // so callers can assert on the result rather than catching exceptions.
        protected string GetText(By locator, int timeoutSeconds = 10)
        {
            try
            {
                var el = Wait.WaitForElementVisible(locator, timeoutSeconds);
                return el.Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        // Ensures the checkbox or radio button matching the locator is in a checked state.
        // Only clicks if it is currently unchecked to avoid toggling it off.
        protected void EnsureChecked(By locator)
        {
            var el = Wait.WaitForElementVisible(locator);
            Wait.ScrollToElement(el);
            if (!el.Selected)
                el.Click();
        }


        // Interacts with a Chosen.js custom dropdown: opens it by clicking the container,
        // then clicks the list item whose text matches optionText.
        // Uses a last-item XPath fallback when optionText contains a single-quote character
        // to avoid XPath injection issues.
        protected void SelectFromChosenDropdown(By chosenContainerLocator, string optionText)
        {
            var container = Wait.WaitForElementClickable(chosenContainerLocator);
            Wait.ScrollToElement(container);
            container.Click();

            // If the option text contains a single quote, fall back to selecting the last active result
            // because XPath cannot embed a single quote inside a single-quoted string literal.
            var optionLocator = optionText.Contains("'")
                ? By.XPath("//ul[@class='chosen-results']//li[contains(@class,'active-result')][last()]")
                : By.XPath($"//ul[@class='chosen-results']//li[contains(@class,'active-result') and normalize-space(.)='{optionText}']");

            var option = Wait.WaitForElementClickable(optionLocator);
            option.Click();
        }


        // Attempts to find and click the "Accept All" button on the cookie consent banner.
        // Then waits for the banner to disappear.
        // Silently ignores any failure because the banner may not appear on every page load.
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
            catch { /* The banner may not exist — it's not an error. */ }
        }
    }
}