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


    //TODO: Movement cooldown, Movement left.


    public partial class MainWindow : Window
    {
        
       
        
        public bool CD = false;
        bool tess = false;
        public double Size = 100d;
        public double level = 100;
        public bool jumping =false;
        DispatcherTimer coolDown= new DispatcherTimer();// 
        
        Storyboard move = new Storyboard();//create storyboard for animating


        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            Generate();
            level = Canvas.GetBottom(player);
            
            ts = Canvas.GetBottom(player);
            JumpGravity();
            Right();

            #region keys
            this.KeyDown += (s, e) =>
            {
                test.Content = Canvas.GetLeft(player);
                switch (e.Key)
                    {
                        case Key.Space:
                            Jump();
                            
                            break;
                        case Key.D:
                            
                            Right();
                            break;
                        case Key.A:
                            Left();
                            break;
                    }
                
            };
            

        }

        #endregion

        #region jump
        public void Jump()
        {
            if (CD || jumping) { return;}
            jumping = true;
            move.Completed -= JumpCompleted;
            move.Completed += JumpCompleted;
            level = Canvas.GetBottom(player);
           
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d,false);
            
        }

        public void JumpCompleted(object sender,EventArgs e) //in order to avoid a memory leak
        {
            move.Completed -= JumpCompleted;
            move.FillBehavior = FillBehavior.HoldEnd;

            JumpGravity();
        }


        public void JumpGravity()
        {
            move.Completed -= JumpGravityCompleted;
            move.Completed += JumpGravityCompleted;
           
            if (tess) { return; }
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), level, false);
        }

        public void JumpGravityCompleted(object sender, EventArgs e)
        {
            move.Completed -= JumpGravityCompleted;
            cooldown(new object() ,new EventArgs());
            
        }
        #endregion
        public void Animation(double x1, double x2, double y1, double y2,bool Grav)
        {

            move.FillBehavior = FillBehavior.HoldEnd;

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
            

            move.Completed += (s, e) =>
            {
                // if(Canvas.GetLeft(player) != x2) { Canvas.SetLeft(player, x2); }
                //if (Canvas.GetBottom(player) != y2) { Canvas.SetLeft(player, y2); }
               
                Canvas.SetLeft(player, x2);
                Canvas.SetBottom(player, y2);

            };

            
            if (Grav == true) { 
                move.Completed += FallGravity;
                test.Background = Brushes.PaleGreen;
            }

            
            move.Begin();
 
        }
        
        public void Right()//movement right
        {
         
            
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
                           // Canvas.SetBottom(player, level+Size);
                            test.Content += "x";
                            //Canvas.SetLeft(player, Canvas.GetLeft(control));
                            
                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(control), Canvas.GetBottom(player), level + Size, true);
                            level += 100;
                            tess = false;
                            return;
                        }

                    }
                }
                
            }
            else if(Canvas.GetBottom(player) < level) 
            { Canvas.SetBottom(player, level); 
            }
            else
            {
                if (CD) { return; }
                cooldown(new object(), new EventArgs());
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>())//checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((collCheck.Tag as string)[0] == 'G')
                    {
                        if((Canvas.GetLeft(collCheck)==Canvas.GetLeft(player)+Size)&&(Canvas.GetBottom(player)==Canvas.GetBottom(collCheck)))
                        {
                            return;
                        }
                    }
                }
                    Animation(Canvas.GetLeft(player), Canvas.GetLeft(player)+100, Canvas.GetBottom(player), Canvas.GetBottom(player), true);
                    
                    
            }
            
        }


        public void Left()//movement Left
        {


            if (Canvas.GetBottom(player) > level)
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>()) //checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) - Size) && (level + Size == Canvas.GetBottom(collCheck)))//checks if there is a block on the "desired position"
                    {
                        return;
                    }
                }
                foreach (var control in MyCan.Children.OfType<Rectangle>()) //if there is a block 1 higher and to the right, moves to it
                {
                    if ((control.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetLeft(control) == Canvas.GetLeft(player)-Size) && (level == Canvas.GetBottom(control)))//checks if there is a block on the "desired position"
                        {
                            tess = true;

                            JumpGravityCompleted(new object(), new EventArgs());
                            move.Stop();
                            // Canvas.SetBottom(player, level+Size);
                            test.Content += "x";
                            //Canvas.SetLeft(player, Canvas.GetLeft(control));

                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(control), Canvas.GetBottom(player), level + Size, true);
                            level += 100;
                            tess = false;
                            return;
                        }

                    }
                }

            }
            else if (Canvas.GetBottom(player) < level)
            {
                Canvas.SetBottom(player, level);
            }
            else
            {
                if (CD) { return; }
                cooldown(new object(), new EventArgs());
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>())//checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((collCheck.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) - Size) && (Canvas.GetBottom(player) == Canvas.GetBottom(collCheck)))
                        {
                            return;
                        }
                    }
                }
                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player) - 100, Canvas.GetBottom(player), Canvas.GetBottom(player), true);
                
            }

        }








        double ts = 0;
        public void FallGravity(object sender, EventArgs e)
        {
            
            
            foreach(var collCheck in MyCan.Children.OfType<Rectangle>())
            {
                if((collCheck.Tag as string)[0] == 'G')
                {
                    if((Canvas.GetBottom(collCheck)==Canvas.GetBottom(player)-Size)&&(Canvas.GetLeft(player)==Canvas.GetLeft(collCheck)))//zjistí zda existuje block který je na stejné x souřadnici jako a hráč ale o blok níž
                    {
                        
                        return;
                    }
                }
            }


            for (int i = 2; i < 12; i++)
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                {
                    if ((collCheck.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetBottom(collCheck) == Canvas.GetBottom(player) - (Size * i)) && (Canvas.GetLeft(player) == Canvas.GetLeft(collCheck)))//zjistí zda existuje block který je na stejné x souřadnici jako a hráč ale o blok níž
                        {
                            player.Fill = Brushes.Yellow;

                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) - 100, false);
                            move.Completed += levelCheck;
                            return;

                        }

                    }
                }
            }

        }

        public void levelCheck(object sender, EventArgs e)
        {
            level = Canvas.GetBottom(player);
            move.Completed -= levelCheck;
        }




        #region working
        public void cooldown(object sender, EventArgs e)
        {
            jumping = false;

            CD = true;
            
            coolDown.Interval = new TimeSpan(0, 0, 0, 0, 500);
            coolDown.Start();
            coolDown.Tick += (sender, e) =>
            {
                CD = false;
                move.Completed -= cooldown;
                coolDown.Stop();
                
            };
        }









        public void Generate()
        {
            MessageBox.Show("x");
            for(int i=0; i<100; i++)
            {


                Rectangle bedrock = new Rectangle();
                bedrock.Fill = Brushes.DarkGray;
                bedrock.Width = 100;
                bedrock.Height = 100;
                bedrock.Tag = "GY";
                Canvas.SetBottom(bedrock, -100);
                Canvas.SetLeft(bedrock, i * 100);
                MyCan.Children.Add(bedrock);

                if (i > 5)
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
#endregion
