using System;
namespace shared_lib
{
    public class RefreshTokenRequestDto
    {
        public Guid Id { get; set; }
        public string RefreshToken { get; set; }

    }
}
