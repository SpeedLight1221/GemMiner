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

        public bool D = false;
        public int Span = 9999;
        public double Size = 100d;
        public double level = 0;

        DispatcherTimer timer = new DispatcherTimer();

        double maxHeight = 0.0;
       
        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            timer.Interval = new TimeSpan(Span);




#region down
            this.KeyDown += (s, e) =>
            {
                if (blockedY == false)
                {
                    switch (e.Key)
                    {
                        


                        case (Key.Space):

                            
                            blockedY = true;
                            jump();
                            break;




                        case (Key.D):
                            D = true;
                            GoRight();
                            break;



                        
                    }
                    



                    
                }
            };
#endregion
#region Up
            this.KeyUp += (s, e) =>
            {
                switch(e.Key)
                {
                    case (Key.Space):
                        
                        
                        break;
                    case Key.D:
                        D = false;
                        break;

                    
                        
                }


            };
#endregion






        }


        public void jump()
        {
            maxHeight = Canvas.GetBottom(player) + 150d;
            Canvas.SetBottom(player, maxHeight - 69.5d);
            //MessageBox.Show(maxHeight.ToString());
            timer.Tick += (s, e) =>
            {
                double y = Canvas.GetBottom(player);
                Canvas.SetBottom(player, y + 0.5d);
                if (D == true)
                {
                    foreach (var control in MyCan.Children.OfType<Rectangle>())
                    {
                        if (control.Name != "player")
                        {
                            if ((Canvas.GetLeft(player) + Size > Canvas.GetLeft(control)-1)
                                && (Canvas.GetLeft(player) < Canvas.GetLeft(control) + Size)
                                && (Canvas.GetBottom(player) > Canvas.GetBottom(control)+Size))
                            {
                                level = 100;
                                blockedY = false;
                            }
                            else if ((Canvas.GetLeft(player) + Size > Canvas.GetLeft(control) - 1)
                                && (Canvas.GetLeft(player) < Canvas.GetLeft(control) + Size))
                            {

                            }
                            else
                            {
                                Canvas.SetLeft(player, Canvas.GetLeft(player) + 2d);
                            }
                        }
                    }
                    if (y + 0.5 >= maxHeight)
                    {
                        timer.Stop();
                        Cooldown();
                        Gravity();
                    }
                }
                else
                {
                    if (y + 0.5 >= maxHeight)
                    {
                        timer.Stop();
                        Cooldown();
                        Gravity();
                    }
                }
            };
            timer.Start();
        }
        public void Gravity()
        {
            
            double y = Canvas.GetBottom(player);
           
            DispatcherTimer gravTimer = new DispatcherTimer();
            gravTimer.Interval = new TimeSpan(Span);
            gravTimer.Tick += (s, e) =>
            {
                if (y > level)
                {
                    Canvas.SetBottom(player, y - 0.5d);
                    y = Canvas.GetBottom(player);
                }
                else if (y < level)
                {
                    Canvas.SetBottom(player, 0);
                    gravTimer.Stop();
                }
                else
                {
                    gravTimer.Stop();
                }
            };
            gravTimer.Start();
        }


        public void Cooldown()
        {
            DispatcherTimer cdTimer = new DispatcherTimer();
            player.Fill = Brushes.Red;
            cdTimer.Interval = new TimeSpan(0,0,0,0,60);
            cdTimer.Tick += (s, e) =>
            {
                if (Canvas.GetBottom(player) == 0)
                {
                    blockedY = false;
                    cdTimer.Stop();
                    player.Fill = Brushes.Blue;
                }
            };
            cdTimer.Start();
            
            
        }


        public void GoRight()
        {

            
            foreach (var control in MyCan.Children.OfType<Rectangle>())
            {
                if (control.Name != "player")
                {
                    if ((Canvas.GetLeft(player) + Size > Canvas.GetLeft(control) - 1)
                        && (Canvas.GetLeft(player) < Canvas.GetLeft(control) + Size)
                        && (Canvas.GetBottom(control) + Size > Canvas.GetBottom(player)+1))
                    {

                    }
                    else
                    {
                        Canvas.SetLeft(player, Canvas.GetLeft(player) + 1d);
                    }
                }
            }
        }



    }


    


}
