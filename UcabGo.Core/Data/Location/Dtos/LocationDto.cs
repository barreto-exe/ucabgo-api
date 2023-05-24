using System;
using System.Collections.Generic;
using System.Linq;
using UcabGo.Core.Data.User.Dto;

namespace UcabGo.Core.Data.Location.Dtos
{
    public class LocationDto
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Zone { get; set; }
        public string? Detail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public UserDto User { get; set; }
    }
}
