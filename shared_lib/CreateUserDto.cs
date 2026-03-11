using System.ComponentModel.DataAnnotations;

namespace shared_lib
{
    public class CreateUserDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
