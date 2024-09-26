using E_Prescribing.Data;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Surgeon
    {
        public int SurgeonId { get; set; }
        public string Name { get; set;}
        public string Surname { get; set;}
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set;}
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set;}
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set;}
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
