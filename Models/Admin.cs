using E_Prescribing.Data;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        public string UserName { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; } 
    }
}
