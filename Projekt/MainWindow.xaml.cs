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
using System.Windows.Threading;


namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();

        double maxHeight = 0.0;
        public double Y = 0;
        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(70);
            ;

            this.KeyDown += (s, e) =>
            {
                switch(e.Key)
                {
                    case (Key.Space):

                        maxHeight = Canvas.GetBottom(player)+70d;
                        //MessageBox.Show(maxHeight.ToString());
                   


                        break;
                }
                timer.Tick += (s, e) =>
                {
                    double y = Canvas.GetBottom(player);
                    Canvas.SetBottom(player, y + 1.0d);
                    if (y + 1.0 >= maxHeight)
                    {
                        timer.Stop();
                    }
                };



                timer.Start();
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
