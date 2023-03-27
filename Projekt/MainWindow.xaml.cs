using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public double Y = 0;
        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            
            this.KeyDown += (s, e) =>
            {
                switch(e.Key)
                {
                    case (Key.Space):
                        if(blockedY==false)
                        {
                            blockedY = true;


                                Y = Canvas.GetBottom(player);

                                player.SetValue(Canvas.BottomProperty, Y);
                                
                            


                        }

                        
                       
                        break;
                }
            };


            this.KeyUp += (s, e) =>
            {
                switch(e.Key)
                {
                    case (Key.Space):
                        blockedY = false;
                        break;
                        
                }
            };



        }
    }
}
