using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe a senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é invalido")]
        public string Email { get; set; }
    }
}
