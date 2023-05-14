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
using System.Windows.Threading;

namespace Projekt
{
    /// <summary>
    /// Interakční logika pro Inventory.xaml
    /// </summary>
    public partial class Inventory : Window
    {
       public Item slot1 = null;
        public Item slot2 = null;
        public Inventory( List<Item> inventory)
        {
            InitializeComponent();
            
            
            InvList.ItemsSource = inventory;


        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if((e.Key == Key.I) || (e.Key == Key.Escape))
            {
                this.Close();
            }
            else if (e.Key == Key.Q)
            {
                if ((InvList.SelectedItem as Item).type == "Tool")
                {

                    slot1 = InvList.SelectedItem as Item;
                    
                }
                else
                {
                    Error();
                }
            }
            else if (e.Key == Key.E)
            {
                if ((InvList.SelectedItem as Item).type == "Block")
                {
                    slot2 = InvList.SelectedItem as Item;
                   
                }
            }
        }

        public void Error()
        {
            DispatcherTimer flash = new DispatcherTimer();
            flash.Interval = new TimeSpan(0, 0, 0, 0, 500);
            InvList.Background = Brushes.Red;

            flash.Tick += (s, e) =>
            {
                InvList.Background = Brushes.Transparent;
                flash.Stop();
            };
            flash.Start();


        }

      
            
        
    }
}
