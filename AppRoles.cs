namespace eStoreJA
{
    /// <summary>
    /// Role names used across the store. Admin and Customer are "system" roles:
    /// they can't be renamed or deleted, and every user must keep at least one of them.
    /// </summary>
    public static class AppRoles
    {
        /// <summary>May open the /Admin area.</summary>
        public const string Admin = "Admin";

        /// <summary>A shopper. Customer pages and statistics are built from users in this role.</summary>
        public const string Customer = "Customer";

        /// <summary>Built-in roles that are protected from rename/delete.</summary>
        public static readonly string[] System = { Admin, Customer };

        /// <summary>True when the name is a built-in role (case-insensitive, like role names in SQL).</summary>
        public static bool IsSystem(string roleName)
        {
            foreach (var r in System)
                if (string.Equals(r, (roleName ?? "").Trim(), global::System.StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
