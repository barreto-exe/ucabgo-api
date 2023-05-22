using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Vehicle.Dtos
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Plate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public UserDto Owner { get; set; }
    }
}
