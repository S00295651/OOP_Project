using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OOP_Project.Services
{
    public class SteamApiService
    {
        public const string API_KEY = "FAF04C2D878466190C998FC6C031FDC9";
        private const string OWNED_GAMES_URL = "https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/";
        private const string ACHIEVEMENTS_URL = "https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/";
        private const string SCHEMA_URL = "https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/";
        public static readonly HttpClient _http = new HttpClient();

         public async Task<List<Game>> GetOwnedGamesAsync(string steamId)
        {
            var url = $"{OWNED_GAMES_URL}?key={API_KEY}&steamid={steamId}&include_appinfo=1&format=json";
            var json = await _http.GetStringAsync(url);
 
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = JsonSerializer.Deserialize<SteamResponse>(json, options);
 
            var games = new List<Game>();
            if (root?.Response?.Games == null) return games;
 
            foreach (var sg in root.Response.Games)
            {
                games.Add(new Game
                {
                    Id           = sg.Appid,
                    SteamAppId   = sg.Appid,
                    Name         = sg.Name ?? $"App {sg.Appid}",
                    ImageUrl     = $"https://cdn.akamai.steamstatic.com/steam/apps/{sg.Appid}/header.jpg",
                    PlatformList = "PC",
                    Released     = "",
                    GenreList    = ""
                });
            }
 
            return games;
        }

        public async Task<(int Unlocked, int Total)> GetAchievementsAsync(string steamId, int appId)
        {
            try
            {
                var url = $"{ACHIEVEMENTS_URL}?key={API_KEY}&steamid={steamId}&appid={appId}&format=json";
                var json = await _http.GetStringAsync(url);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonSerializer.Deserialize<AchievementsResponse>(json, options);

                var list = root?.Playerstats?.Achievements;
                if (list == null || list.Count == 0) return (0, 0);

                int unlocked = 0;
                foreach (var a in list)
                    if (a.Achieved == 1) unlocked++;

                return (unlocked, list.Count);
            }
            catch
            {
                // Game has no achievements or profile is private (no error)
                return (0, 0);
            }
        }

        // JSON models
        private class SteamResponse
        {
            public SteamResponseInner Response { get; set; }
        }

        private class SteamResponseInner
        {
            public int Game_Count { get; set; }
            public List<SteamGame> Games { get; set; }
        }

        private class SteamGame
        {
            public int Appid { get; set; }
            public string Name { get; set; }
            public int Playtime_Forever { get; set; }
            public string Img_Icon_Url { get; set; }
        }

        private class AchievementsResponse
        {
            public PlayerStats Playerstats { get; set; }
        }
        private class PlayerStats
        {
            public string SteamID { get; set; }
            public string GameName { get; set; }
            public List<AchievementEntry> Achievements { get; set; }
            public bool Success { get; set; }
        }

        private class AchievementEntry
        {
            public string Apiname { get; set; }
            public int Achieved { get; set; }  // 1 = unlocked, 0 = locked
            public long Unlocktime { get; set; }
        }
    }
}
