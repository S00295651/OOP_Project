using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OOP_Project
{
    public class FirebaseAuthService
    {
        private static readonly HttpClient _http = new HttpClient();

        private const string SIGN_UP_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=";
        private const string SIGN_IN_URL = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";

        public async Task RegisterAsync(string email, string password)
        {
            var response = await PostAuthRequestAsync(SIGN_UP_URL + FirebaseConfig.ApiKey, email, password);
            ApplySession(response, email);
        }

        public async Task LoginAsync(string email, string password)
        {
            var response = await PostAuthRequestAsync(SIGN_IN_URL + FirebaseConfig.ApiKey, email, password);
            ApplySession(response, email);
        }


        private async Task<AuthResponse> PostAuthRequestAsync(string url, string email, string password)
        {
            var payload = JsonSerializer.Serialize(new
            {
                email = email,
                password = password,
                returnSecureToken = true
            });

            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var httpResp = await _http.PostAsync(url, content);
            var json = await httpResp.Content.ReadAsStringAsync();

            if (!httpResp.IsSuccessStatusCode)
            {
                // Parse Firebase error message and throw a friendly exception
                var error = ParseFirebaseError(json);
                throw new Exception(error);
            }

            return JsonSerializer.Deserialize<AuthResponse>(json)
                ?? throw new Exception("Empty response from Firebase.");
        }

        private static void ApplySession(AuthResponse response, string email)
        {
            UserSession.IdToken = response.idToken;
            UserSession.UserId = response.localId;
            UserSession.Email = email;
        }

        private static string ParseFirebaseError(string json)
        {
            try
            {
                // 2. Using JObject (Newtonsoft.Json)
                JObject doc = JObject.Parse(json);
                string message = doc["error"]?["message"]?.ToString() ?? "Unknown error";

                switch (message)
                {
                    case "EMAIL_EXISTS":
                        return "This email is already registered.";
                    case "INVALID_EMAIL":
                        return "The email address is invalid.";
                    case "WEAK_PASSWORD : Password should be at least 6 characters":
                        return "Password must be at least 6 characters.";
                    case "EMAIL_NOT_FOUND":
                        return "No account found with that email.";
                    case "INVALID_PASSWORD":
                        return "Incorrect password.";
                    case "USER_DISABLED":
                        return "This account has been disabled.";
                    case "INVALID_LOGIN_CREDENTIALS":
                        return "Incorrect email or password.";
                    default:
                        return message;
                }
            }
            catch
            {
                return "An unexpected error occurred.";
            }
        }


        private class AuthResponse
        {
            public string idToken { get; set; } = string.Empty;
            public string localId { get; set; } = string.Empty;
            public string email { get; set; } = string.Empty;
        }
    }
}
