using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public static class UserSession
    {
        public static string IdToken { get; set; } = string.Empty;
        public static string UserId { get; set; } = string.Empty;
        public static string Email { get; set; } = string.Empty;

        public static bool IsLoggedIn => !string.IsNullOrEmpty(IdToken);

        public static void Clear()
        {
            IdToken = string.Empty;
            UserId = string.Empty;
            Email = string.Empty;
        }
    }
}
