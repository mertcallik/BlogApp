using System.ComponentModel.DataAnnotations;

namespace BlogApp
{
    public class UserRegisterModel
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Email alanı boş geçilemez")]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        [RegularExpression("^[a-zA-Z0-9._%+-]+@gmail\\.com$",ErrorMessage = "Sadece gmail hesabınız ile giriş yapabilirsiniz.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Şifre boş geçilemez")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "{0} alanı en az {2} karakter uzunluğunda olmalıdır")]
        [Display(Name = "Parola")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage = "Şifreler Uyuşmuyor")]

        
        public string PasswordConfirm { get; set; }

        public string Image { get; set; } = "Avatar.png";
    }
}