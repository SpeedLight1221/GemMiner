using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projekt
{
    public class Recipe
    {
     
        
        public string Name { get;set; }
        public Item Output { get; set; }
        public int OAmount { get; set; }

        public Uri Image { get; set; }
        
        public string Res1 { get; set; }
        public int Amount1 { get; set; }

        public string? Res2 { get; set; }
        public int? Amount2 { get; set; }

        public string? Res3 { get; set; }
        public int? Amount3 { get; set;}



        public Recipe(string n, Item o,int AO, string r1, int a1, string r2, int? a2, string r3, int? a3, Uri im)
        {
            Name = n;
            Output = o;
            OAmount = AO;
            Image = im;
            Res1 = r1;
            Res2 = r2;
            Amount1 = a1;
            Amount2 = a2;
            Res3 = r3;
            Amount3 = a3;

        }



    }
}
