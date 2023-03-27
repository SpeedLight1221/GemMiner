using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        public int Span = 200;
        public double Size = 100d;
        public double level = 0;
        public bool moving =false;

        DispatcherTimer Movetimer = new DispatcherTimer();
        //create storyboard
        Storyboard move = new Storyboard();
        double maxHeight = 0.0;
        double maxRight = 0.0;

        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();


           

















            this.KeyDown += (s, e) =>
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
                
            };









        }

        


        public void Jump()
        {
            if (CD || moving) { return; }
            moving = true;
            move.Completed -= JumpCompleted;
            move.Completed += JumpCompleted;
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d,true);
            
        }

        public void JumpCompleted(object sender,EventArgs e)
        {
            move.Completed -= JumpCompleted;
           
            Gravity();
        }


        public void Gravity()
        {
            moving = false;

            if (CD|| moving) { return; }
            moving = true;
            move.Completed -= GravityCompleted;
            move.Completed += GravityCompleted;
            foreach (var control in MyCan.Children.OfType<Rectangle>()) // checks if we arent on a higher ground
            {
                if (control.Tag as string== "CollideY")
                {
                    if ((Canvas.GetLeft(control) < Canvas.GetLeft(player) + Size) && (Canvas.GetLeft(control) + Size > Canvas.GetLeft(player)))
                       {
                        level = Canvas.GetBottom(control) + 100;
                        
                    }
                }
            }

            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), level, false);
            

        }

        public void GravityCompleted(object sender, EventArgs e)
        {
            moving = false;
            move.Completed -= GravityCompleted;
            CD = false;
            

        }
        public void Animation(double x1, double x2, double y1, double y2,bool Grav)
        {
            


            //definy movement X (left-right)
            DoubleAnimation AnimX = new DoubleAnimation();
            AnimX.From = x1;
            AnimX.To = x2;
            AnimX.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            //define movement Y (Down-Up)
            DoubleAnimation AnimY = new DoubleAnimation();
            AnimY.From = y1;
            AnimY.To = y2;
            AnimY.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            //set target x
            Storyboard.SetTarget(AnimX, player);
            Storyboard.SetTargetProperty(AnimX, new PropertyPath("(Canvas.Left)"));
            //set target y
            Storyboard.SetTarget(AnimY, player);
            Storyboard.SetTargetProperty(AnimY, new PropertyPath("(Canvas.Bottom)"));

            //adding children
            move.Children.Add(AnimY);
            move.Children.Add(AnimX);
            

          
               

            
            move.Completed += (s, e) => { CD = false; moving = false; };
            move.Begin();
            
            
            
            
            
        }

        
    }
}
