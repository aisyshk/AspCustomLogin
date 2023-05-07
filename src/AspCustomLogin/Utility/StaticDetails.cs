namespace AspCustomLogin.Utility
{
    public static class StaticDetails
    {
        public static string Role_Admin = "Admin";
        public static string Role_Employee = "Employee";
        public static string Role_Customer = "Customer";
        
        /// Note, the way that HasRole works is that if a user were to have multiple roles, they'd have to be separated by a comma.
        /// I.e: "Admin,User"
        /// So a full access role would be something like:
        public static string Full_Access = "Admin,Employee,Customer";
    }
}