
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EcommerceApp.Models
{

    public class LoginModel : PageModel

    {
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public LoginModel(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;

            Input = new InputModel()
            {
                Email = string.Empty,
                Password = string.Empty
            };


        }

        public class InputModel
        {

            public required string Email { get; set; }


            public required string Password { get; set; }

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToPage("Index"); // Redirecionar após o login bem-sucedido
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return Page();
        }
    }
}
