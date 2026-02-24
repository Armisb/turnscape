using System.ComponentModel.DataAnnotations;

namespace GameAPI.NewFolder
{
    public class LoginUserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
