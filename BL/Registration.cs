using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class Registration : BaseEntity
    {
        public int id_resident { get; set; }
        public string? dateRegistration { get; set; }
        public string? dataDeregistration { get; set; }
        public int id_house { get; set; }

        public Registration(int id, string? dateRegistration, string? dateDeregistration, int idResident, int idHouse)
        {
            Id = id;
            this.dateRegistration = dateRegistration;
            dataDeregistration = dateDeregistration;
            id_resident = idResident;
            id_house = idHouse;
        }

        public Registration()
        {
            
        }
    }
}
