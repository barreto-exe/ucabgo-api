namespace UcabGo.Core.Data.Passenger.Dtos
{
    public class CooldownDto
    {
        public bool IsInCooldown { get; set; }
        public TimeSpan Cooldown { get; set; }
    }
}
