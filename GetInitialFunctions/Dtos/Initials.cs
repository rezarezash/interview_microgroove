using System.Text.Json.Serialization;

namespace GetInitialFunctions.Dtos
{
    public record InitialsDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
    }
}