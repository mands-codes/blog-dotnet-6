using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Categories
{
    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 100 caracteres.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O slug é obrigatório")]
        public string Slug { get; set; }
    }
}
