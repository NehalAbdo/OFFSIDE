namespace OFF.PL.ViewModels
{
    public class UserRoleVM
    {
        public String ID { get; set; }
        public string Email { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string userName { get; set; }
        public string ImageName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsVIP { get; set; }

    }
}
