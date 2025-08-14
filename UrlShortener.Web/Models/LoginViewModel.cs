using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Web.Models;

public class LoginViewModel
{
    [Required]
    [Display(Name = "Login")]
    public string Login { get; set; } = string.Empty;
    
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
    
    public string? ReturnUrl { get; set; }
}