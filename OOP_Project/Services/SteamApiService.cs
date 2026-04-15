using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OOP_Project.Services
{
    public class SteamApiService
    {
        private const string API_KEY = "FAF04C2D878466190C998FC6C031FDC9";
        private const string OWNED_GAMES_URL = "https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/";
        private static readonly HttpClient _http = new HttpClient();

        public async Task<List<Game>> GetOwnedGamesAsync(string steamId)
        {
            var url = $"{OWNED_GAMES_URL}?key={API_KEY}&steamid={steamId}&include_appinfo=1&format=json";

            var json = await _http.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var root = JsonSerializer.Deserialize<SteamResponse>(json, options);

            var games = new List<Game>();

            if (root?.Response?.Games == null)
                return games;

            foreach (var sg in root.Response.Games)
            {
                games.Add(new Game
                {
                    Id = sg.Appid,
                    Name = sg.Name ?? $"App {sg.Appid}",
                    ImageUrl = $"https://cdn.akamai.steamstatic.com/steam/apps/{sg.Appid}/header.jpg",
                    PlatformList = "PC",
                    Released = "",
                    GenreList = ""
                });
            }

            return games;
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
    }
}
