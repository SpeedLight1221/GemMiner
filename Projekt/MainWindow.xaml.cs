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
using System.Windows.Shell;
using System.Windows.Threading;


namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {

        public bool D = false;
        public bool A = false;
        public bool CD = false;
        public int Span = 10000;
        public double Size = 100d;
        public double level = 0;

        DispatcherTimer Movetimer = new DispatcherTimer();

        double maxHeight = 0.0;
        double maxRight = 0.0;

        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            Movetimer.Interval = new TimeSpan(Span);

            this.KeyDown += (s,e) =>
            {
                if (CD == false)
                {
                    switch (e.Key)
                    {
                        case Key.Space:
                            Jump();
                            break;
                        case Key.D:
                            D = true;
                            A = false;
                            break;
                        case Key.A:
                            A = true;
                            D = false;
                            break;
                    }
                }
            };






            

        }


        public void Jump()
        {
            maxHeight = Canvas.GetBottom(player)+101d ;
            maxRight = Canvas.GetLeft(player)+10d;
           
            Movetimer.Tick += (s, e) =>
            {
                double Y = 0;
                double X = 0;
                if (D == true)//if jumping and moving right
                {
                    CoolDown();


                    Y = Canvas.GetBottom(player);
                    X = Canvas.GetLeft(player);


                    Canvas.SetBottom(player, Y + 0.5d);
                    Y = Canvas.GetBottom(player);
                    Canvas.SetLeft(player, X + 0.2d);
                    X = Canvas.GetLeft(player);
                }
                else if(A ==true)//if jumping and moving left
                {
                    

                }
                else// if only  jumping
                {
                    
                    CoolDown();
                    
                    
                    Y = Canvas.GetBottom(player);
                    

                    Canvas.SetBottom(player, Y + 0.5d);
                     Y = Canvas.GetBottom(player);
                    
                }
                if (Y + 0.5d >= maxHeight)
                {
                    
                    Movetimer.Stop();
                    D = false;
                    A = false;
                    //Gravity
                }
            };
            Movetimer.Start();
        }


        public void CoolDown()
        {
           
            CD = true;
            
            DispatcherTimer CDTimer = new DispatcherTimer();
            CDTimer.Interval = new TimeSpan(0,0,1);
            CDTimer.Tick += (s, e) =>
            {
                CD = false;
                
                
                CDTimer.Stop();

            };
            CDTimer.Start();
        }
    }
}
