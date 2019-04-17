using System;
using System.Collections.Generic;
using System.Linq;
using PtcApi.Model;

namespace PtcApi.Security
{
	public class SecurityManager
	{
		public AppUserAuth ValidateUser(AppUser user)
		{
			AppUserAuth ret = new AppUserAuth();
			AppUser authUser = null;

			using (PtcDbContext db = new PtcDbContext())
			{
				// Attempt to validate user
				authUser = db.Users.FirstOrDefault(u =>
					string.Equals(u.UserName, user.UserName, StringComparison.CurrentCultureIgnoreCase)
					&& u.Password == user.Password);
			}

			if (authUser != null) ret = BuildUserAuthObject(authUser);
			return ret;
		}

		private List<AppUserClaim> GetUserClaims(AppUser authUser)
		{
			List<AppUserClaim> list = new List<AppUserClaim>();
			try
			{
				using (PtcDbContext db = new PtcDbContext())
				{
					list = db.Claims.Where(u => u.UserId == authUser.UserId).ToList();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Exception trying to retrieve user claims", ex);
			}

			return list;
		}

		private AppUserAuth BuildUserAuthObject(AppUser authUser)
		{
			AppUserAuth ret = new AppUserAuth();
			var claims = new List<AppUserClaim>();

			// Set User Properties
			ret.UserName = authUser.UserName;
			ret.IsAuthenticated = true;
			ret.BearerToken = new Guid().ToString();

			// Get all claims for the user
			claims = GetUserClaims(authUser);

			// Loop through all claims and
			// set properties of user object
			foreach (AppUserClaim claim in claims)
				try
				{
					typeof(AppUserAuth).GetProperty(claim.ClaimType).SetValue(ret, Convert.ToBoolean(claim.ClaimValue), null);
				}
				catch
				{
				}

			return ret;
		}
	}
}