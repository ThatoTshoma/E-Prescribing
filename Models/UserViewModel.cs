using E_Prescribing.Data;

namespace E_Prescribing.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
        public string Name { get; set; }    
        public string Surname { get; set; }
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string RegistrationNumber { get; set; }
    }
}
