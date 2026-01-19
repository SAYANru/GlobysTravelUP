using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusTravelManager.Models
{
    public class Tour
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int DurationDays { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Price { get; set; }
        public int BusTypeId { get; set; }
        public string BusTypeName { get; set; }
        public int Capacity { get; set; }
        public int AvailableSeats { get; set; }
        public string PhotoFileName { get; set; }

        // Свойства для подсветки
        public bool IsSpecialOffer => Price < 85000 * 0.85m; // Скидка >15%
        public bool IsFewSeats => (double)AvailableSeats / Capacity < 0.1; // <10%
        public bool IsStartingSoon => (StartDate - DateTime.Now).Days < 7;
    }
}
