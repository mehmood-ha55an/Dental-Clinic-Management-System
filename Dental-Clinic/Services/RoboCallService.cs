namespace Dental_Clinic.Services
{
    using Microsoft.AspNetCore.WebUtilities;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class RoboCallService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl = "https://portal.robocall.pk/api/calls";

        public RoboCallService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["RoboCall:ApiKey"];
        }

        public async Task<string> SendAppointmentCall(
            string phoneNumber,
            int voiceId,
            string clinicName,
            string appointmentDate)
        {
            var queryParams = new Dictionary<string, string>
        {
            { "api_key", _apiKey },
            { "caller_id", phoneNumber },   // ✅ PATIENT PHONE
            { "voice_id", voiceId.ToString() },

            // Optional placeholders (depending on voice template)
            { "text1", clinicName },
            { "text2", appointmentDate },
            { "key1", "0" },
            { "key2", "0" },
            { "key3", "0" },
            { "key4", "0" },
            { "key5", "0" }
        };

            string url = QueryHelpers.AddQueryString(_baseUrl, queryParams);

            var response = await _httpClient.GetStringAsync(url);
            return response;
        }
    }


}
