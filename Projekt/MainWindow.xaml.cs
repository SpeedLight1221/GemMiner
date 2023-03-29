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


    // Tag Logic : [0] Type of object (G-Ground) [1] - Y top collision (Y-true, _-False) 


    public partial class MainWindow : Window
    {
        int ttt = 0;
       
        public bool D = false;
        public bool A = false;
        public bool CD = false;
        bool tess = false;
        public int Span = 200;
        public double Size = 100d;
        public double level = 100;
        public bool jumping =false;

        DispatcherTimer coolDown= new DispatcherTimer();
        //create storyboard
        Storyboard move = new Storyboard();
        double maxHeight = 0.0;
        double maxRight = 0.0;

        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            Generate();
            level = Canvas.GetBottom(player);
            test.Content = level;
            ts = Canvas.GetBottom(player);
            JumpGravity();
            test_Copy.Content = ts;

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
                            Right();
                            break;
                        case Key.A:
                            A = true;
                            D = false;
                            break;
                    }
                
            };
            
            test_Copy1.Content = ttt;








        }

        


        public void Jump()
        {
            if (CD || jumping) { return; }
            jumping = true;
            move.Completed -= JumpCompleted;
            move.Completed += JumpCompleted;
            level = Canvas.GetBottom(player);
            
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d,true);
            
        }

        public void JumpCompleted(object sender,EventArgs e)
        {
            move.Completed -= JumpCompleted;
           
            
            JumpGravity();
        }


        public void JumpGravity()
        {


            
            move.Completed -= JumpGravityCompleted;
            move.Completed += JumpGravityCompleted;
            //foreach (var control in MyCan.Children.OfType<Rectangle>()) // checks if we arent on a higher ground
            //{

            //    if ((control.Tag as string)[1] == 'Y')
            //    {
            //        if ((Canvas.GetLeft(control) == Canvas.GetLeft(player))) //&& (Canvas.GetLeft(control) + Size > Canvas.GetLeft(player)))
            //        {

            //            Canvas.SetBottom(player, level);

            //        }
            //    }
            //}
            if (tess) { return; }
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), level, false);
            

        }

        public void JumpGravityCompleted(object sender, EventArgs e)
        {
            
            move.Completed -= JumpGravityCompleted;
            cooldown();
            test.Content = level;
            //FallGravity();


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
            

          
               

            
            
            move.Begin();
            
            
            
            
            
        }
        
        public void Right()//movement right
        {
            ttt++;
            test_Copy1.Content = ttt;
            if (Canvas.GetBottom(player)>level)
            {
                
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>()) //checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) + Size) && (level+Size == Canvas.GetBottom(collCheck)))//checks if there is a block on the "desired position"
                    {
                        
                        return;


                    }
                }

                
               

                foreach (var control in MyCan.Children.OfType<Rectangle>()) //if there is a block 1 higher and to the right, moves to it
                {

                    if ((control.Tag as string)[0] == 'G')
                    {


                        if ((Canvas.GetLeft(control) == Canvas.GetLeft(player) + Size) && (level  == Canvas.GetBottom(control)))//checks if there is a block on the "desired position"
                        {
                            tess = true;
                            
                            JumpGravityCompleted(new object(), new EventArgs());
                            move.Stop();
                            Canvas.SetBottom(player, level+Size);
                            Canvas.SetLeft(player, Canvas.GetLeft(control));
                            level += 100;
                            test_Copy.Content = "" + Canvas.GetBottom(player)+" " + level;
                            tess = false;
                            return;
                           
                           
                            
                        }

                    }
                }
                
            }
            

            //FallGravity();


        }

       double ts = 0;
        public void FallGravity()
        {
            

            
            
            //if ((Canvas.GetBottom(player) % 100 != 0) || (Canvas.GetBottom(player) != 0))
            //{
            //    for (int i = 0; i < 20; i++)
            //    {
            //        if (((i + 1) * 100) > (Canvas.GetBottom(player)) && ((Canvas.GetBottom(player) > i * 100)))
            //        {
            //            Canvas.SetBottom(player, i * 100);
            //            break;
            //        }
            //    }
            //}


            //foreach(var collCheck in MyCan.Children.OfType<Rectangle>())
            //{
            //    if((collCheck.Tag as string)[0] == 'G' )
            //    {
            //        if((Canvas.GetBottom(collCheck)+Size == Canvas.GetBottom(player))&&(Canvas.GetLeft(collCheck) == (Canvas.GetLeft(player))))
            //        {

            //            return;
            //        }
            //    }
            //}

            //level -= 100;

            //ts = Canvas.GetBottom(player);
            //test_Copy.Content = ts;




        }        

        public void cooldown()
        {
            jumping = false;

            CD = true;
            test.Background = Brushes.Red;
            coolDown.Interval = new TimeSpan(0, 0, 0, 0, 500);
            coolDown.Start();
            coolDown.Tick += (sender, e) =>
            {
                CD = false;
                test.Background = Brushes.Blue;
                coolDown.Stop();
            };
        }









        public void Generate()
        {
            MessageBox.Show("x");
            for(int i=0; i<100; i++)
            {


                //Rectangle bedrock = new Rectangle();
                //bedrock.Fill = Brushes.DarkGray;
                //bedrock.Width = 100;
                //bedrock.Height = 100;
                //bedrock.Tag = "GY";
                //Canvas.SetBottom(bedrock, -100);
                //Canvas.SetLeft(bedrock, i * 100);
                //MyCan.Children.Add(bedrock);
                
                if (i>5) 
                {
                    Rectangle block = new Rectangle();
                    block.Fill = Brushes.Brown;
                    block.Width = 100;
                    block.Height = 100;
                    block.Tag = "GY";
                    Canvas.SetBottom(block, 0);
                    Canvas.SetLeft(block, i * 100);
                    MyCan.Children.Add(block);
                    if (i % 10 == 0) { block.Fill = Brushes.Green; }
                }



                    
            }
        }

        
    }
}
