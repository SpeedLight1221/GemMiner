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


    // Tag Logic : [0] Type of object (G-Ground) [1] - Y top collision (Y-true, _-False)  [2] - U - Ubreakable (U-true,_-false) [3] resource type (D-dirt,S-stone)


    //TODO: Screen shift  up - Resources


    public partial class MainWindow : Window
    {
        public Item Slot1;
        public Item Slot2;

        public List<Recipe> Recipes = new List<Recipe>();
        public Recipe PH = new Recipe("ph", new Item(0, "h", new Uri($"pack://application:,,,/Images/Icons/0.png"), "None"), 0, "e",0, "i", 0, null, 0, new Uri("pack://application:,,,/Images/Ores/0.png"));
      

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
            Recipes.Add(PH);
            CreateRecipes();
            Generate();//Generates the world
            
            level = Canvas.GetBottom(player);

            ts = Canvas.GetBottom(player);

            JumpGravity();//calls jumpgravity  in order to avoid problems 
            Right();// calls right in order to avoid problems
            breaktimer.Interval = new TimeSpan(0, 0, 0, 0,500); //sets the interval for breaking blocks
            selector.Width = 100; //creates the selector
            selector.Height = 100;
            selector.Fill = new SolidColorBrush((Color.FromArgb(128, 20, 20, 220)));
            MyCan.Children.Add(selector);
            Canvas.SetLeft(selector, Canvas.GetLeft(player) + 100);
            Canvas.SetBottom(selector, Canvas.GetBottom(player));
            selector.Tag = "__U____";

            Item coal = new Item(1, "coal", new Uri($"pack://application:,,,/Images/Icons/coal.png"), "item");
            Item c1 = new Item(2, "Copper", new Uri($"pack://application:,,,/Images/Icons/copper.png"), "item");
            InventoryList.Add(coal);
            InventoryList.Add(c1);




            #region keys
            this.KeyDown += (s, e) => //movement keys
            {
                if (down == true) { return; }


                if(e.Key == Key.I)
                {
                    OpenInventory();
                   

                }
                else if (e.Key == Key.C)
                {
                    OpenCraft();
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
                if (((collCheck.Tag as string)[0] == 'G') && ((collCheck.Tag as string)[0] != '_'))
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
                        if (((collCheck.Tag as string)[0] == 'G')&& ((collCheck.Tag as string)[0] != '_'))
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
                    if ((breakCheck.Tag as string)[0] != '_')
                    {
                        if ((Canvas.GetLeft(breakCheck) == Canvas.GetLeft(selector)) && (Canvas.GetBottom(breakCheck) == Canvas.GetBottom(selector)))
                        {
                            toBreak = breakCheck;
                        }
                    }
                }
            }

            breaktimer.Tick += (s, e) =>
            {
                if(toBreak != null)
                {
                    MyCan.Children.Remove(toBreak);



                    AddToInventory(toBreak.Tag as string,"Block");


                    toBreak = null;
                    FallGravity(new object(),new EventArgs());
                    breaktimer.Stop();
                }
            };
            breaktimer.Start();


        }



        #endregion

        // D- dirt S-stone C- copper T- tin I- iron

        public void AddToInventory(string toAdd,string type)
        {
            string itemName = "";
            switch(toAdd[3])
            {
                case 'D':
                    itemName = "Dirt";
                    break;

                case 'S':
                    itemName = "Stone";
                    break;

                case 'C':
                    itemName = "Copper";
                    break;

                case 'T':
                    itemName = "Tin";
                    break;

                case 'I':
                    itemName = "Iron";
                    break;
            }



            foreach(Item i in InventoryList) //check if item of this type is already in inventory
            {
                if(i.Name == itemName)
                {
                    i.Amount++;
                    return;
                }
                
            }

            Item ni = new Item(1, itemName, new Uri($"pack://application:,,,/Images/Icons/{itemName}.png"),type);
            InventoryList.Add(ni);






        }


        #endregion


        public void Generate()
        {
            Random rnd = new Random();
            string SeedPlus = "";
            for (int i = 0; i < 201; i++)
            {

                SeedPlus += Convert.ToString(rnd.Next(0, 10));
            }
            test.Content = SeedPlus;





            int k = 0;
            for (int i = -100; i < 100; i++)
            {
                
                for (int j = 2; j < 40; j++)
                {

                    if(k+1 < SeedPlus.Length)
                    {
                        k++;
                    }
                    else
                    {
                        k = 0;
                    }


                    
                    Rectangle stone = new Rectangle();
                    stone.Width = 100;
                    stone.Height = 100;
                    stone.Tag = "GY_S";

                    if ((SeedPlus[Math.Abs(k)] == '1')&& (k%2 ==0) && (j<25))
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/copper.png")) };
                        stone.Tag = "GY_C";
                    }
                    else if ((SeedPlus[Math.Abs(k)] == '2') && (k % 2 == 0) && (j > 15))
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Tin.png")) };
                        stone.Tag = "GY_T";
                    }
                    else if ((SeedPlus[Math.Abs(k)] == '3') && (k % 2 == 0) && (j > 25))
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Iron.png")) };
                        stone.Tag = "GY_I";
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
                baseDirt.Tag = "GY_D";
                baseDirt.Name = "Dirt";
                Canvas.SetLeft(baseDirt, 100 * i);
                Canvas.SetBottom(baseDirt, -80);
                MyCan.Children.Add(baseDirt);

                Rectangle Bedrock = new Rectangle();
                Bedrock.Width = 100;
                Bedrock.Height = 100;
                Bedrock.Fill = Brushes.Black;
                Bedrock.Tag = "GYU_";
                Bedrock.Name = "Bedrock";
                Canvas.SetLeft(Bedrock, 100 * i);
                Canvas.SetBottom(Bedrock, -80 * 41);
                MyCan.Children.Add(Bedrock);

            }

            #region valley

            int offsetG = 0; // will be used to make the valley genarate on any point on map

            //Generates a valley
            int d = 0;// depth of the valley
            int w = 0; //width of the valley
            if (Convert.ToInt16(SeedPlus[k])>7)
            {
                d = 6;
                w = 9;
            }
            else if (Convert.ToInt16(SeedPlus[k])>4)
            {
                d = 5;
                w = 6;
            }
            else
            {
                d = 4;
                w = 5;
            }


            List<Rectangle> toremove = new List<Rectangle>();

            foreach (var removeCheck in MyCan.Children.OfType<Rectangle>())
            {
                int minW = 0; // changes the width for each layer
                int maxW = 0;

                for (int i = 0; i <= d; i++)
                {
                    for (int j = 0+ minW; j <= w-maxW; j++)
                    {
                       if(Canvas.GetBottom(removeCheck) == -80 - (100 * i))
                        {
                            if(Canvas.GetLeft(removeCheck) == j*100+500+offsetG)
                            {
                                if ((removeCheck.Tag as string)[0] != '_')
                                {
                                    toremove.Add(removeCheck);
                                }
                            }
                        }
                    }
                    if ((SeedPlus[i] == '6') || (SeedPlus[i] == '7')) //makes it so the valley isnt symmetric
                    {
                        minW += 0;
                        maxW += 1;
                    }
                    else if (SeedPlus[i] == '4')
                    {
                        maxW += 2;
                        minW += 1;
                    }
                    else
                    {
                        minW += 1;
                        maxW += 1;
                    }


                }


            }

            foreach(Rectangle c in toremove)
            {
                test2.Content += (c.Tag as string) + "\n";
                MyCan.Children.Remove(c);
                
            }
           



            #endregion



        }

        public void ShiftScreen(byte direction)//0-left 1-right 2-down 3-up
        {
            if (ShiftCD == false)
            {
                if (direction == 1)//right
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {

                            Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) - 1500);
                        }



                    }
                    Animation(Canvas.GetLeft(player), 0, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);
                }
                else if (direction == 0)//left
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {
                            Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) + 1500);
                        }

                    }
                    Animation(Canvas.GetLeft(player), 1400, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);

                }
                else if (direction == 2)//down
                {
                    
                    Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), 820, true, 0.2);
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {
                            Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) + 800);
                        }

                    }
                    
                }
                else if (direction == 3)
                {
                    
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {

                            if (collCheck.Name != "player")
                            {
                                Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) - 800);
                            }
                            else
                            {
                                move.Stop();
                                Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), 100, true, 0.2);

                            }
                        }
                        

                    }
                    

                   
                }
            }
        }
        




        void OpenInventory()
        {
            Inventory Inv = new Inventory(InventoryList);
            blockMove = true;
            Inv.Show();

            Inv.Closing += (s, e) =>
            {
                EquipSlots(Inv.slot1, Inv.slot2);
            };


            Inv.Closed += (s, e) =>
            {
                blockMove = false;
            };

        }

        public void EquipSlots(Item s1, Item s2)
        {
            if(s1 != null)
            {
                Slot1 = s1;
                PrimImg.Source = new BitmapImage(Slot1.Image);
            }

            if (s2 != null)
            {
                Slot2 = s2;
                SecImg.Source = new BitmapImage(Slot2.Image);
            }
        }

        
      
        public void CreateRecipes()
        {
            Recipe CopperBar = new Recipe("Copper Bar", new Item(2, "Copper Bar", new Uri($"pack://application:,,,/Images/Icons/Copper Bar.png"), "Item"),2, "copper", 2, "coal", 1, null, null, new Uri("pack://application:,,,/Images/Ores/Copper Bar.png"));

            Recipes.Add(CopperBar);
        }

        public void OpenCraft()
        {
            

            if (Recipes[0].Name == "ph")
            {
                Recipes.RemoveAt(0);
            }


            RecipesW RecipeWindow = new RecipesW(Recipes, InventoryList);




            RecipeWindow.Show();


            RecipeWindow.Closing += (s, e) =>
            {
                InventoryList = RecipeWindow.InventoryCraft;
            };

        }

        
    }
}

