using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class House : BaseEntity
    {
        public int numberHouse { get; set; }
        public int floorHouse { get; set; }
        public int id_street { get; set; }

        public House(int id, int number, int floors, int idStreet)
        {
            Id = id;
            numberHouse = number;
            floorHouse = floors;
            id_street = idStreet;
        }

        public House()
        {
            
        }
    }
}
