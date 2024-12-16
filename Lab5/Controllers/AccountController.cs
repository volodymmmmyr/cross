using Lab5.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Auth0.AspNetCore.Authentication;

namespace Lab5.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Login(string returnUrl = "/")
        {
            var loginProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, loginProperties);
        }

        [Authorize]
        public async Task Logout()
        {
            var logoutProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(Url.Action("Index", "Home"))
                .Build();

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, logoutProperties);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            var profileData = new UsViewModel
            {
                Username = User.Claims.FirstOrDefault(claim => claim.Type == "nickname")?.Value,
                Email = User.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value
            };

            if (userId != null)
            {
                var metadata = await GetUserMetadataAsync(userId);
                if (metadata != null)
                {
                    profileData.FullName = metadata.FullName;
                    profileData.PhoneNumber = metadata.PhoneNumber;
                }
            }

            return View(profileData);
        }

        private async Task<UserMetadata> GetUserMetadataAsync(string userId)
        {
            using var httpClient = new HttpClient();
            var accessToken = await FetchManagementApiTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var auth0Domain = _configuration["Auth0:Domain"];
            var response = await httpClient.GetAsync($"https://{auth0Domain}/api/v2/users/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var auth0User = await response.Content.ReadFromJsonAsync<Auth0UserProfile>();
                return auth0User?.UserMetadata;
            }

            return null;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UsModel userModel)
        {
            if (!ModelState.IsValid)
                return View(userModel);

            using var httpClient = new HttpClient();
            var accessToken = await FetchManagementApiTokenAsync();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var newUserRequest = new
            {
                email = userModel.Email,
                user_metadata = new
                {
                    full_name = userModel.FullName,
                    phone_number = userModel.PhoneNumber
                },
                connection = "Username-Password-Authentication",
                password = userModel.Password,
                username = userModel.Username
            };

            var auth0Domain = _configuration["Auth0:Domain"];
            var response = await httpClient.PostAsJsonAsync($"https://{auth0Domain}/api/v2/users", newUserRequest);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration failed: {response.StatusCode}, {errorResponse}");
            ModelState.AddModelError(string.Empty, $"Registration failed: {errorResponse}");

            return View(userModel);
        }

        private async Task<string> FetchManagementApiTokenAsync()
        {
            using var httpClient = new HttpClient();
            var clientId = _configuration["Auth0:ClientId"];
            var clientSecret = _configuration["Auth0:ClientSecret"];
            var auth0Domain = _configuration["Auth0:Domain"];

            var tokenRequestPayload = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                audience = $"https://{auth0Domain}/api/v2/",
                grant_type = "client_credentials"
            };

            var tokenResponse = await httpClient.PostAsJsonAsync($"https://{auth0Domain}/oauth/token", tokenRequestPayload);
            var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();

            return tokenData?.AccessToken;
        }

        private class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }
        }

        private class Auth0UserProfile
        {
            [JsonPropertyName("user_id")]
            public string UserId { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("user_metadata")]
            public UserMetadata UserMetadata { get; set; }
        }

        private class UserMetadata
        {
            [JsonPropertyName("full_name")]
            public string FullName { get; set; }

            [JsonPropertyName("phone_number")]
            public string PhoneNumber { get; set; }
        }
    }

}
