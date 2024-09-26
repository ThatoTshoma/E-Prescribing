using E_Prescribing.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_Prescribing.Data
{
    public class Cart
    {
       // [Key]
        public int CartId { get; set; }
        public ApplicationDbContext _db { get; set; }
        public List<MedicationCart> Medications { get; set; }


        public Cart(ApplicationDbContext db)
        {
            _db = db;
        }

        public static Cart GetMedicationCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<ApplicationDbContext>();

            int cartId = session.GetInt32("CartId") ?? GenerateRandomCartId();

            session.SetInt32("CartId", cartId);

            return new Cart(context) { CartId = cartId };
        }

        private static int GenerateRandomCartId()
        {
            Random rand = new Random();
            return rand.Next(1, 1000001);
        }

        public void AddMedication(Medication medication)
        {
            var medicationCart = _db.MedicationCarts.FirstOrDefault(n => n.Medication.MedicationId == medication.MedicationId && n.CartId == CartId);

            if (medicationCart == null)
            {
                medicationCart = new MedicationCart()
                {
                    CartId = CartId,
                    Medication = medication,
                    Quantity = 1
                };

                _db.MedicationCarts.Add(medicationCart);
            }
            else
            {
                medicationCart.Quantity++;
            }
            _db.SaveChanges();

        }

        public void RemoveMedication(Medication medication)
        {
            var medicationCart = _db.MedicationCarts.FirstOrDefault(n => n.Medication.MedicationId == medication.MedicationId && n.CartId == CartId);

            if (medicationCart != null)
            {
                if (medicationCart.Quantity > 1)
                {
                    medicationCart.Quantity--;
                }
                else
                {
                    _db.MedicationCarts.Remove(medicationCart);
                }
            }
            _db.SaveChanges();

        }

        public List<MedicationCart> GetMedicationCart()
        {
            return Medications ?? (Medications = _db.MedicationCarts.Where(n => n.CartId == CartId).Include(n => n.Medication).ToList());
        }
        //public double GetShoppingCartTotal() => (double)_db.MedicationCarts.Where(n => n.CartId == CartId).Select(n => n.Medication.Price * n.Quantity).Sum();

        public async Task ClearCart()
        {
            var items = await _db.MedicationCarts.Where(n => n.CartId == CartId).ToListAsync();
            _db.MedicationCarts.RemoveRange(items);
            await _db.SaveChangesAsync();
        }

    }
}
