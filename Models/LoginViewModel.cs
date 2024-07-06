using System.ComponentModel.DataAnnotations;

namespace BlogApp.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@gmail\\.com$")]
        public string Email{ get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20,MinimumLength=1,ErrorMessage = "{0} alanı en az {2} karakter uzunluğunda olmalıdır")]
        [Display(Name = "Parola")]
        public string Password{ get; set; }
    }
}
