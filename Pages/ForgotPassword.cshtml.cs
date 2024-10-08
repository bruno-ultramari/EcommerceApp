using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration; // Importante para acessar as configurações
using EcommerceApp.Models;

namespace EcommerceApp.Pages
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public InputModel Input { get; set; }

        // Ajuste para injetar SmtpClient e IConfiguration
        public ForgotPasswordModel(UserManager<User> userManager, SmtpClient smtpClient, IConfiguration configuration)
        {
            _userManager = userManager;
            _smtpClient = smtpClient;
            _configuration = configuration;

            Input = new InputModel
            {
                Email = string.Empty,
            };
        }

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
                    // Gera o token de recuperação de senha
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Cria o link de reset de senha
                    string? resetLink = Url.Page(
                       "/ResetPassword",
                       pageHandler: null,
                       values: new { userId = user.Id, token = token },
                       protocol: Request.Scheme);

                    // Envia o e-mail de recuperação
                    if (!string.IsNullOrEmpty(resetLink))
                    {
                        // Envia o e-mail de recuperação
                        await SendPasswordResetEmail(Input.Email, resetLink);
                    }
                    else
                    {
                        // Lida com o caso de resetLink ser null (opcional: logar ou exibir mensagem)
                        ModelState.AddModelError(string.Empty, "Ocorreu um erro ao gerar o link de recuperação de senha.");
                    }
                }

                // Não revele se o email é válido ou não
                return RedirectToPage("ForgotPasswordConfirmation");
            }

            return Page();
        }

        private async Task SendPasswordResetEmail(string email, string resetLink)
        {
            // Obtenha o nome de usuário do arquivo de configuração
            var userName = _configuration["Smtp:UserName"];

            // Verifica se o nome de usuário do e-mail é nulo ou vazio
            if (string.IsNullOrEmpty(userName))
            {
                // Lida com o caso onde o nome de usuário não está configurado corretamente
                throw new InvalidOperationException("O nome de usuário do SMTP não está configurado.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(userName), // Garante que o userName não seja nulo
                Subject = "Recuperação de Senha",
                Body = $"Clique no link para resetar sua senha: <a href='{resetLink}'>Resetar Senha</a>",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            await _smtpClient.SendMailAsync(mailMessage);
        }

    }
}
