namespace UcabGo.Core.Data.Destination.Inputs
{
    public class DestinationUpdateInput : BaseRequest
    {
        public int Id { get; set; }
        public string Alias { get; set; }
        public string Zone { get; set; }
        public string Detail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public bool IsActive { get; set; }
    }
}
