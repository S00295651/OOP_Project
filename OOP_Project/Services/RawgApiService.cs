using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class RawgApiService
    {
        private const string API_KEY = "7dcd080536e44d449ba1da3ba9122131";
        private const string BASE_URL = "https://api.rawg.io/api";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<GameDetails> GetGameDetailsAsync(int gameId)
        {
            try
            {
                var url = $"{BASE_URL}/games/{gameId}?key={API_KEY}";
                var json = await _httpClient.GetStringAsync(url);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var rawDetails = JsonSerializer.Deserialize<RawgGameDetails>(json, options);

                return new GameDetails
                {
                    Id = rawDetails.id,
                    Name = rawDetails.name,
                    Description = rawDetails.description_raw ?? "No description available.",
                    Released = rawDetails.released,
                    Rating = rawDetails.rating,
                    RatingsCount = rawDetails.ratings_count,
                    Metacritic = rawDetails.metacritic,
                    BackgroundImage = rawDetails.background_image,
                    Website = rawDetails.website,
                    Genres = rawDetails.genres != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.genres, g => g.name))
                        : "Unknown",
                    Platforms = rawDetails.platforms != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.platforms, p => p.platform.name))
                        : "Unknown",
                    Developers = rawDetails.developers != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.developers, d => d.name))
                        : "Unknown",
                    Publishers = rawDetails.publishers != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.publishers, p => p.name))
                        : "Unknown",
                    Playtime = rawDetails.playtime,
                    AchievementsCount = rawDetails.achievements_count
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch game details: {ex.Message}", ex);
            }
        }

        public async Task<GameDetails> GetGameDetailsByNameAsync(string name)
        {
            try
            {
                var searchUrl = $"{BASE_URL}/games?key={API_KEY}&search={Uri.EscapeDataString(name)}&page_size=1";
                var searchJson = await _httpClient.GetStringAsync(searchUrl);

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var searchResult = JsonSerializer.Deserialize<RawgSearchResult>(searchJson, options);

                if (searchResult?.Results == null || searchResult.Results.Length == 0)
                    throw new Exception("Game not found on RAWG.");

                int rawgId = searchResult.Results[0].Id;

                var detailUrl = $"{BASE_URL}/games/{rawgId}?key={API_KEY}";
                var detailJson = await _httpClient.GetStringAsync(detailUrl);
                var rawDetails = JsonSerializer.Deserialize<RawgGameDetails>(detailJson, options);

                return MapToGameDetails(rawDetails);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch game details: {ex.Message}", ex);
            }
        }

        private static GameDetails MapToGameDetails(RawgGameDetails rawDetails)
        {
            return new GameDetails
            {
                Id = rawDetails.id,
                Name = rawDetails.name,
                Description = rawDetails.description_raw ?? "No description available.",
                Released = rawDetails.released,
                Rating = rawDetails.rating,
                RatingsCount = rawDetails.ratings_count,
                Metacritic = rawDetails.metacritic ?? 0,
                BackgroundImage = rawDetails.background_image,
                Website = rawDetails.website,
                Genres = rawDetails.genres != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.genres, g => g.name))
                        : "Unknown",
                Platforms = rawDetails.platforms != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.platforms, p => p.platform.name))
                        : "Unknown",
                Developers = rawDetails.developers != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.developers, d => d.name))
                        : "Unknown",
                Publishers = rawDetails.publishers != null
                        ? string.Join(", ", System.Linq.Enumerable.Select(rawDetails.publishers, p => p.name))
                        : "Unknown",
                Playtime = rawDetails.playtime,
                AchievementsCount = rawDetails.achievements_count
            };
        }
    }

    

    // Models for API response
    public class RawgGameDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description_raw { get; set; }
        public string released { get; set; }
        public double rating { get; set; }
        public int ratings_count { get; set; }
        public int? metacritic { get; set; }
        public string background_image { get; set; }
        public string website { get; set; }
        public RawgGenre[] genres { get; set; }
        public RawgPlatformWrapper[] platforms { get; set; }
        public RawgDeveloper[] developers { get; set; }
        public RawgPublisher[] publishers { get; set; }
        public int playtime { get; set; }
        public int achievements_count { get; set; }
    }

    public class RawgGenre
    {
        public string name { get; set; }
    }

    public class RawgPlatformWrapper
    {
        public RawgPlatformInfo platform { get; set; }
    }

    public class RawgPlatformInfo
    {
        public string name { get; set; }
    }

    public class RawgDeveloper
    {
        public string name { get; set; }
    }

    public class RawgPublisher
    {
        public string name { get; set; }
    }

    public class RawgSearchResult
    {
        public RawgSearchEntry[] Results { get; set; }
    }

    public class RawgSearchEntry
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Simplified model for display
    public class GameDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Released { get; set; }
        public double Rating { get; set; }
        public int RatingsCount { get; set; }
        public int? Metacritic { get; set; }
        public string BackgroundImage { get; set; }
        public string Website { get; set; }
        public string Genres { get; set; }
        public string Platforms { get; set; }
        public string Developers { get; set; }
        public string Publishers { get; set; }
        public int Playtime { get; set; }
        public int AchievementsCount { get; set; }
    }
}