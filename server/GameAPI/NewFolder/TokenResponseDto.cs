using GameAPI.Controllers;
using System;

namespace GameAPI.NewFolder
{

	public class TokenResponseDto
	{
		public required string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
