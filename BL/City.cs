using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class City : BaseEntity
    {
        public string nameCity { get; set; }
        public int population { get; set; }

        public City(int id, string name, int population)
        {
            Id = id;
            nameCity = name;
            this.population = population;
        }
        public City()
        {
            
        }
    }
}
