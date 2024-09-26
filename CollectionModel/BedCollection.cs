using E_Prescribing.Models;
using System.Diagnostics.Contracts;

namespace E_Prescribing.CollectionModel
{
    public class BedCollection
    {
        public Bed Bed { get; set; }
        public IEnumerable<Patient> Patients { get; set; }

    }
}
