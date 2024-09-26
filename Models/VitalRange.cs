using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Prescribing.Models
{
    public class VitalRange
    {
        [Key]
        public int VitalId { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? Temperature { get; set; }
        public string? BloodPressure { get; set; }
        public string? PulseRate { get; set; }
        public string? RespiratoryRate { get; set; }
        public string? BloodGlucoseLevel { get; set; }
        public string? BloodOxegenLevel { get; set; }
        public string? OxygenSaturation { get; set; }
        public string? HeartRate { get; set; }
        public DateTime Time {  get; set; }
        public Patient Patient { get; set; }
        [Display(Name = "Patient Name")]
        public int PatientId { get; set;}
        [NotMapped]
        public IEnumerable<Patient> Patients { get; set; }


    }
}
