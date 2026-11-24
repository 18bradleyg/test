using System;
using System.Security.Claims;
using System.Security.Principal;

namespace eStoreJA
{
    /// <summary>
    /// First-name greeting helpers. The first name travels in the sign-in cookie as a claim
    /// (added in ApplicationUser.GenerateUserIdentity), so the header can say "Hi, John"
    /// without a database query on every page.
    /// </summary>
    public static class IdentityNameExtensions
    {
        /// <summary>Claim type that holds the user's display first name.</summary>
        public const string FirstNameClaim = "eStoreJA:FirstName";

        /// <summary>
        /// The signed-in user's first name from the cookie claim. Older cookies (issued before names
        /// existed) don't have it, so it falls back to the email rule: "john@estoreja.com" → "John".
        /// </summary>
        public static string GetFirstName(this IIdentity identity)
        {
            var claims = identity as ClaimsIdentity;
            var claim = claims != null ? claims.FindFirst(FirstNameClaim) : null;
            if (claim != null && !string.IsNullOrWhiteSpace(claim.Value)) return claim.Value;
            return NameFromEmail(identity != null ? identity.Name : null);   // Name = UserName = the email in this app
        }

        /// <summary>The part of an email before "@", first letter capitalised ("mary@x.com" → "Mary"); "" if none.</summary>
        public static string NameFromEmail(string email)
        {
            string local = (email ?? "").Trim();
            int at = local.IndexOf('@');
            if (at >= 0) local = local.Substring(0, at);
            if (local.Length == 0) return "";
            return char.ToUpperInvariant(local[0]) + local.Substring(1);
        }
    }
}
