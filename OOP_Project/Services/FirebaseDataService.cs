using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OOP_Project
{
    public class FirebaseDataService
    {
        private static readonly HttpClient _http = new HttpClient();


        private static string Url(string path)
            => $"{FirebaseConfig.DatabaseUrl}/users/{UserSession.UserId}/{path}.json?auth={UserSession.IdToken}";


        public async Task SaveProfileAsync(UserProfile profile)
        {
            var json = JsonSerializer.Serialize(profile);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync(Url("profile"), content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<UserProfile> LoadProfileAsync()
        {
            var resp = await _http.GetAsync(Url("profile"));
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();

            // Firebase returns literal "null" when no data exists yet
            if (json == "null" || string.IsNullOrWhiteSpace(json))
                return new UserProfile();

            return JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile();
        }

        public async Task SaveLibraryAsync(IEnumerable<Game> games)
        {
            var json = JsonSerializer.Serialize(games);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync(Url("library"), content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<List<Game>> LoadLibraryAsync()
        {
            var resp = await _http.GetAsync(Url("library"));
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();

            if (json == "null" || string.IsNullOrWhiteSpace(json))
                return new List<Game>();

            return JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();
        }

        public async Task SaveRatingsAsync(Dictionary<int, int> ratings)
        {
            var json = JsonSerializer.Serialize(ratings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync(Url("ratings"), content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task<Dictionary<int, int>> LoadRatingsAsync()
        {
            var resp = await _http.GetAsync(Url("ratings"));
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync();

            if (json == "null" || string.IsNullOrWhiteSpace(json))
                return new Dictionary<int, int>();

            return JsonSerializer.Deserialize<Dictionary<int, int>>(json)
                ?? new Dictionary<int, int>();
        }
    }
}
