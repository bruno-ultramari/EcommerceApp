using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace EcommerceApp.Models
{
    public class ForgotPasswordModel(UserManager<User> userManager) : PageModel
    {
        private readonly UserManager<User> _userManager = userManager;

        [BindProperty]
        public required InputModel Input { get; set; }

        public class InputModel
        {
            public required string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user != null)
                {
                    // Enviar email de recuperação de senha (não implementado)
                }

                // Não revele se o email é válido ou não
                return RedirectToPage("ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
