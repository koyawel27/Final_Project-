using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll
{
    public static class UserSession
    {
        // Properties to store user session details
        public static int UserID { get; set; }          // User's unique ID
        public static string FullName { get; set; }     // User's full name
        public static string UserType { get; set; }     // User type (Admin or Cashier)

        // Clear the session details (optional for logout)
        public static void ClearSession()
        {
            UserID = 0;
            FullName = string.Empty;
            UserType = string.Empty;
        }
    }
}

