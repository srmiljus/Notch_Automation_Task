using OpenQA.Selenium;
using NotchContactFormTests.Config;
using NotchContactFormTests.Helpers;

namespace NotchContactFormTests.Pages
{
    // Page object representing the Notch contact form page.
    // Exposes one method per user action and hides all locator details from the step definitions.
    public class ContactFormPage : BasePage
    {
        // The URL to navigate to when opening the contact form.
        private readonly string _pageUrl;

        #region Locators

        // CSS selectors and XPath locators for every interactive element on the form.
        // Keeping them as properties makes it easy to update a selector in one place
        // if the application HTML changes.
        private By FirstNameInput => By.CssSelector("input[name='input_5']");
        private By LastNameInput => By.CssSelector("input[name='input_18']");
        private By EmailInput => By.CssSelector("input[name='input_17']");
        private By PhoneInput => By.CssSelector("input[name='input_8']");
        private By HowDidYouHearDropdown => By.CssSelector("#input_7_9_chosen");
        private By CompanyInput => By.CssSelector("input[name='input_11']");
        // The budget dropdown is the only Chosen container that is NOT the "how did you hear" one.
        private By BudgetDropdown => By.CssSelector("div.chosen-container.chosen-container-single:not(#input_7_9_chosen)");
        private By ProjectDetailsTextarea => By.CssSelector("textarea[name='input_15']");
        // Resolves the service checkbox by matching the visible label text, avoiding fragile IDs.
        private By ServiceCheckboxByLabel(string label) => By.XPath($"//div[@class='gfield_checkbox ']//label[contains(.,'{label}')]");
        private By ConsentCheckbox => By.CssSelector("input[name='input_16.1']");
        private By SubmitButton => By.CssSelector("input[type='submit'], button[type='submit']");
        // Gravity Forms renders the confirmation inside a specific div; wait for it after submit.
        private By SuccessMessage => By.XPath("//div[@class='contact-form']//div[@id='gform_confirmation_message_7']");
        // Inline validation error locators — one per required field.
        private By FirstNameError => By.XPath("(//*[contains(@id,'validation_message_7_5')])[1]");
        private By LastNameError => By.XPath("(//*[contains(@id,'validation_message_7_18')])[1]");
        private By EmailError => By.XPath("(//*[contains(@id,'validation_message_7_17')])[1]");
        private By ConsentError => By.XPath("(//*[contains(@id,'validation_message_7_16')])[1]");
        private By FileUploadInput => By.CssSelector("input[type='file']");
        // Gravity Forms shows upload progress as a percentage inside this element.
        private By FileUploadProgress => By.CssSelector(".gfield_fileupload_percent");

        #endregion 


        // Receives the driver, wait helper, and the base URL from the DI container via Hooks.
        public ContactFormPage(IWebDriver driver, WaitHelper waitHelper, string baseUrl)
            : base(driver, waitHelper)
        {
            _pageUrl = baseUrl;
        }

        #region Actions

        // Navigates the browser to the contact form page URL.
        public void NavigateTo() => GoToUrl(_pageUrl);

        // Types the supplied value into the First Name field.
        public void EnterFirstName(string value) => TypeInto(FirstNameInput, value);

        // Types the supplied value into the Last Name field.
        public void EnterLastName(string value) => TypeInto(LastNameInput, value);

        // Types the supplied value into the Email field.
        public void EnterEmail(string value) => TypeInto(EmailInput, value);

        // Types the supplied value into the Phone Number field.
        public void EnterPhoneNumber(string value) => TypeInto(PhoneInput, value);

        // Types the supplied value into the Company field.
        public void EnterCompany(string value) => TypeInto(CompanyInput, value);

        // Types the supplied value into the Project Details textarea.
        public void EnterProjectDetails(string value) => TypeInto(ProjectDetailsTextarea, value);

        // Opens the "How did you hear about us?" Chosen dropdown and selects the given option.
        public void SelectHowDidYouHear(string option) =>
            SelectFromChosenDropdown(HowDidYouHearDropdown, option);

        // Opens the Budget Chosen dropdown and selects the given option.
        public void SelectBudget(string option) =>
            SelectFromChosenDropdown(BudgetDropdown, option);

        // Clicks the checkbox label that matches the given service name.
        public void SelectService(string serviceLabel) =>
            ClickOn(ServiceCheckboxByLabel(serviceLabel));

        // Checks the privacy policy consent checkbox if it is not already checked.
        public void AcceptConsent() => EnsureChecked(ConsentCheckbox);

        // Clicks the form's submit button to send the message.
        public void ClickSendMessage() => ClickOn(SubmitButton);

        // Uploads a file by iterating over all file input elements on the page,
        // making each one visible via JavaScript (Gravity Forms hides them by default),
        // sending the file path, waiting for the upload progress to reach 100%, and then
        // waiting for the progress indicator to disappear before returning.
        // Throws InvalidOperationException if no valid file input can accept the file.
        public void UploadFile(string filePath)
        {
            var inputs = Driver.FindElements(FileUploadInput);
            foreach (var input in inputs)
            {
                try
                {
                    // Make the hidden file input interactable so Selenium can send keys to it.
                    ((IJavaScriptExecutor)Driver).ExecuteScript(
                        "arguments[0].style.display='block'; arguments[0].style.visibility='visible';", input);
                    input.SendKeys(filePath);

                    // Poll until the upload progress indicator shows 100%.
                    Wait.WaitForCondition(
                        d => d.FindElement(FileUploadProgress).Text.Trim() == "100%",
                        timeoutSeconds: 30);

                    // Wait for the progress bar to disappear before declaring the upload complete.
                    Wait.WaitForElementNotVisible(FileUploadProgress, timeoutSeconds: 10);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File upload attempt failed: {ex.Message}");
                }
            }
            throw new InvalidOperationException($"Failed to upload file: {filePath}. No valid file input found.");
        }

        // Returns the full text of the success confirmation message displayed after form submission.
        // Uses the page-load timeout to give the server enough time to process the request.
        public string GetSuccessMessageText() =>
            GetText(SuccessMessage, timeoutSeconds: ConfigReader.PageLoadTimeoutSeconds);

        // Returns the inline validation error text for the named field.
        // Throws ArgumentException for unknown field names and NoSuchElementException
        // if the error message does not appear within the wait timeout.
        public string GetValidationErrorText(string fieldName)
        {
            // Map human-readable field names from feature files to the corresponding locators.
            var locator = fieldName.ToLower() switch
            {
                "first name" => FirstNameError,
                "last name" => LastNameError,
                "email" => EmailError,
                "consent" => ConsentError,
                _ => throw new ArgumentException($"Unknown field name: '{fieldName}'")
            };

            var text = GetText(locator, timeoutSeconds: 8);

            // An empty result means the error element was not rendered — treat it as missing.
            if (string.IsNullOrEmpty(text))
                throw new NoSuchElementException(
                    $"Validation error for '{fieldName}' not found or empty after 8s.");

            return text;
        }

        #endregion
    }
}