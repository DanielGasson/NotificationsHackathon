using System.ComponentModel.DataAnnotations;

namespace Notifications.Web.Models
{

    public class LoginModel
    {

        [Required]
        public string Name { get; set; }
    }
}