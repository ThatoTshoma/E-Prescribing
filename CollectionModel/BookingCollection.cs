using E_Prescribing.Models;
using System.Numerics;

namespace E_Prescribing.CollectionModel
{
    public class BookingCollection
    {
        public Booking Booking { get; set; }
        public PatientTreatment PatientTreatment { get; set; }
        public IEnumerable<Treatment> Treatments { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Anaesthesiologist> Anaesthesiologists { get; set; }
        public IEnumerable<Theatre> Theatres { get; set; }
        public IEnumerable<Ward> Wards { get; set; }


    }
}
