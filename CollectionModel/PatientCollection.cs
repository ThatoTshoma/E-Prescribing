using E_Prescribing.Models;
using static E_Prescribing.Areas.Identity.Pages.Account.RegisterModel;

namespace E_Prescribing.CollectionModel
{
    public class PatientCollection
    {
        public Patient Patient { get; set; }
        public InputModel ApplicationUser { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<Bed> Beds { get; set; }
        public IEnumerable<Suburb> Suburbs { get; set; }

    }
}
