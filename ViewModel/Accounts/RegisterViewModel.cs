using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é invalido")]
        public string Email { get; set; }

    }
}
