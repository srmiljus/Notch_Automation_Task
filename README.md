# Notch Contact Form — Test Automation Suite

Automated test suite for the [Notch QA Task contact form](https://wearenotch.com/qa_task/).

**Stack:** C# · Selenium WebDriver · Reqnroll (BDD) · NUnit · ExtentReports · BoDi

---

## Project Structure

```
NotchContactFormTests/
├── Config/
│   └── ConfigReader.cs          # Reads appsettings.json into typed TestSettings
├── Drivers/
│   ├── DriverContext.cs         # BoDi-managed WebDriver lifecycle wrapper
│   └── WebDriverFactory.cs      # Creates ChromeDriver with configured options
├── Features/
│   └── ContactForm.feature      # All Gherkin scenarios
├── Helpers/
│   ├── ExtentReportHelper.cs    # Singleton Extent Reports wrapper
│   ├── ScreenshotHelper.cs      # Screenshot capture (file + base64)
│   └── WaitHelper.cs            # Explicit wait helpers (ExpectedConditions)
├── Models/
│   └── ContactFormData.cs       # Form data model with factory methods
├── Pages/
│   └── ContactFormPage.cs       # Page Object Model for the contact form
├── StepDefinitions/
│   ├── ContactFormSteps.cs      # Reqnroll step bindings
│   └── Hooks.cs                 # BeforeScenario / AfterScenario / AfterStep hooks
├── Reports/                     # Generated HTML reports (git-ignored)
├── Screenshots/                 # Captured screenshots (git-ignored)
├── appsettings.json             # Test configuration
└── reqnroll.json                # Reqnroll framework configuration
```

---

## Prerequisites

| Tool | Version | Notes |
|------|---------|-------|
| .NET SDK | 8.0+ | [Download](https://dotnet.microsoft.com/download) |
| Google Chrome | Latest stable | Must match ChromeDriver version |
| Git | Any | For cloning the repo |

> **ChromeDriver version**: The project references `Selenium.WebDriver.ChromeDriver` via NuGet.  
> The package version must match your installed Chrome. Update the version in the `.csproj` if needed.  
> Check your Chrome version at `chrome://settings/help`.

---

## Getting Started

### 1. Clone the repository

```bash
git clone <repository-url>
cd NotchContactFormTests
```

### 2. Restore NuGet packages

```bash
dotnet restore NotchContactFormTests/NotchContactFormTests.csproj
```

### 3. Configure settings

Edit `appsettings.json` to match your environment:

```json
{
  "TestSettings": {
    "BaseUrl": "https://wearenotch.com/qa_task/",
    "Browser": "Chrome",
    "Headless": false,
    "ImplicitWaitSeconds": 10,
    "ExplicitWaitSeconds": 15,
    "PageLoadTimeoutSeconds": 30
  }
}
```

Set `"Headless": true` to run without a visible browser window (useful for CI).

### 4. Build the project

```bash
dotnet build NotchContactFormTests/NotchContactFormTests.csproj
```

### 5. Run all tests

```bash
dotnet test NotchContactFormTests/NotchContactFormTests.csproj
```

---

## Running Specific Test Subsets

Reqnroll tags are used to filter scenarios. Use the `--filter` flag with NUnit category format:

```bash
# Smoke tests only
dotnet test --filter "Category=smoke"

# All happy path tests
dotnet test --filter "Category=happy-path"

# All validation tests
dotnet test --filter "Category=validation"

# UI visibility tests
dotnet test --filter "Category=ui"

# Skip the intentional fail test
dotnet test --filter "Category!=intentional-fail"

# Run only the intentional fail test
dotnet test --filter "Category=intentional-fail"
```

---

## Test Report

After each run, an HTML report is generated in the `Reports/` folder:

```
NotchContactFormTests/bin/Debug/net8.0/Reports/NotchContactFormReport_yyyyMMdd_HHmmss.html
```

Open it in any browser. The report includes:
- Pass/fail status per scenario
- Step-by-step execution log
- Screenshots embedded for every scenario (pass and fail)
- System info (browser, URL, framework)

---

## Screenshots

Screenshots are saved in the `Screenshots/` folder alongside the report:

```
NotchContactFormTests/bin/Debug/net8.0/Screenshots/
```

A screenshot is captured:
- **After every scenario** (final state)
- **At the failing step** when a scenario fails

---

## Test Scenarios Overview

| Tag | Count | Description |
|-----|-------|-------------|
| `@smoke` | 2 | Critical path — quick sanity check |
| `@happy-path` | 12 | Successful form submissions |
| `@validation` | 13 | Required field and format validation |
| `@ui` | 3 | Element visibility and page load |
| `@intentional-fail` | 1 | Deliberately failing — for demo purposes |

### Intentionally Failing Test

The scenario tagged `@intentional-fail` is **designed to fail**. It asserts a success message text that does not match what the application actually displays. This demonstrates:

1. How assertion failures are reported in ExtentReports
2. Screenshot capture at the point of failure  
3. Step-level failure logging in the report

This is **not a bug in the application**. The scenario is documented as expected-to-fail behavior.

---

## Updating Locators

All locators are centralized in `Pages/ContactFormPage.cs`. If the Notch website updates its DOM, update the `By` selectors in that file only — no changes needed in step definitions or feature files.

The form uses **Gravity Forms** (WordPress), so field `name` attributes follow the pattern `input_N` or `input_N.M` for sub-fields (e.g. name parts).

---

## Risks & Known Limitations

| Risk | Impact | Notes |
|------|--------|-------|
| Live site dependency | High | Tests run against production; the form could be down or changed |
| Real form submissions | Medium | Each happy-path test submits a real contact request to Notch |
| ChromeDriver version mismatch | High | Must match installed Chrome version — update NuGet package if needed |
| Cookie consent banner | Low | Hooks attempt to dismiss it automatically; may need updating |
| Rate limiting / CAPTCHA | Medium | If Notch adds CAPTCHA, happy-path tests will fail |
| Flaky locators | Medium | Gravity Forms generates `input_N` names — stable unless form is rebuilt |

---

## Possible Improvements to the Form

Based on testing observations:

1. **No character limit indicator** on the Project Details textarea — users don't know the limit
2. **Phone field accepts any input** — no format validation for phone numbers
3. **No autofocus** on first field after page load
4. **File upload** gives no feedback about accepted file types before upload
5. **No inline validation** — errors only appear after submit, not on field blur
6. **Consent checkbox** is visually small — accessibility concern on mobile

---

## CI/CD Integration

To run in a CI pipeline (e.g. GitHub Actions), set `Headless: true` in `appsettings.json` or override via environment:

```yaml
# .github/workflows/tests.yml
- name: Run Selenium Tests
  run: dotnet test NotchContactFormTests/NotchContactFormTests.csproj --filter "Category!=intentional-fail"
```

For the intentional fail test, run it separately and mark it as `continue-on-error: true`.
