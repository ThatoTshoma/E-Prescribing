using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PrescriptionCollection
    {
        public Prescription Prescription { get; set; }
        public MedicationPrescription MedicationPrescription { get; set; }
        public IEnumerable<Medication> Medications { get; set; }
        public IEnumerable<Surgeon> Surgeons { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public Dictionary<int, string> Instructions { get; set; }
        public Dictionary<int, string> Notes { get; set; }
        public Dictionary<int, int> Quantities { get; set; }
        public Dictionary<int, bool> SelectedItems { get; set; } // MedicationId as key, boolean for whether it's selected





    }
}
