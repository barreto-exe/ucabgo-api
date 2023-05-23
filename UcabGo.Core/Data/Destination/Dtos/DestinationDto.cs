using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Destination.Dtos
{
    public class DestinationDto
    {
        public int Id { get; set; }
        public UserDto Driver { get; set; }
        public string Alias { get; set; } = null!;
        public string Zone { get; set; } = null!;
        public string? Detail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
