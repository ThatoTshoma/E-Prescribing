using E_Prescribing.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Anaesthesiologist
    {
        public int AnaesthesiologistId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [DisplayName("Full Name")]
        public string FullName { get; set; }
        [DisplayName("Contact Number")]
        public string ContactNumber { get; set; }
        [EmailAddress]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        [DisplayName("Registration Number")]
        public string RegistrationNumber { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
