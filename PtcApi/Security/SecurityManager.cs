using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PtcApi.Model;

namespace PtcApi.Security
{
	public class SecurityManager
	{
		private readonly JwtSettings _settings;

		public SecurityManager(JwtSettings settings)
		{
			_settings = settings;
		}

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

		protected string BuildJwtToken(AppUserAuth authUser)
		{
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
			List<Claim> jwtClaims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, authUser.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim("isAuthenticated", authUser.IsAuthenticated.ToString().ToLower()),
				new Claim("CanAccessProducts", authUser.CanAccessProducts.ToString().ToLower()),
				new Claim("CanAddProduct", authUser.CanAddProduct.ToString().ToLower()),
				new Claim("CanSaveProduct", authUser.CanSaveProduct.ToString().ToLower()),
				new Claim("CanAccessCategories", authUser.CanAccessCategories.ToString().ToLower()),
				new Claim("CanAddCategory", authUser.CanAddCategory.ToString().ToLower())
			};
			// Create the JwtSecurityToken object
			JwtSecurityToken token = new JwtSecurityToken(
				_settings.Issuer,
				_settings.Audience,
				jwtClaims,
				DateTime.UtcNow,
				DateTime.UtcNow.AddMinutes(_settings.MinutesToExpiration),
				new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
			);

			// Create a string representation of the Jwt token
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private AppUserAuth BuildUserAuthObject(AppUser authUser)
		{
			AppUserAuth ret = new AppUserAuth
			{
				// Set User Properties
				UserName = authUser.UserName,
				IsAuthenticated = true,
				BearerToken = new Guid().ToString()
			};

			// Get all claims for the user
			List<AppUserClaim> claims = GetUserClaims(authUser);

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

			ret.BearerToken = BuildJwtToken(ret);
			return ret;
		}
	}
}