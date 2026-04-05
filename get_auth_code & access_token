//use latest selliunm chrome driver
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers; // Requires 'DotNetSeleniumExtras.WaitHelpers' NuGet package
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using OtpNet; // For TOTP

private async void get_auth_access_token()
{
    try
    {
        // 1. Generate TOTP (Fine on UI thread)
        var secretKeyBytes = Base32Encoding.ToBytes(ent_shoonya_totp_key.Text);
        var totp = new Totp(secretKeyBytes);
        string totpCode = totp.ComputeTotp();
        

        // Capture UI values into local variables to use inside Task.Run safely
        string userId = "user_id";
        string password = "password";
        string venderCode = "vendor_code";
        string secretKey = "seceret_key";

        ChromeOptions options = new ChromeOptions();
        //options.AddArgument("--headless"); 
        //options.AddArgument("--no-sandbox");
        //options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-first-run");
        options.AddArgument("--no-default-browser-check");
        options.AddArgument("--disable-search-engine-choice-screen"); // New for recent versions
        options.AddArgument("--user-data-dir=C:\\temp\\shoonya_profile"); // Forces a dedicated profile

        IWebDriver driver = new ChromeDriver(options);
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));


        await Task.Run(() => {
            
            
            try
            {
                
                // 1. Clear cookies first
                driver.Manage().Cookies.DeleteAllCookies();

                // 2. Go straight to the login URL
                driver.Navigate().GoToUrl($"https://trade.shoonya.com/OAuthlogin/investor-entry-level/login?api_key={venderCode}");

                
                var userField = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lgnusrid")));
                userField.SendKeys(userId);
                driver.FindElement(By.Id("lgnpwd")).SendKeys(password);

                var otpField = driver.FindElement(By.Id("lgnotp"));
                otpField.SendKeys(totpCode + Keys.Enter);

                // 3. Wait for the URL to change and contain the code
                wait.Until(d => d.Url.Contains("code="));

                string authCode = driver.Url.Split('=').Last();

                // 4. Update UI
                this.Invoke(new Action(() => { ent_shoonya_code.Text = authCode; }));

                // Success! Now close it.
                driver.Quit();


            }
            catch (Exception ex)
            {
                driver.Quit(); // Ensure it closes on error too
                Console.WriteLine("Error during Shoonya login: " + ex.ToString());
            }



        });


        if (authCode != null)
            {
                

                string rawData = venderCode + secretKey + authCode;
                string checksum = "";
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                    checksum = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                }

                const string TOKEN_URL = "https://api.shoonya.com/NorenWClientAPI/GenAcsTok";
                var payload = new { code = authCode, checksum = checksum };
                string jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent($"jData={jsonPayload}", Encoding.UTF8, "application/x-www-form-urlencoded");

                
                var response = _httpClient.PostAsync(TOKEN_URL, content).GetAwaiter().GetResult();
                string result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                Console.WriteLine(result);

                using JsonDocument doc = JsonDocument.Parse(result);
                JsonElement root = doc.RootElement;

            if (root.GetProperty("stat").GetString() == "Ok")
            {
                string accessToken = root.GetProperty("susertoken").GetString();
                Console.WriteLine(accessToken);

               
            }
            else
            {

                Console.WriteLine("stat ok not found");
            }

        }
        else
        {
            
            Console.WriteLine("authCode is Null") ;
        }
       
        
    }
    catch (Exception ex)
    {
        
        Console.WriteLine(ex.ToString());
    }
}
