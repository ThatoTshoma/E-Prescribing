using E_Prescribing.Data;
using E_Prescribing.Models;
using static E_Prescribing.Areas.Identity.Pages.Account.RegisterModel;

namespace E_Prescribing.CollectionModel
{
    public class UserCollection
    {
        public InputModel ApplicationUser { get; set; }
        public ApplicationUser ApplicationUsers { get; set; }

        public Nurse Nurse { get; set; }
        public Pharmacist Pharmacist { get; set; }
        public Surgeon Surgeon { get; set; }
        public Anaesthesiologist Anaesthesiologist { get; set; }
        public string ReturnUrl { get; set; }
    }
}
