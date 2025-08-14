using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Web.Models.Domain;

public enum UserRole
{
    [Display(Name="User")]
    User=1,
    [Display(Name="Admin")]
    Admin=2
}