using BrewMVC.Models;
using BrewMVC.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BrewMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _client;

        public AuthController(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("BrewAPI");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserVM loginUser)
        {
            var response = await _client.PostAsJsonAsync("Auth/Login", loginUser);
            if (!response.IsSuccessStatusCode)
            {
                // TODO: Login failed - Return the user to the login view with error
                return View(loginUser);
            }

            // Login successful: Read the JWT token from the API response
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseModel>();
            var jwt = tokenResponse.Token;

            // Store the JWT (claim) token in a object
            var handler = new JwtSecurityTokenHandler();
            var jwtObject = handler.ReadJwtToken(jwt);

            // Create list of claims from the object
            var claims = jwtObject.Claims.ToList();
            // Create claims identity and principal for cookie authentication (login)
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            // Sign in the user async with the claims principal, authentication properties and cookie authentication scheme
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                claimsPrincipal, new AuthenticationProperties
                {   // User will stay logged in after closing the browser
                    IsPersistent = true,
                    ExpiresUtc = jwtObject.ValidTo
                });

            // Store the JWT token in a secure HttpOnly cookie
            HttpContext.Response.Cookies.Append("jwtToken", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                // Cookie expiration depends on value from API/Backend
                Expires = jwtObject.ValidTo
            });

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            HttpContext.Response.Cookies.Delete("jwtToken");

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
