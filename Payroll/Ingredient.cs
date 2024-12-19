using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payroll
{
    public class Ingredient
    {
        public int ItemID { get; set; }
        public string IngredientName { get; set; }
        public string UnitOfMeasure { get; set; }
        public decimal QuantityUsed { get; set; }  // Add this property to store quantity used
    }
}
