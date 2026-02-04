using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Released { get; set; }
        public string ImageUrl { get; set; }
        public string PlatformList { get; set; }
        public string GenreList { get; set; }
        public int UserRating { get; set; } // 1–5
        public int PersonalRating { get; set; }
    }
}
