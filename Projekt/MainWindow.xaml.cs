using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


    //TODO: Screen shift  up - Resources


    public partial class MainWindow : Window
    {


        public List<Item> InventoryList = new List<Item>();

        public bool ShiftCD = false;
        int leftScreen = 1;
        public char facing = 'd';//l-left r-right t-top d-down
        public bool CD = false;
        bool tess = false;
        bool down = false;

        public bool blockMove = false;

        public double Size = 100d;
        public double level = 120;
        public bool jumping = false;
        DispatcherTimer coolDown = new DispatcherTimer();// 
        Rectangle toBreak = null;

        DispatcherTimer breaktimer = new DispatcherTimer();

       

        Storyboard move = new Storyboard();//create storyboard for animating
        Rectangle selector = new Rectangle();

        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();

            Generate();//Generates the world
            
            level = Canvas.GetBottom(player);

            ts = Canvas.GetBottom(player);

            JumpGravity();//calls jumpgravity  in order to avoid problems 
            Right();// calls right in order to avoid problems
            breaktimer.Interval = new TimeSpan(0, 0, 0, 2); //sets the interval for breaking blocks

            
            selector.Width = 100; //creates the selector
            selector.Height = 100;
            selector.Fill = new SolidColorBrush((Color.FromArgb(128, 20, 20, 220)));
            MyCan.Children.Add(selector);
            Canvas.SetLeft(selector, Canvas.GetLeft(player) + 100);
            Canvas.SetBottom(selector, Canvas.GetBottom(player));
            selector.Tag = "__U____";



            Item alpha = new Item(5, "Alpha", new Uri("pack://application:,,,/Images/Ores/copper.png"));
            InventoryList.Add(alpha);

            #region keys
            this.KeyDown += (s, e) => //movement keys
            {
                if (down == true) { return; }


                if(e.Key == Key.E)
                {
                    OpenInventory();
                }

                if(blockMove == true) { return; }
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
                test.Content = Canvas.GetBottom(player);
                down = true;
                
            };


            this.KeyUp += (s, e) =>//fix  breakign
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
                toBreak = null;
            };

            this.MouseMove += (s, e) =>
            {
                double X = Mouse.GetPosition(player).X;
                double Y = Mouse.GetPosition(player).Y;
                if ((X < 0) && (Y < 0)) {  Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) + 100)); } //top left
                else if ((X>100) && (Y<0)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) + 100)); }  //top right
                else if ((X < 0) && (Y > 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player)- 100)); } //bottom left
                else if ((X > 100) && (Y> 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) - 100)); }  //bottom right
                else if ((X<0)&&((0<Y))&&(Y<100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player))); }//left
                else if ((X > 100) && ((0 < Y)) && (Y < 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player))); }//right
                else if (((0 < X)) && (X < 100)&& (Y < 0)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player))); Canvas.SetBottom(selector, (Canvas.GetBottom(player)+100)); }//top
                else if (((0 < X)) && (X < 100) && (Y > 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player))); Canvas.SetBottom(selector, (Canvas.GetBottom(player)-100)); }//top


            };




        }


        #endregion
        #region working

        #region jump
        public void Jump()
        {
            if (CD || jumping) { return; }
            jumping = true;
            move.Completed -= JumpCompleted;
            move.Completed += JumpCompleted;
            level = Canvas.GetBottom(player);

            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d, false, 0.2);

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
            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), level, false, 0.4);
        }

        public void JumpGravityCompleted(object sender, EventArgs e)
        {
            move.Completed -= JumpGravityCompleted;
            cooldown(new object(), new EventArgs());
            move.Children.Clear();

            if(Canvas.GetBottom(player)>=820)
            {
                test.Background = Brushes.Red;
                ShiftScreen(3);
            }



        }
        #endregion
        public void Animation(double x1, double x2, double y1, double y2, bool Grav, double time)
        {

            move.FillBehavior = FillBehavior.HoldEnd;

            //definy movement X (left-right)
            DoubleAnimation AnimX = new DoubleAnimation();
            AnimX.From = x1;
            AnimX.To = x2;
            AnimX.Duration = new Duration(TimeSpan.FromSeconds(time));
            //define movement Y (Down-Up)
            DoubleAnimation AnimY = new DoubleAnimation();
            AnimY.From = y1;
            AnimY.To = y2;
            AnimY.Duration = new Duration(TimeSpan.FromSeconds(time));

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
                    if ((collCheck.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) + Size) && (level + Size == Canvas.GetBottom(collCheck)))//checks if there is a block on the "desired position"
                        {
                            return;
                        }
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

                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(control), Canvas.GetBottom(player), level + Size, true, 0.2);
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
                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player) + 100, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);


            }
            if (Canvas.GetLeft(player) > 1500) { ShiftScreen(1); }

        }


        public void Left()//movement Left
        {


            if (Canvas.GetBottom(player) > level)
            {
                foreach (var collCheck in MyCan.Children.OfType<Rectangle>()) //checks for collision, if the player is next to block and tries moving into it, cancels the movement
                {
                    if ((collCheck.Tag as string)[0] == 'G')
                    {
                        if ((Canvas.GetLeft(collCheck) == Canvas.GetLeft(player) - Size) && (level + Size == Canvas.GetBottom(collCheck)))//checks if there is a block on the "desired position"
                        {
                            return;
                        }
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

                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(control), Canvas.GetBottom(player), level + Size, true, 0.2);
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
                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player) - 100, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);

            }
            if (Canvas.GetLeft(player) < 100) { ShiftScreen(0); }


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

                                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) - 100, false, 0.4);
                                move.Completed += levelCheck;
                                if (Canvas.GetBottom(player) < 120) { ShiftScreen(2); }
                                return;

                            }

                        }
                    }
                }

        } // ensures that the player falls when they move 

        public void levelCheck(object sender, EventArgs e)
        {
            level = Canvas.GetBottom(player);
            move.Completed -= levelCheck;
        } //ensures that the level variable is up to dae




        
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
                if ((breakCheck.Tag as string)[2] != 'U')
                {
                    if ((Canvas.GetLeft(breakCheck) == Canvas.GetLeft(selector))&& (Canvas.GetBottom(breakCheck) == Canvas.GetBottom(selector)))
                    {
                        toBreak = breakCheck;
                    }
                
                }
            }

            breaktimer.Tick += (s, e) =>
            {
                if(toBreak != null)
                {
                    MyCan.Children.Remove(toBreak);
                    toBreak = null;
                    FallGravity(new object(),new EventArgs());
                    breaktimer.Stop();
                }
            };
            breaktimer.Start();


        }



        #endregion



    



        public void Generate()
        {
            Random rnd = new Random();
            string SeedPlus = "";
            for (int i = 0; i < 101; i++)
            {

                SeedPlus += Convert.ToString(rnd.Next(0, 10));
            }
            test.Content = SeedPlus;

            //for(int i=1; i < 20; i++)
            //{
            //    Rectangle baseDirt = new Rectangle();
            //    baseDirt.Width = 100;
            //    baseDirt.Height = 100;
            //    baseDirt.Fill = Brushes.Brown;
            //    baseDirt.Tag = "GY_";
            //    baseDirt.Name = "Dirt";
            //    Canvas.SetLeft(baseDirt, 100 * i);
            //    Canvas.SetBottom(baseDirt,( 100 * i)+20);
            //    MyCan.Children.Add(baseDirt);
            //}





            for (int i = -100; i < 100; i++)
            {
                for (int j = 1; j < 40; j++)
                {

                    Rectangle stone = new Rectangle();
                    stone.Width = 100;
                    stone.Height = 100;
                    stone.Tag = "GY_";

                    if (SeedPlus[Math.Abs(i)] == '1')
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/copper.png")) };
                    }
                    else if ((SeedPlus[Math.Abs(i)] == '2') && (j > 25))
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
                    Canvas.SetBottom(stone, 20-(100 * j));








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
                Canvas.SetBottom(Bedrock, -80 * 41);
                MyCan.Children.Add(Bedrock);

            }




        }

        public void ShiftScreen(byte direction)//0-left 1-right 2-down 3-up
        {
            if (ShiftCD == false)
            {
                if (direction == 1)//right
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {

                        Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) - 1500);



                    }
                    Animation(Canvas.GetLeft(player), 0, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);
                }
                else if (direction == 0)//left
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {

                        Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) + 1500);

                    }
                    Animation(Canvas.GetLeft(player), 1400, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);

                }
                else if (direction == 2)//down
                {
                    
                    Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), 820, true, 0.2);
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        
                        Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) + 800);

                    }
                    
                }
                else if (direction == 3)
                {
                    
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {

                        if (collCheck.Name != "player")
                        {
                            Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) - 800);
                        }
                        else
                        {
                           
                        }
                        

                    }
                    

                   
                }
            }
        }
        #endregion




        void OpenInventory()
        {
            Inventory Inv = new Inventory(InventoryList);
            blockMove = true;
            Inv.Show();

            Inv.Closed += (s, e) =>
            {
                blockMove = false;
            };

        }
    }
}

