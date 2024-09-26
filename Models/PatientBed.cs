using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PatientBed
    {
        [Key]
        public int PatientBedId { get; set; }
        public Bed Bed { get; set; }
        [Display(Name = "Bed Number")]
        public int BedId { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set;}
        [NotMapped]
        public List<int> SelectedBeds { get; set; }

    }
}
