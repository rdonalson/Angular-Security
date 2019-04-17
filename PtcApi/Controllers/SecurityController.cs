using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PtcApi.Model;
using PtcApi.Security;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PtcApi.Controllers
{
	[Route("api/[controller]")]
	public class SecurityController : Controller
	{
		// GET: api/<controller>
		[HttpPost("login")]
		public IActionResult Login([FromBody] AppUser user)
		{
			IActionResult ret = null;
			AppUserAuth auth = new AppUserAuth();
			SecurityManager mgr = new SecurityManager();

			auth = mgr.ValidateUser(user);
			ret = auth.IsAuthenticated
				? StatusCode(StatusCodes.Status200OK, auth)
				: StatusCode(StatusCodes.Status404NotFound, "Invalid User Name/Password");
			return ret;
		}
	}
}