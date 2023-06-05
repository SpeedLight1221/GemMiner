using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>
    /// Interakční logika pro SeedW.xaml
    /// </summary>
    public partial class SeedW : Window
    {
        public string seeed = "";
        public SeedW()
        {
            InitializeComponent();
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            seeed = SeedBox.Text;
            foreach (char c in seeed) { 
                if(!Char.IsDigit(c)) {
                    seeed = "";
                    break;
                }
            }

            this.Close();
          



        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            seeed = "";
            this.Close();
        }
    }
}
