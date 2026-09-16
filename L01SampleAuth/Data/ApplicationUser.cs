using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace L01SampleAuth.Data;
// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

}
