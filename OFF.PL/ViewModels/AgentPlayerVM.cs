using OFF.DAL.Model;

namespace OFF.PL.ViewModels
{
    public class AgentPlayerVM
    {
        public Agent Agent { get; set; }
        public PLayer PLayer { get; set; }
        public string? Id { get; set; }
        public Contact Contact { get; set; }
    }
}
