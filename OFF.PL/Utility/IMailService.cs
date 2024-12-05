using OFF.DAL.Model;

namespace OFF.PL.Utility
{
    public interface IMailService
    {
        void SendEmail(Email email);
    }
}
