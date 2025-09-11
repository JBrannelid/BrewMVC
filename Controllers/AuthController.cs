using BrewMVC.Models;
using BrewMVC.ViewModel.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

// Error handling: Helper methods for TempData.
// ModelState is lost on redirect but TempData persists for one http reques
// Same View() errors: ModelState since we return View()
// Redirect errors: TempData since we return RedirectToAction()

namespace BrewMVC.Controllers
{
    public class AuthController : BaseController 
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
            if (!ModelState.IsValid)
            {
                return View(loginUser);
            }

            try
            {
                var response = await _client.PostAsJsonAsync("Auth/Login", loginUser);

                if (!response.IsSuccessStatusCode)
                {
                    // Handle different API errors for login view()
                    string errorMessage;
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            errorMessage = "Invalid email or password";
                            break;
                        case HttpStatusCode.BadRequest:
                            errorMessage = "Please check your email and password";
                            break;
                        default:
                            errorMessage = "Login failed. Please try again";
                            break;
                    }

                    ModelState.AddModelError("", errorMessage);
                    return View(loginUser);
                }

                // Login successful: Read the JWT token from the API response
                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponseModel>();
                if (tokenResponse?.Token == null)
                {
                    ModelState.AddModelError("", "Login failed. Please try again");
                    return View(loginUser);
                }

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
                    {
                        // User will stay logged in after closing the browser
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
            catch (Exception)
            {
                ModelState.AddModelError("", "An error occurred during login. Please try again");
                return View(loginUser);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out the user from cookie authentication
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Response.Cookies.Delete("jwtToken");

                SetSuccessMessage("You have been logged out successfully");
            }
            catch (Exception)
            {
                SetErrorMessage("An error occurred during logout");
            }

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}