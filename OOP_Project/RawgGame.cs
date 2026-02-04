using System.Collections.Generic;

namespace OOP_Project
{
    public class RawgRoot
    {
        public List<RawgGame> results { get; set; }
    }

    public class RawgGame
    {
        public int id { get; set; }
        public string name { get; set; }
        public string released { get; set; }
        public string background_image { get; set; }
    }
}