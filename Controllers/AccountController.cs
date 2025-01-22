using EmpowerU.Models;
using EmpowerU.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace EmpowerU.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager; // Change here if User is int
        private readonly EmpowerUContext _context;
        private readonly Models.IEmailSender _emailSender;
        private readonly UrlEncoder _urlEncoder;

        public AccountController(EmpowerUContext context, ILogger<HomeController> logger, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole<int>> roleManager, Models.IEmailSender emailSender, UrlEncoder urlEncoder)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _urlEncoder = urlEncoder;
        }

        // GET: Home/Login
        // GET: Render the Login view
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Helps protect against CSRF attacks
        public async Task<IActionResult> Login([FromForm] Login loginModel)
        {


            if (!ModelState.IsValid)
            {
                return View(loginModel); // Return the view with validation errors
            }

            // Find the user by email
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user != null)
            {

                // Check if the email is confirmed
                if (!user.EmailConfirmed)
                {
                    // Add an error to the ModelState if the email is not confirmed
                    ModelState.AddModelError(string.Empty, "Please verify your email before logging in.");
                    ViewData["LoginError"] = "Please verify your email before logging in.";
                    return View(loginModel);
                }

                // Attempt to sign in using the UserName and Password
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Set the last login time
                    user.LastLogin = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    // Set session variables for the logged-in user
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserName", user.UserName);

                    // Redirect to the home page or desired page after successful login
                    return RedirectToAction("Index", "Home");
                }
            }

            // Add an error to the model state for invalid login attempts
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            ViewData["LoginError"] = "Invalid login attempt.";
            return View(loginModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Log out the user
            await _signInManager.SignOutAsync();

            // Clear all session variables
            HttpContext.Session.Clear();

            // Redirect to the login page after logout
            return RedirectToAction("Login", "Account");
        }


        public IActionResult RegisterConsumer()
        {
            return View();  // This will render the RegisterConsumer view.
        }

        // POST: Consumers/RegisterConsumer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterConsumer([Bind("PreferredCategories,UserName,Name,Surname,Email,PhoneNumber,Password,ConfirmPassword")] Consumer consumer)
        {
            if (ModelState.IsValid)
            {
                // Phone Number validation: Ensure correct format
                if (!Regex.IsMatch(consumer.PhoneNumber, @"^\d{10}$")) // Example for a 10-digit phone number
                {
                    ModelState.AddModelError("PhoneNumber", "Please enter a valid 10-digit phone number.");
                    return View(consumer);
                }

                // Enhanced Email validation using a stricter regular expression
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(consumer.Email, emailRegex))
                {
                    ModelState.AddModelError("Email", "Please enter a valid email address.");
                    return View(consumer);
                }

                // Check if the email is already in use
                var existingUserWithEmail = await _userManager.FindByEmailAsync(consumer.Email);
                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(consumer);
                }

                // Check if the phone number is already in use
                var existingUserWithPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == consumer.PhoneNumber);
                if (existingUserWithPhone != null)
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(consumer);
                }

                var user = new Consumer
                {
                    Email = consumer.Email,
                    Name = consumer.Name,
                    Surname = consumer.Surname,
                    PhoneNumber = consumer.PhoneNumber,
                    PreferredCategories = consumer.PreferredCategories,
                    UserName = consumer.UserName,
                    Role = "Consumer"
                };

                var result = await _userManager.CreateAsync(user, consumer.Password);
                if (result.Succeeded)
                {
                    // Ensure the role exists before assigning
                    if (!await _roleManager.RoleExistsAsync("Consumer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("Consumer")); // Use int if User is int
                    }

                    await _userManager.AddToRoleAsync(user, "Consumer");

                    // Generate the email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Create a confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    // Send the email confirmation
                    var emailMessage = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #ddd;
            border-radius: 10px;
            background-color: #fff;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #fff;
            text-align: center;
            padding: 15px;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            text-align: center;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .content a {{
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            font-size: 16px;
            border-radius: 5px;
        }}
        .content a:hover {{
            background-color: #0056b3;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 12px;
            color: #aaa;
            border-top: 1px solid #ddd;
        }}
        .footer p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>Email Confirmation</h1>
        </div>
        <div class='content'>
            <p>Thank you for signing up! Please confirm your email address to activate your account.</p>
            <p>Click the button below to verify your email:</p>
            <a href='{confirmationLink}'>Confirm Email</a>
        </div>
        <div class='footer'>
            <p>If you did not sign up, you can safely ignore this email.</p>
            <p>&copy; {DateTime.UtcNow.Year} EmpowerU. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";
                    await _emailSender.SendEmailAsync(consumer.Email, "Email Confirmation", emailMessage);

                    // Notify the user to check their email for confirmation
                    TempData["Message"] = "Registration successful! Please check your email to confirm your account.";

                    TempData["Email"] = consumer.Email;
                    return RedirectToAction("EmailVerificationPending", "Account");
                }

                // Capture and log any creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }
            }

            // If model state is invalid, redisplay the form with errors
            return View(consumer);
        }



        // GET: Businesses/RegisterBusiness
        public IActionResult RegisterBusiness()
        {
            ViewData["ActivePage"] = "RegisterBusiness"; // Set active page for List Business
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address");
            return View();
        }

        // POST: Businesses/RegisterBusiness
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterBusiness([Bind("Description,Rating,Name,UserName,Email,PhoneNumber,Password,ConfirmPassword,BusinessCategory,LocationService")] Business business)
        {
            if (ModelState.IsValid)
            {
                // Phone Number validation: Ensure correct format
                if (!Regex.IsMatch(business.PhoneNumber, @"^\d{10}$")) // Example for a 10-digit phone number
                {
                    ModelState.AddModelError("PhoneNumber", "Please enter a valid 10-digit phone number.");
                    return View(business);
                }

                // Enhanced Email validation using a stricter regular expression
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(business.Email, emailRegex))
                {
                    ModelState.AddModelError("Email", "Please enter a valid email address.");
                    return View(business);
                }

                // Check if the email is already in use
                var existingUserWithEmail = await _userManager.FindByEmailAsync(business.Email);
                if (existingUserWithEmail != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(business);
                }

                // Check if the phone number is already in use
                var existingUserWithPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == business.PhoneNumber);
                if (existingUserWithPhone != null)
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(business);
                }

                // Check if LocationService is provided
                if (business.LocationService != null)
                {
                    // Ensure the LocationService has necessary fields set
                    if (string.IsNullOrWhiteSpace(business.LocationService.Address))
                    {
                        ModelState.AddModelError("LocationService.Address", "Address is required.");
                        return View(business);
                    }

                    // Add LocationService and save changes to get LocationID
                    _context.LocationServices.Add(business.LocationService);
                    await _context.SaveChangesAsync(); // Save to get the LocationID
                }

                var user = new Business
                {
                    Email = business.Email,
                    Name = business.Name,
                    UserName = business.UserName,
                    PhoneNumber = business.PhoneNumber,
                    Role = "Business",
                    LocationID = business.LocationService?.LocationID ?? 0, // Use the newly created LocationID
                    Description = business.Description,
                    Rating = 0.00m,
                    BusinessCategory = business.BusinessCategory
                };

                // Create user in the Identity framework
                var result = await _userManager.CreateAsync(user, business.Password);
                if (result.Succeeded)
                {
                    // Ensure the role exists before assigning
                    if (!await _roleManager.RoleExistsAsync("Business"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<int>("Business")); // Use int if User is int
                    }
                    await _userManager.AddToRoleAsync(user, "Business");

                    // Generate the email confirmation token
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // Create a confirmation link
                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                    // Improved Email Content
                    var emailMessage = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Confirmation</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #ddd;
            border-radius: 10px;
            background-color: #fff;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #007bff;
            color: #fff;
            text-align: center;
            padding: 15px;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            text-align: center;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .content a {{
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #007bff;
            color: #fff;
            text-decoration: none;
            font-size: 16px;
            border-radius: 5px;
        }}
        .content a:hover {{
            background-color: #0056b3;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 12px;
            color: #aaa;
            border-top: 1px solid #ddd;
        }}
        .footer p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>Email Confirmation</h1>
        </div>
        <div class='content'>
            <p>Thank you for signing up! Please confirm your email address to activate your account.</p>
            <p>Click the button below to verify your email:</p>
            <a href='{confirmationLink}'>Confirm Email</a>
        </div>
        <div class='footer'>
            <p>If you did not sign up, you can safely ignore this email.</p>
            <p>&copy; {DateTime.UtcNow.Year} EmpowerU. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";

                    // Send the email
                    await _emailSender.SendEmailAsync(business.Email, "Email Confirmation", emailMessage);


                    // Notify the user to check their email for confirmation
                    TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
                    TempData["Email"] = business.Email;
                    return RedirectToAction("EmailVerificationPending", "Account");
                }

                // Capture and log any creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }
            }

            // Ensure that LocationID is populated for dropdown lists in the View
            ViewData["LocationID"] = new SelectList(_context.LocationServices, "LocationID", "Address", business.LocationService?.LocationID);

            return View(business);
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            // Return the login view with a flag to indicate forgot password
            return View(new Login { IsForgotPassword = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(Login model)
        {
            ModelState.Remove("Password");

            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                // Generate the password reset token
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, email = user.Email, token = token }, Request.Scheme);

                var emailMessage = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Password</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            background-color: #f4f4f4;
        }}
        .email-container {{
            width: 100%;
            max-width: 600px;
            margin: 20px auto;
            border: 1px solid #ddd;
            border-radius: 10px;
            background-color: #fff;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #dc3545;
            color: #fff;
            text-align: center;
            padding: 15px;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
        }}
        .content {{
            padding: 20px;
            text-align: center;
            color: #333;
        }}
        .content p {{
            font-size: 16px;
            line-height: 1.6;
        }}
        .content a {{
            display: inline-block;
            margin-top: 20px;
            padding: 10px 20px;
            background-color: #dc3545;
            color: #fff;
            text-decoration: none;
            font-size: 16px;
            border-radius: 5px;
        }}
        .content a:hover {{
            background-color: #c82333;
        }}
        .footer {{
            text-align: center;
            padding: 10px;
            font-size: 12px;
            color: #aaa;
            border-top: 1px solid #ddd;
        }}
        .footer p {{
            margin: 0;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>Reset Your Password</h1>
        </div>
        <div class='content'>
            <p>We received a request to reset your password. If you made this request, click the button below to reset your password:</p>
            <a href='{callbackUrl}'>Reset Password</a>
            <p>If you did not request a password reset, you can safely ignore this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; {DateTime.UtcNow.Year} EmpowerU. All rights reserved.</p>
        </div>
    </div>
</body>
</html>
";


                // Send email with the reset link
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", emailMessage);

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            else
            {
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    // Log error message
                    Console.WriteLine(error);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public IActionResult VerificationSuccess()
        {
            return View();
        }


        public IActionResult EmailVerificationPending()
        {
            return View();
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(int userId, string token)
        {
            if (userId == 0 || string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("Error", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return RedirectToAction("Error", "Home");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Message"] = "Email confirmed successfully!";
                return RedirectToAction("VerificationSuccess", "Account");
            }

            TempData["Error"] = "Email confirmation failed.";
            return RedirectToAction("Error", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> ResendVerification()
        {
            // Get the user by their email from the session or other mechanism (e.g., logged-in user)
            var userId = HttpContext.Session.GetString("UserId");
            if (userId == null)
            {
                _logger.LogError("User ID not found in session.");
                return RedirectToAction("Login"); // Redirect to login if user is not authenticated
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect to login if user doesn't exist
            }

            // Check if the user's email is already confirmed
            if (user.EmailConfirmed)
            {
                // If already confirmed, no need to resend the verification email
                TempData["Error"] = $"Your email ({user.Email}) is already verified.";
                return RedirectToAction("Login"); // Or redirect as needed
            }

            // Generate the email verification token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            // Send the email (you need to configure an email service here)
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", $"Please confirm your email by clicking <a href='{callbackUrl}'>here</a>");

            // Provide feedback to the user
            TempData["Success"] = "Verification email has been resent. Please check your inbox.";
            return RedirectToAction("EmailVerificationPending"); // Redirect back to the verify email page
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("Missing email or token.");
                return RedirectToAction("ForgotPassword");
            }

            ViewData["Token"] = token; // Pass token to the view
            var user = new User { Email = email };
            return View(user); // Use the User model
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(User model, string token)
        {
            ModelState.Remove("Name");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }

                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with the given email address.");
                return View(model);
            }

            // Reset the password using the provided token
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your password has been successfully reset.";
                return RedirectToAction("ResetPasswordSuccessful");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ResetPasswordSuccessful()
        {
            return View();
        }

    }

}
