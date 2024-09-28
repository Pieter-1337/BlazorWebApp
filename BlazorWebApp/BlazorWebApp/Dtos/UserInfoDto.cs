using System.Text.Json.Serialization;

namespace BlazorWebApp.Dtos
{
    public class UserInfoDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }
        [JsonPropertyName("applicationRoles")]
        public IEnumerable<string> ApplicationRoles { get; set; } = [];
        [JsonPropertyName("impersonatorDisplayName")]
        public string ImpersonatorDisplayName { get; set; }
        public bool IsImpersonationActive => ImpersonatorDisplayName != null;
    }
}
