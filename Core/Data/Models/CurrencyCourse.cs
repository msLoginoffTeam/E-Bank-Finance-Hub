using Core.Data.Models;

namespace Core_Api.Data.Models
{
    public class CurrencyCourse
    {
        public Currency Currency { get; set; }

        public int Course { get; set; }



        public CurrencyCourse(Currency Currency, int Course)
        {
            this.Currency = Currency;
            this.Course = Course;
        }
    }
}
