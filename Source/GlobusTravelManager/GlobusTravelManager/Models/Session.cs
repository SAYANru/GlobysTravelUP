using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobusTravelManager.Models
{
    public static class Session
    {
        public static User CurrentUser { get; set; }
        public static bool IsGuestMode => CurrentUser == null;
    }
}
