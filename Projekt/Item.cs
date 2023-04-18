using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Projekt
{
    public class Item
    {
        public int Amount { get; set; }
        public string Name { get; set; }
        public Uri Image { get; set; }

        public string type { get; set; }



        public Item(int a,string n, Uri i,string t)
        {
            Amount = a;
            Name = n;
            Image = i;
            type = t;
        }

        public override string ToString()
        {
            return Name;        }


    }
}
