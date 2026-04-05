string uid = user_id;
string token = access_token;
           
string jsonPayload = $"{{\"uid\":\"{uid}\"}}";

// 3. Wrap it in the jData parameter and use application/x-www-form-urlencoded
var content = new StringContent($"jData={jsonPayload}", Encoding.UTF8, "application/x-www-form-urlencoded");

const string ORDER_URL = "https://trade.shoonya.com/NorenWClientAPI/UserDetails";

_httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", token);
var response = await _httpClient.PostAsync(ORDER_URL, content);
string result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
