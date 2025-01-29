using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class Street : BaseEntity
    {
        public string nameStreet { get; set; }
        public double sizeStreet { get; set; }
        public int id_district { get; set; }

        public Street(int id, string name, double size, int idDistrict)
        {
            Id = id;
            nameStreet = name;
            sizeStreet = size;
            id_district = idDistrict;
        }

        public Street()
        {
            
        }
    }
}
