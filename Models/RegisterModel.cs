using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EcommerceApp.Models
{

    public class RegisterModel : PageModel

    {
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        private InputModel Input { get; set; }

        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            Input = new InputModel
            {
                FirstName = string.Empty, // Inicializa com valores padrão
                LastName = string.Empty,
                Email = string.Empty,
                Password = string.Empty,
                ConfirmPassword = string.Empty,
                State = string.Empty,
                Country = string.Empty,
                Address = string.Empty,
                City = string.Empty,
                DateOfBirth = DateTime.Today
            };
        }



        public class InputModel
        {

            public required string FirstName { get; set; }
            public required string LastName { get; set; }

            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public required string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação não coincidem.")]
            public required string ConfirmPassword { get; set; }

            public required string Country { get; set; }
            public required string State { get; set; }
            public required string Address { get; set; }

            public required string City { get; set; }
            public required DateTime DateOfBirth { get; set; }



        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    State = Input.State,
                    Country = Input.Country,
                    Address = Input.Address,
                    DateOfBirth = Input.DateOfBirth,
                    City = Input.City

                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    // Redirecionar ou realizar login
                    return RedirectToPage("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}