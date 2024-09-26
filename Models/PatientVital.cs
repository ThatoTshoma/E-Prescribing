using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class PatientVital
    {
        [Key]
        public int PatientVitalId { get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set;}
        public Vital Vital { get; set; }
        [Display(Name = "Vital Name")]
        public int VitalId { get; set;}
        [NotMapped]
        public List<int> SelectedVital { get; set; }

   
        [NotMapped]
        public Dictionary<string, double> VitalValues { get; set; }

    }
}
