using E_Prescribing.Models;

namespace E_Prescribing.CollectionModel
{
    public class PatientBedCollection
    {
        public PatientBed PatientBed { get; set; }
        public IEnumerable<Bed> Beds { get; set; }
        public IEnumerable<Patient> Patients { get; set; }


    }
}
