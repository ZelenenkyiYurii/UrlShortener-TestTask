using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Web.Models.Domain;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }=string.Empty;
    public string PasswordHash { get; set; }=string.Empty;
    public UserRole Role { get; set; }= UserRole.User;
    public DateTime Created { get; set; }=DateTime.UtcNow;
    public virtual ICollection<UrlMapping> CreatedUrls { get; set; } = new List<UrlMapping>();
}