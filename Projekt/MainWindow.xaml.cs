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


    // Tag Logic : [0] Type of object (G-Ground) [1] - Y top collision (Y-true, _-False)  [2] - U - Ubreakable (U-true,_-false)


    //TODO: Screen shift down and up - Breaking and Resources


    public partial class MainWindow : Window
    {


        int leftScreen = 1;
        public char facing = 'l';//l-left r-right t-top d-down
        public bool CD = false;
        bool tess = false;
        bool down = false;
        public double Size = 100d;
        public double level = 120;
        public bool jumping = false;
        DispatcherTimer coolDown = new DispatcherTimer();// 

        DispatcherTimer breaktimer = new DispatcherTimer();
        

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
            breaktimer.Interval = new TimeSpan(0, 0, 0, 2);

            #region keys
            this.KeyDown += (s, e) => //movement keys
            {
                if(down == true) { return; }
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
                test.Content = Canvas.GetLeft(player);
                down = true;

            };


            this.KeyUp += (s, e) => 
            {
                down = false;


                breaktimer.Stop();
            };

            this.MouseLeftButtonDown += (s, e) => //mouse btns
            {
                Break();
            };

            this.MouseLeftButtonUp += (s, e) => //mouse btns
            {
                breaktimer.Stop();
            };




        }

        #endregion

        #region jump
        public void Jump()
        {
            if (CD || jumping) { return; }
            jumping = true;
            move.Completed -= JumpCompleted;
            move.Completed += JumpCompleted;
            level = Canvas.GetBottom(player);

            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d, false);

        }

        public void JumpCompleted(object sender, EventArgs e) //in order to avoid a memory leak
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
            cooldown(new object(), new EventArgs());
            move.Children.Clear();

        }
        #endregion
        public void Animation(double x1, double x2, double y1, double y2, bool Grav)
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


            if (Grav == true)
            {
                move.Completed += FallGravity;
                test.Background = Brushes.PaleGreen;
            }


            move.Begin();

        }

        public void Right()//movement right
        {


            if (Canvas.GetBottom(player) > level)
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>()) //checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) + Size) && (level + Size == Canvas.GetBottom(collCheck)))//checks if there is a block on the "desired position"
                    {
                        return;
                    }
                }
                foreach (var control in MyCan.Children.OfType<Rectangle>()) //if there is a block 1 higher and to the right, moves to it
                {
                    if ((control.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetLeft(control) == Canvas.GetLeft(player) + Size) && (level == Canvas.GetBottom(control)))//checks if there is a block on the "desired position"
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
                        if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) + Size) && (Canvas.GetBottom(player) == Canvas.GetBottom(collCheck)))
                        {
                            return;
                        }
                    }
                }
                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player) + 100, Canvas.GetBottom(player), Canvas.GetBottom(player), true);


            }
            if (Canvas.GetLeft(player) > 1500) { ShiftScreen(1); }

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
                        if ((Canvas.GetLeft(control) == Canvas.GetLeft(player) - Size) && (level == Canvas.GetBottom(control)))//checks if there is a block on the "desired position"
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
            if (Canvas.GetLeft(player) <100) { ShiftScreen(0); }


        }








        double ts = 0;
        public void FallGravity(object sender, EventArgs e)
        {


            foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
            {
                if ((collCheck.Tag as string)[0] == 'G')
                {
                    if ((Canvas.GetBottom(collCheck) == Canvas.GetBottom(player) - Size) && (Canvas.GetLeft(player) == Canvas.GetLeft(collCheck)))//zjistí zda existuje block který je na stejné x souřadnici jako a hráč ale o blok níž
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

            coolDown.Interval = new TimeSpan(0, 0, 0, 0, 200);
            coolDown.Start();
            coolDown.Tick += (sender, e) =>
            {
                CD = false;
                move.Completed -= cooldown;
                coolDown.Stop();

            };
        }



        #region break
        public void Break()
        {
            

            foreach (var breakCheck in MyCan.Children.OfType<Rectangle>())
            {
                if((breakCheck.Tag as string)[0] != 'U')
                {
                    if((facing == 'l')&&(Canvas.GetLeft(breakCheck) == Canvas.GetLeft(player)-100)&&(Canvas.GetBottom(breakCheck) == Canvas.GetBottom(player)))
                    {
                        breaktimer.Tick += (s, e) =>
                        {
                            MyCan.Children.Remove(breakCheck);
                            test2.Content = Canvas.GetLeft(breakCheck);
                        };
                        breaktimer.Start();
                    }
                    else if ((facing == 'r') && (Canvas.GetLeft(breakCheck) == Canvas.GetLeft(player) +100) && (Canvas.GetBottom(breakCheck) == Canvas.GetBottom(player)))
                    {
                        breaktimer.Tick += (s, e) =>
                        {
                            MyCan.Children.Remove(breakCheck);
                        };
                        breaktimer.Start();
                    }
                    else if ((facing == 't') && (Canvas.GetBottom(breakCheck) == Canvas.GetBottom(player) + 100)&& (Canvas.GetLeft(breakCheck) == Canvas.GetLeft(player)))
                    {
                        breaktimer.Tick += (s, e) =>
                        {
                            MyCan.Children.Remove(breakCheck);
                        };
                        breaktimer.Start();
                    }
                    else if ((facing == 'd') && (Canvas.GetLeft(breakCheck) == Canvas.GetLeft(player) - 100) && (Canvas.GetLeft(breakCheck) == Canvas.GetLeft(player)))
                    {
                        breaktimer.Tick += (s, e) =>
                        {
                            MyCan.Children.Remove(breakCheck);
                            
                        };
                        breaktimer.Start();
                    }
                }
            }
        }



        #endregion







        public void Generate()
        {
            Random rnd = new Random();
            string SeedPlus = "";
            for (int i = 0; i < 101; i++)
            {
                
                SeedPlus += rnd.Next(0, 10);
            }
            test.Content = SeedPlus;





            for (int i = -100; i < 100; i++)
            {
                for(int j = 2; j < 40; j++)
                {

                    Rectangle stone = new Rectangle();
                    stone.Width = 100;
                    stone.Height = 100;
                    stone.Tag = "GY_";

                    if (SeedPlus[Math.Abs(i)] == '1')
                    {
                        stone.Fill = new ImageBrush { ImageSource=new BitmapImage(new Uri("pack://application:,,,/Images/Ores/copper.png"))};
                    }
                    else if((SeedPlus[Math.Abs(i)] == '2')&&(j >25))
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Iron.png")) };
                    }
                    else if ((SeedPlus[Math.Abs(i)] == '3') && (j > 15))
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Iron.png")) };
                    }
                    else
                    {
                        stone.Fill = Brushes.Gray;

                    }
                    MyCan.Children.Add(stone);
                    Canvas.SetLeft(stone, 100 * i);
                    Canvas.SetBottom(stone, -80 * j);








                }
                Rectangle baseDirt = new Rectangle();
                baseDirt.Width = 100;
                baseDirt.Height = 100;
                baseDirt.Fill = Brushes.Brown;
                baseDirt.Tag = "GY_";
                baseDirt.Name = "Dirt";
                Canvas.SetLeft(baseDirt, 100 * i);
                Canvas.SetBottom(baseDirt, -80);
                MyCan.Children.Add(baseDirt);

                Rectangle Bedrock = new Rectangle();
                Bedrock.Width = 100;
                Bedrock.Height = 100;
                Bedrock.Fill = Brushes.Black;
                Bedrock.Tag = "GYU";
                Bedrock.Name = "Bedrock";
                Canvas.SetLeft(Bedrock, 100 * i);
                Canvas.SetBottom(Bedrock, -80*41);
                MyCan.Children.Add(Bedrock);

            }
            
                

            
        }

        public void ShiftScreen(byte direction)//0-left 1-right 2-down 3-up
        {
            if (direction == 1)//right
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                {

                    Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) - 1500);



                }
                Animation(Canvas.GetLeft(player), 0, Canvas.GetBottom(player), Canvas.GetBottom(player), true);
            }
            else if (direction == 0)//left
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                {

                    Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) + 1500);

                }
                Animation(Canvas.GetLeft(player), 1400, Canvas.GetBottom(player), Canvas.GetBottom(player), true);
            }
        }
    }
}
#endregion
