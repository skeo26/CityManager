using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class District : BaseEntity
    {
        public string nameDistrict { get; set; }
        public int id_city { get; set; }

        public District(int id, string name, int idCity)
        {
            Id = id;
            nameDistrict = name;
            id_city = idCity;
        }

        public District()
        {

        }
    }
}
