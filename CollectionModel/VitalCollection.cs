using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class VitalCollection
    {
        public VitalRange Vital { get; set; }
        public IEnumerable<Patient> Patients { get; set; }

        public List<Vital> SelectedVitals { get; set; } 
        public Dictionary<string, string> VitalInputs { get; set; }
        public List<VitalRange> VitalRanges { get; set; }
        public Patient Patient { get; set; }
    }
}
