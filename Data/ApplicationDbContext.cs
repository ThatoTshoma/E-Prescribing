using E_Prescribing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Protocol.Core.Types;

namespace E_Prescribing.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string UserRole { get; set; }

        public DateTime RegisteredDate { get; set; }
    }
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<City>Cities { get; set; }
        public DbSet<Suburb> Suburbs { get; set; }
        public DbSet<ActiveIngredient> ActiveIngredients { get; set; }
        public DbSet<ActiveIngredientStrength> ActiveIngredientStrengths { get; set; }
        public DbSet<PatientAllergy> PatientAllergies { get; set; }
        public DbSet<Anaesthesiologist> Anaesthesiologists { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ConditionDiagnosis> ConditionDiagnoses { get; set; }
        public DbSet<ContraIndication> ContraIndications { get; set; }
        public DbSet<DosageForm> DosageForms { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicationInteraction> MedicationInteractions { get; set; }
        public DbSet<MedicationOrder> MedicationOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<PatientCondition> PatientConditions { get; set; }
        public DbSet<PatientMedication> PatientMedications { get; set; }    
        public DbSet<PatientVital> PatientVitals { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<StockOrder> StockOrders { get; set; }
        public DbSet<Surgeon> Surgeons { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<VitalRange> VitalRanges { get; set; }
        public DbSet<MedicationIngredient> MedicationIngredients { get; set; }
        public DbSet<PatientBed> PatientBeds { get; set; }
        public DbSet<MedicationCart> MedicationCarts { get; set; }
        public DbSet<MedicationPrescription> MedicationPrescriptions { get; set; }
        public DbSet<PrescribedMedication> PrescribedMedications { get; set; }
        public DbSet<PatientTreatment> PatientTreatments { get; set;}
        public DbSet<PharmacistOrder> PharmacistOrders { get;set; }
        public DbSet<Order2> order2s { get; set; }
        public DbSet<AdministeredMedication> AdministeredMedications { get;set; }
        public DbSet<RejectedPrescription> RejectedPrescriptions { get;set; }
        public DbSet<Vital>Vitals { get; set; }
        public DbSet<Province> Provinces { get; set; }


    }
}