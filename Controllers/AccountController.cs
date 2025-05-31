using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModel;
using Blog.ViewModel.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {

        [HttpPost("v1/accounts/")]
        public async Task<IActionResult> Post([FromBody] RegisterViewModel model, [FromServices] BlogDataContext context, [FromServices] EmailService emailService)
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = new User
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(length: 32);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                emailService.Send(user.Name, user.Email, "Seja bem vindo", $"Sua senha é <strong>{password}</strong>");

                return Created($"v1/accounts/{user.Id}", new ResultViewModel<dynamic>(new
                {
                    user = user.Email,
                    password
                }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<Category>("04X01 - Não foi possível criar usuário"));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("04X02 - Falha interna do servidor."));
            }
        }

        [HttpPost("v1/accounts/login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model, [FromServices] BlogDataContext context, [FromServices] TokenService tokenService)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            var user = await context.Users
                .AsNoTracking()
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.Email == model.Email);

            if (user == null)
                return StatusCode(400, new ResultViewModel<string>("Usuário ou senha inválidos"));

            if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                return StatusCode(400, new ResultViewModel<string>("Usuário ou senha inválidos"));

            try
            {
                var token = tokenService.GenerateToken(user);

                return Ok(new ResultViewModel<string>(token, null));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>("04X02 - Falha interna do servidor."));
            }
        }

        [Authorize]
        [HttpPost("v1/accounts/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel model, [FromServices] BlogDataContext context) 
        {
            var filename = $"{Guid.NewGuid().ToString()}.jpg";

            var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");

            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{filename}", bytes);

                var user = await context.Users.FirstOrDefaultAsync(x=> x.Email == User.Identity.Name);

                if (user == null) return NotFound(new ResultViewModel<User>("Usuário não encontrado."));

                user.Image = $"https://localhost:0000/images{filename}.jpg";

                context.Users.Update(user);

                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<string>("Imagem atualizada com sucesso!"));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05X04 Erro interno do servidor."));
            }
        }

    }
}
