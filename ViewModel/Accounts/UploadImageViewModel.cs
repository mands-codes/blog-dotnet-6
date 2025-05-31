using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts
{
    public class UploadImageViewModel
    {
        [Required]
        public string Base64Image { get; set; }
    }
}
