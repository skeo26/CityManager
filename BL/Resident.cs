using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class Resident : BaseEntity
    {
        public string fullName { get; set; }
        public string birthday { get; set; }
        public string passportNumber { get; set; }
        public string passportSeries { get; set; }
        public int id_house { get; set; }

        public Resident(int id, string fullName, string birthday, string passportNumber, string passportSeries, int idHouse)
        {
            Id = id;
            this.fullName = fullName;
            this.birthday = birthday;
            this.passportNumber = passportNumber;
            this.passportSeries = passportSeries;
            id_house = idHouse;
        }

        public Resident()
        {
            
        }
    }
}
