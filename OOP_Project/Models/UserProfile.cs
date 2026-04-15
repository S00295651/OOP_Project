using System;

namespace OOP_Project
{
    public class UserProfile
    {
        public string Username { get; set; } = "Player";
        public string FavouriteGenre { get; set; } = "";
        public string Bio { get; set; } = "";
        public string MemberSince { get; set; } = DateTime.Now.ToString("MMMM yyyy");
        public string SteamId { get; set; } = "";
    }
}