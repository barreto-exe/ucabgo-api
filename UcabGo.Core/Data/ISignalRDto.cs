namespace UcabGo.Core.Data
{
    public interface ISignalRDto
    {
        public IEnumerable<string> UsersToMessage { get; set; }
    }
}
