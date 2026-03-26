using OpenQA.Selenium;
using NotchContactFormTests.Helpers;

namespace NotchContactFormTests.Pages
{
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

            var optionLocator = optionText.Contains("'")
                ? By.XPath("//ul[@class='chosen-results']//li[contains(@class,'active-result')][last()]")
                : By.XPath($"//ul[@class='chosen-results']//li[contains(@class,'active-result') and normalize-space(.)='{optionText}']");

            var option = Wait.WaitForElementClickable(optionLocator);
            option.Click();
        }


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