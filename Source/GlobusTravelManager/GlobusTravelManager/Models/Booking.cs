using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusTravelManager.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public string TourName { get; set; }
        public int UserId { get; set; }
        public string ClientName { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
        public int PeopleCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Comment { get; set; }
    }
}
