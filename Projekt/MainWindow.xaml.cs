﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    // Tag Logic : [0] Type of object (G-Ground) [1] - Y top collision (Y-true, _-False)  [2] - U - Ubreakable (U-true,_-false) [3] resource type (D-dirt,S-stone,W-wood)


    //TODO: Screen shift  up - Resources


    public partial class MainWindow : Window
    {
        public Item Slot1;
        public Item Slot2;
        public string Seed = "";
      
        public bool title = true;

        public List<Recipe> Recipes = new List<Recipe>();
        public Recipe PH = new Recipe("ph", new Item(0, "h", new Uri($"pack://application:,,,/Images/Icons/0.png"), "None"), 0, "e", 0, "i", 0, null, 0, new Uri("pack://application:,,,/Images/Ores/0.png"));

        public bool tse = false;
        public List<Item> InventoryList = new List<Item>();

        public bool ShiftCD = false;
    
        public char facing = 'd';//l-left r-right t-top d-down
        public bool CD = false;
        bool tess = false;
        bool down = false;
        int count = 0;

        public Button Exit;
        public Button Play;
        
        public bool blockMove = true;
        public bool moving = false;
        public double Size = 100d;
        public double level = 120;
        public bool jumping = false;
        public TimeSpan BreakTime = new TimeSpan(0, 0, 0, 0, 500);
        DispatcherTimer coolDown = new DispatcherTimer();// 
        Rectangle toBreak = null;

        public bool Overlay = false;


        //Completion CHecks

        public bool ADiamond = false;
        public bool AOpal = false;
        public bool AEmerald = false;
        public bool ASaphire = false;
        public bool ARuby = false;

        //Gen Checks

        public bool GDiamond = false;
        public bool GOpal = false;
        public bool GEmerald = false;
        public bool GSaphire = false;
        public bool GRuby = false;

        public DateTime Start;
        DispatcherTimer breaktimer = new DispatcherTimer();

        Dictionary<string, char> BlockTags = new Dictionary<string, char>();


        Storyboard move = new Storyboard();//create storyboard for animating
        Rectangle selector = new Rectangle();

        public bool blockedY = false;
        public MainWindow()
        {
            InitializeComponent();
            Recipes.Add(PH);
            CreateRecipes();
            
            Timeline.SetDesiredFrameRate(move, 40);

            level = Canvas.GetBottom(player);
            JumpGravity();//calls jumpgravity  in order to avoid problems 
            Right();// calls right in order to avoid problems
            breaktimer.Interval = BreakTime; //sets the interval for breaking blocks
            selector.Width = 100; //creates the selector
            selector.Height = 100;
            Panel.SetZIndex(selector, 6);
            selector.Fill = new SolidColorBrush((Color.FromArgb(128, 20, 20, 220)));
            MyCan.Children.Add(selector);
            Canvas.SetLeft(selector, Canvas.GetLeft(player) + 100);
            Canvas.SetBottom(selector, Canvas.GetBottom(player));
            selector.Tag = "__U____";

            //Start item
            
            Item cp = new Item(1, "Broken Copper Pick", new Uri($"pack://application:,,,/Images/Icons/old copper pick.png"), "Tool");

   

            InventoryList.Add(cp);

            player.Fill = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Images/Characters/Player/Right.png")));




            BlockTags.Add("Dirt", 'D');
            BlockTags.Add("Stone", 'S');
            BlockTags.Add("Plank", 'W');



            


            #region keys
            this.KeyDown += (s, e) => //movement keys
            {
                if (down == true) { return; }






                if (e.Key == Key.I)
                {
                    if (Overlay) { return; }
                    OpenInventory();


                }
                else if (e.Key == Key.C)
                {
                    if (Overlay) { return; }
                    OpenCraft();
                }
                else if(e.Key == Key.Escape)
                {
                    if(blockMove == false)
                    {
                        blockMove = true;
               
                        Canvas.SetTop(TitleImg, 116);




                        MyCan.Children.Add(Play);
                        MyCan.Children.Add(Exit);
                    }
                    else
                    {
                       blockMove = false;
                      
                        Canvas.SetTop(TitleImg, 2000);

                        Exit = ExitBtn;
                        Play = PlayBtn;

                        MyCan.Children.Remove(PlayBtn);
                        MyCan.Children.Remove(ExitBtn);
                    }
                }
                else if(e.Key == Key.H)
                {
                    MessageBox.Show("Press D/A for Movement \n Press Space for Jumping \n Press C to open Crafting Menu \n Press I to show Inventory \n Press Esc to Pause \n Left click to Mine \n Right Click to Place \n \n Your goal is to collect the five Legendary Gems! Good Luck!");
                }
                

               
                


                if (blockMove == true) { return; }
                switch (e.Key)
                {
                    case Key.Space:
                        Jump();

                        break;
                    case Key.D:
                        player.Fill = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Images/Characters/Player/Right.png")));
                        Right();



                        break;
                    case Key.A:
                        player.Fill = new ImageBrush(new BitmapImage(new Uri($"pack://application:,,,/Images/Characters/Player/lleft.png")));
                        Left();
                        break;
                   
                   

                  


                }
                MoveCursor();
                
                down = true;

            };


            this.KeyUp += (s, e) =>//fix  breakign
            {
                if (blockMove == true) { return; }
                down = false;


                breaktimer.Stop();
            };

            this.MouseLeftButtonDown += (s, e) => //mouse btns
            {
                if (blockMove == true) { return; }

                Break();
            };

            this.MouseLeftButtonUp += (s, e) => //mouse btns
            {
                if (blockMove == true) { return; }
                breaktimer.Stop();
                toBreak = null;
            };


            this.MouseRightButtonDown += (s, e) =>
            {
                if (blockMove == true) { return; }
                Place();
            };

            this.MouseMove += (s, e) =>
            {
                if (blockMove == true) { return; }
                MoveCursor();

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
            cooldown(new object(), new EventArgs());
            level = Canvas.GetBottom(player);

            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), Canvas.GetBottom(player) + 150d, false, 0.2);

        }

        public void JumpCompleted(object sender, EventArgs e) //in order to avoid a memory leak
        {
            move.Completed -= JumpCompleted;
            move.FillBehavior = FillBehavior.HoldEnd;
            if (tse == true) { return; }

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

            move.Children.Clear();

            if (Canvas.GetBottom(player) > 820)
            {
                
                ShiftScreen(3);
            }



        }
        #endregion
        public void Animation(double x1, double x2, double y1, double y2, bool Grav, double time)
        {
            moving = true;
            move.Duration = new Duration(TimeSpan.FromSeconds(time));


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


            if (move.GetAnimationBaseValue(Canvas.BottomProperty) != null)
            {
                count++;
            }



            move.Completed += (s, e) =>
            {
                // if(Canvas.GetLeft(player) != x2) { Canvas.SetLeft(player, x2); }
                //if (Canvas.GetBottom(player) != y2) { Canvas.SetLeft(player, y2); }

                Canvas.SetLeft(player, x2);
                Canvas.SetBottom(player, y2);

                jumping = false;
              

            };


            if (Grav == true)
            {
                move.Completed += FallGravity;

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
            if (Canvas.GetLeft(player) >= 1810) { ShiftScreen(1); }

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
            if (Canvas.GetLeft(player) <= 10) { ShiftScreen(0); }


        }

        public void FallGravity(object sender, EventArgs e)
        {
            double yy = Canvas.GetBottom(player) - 100;
            yy = Canvas.GetBottom(player) - 100;

      

            double time = 1;
            if (tse == true)
            {
                time = 0.01;
            }

            // ----------------- Implent that if count is 1 then shift to bottom
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
                    if (((collCheck.Tag as string)[0] == 'G') && ((collCheck.Tag as string)[0] != '_'))
                    {
                        if ((Canvas.GetBottom(collCheck) == Canvas.GetBottom(player) - (Size * i)) && (Canvas.GetLeft(player) == Canvas.GetLeft(collCheck)))//zjistí zda existuje block který je na stejné x souřadnici jako a hráč ale o blok níž
                        {
                            


                            if(count == 1)
                            {
                                yy = 20;
                                count = 0;
                               
                            }


                         
                            Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), yy, false, time);
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
            int cd = 200;
            if (jumping == true) { cd = 750; }


            jumping = false;

            CD = true;

            coolDown.Interval = new TimeSpan(0, 0, 0, 0, cd);
            coolDown.Start();
            coolDown.Tick += (sender, e) =>
            {
                CD = false;
                move.Completed -= cooldown;
                coolDown.Stop();

            };
        }

        #region place

        public void Place()
        {
            if(((Canvas.GetBottom(selector)-20)%100 ==0)||(Canvas.GetBottom(selector)==0 ))
            {
                
            }
            else
            {
                return;
            }


            foreach (var PlaceCheck in MyCan.Children.OfType<Rectangle>())
            {

                if ((Canvas.GetBottom(PlaceCheck) == Canvas.GetBottom(selector)) && (Canvas.GetLeft(PlaceCheck) == Canvas.GetLeft(selector)))
                {
                    if (PlaceCheck.Name != selector.Name)
                    {
                        return;
                    }

                }
            }
         
            if (Slot2 != null)
            {
                if (Slot2.type != "Block")
                {
                    return;
                }

                Rectangle nb = new Rectangle();
                nb.Width = 100;
                nb.Height = 100;
                Canvas.SetBottom(nb, Canvas.GetBottom(selector));
                Canvas.SetLeft(nb, Canvas.GetLeft(selector));
                MyCan.Children.Add(nb);
                nb.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/Ores/{Slot2.Name}.png")) };
                nb.Tag = $"GY_{BlockTags[Slot2.Name]}___";


                Item? toRemove = new Item(-69, "placeholder", new Uri($"pack://application:,,,/Images/Icons/old copper pick.png"),"douyoufeeltheweightofyoursins?");
                foreach(Item i in InventoryList)
                {
                    if(i == Slot2)
                    {
                        if(i.Amount > 1)
                        {
                            i.Amount--;
                        }
                        else
                        {
                            toRemove = i;
                        }
                    }
                }

                if(toRemove.Name != "placeholder")
                {
                    InventoryList.Remove(toRemove);
                    Slot2 = null;
                    SecImg.Source = null;
                    
                }
               




            }


        }



        #endregion





        #region break
        public void Break()
        {


            foreach (var breakCheck in MyCan.Children.OfType<Rectangle>())
            {
                if (((breakCheck.Tag as string)[2] != 'U')&&(breakCheck.Tag as string != "Player"))
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
            #region toolCheck

            string tool = "";
            int tier = -1;
            string[] toolInfo;
            if (Slot1 != null)
            {
                toolInfo = Slot1.Name.Split(' ');

                if (toolInfo.Contains("Pick"))
                {
                    tool = "Pick";
                    if (toolInfo.Contains("Broken"))
                    {
                        tier = 0;
                    }
                    else if (toolInfo.Contains("Copper"))
                    {
                        tier = 1;

                    }
                    else if (toolInfo.Contains("Bronze"))
                    {
                        tier = 2;
                    }
                    else if (toolInfo.Contains("Iron"))
                    {
                        tier = 3;
                    }
                }
                else if (Slot1.Name.Split(' ')[1] == "Axe")
                {
                    tool = "Axe";
                    if (toolInfo.Contains("Broken"))
                    {
                        tier = 0;
                    }
                    else if (toolInfo.Contains("Copper"))
                    {
                        tier = 1;


                    }
                    else if (toolInfo.Contains("Bronze"))
                    {
                        tier = 2;
                    }
                    else if (toolInfo.Contains("Iron"))
                    {
                        tier = 3;
                    }
                }
            }

            if (toBreak != null)
            {

                switch ((toBreak.Tag as string)[3])
                {
                    case 'W':
                        if (tool != "Axe")
                        {
                            BreakTime = new TimeSpan(0, 0, 0, 2, 500);

                        }
                        else
                        {

                            Tiercheck();
                        }
                        break;
                    case 'S':
                    case 'C':
                        if (tool != "Pick")
                        {
                            return;
                        }
                        else
                        {
                            Tiercheck();
                        }
                        break;
                    case 'T':
                        if (tier < 1)
                        {
                            return;
                        }
                        else
                        {
                            if (tool != "Pick")
                            {
                                return;
                            }
                            else
                            {
                                Tiercheck();
                            }
                        }
                        break;
                    case 'I':
                        if (tier < 2)
                        {
                            return;
                        }
                        else
                        {
                            if (tool != "Pick")
                            {
                                return;
                            }
                            else
                            {
                                Tiercheck();
                            }
                        }
                        break;
                    case '1':// The Legendary Gems
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                        if (tool != "Pick")
                        {
                            return;
                        }
                        else
                        {
                            Tiercheck();
                        }
                        break;

                        /* 
                         ¨Logic for check if the player has the right tool
                            right now it should work as follows: tool will be set to either pick , axe or nothing
                            tier will be se acording to the tool, if there is no tool, it will be -1

                        It first checks what kind of block youre mining, then which tool youre using.
                        Must be tested: Give yourself the tools, check whether you can still mine stone w/o a pick 
                        Check timings
                        good luck ig


                         */


                }










            }

            #endregion

            breaktimer.Interval = BreakTime;

            breaktimer.Tick += (s, e) =>
            {
                if (toBreak != null)
                {
                    MyCan.Children.Remove(toBreak);



                    AddToInventory(toBreak.Tag as string, "Block");


                    toBreak = null;
                    FallGravity(new object(), new EventArgs());
                    breaktimer.Stop();
                }
            };
            breaktimer.Start();

            void Tiercheck()
            {
                switch (tier)
                {
                    case -1:
                    case 0:
                        BreakTime = new TimeSpan(0, 0, 0, 3);
                        break;
                    case 1:
                        BreakTime = new TimeSpan(0, 0, 0, 1, 500);

                        break;
                    case 2:
                        BreakTime = new TimeSpan(0, 0, 0, 0, 750);
                        break;
                    case 3:
                        BreakTime = new TimeSpan(0, 0, 0, 0, 300);
                        break;
                }
            }
        }



        #endregion

        // D- dirt S-stone C- copper T- tin I- iron c-coal

        public void AddToInventory(string toAdd, string type)
        {
            string itemName = "";
            switch (toAdd[3])
            {
                case '_':
                    return;
                    break;

                case 'D':
                    itemName = "Dirt";
                    type = "Block";
                    break;

                case 'S':
                    itemName = "Stone";
                    type = "Block";
                    break;

                case 'C':
                    itemName = "Copper";
                    type = "Item";
                    break;

                case 'T':
                    itemName = "Tin";
                    type = "Item";
                    break;

                case 'I':
                    itemName = "Iron";
                    type = "Item";
                    break;
                case 'c':
                    itemName = "Coal";
                    type = "Item";
                    break;
                case 'W':
                    itemName = "Plank";
                    type = "Block";
                    break;
                case '1':// The Legendary Gems //Gem OverWrite 1-Saphire 2-ruby 3-Emerald 4-Opal 5-DIamond
                    ASaphire = true;
                    SaphireIm.Opacity = 1;
                    CheckWin();
                    return;
                    break;
                case '2':
                    ARuby = true;
                    RubyIm.Opacity = 1;
                    CheckWin();
                    return;
                    break;
                case '3':
                    AEmerald = true;
                    EmeraldIm.Opacity = 1;
                    CheckWin();
                    return;
                    break;
                case '4':
                    AOpal = true;
                    OpalIm.Opacity = 1;
                    CheckWin();
                    return;
                    break;
                case '5':
                    ADiamond = true;
                    DiamondIm.Opacity = 1;
                    CheckWin();
                    return;

                    break;
            }



            foreach (Item i in InventoryList) //check if item of this type is already in inventory
            {
                if (i.Name == itemName)
                {
                    i.Amount++;
                    return;
                }

            }

            Item ni = new Item(1, itemName, new Uri($"pack://application:,,,/Images/Icons/{itemName}.png"), type);
            InventoryList.Add(ni);






        }
        public void AddToInventory(Item e)
        {
            foreach (Item i in InventoryList) //check if item of this type is already in inventory
            {
                if (i.Name == e.Name)
                {
                    i.Amount += e.Amount;
                    return;
                }

            }
            InventoryList.Add(e);
        }


        #endregion

        public void MoveCursor()
        {
            double X = Mouse.GetPosition(player).X;
            double Y = Mouse.GetPosition(player).Y;
            if ((X < 0) && (Y < 0)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) + 100)); } //top left
            else if ((X > 100) && (Y < 0)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) + 100)); }  //top right
            else if ((X < 0) && (Y > 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) - 100)); } //bottom left
            else if ((X > 100) && (Y > 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player) - 100)); }  //bottom right
            else if ((X < 0) && ((0 < Y)) && (Y < 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) - 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player))); }//left
            else if ((X > 100) && ((0 < Y)) && (Y < 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player) + 100)); Canvas.SetBottom(selector, (Canvas.GetBottom(player))); }//right
            else if (((0 < X)) && (X < 100) && (Y < 0)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player))); Canvas.SetBottom(selector, (Canvas.GetBottom(player) + 100)); }//top
            else if (((0 < X)) && (X < 100) && (Y > 100)) { Canvas.SetLeft(selector, (Canvas.GetLeft(player))); Canvas.SetBottom(selector, (Canvas.GetBottom(player) - 100)); }//top

        } // moves cursor when moving the mouse


        int e = 0;
        public void Generate()
        {
            Random rnd = new Random();
            string SeedPlus = Seed;

            if(SeedPlus.Length < 200) {

                for (int i = SeedPlus.Length; i < 201; i++)
                {

                    SeedPlus += Convert.ToString(rnd.Next(0, 10));
                }
            }


            
            Seed = SeedPlus;
            //test.Content = SeedPlus;

            List<int> TreeLocsX = new List<int>();


         
            int k = 0;


            for (int i = -100; i < 100; i++)
            {

                if ((SeedPlus[Math.Abs(i)] == '5') || ((SeedPlus[Math.Abs(i)] == '8') && (i % 2 == 0)))
                {
                    if((i > 5 )||( i < -5))
                    {
                        TreeLocsX.Add(10 + (100 * i));
                       
                    }

                   
                }

                string RubX = Convert.ToString(SeedPlus[78]) + Convert.ToString(SeedPlus[8]);
                
                string EmX = Convert.ToString(SeedPlus[108]) + Convert.ToString(SeedPlus[9]);
                
                string OpX = Convert.ToString(SeedPlus[8]) + Convert.ToString(SeedPlus[14]);
                
                string DiaX = Convert.ToString(SeedPlus[199]) + Convert.ToString(SeedPlus[16]);
                




                for (int j = 2; j < 40; j++)
                {
                   

                    if (k + 1 < SeedPlus.Length)
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

                    if ((SeedPlus[Math.Abs(k)] == '1') && (k % 2 == 0) && (j < 25))
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
                    else if (SeedPlus[Math.Abs(k)] == '4')
                    {
                        stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Coal.png")) };
                        stone.Tag = "GY_c";
                    }
                    else
                    {
                        stone.Fill = Brushes.Gray;

                    }



                    
                    if(GSaphire == false)
                    {
                        if((i == 5)&&(j == 8))
                        {
                            stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Saphire.png")) };
                            stone.Tag = "GY_1";
                            GSaphire = true;
                        }
                    }

                    if (GRuby == false)
                    {
                        if ((i == int.Parse(RubX)) && (j == 38))//38
                        {
                            stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Ruby.png")) };
                            stone.Tag = "GY_2";
                            GRuby = true;
                        }
                    }
                    if (GEmerald == false)
                    {
                        if ((i == int.Parse(EmX)) && (j == 34))//34
                        {
                            stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Emerald.png")) };
                            stone.Tag = "GY_3";
                            GEmerald = true;
                        }
                    }
                    if (GOpal == false)
                    {
                        if ((i == int.Parse(OpX)) && (j == 31))//31
                        {
                            stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Opal.png")) };
                            stone.Tag = "GY_4";
                            GOpal = true;
                        }
                    }
                    if (GDiamond == false)
                    {
                        if ((i == int.Parse(DiaX)) && (j == 37))//37
                        {
                            stone.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Diamond.png")) };
                            stone.Tag = "GY_5";
                            GDiamond = true;
                        }
                    }

                     



                    MyCan.Children.Add(stone);
                    Canvas.SetLeft(stone,10+ (100 * i));
                    Canvas.SetBottom(stone, 20 - (100 * j));








                }
                Rectangle baseDirt = new Rectangle();
                baseDirt.Width = 100;
                baseDirt.Height = 100;
                baseDirt.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Dirt.png")) };
                baseDirt.Tag = "GY_D";
                baseDirt.Name = "Dirt";
                Canvas.SetLeft(baseDirt, 10 + (100 * i));
                Canvas.SetBottom(baseDirt, -80);
                MyCan.Children.Add(baseDirt);

                Rectangle Bedrock = new Rectangle();
                Bedrock.Width = 100;
                Bedrock.Height = 100;
                Bedrock.Fill = Brushes.Black;
                Bedrock.Tag = "GYU_";
                Bedrock.Name = "Bedrock";
                Canvas.SetLeft(Bedrock, 10 + (100 * i));
                Canvas.SetBottom(Bedrock, 20 - (100 * 41));
                MyCan.Children.Add(Bedrock);


            }






            #region valley

            //int offsetG = 0; // will be used to make the valley genarate on any point on map

            ////Generates a valley
            //int d = 0;// depth of the valley
            //int w = 0; //width of the valley
            //if (Convert.ToInt16(SeedPlus[k]) > 7)
            //{
            //    d = 6;
            //    w = 9;
            //}
            //else if (Convert.ToInt16(SeedPlus[k]) > 4)
            //{
            //    d = 5;
            //    w = 6;
            //}
            //else
            //{
            //    d = 4;
            //    w = 5;
            //}


            //List<Rectangle> toremove = new List<Rectangle>();

            //foreach (var removeCheck in MyCan.Children.OfType<Rectangle>())
            //{
            //    int minW = 0; // changes the width for each layer
            //    int maxW = 0;

            //    for (int i = 0; i <= d; i++)
            //    {
            //        for (int j = 0 + minW; j <= w - maxW; j++)
            //        {
            //            if (Canvas.GetBottom(removeCheck) == -80 - (100 * i))
            //            {
            //                if (Canvas.GetLeft(removeCheck) == j * 100 + 500 + offsetG)
            //                {
            //                    if ((removeCheck.Tag as string)[0] != '_')
            //                    {
            //                        toremove.Add(removeCheck);
            //                    }
            //                }
            //            }
            //        }
            //        if ((SeedPlus[i] == '6') || (SeedPlus[i] == '7')) //makes it so the valley isnt symmetric
            //        {
            //            minW += 0;
            //            maxW += 1;
            //        }
            //        else if (SeedPlus[i] == '4')
            //        {
            //            maxW += 2;
            //            minW += 1;
            //        }
            //        else
            //        {
            //            minW += 1;
            //            maxW += 1;
            //        }


            //    }


            //}

            //foreach (Rectangle c in toremove)
            //{
            //    test2.Content += (c.Tag as string) + "\n";
            //    MyCan.Children.Remove(c);

            //}




            #endregion






            #region Trees

            List<double[]> coords = new List<double[]>();//for every x coordinate for a tree, finds a Y coordinate
            List<Rectangle> FoundX = new List<Rectangle>();

            foreach (var TreeCheck in MyCan.Children.OfType<Rectangle>())
            {
                if (((TreeCheck.Tag as string)[0] == 'G') && (TreeLocsX.Contains(Convert.ToInt32(Canvas.GetLeft(TreeCheck)))) && (Canvas.GetBottom(TreeCheck) > -200))
                {
                    e++;

                    FoundX.Add(TreeCheck);

                }
            }

            List<double> DupeCheck = new List<double>();
            foreach (Rectangle Upcheck in FoundX)
            {
                List<Rectangle> FindUp = new List<Rectangle>();
                foreach (Rectangle Uppercheck in FoundX)
                {
                    if (Canvas.GetLeft(Uppercheck) == Canvas.GetLeft(Upcheck))
                    {
                        FindUp.Add(Uppercheck);
                    }
                }



                List<Rectangle> sortList = FindUp.OrderBy(r => Canvas.GetBottom(r)).Reverse().ToList();

                if (DupeCheck.Contains(Canvas.GetLeft(sortList[0])))
                {

                }
                else
                {
                    double[] c = new double[2];
                    c[0] = Canvas.GetLeft(sortList[0]);
                    c[1] = Canvas.GetBottom(sortList[0]);
                    DupeCheck.Add(c[0]);
                    coords.Add(c);

                }




            }





            GenerateTree(coords);
           

        }

        public void GenerateTree(List<double[]> co)
        {




            string text = "";
            foreach (double[] e in co)
            {
                text += " " + e[0] + "\n ";
            }
          

           
            foreach (double[] c in co)
            {
                double X = c[0];
                double Y = c[1] + 100;

                int height = 2;
                if ((X / 100) % 2 == 0)
                {
                    height = 4;
                }


                for (int i = 0; i < height; i++)
                {
                    Rectangle wood = new Rectangle();
                    wood.Width = 100;
                    wood.Height = 100;
                    wood.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Log.png")) };
                    wood.Tag = "GY_W";
                    wood.Name = "Wood";
                    Canvas.SetLeft(wood, X);
                    Canvas.SetBottom(wood, Y + (i * 100));
                    MyCan.Children.Add(wood);
                }

                for (int i = 0; i < 7; i++)
                {
                    Rectangle leave = new Rectangle();
                    leave.Width = 100;
                    leave.Height = 100;
                    leave.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Ores/Leaves.png")) };
                    leave.Tag = "GY__";
                    leave.Name = "Leaves";
                    MyCan.Children.Add(leave);

                    if (i < 3)
                    {
                        Canvas.SetBottom(leave, (height * 100) + 20);
                    }
                    else if (i < 6)
                    {
                        Canvas.SetBottom(leave, (height * 100) + 120);
                    }
                    else
                    {
                        Canvas.SetBottom(leave, (height * 100) + 220);
                    }



                    if ((i == 0) || (i == 3))
                    {
                        Canvas.SetLeft(leave, X - 100);
                    }
                    else if ((i == 2) || (i == 5))
                    {
                        Canvas.SetLeft(leave, X + 100);
                    }
                    else
                    {
                        Canvas.SetLeft(leave, X);
                    }


                }

            }




            Start = DateTime.Now;
        }
        #endregion



        public void ShiftScreen(byte direction)//0-left 1-right 2-down 3-up || Moves everything on the screen when the player crosses a boundary
        {
            if (ShiftCD == false)
            {
                if (direction == 1)//right
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {

                            Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) - 1900);
                        }



                    }
                    Animation(Canvas.GetLeft(player), 10, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);
                }
                else if (direction == 0)//left
                {
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {
                            Canvas.SetLeft(collCheck, Canvas.GetLeft(collCheck) + 1900);
                        }

                    }
                    Animation(Canvas.GetLeft(player), 1810, Canvas.GetBottom(player), Canvas.GetBottom(player), true, 0.2);

                }
                else if (direction == 2)//down
                {

                    Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), 820, true, 0.2);
                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {
                            Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) + 900);
                        }

                    }

                }
                else if (direction == 3)// up
                {

                    


                    foreach (var collCheck in MyCan.Children.OfType<Rectangle>())
                    {
                        if ((collCheck.Tag as string)[0] != '_')
                        {

                            if ((collCheck.Tag) as string == "Player")
                            {
                                
                            }
                            else
                            {
                                Canvas.SetBottom(collCheck, Canvas.GetBottom(collCheck) - 900);
                            }
                        }


                    }

                    DispatcherTimer Shift = new DispatcherTimer();
                    Shift.Interval = new TimeSpan(0, 0, 0, 0, 250);
                    Shift.Tick += (s, e) =>
                    {
                       
                        Animation(Canvas.GetLeft(player), Canvas.GetLeft(player), Canvas.GetBottom(player), 20, false, 0.01);
                        Shift.Stop();
                    };


                    Shift.Start();





                }
            }
        }





        void OpenInventory()
        {
            Inventory Inv = new Inventory(InventoryList);
            blockMove = true;
            Overlay = true;
            Inv.Show();

            Inv.Closing += (s, e) =>
            {
                EquipSlots(Inv.slot1, Inv.slot2);
            };


            Inv.Closed += (s, e) =>
            {
                blockMove = false;
                Overlay = false;
            };

        }

        public void EquipSlots(Item s1, Item s2)
        {
            if (s1 != null)
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

            Recipe CopperBar = new Recipe("Copper Bar", new Item(2, "Copper Bar", new Uri($"pack://application:,,,/Images/Icons/Copper Bar.png"), "Item"), 2, "copper", 2, "coal", 1, null, null, new Uri("pack://application:,,,/Images/Icons/Copper Bar.png"));
            Recipe IronBar = new Recipe("Iron Bar", new Item(2, "Iron Bar", new Uri($"pack://application:,,,/Images/Icons/Iron Bar.png"), "Item"), 2, "iron", 2, "coal", 3, null, null, new Uri("pack://application:,,,/Images/Icons/Iron Bar.png"));
            Recipe BronzeBar = new Recipe("Bronze Bar", new Item(2, "Bronze Bar", new Uri($"pack://application:,,,/Images/Icons/Bronze Bar.png"), "Item"), 2, "tin", 2, "copper", 2, "coal", 2, new Uri("pack://application:,,,/Images/Icons/Bronze Bar.png"));



            Recipe CopperPick = new Recipe("Copper Pick", new Item(1, "Copper Pick", new Uri($"pack://application:,,,/Images/Icons/copper pick.png"), "Tool"), 1, "copper bar", 5, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/copper pick.png"));
            Recipe CopperAxe = new Recipe("Copper Axe", new Item(1, "Copper Axe", new Uri($"pack://application:,,,/Images/Icons/copper axe.png"), "Tool"), 1, "copper bar", 3, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/copper axe.png"));
            Recipe BronzePick = new Recipe("Bronze Pick", new Item(1, "Bronze Pick", new Uri($"pack://application:,,,/Images/Icons/Bronze pick.png"), "Tool"), 1, "bronze bar", 5, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/Bronze pick.png"));
            Recipe BronzeAxe = new Recipe("Bronze Axe", new Item(1, "Bronze Axe", new Uri($"pack://application:,,,/Images/Icons/Bronze axe.png"), "Tool"), 1, "bronze bar", 3, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/Bronze axe.png"));
            Recipe IronPick = new Recipe("Iron Pick", new Item(1, "Iron Pick", new Uri($"pack://application:,,,/Images/Icons/Iron pick.png"), "Tool"), 1, "iron bar", 5, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/Iron pick.png"));
            Recipe IronAxe = new Recipe("Iron Axe", new Item(1, "Iron Axe", new Uri($"pack://application:,,,/Images/Icons/Iron axe.png"), "Tool"), 1, "iron bar", 3, "plank", 3, null, null, new Uri("pack://application:,,,/Images/Icons/Iron axe.png"));


            Recipes.Add(CopperBar);
            Recipes.Add(BronzeBar);
            Recipes.Add(IronBar);
            Recipes.Add(CopperPick);
            Recipes.Add(CopperAxe);
            Recipes.Add(BronzePick);
            Recipes.Add(BronzeAxe);
            Recipes.Add(IronPick);
            Recipes.Add(IronAxe);

        }

        public void OpenCraft()
        {

            Overlay = true;
            if (Recipes[0].Name == "ph")
            {
                Recipes.RemoveAt(0);
            }


            RecipesW RecipeWindow = new RecipesW(Recipes, InventoryList);




            RecipeWindow.Show();


            RecipeWindow.Closing += (s, e) =>
            {
                if (RecipeWindow.Crafted != null)
                {
                    foreach (Item i in RecipeWindow.Crafted)
                    {
                        AddToInventory(i);
                    }
                }
            };

            RecipeWindow.Closed += (s, e) =>
            {
                Overlay = false;
            };

        }


        public void CheckWin()
        {
            if((ASaphire == true)&& (ARuby == true) && (AEmerald == true) && (AOpal == true) && (ADiamond == true))
            {

                TimeSpan End = DateTime.Now.Subtract(Start);
                MessageBoxResult r = MessageBox.Show($"You won \n Time: {End.Hours}:{End.Minutes}:{End.Seconds}:{End.Milliseconds} \n Seed:{Seed} \n Would you like to copy the seed?","Congratulations!",MessageBoxButton.YesNo );
                if(r == MessageBoxResult.Yes)
                {
                    Clipboard.SetText(Seed);
                    Environment.Exit(1);
                }
                else
                {
                    Environment.Exit(1);
                }

                    
            }
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
           
            MessageBoxResult result = MessageBox.Show("Are you sure?","Exit",MessageBoxButton.YesNo, MessageBoxImage.Question);

            if(result == MessageBoxResult.Yes)
            {
                this.Close();

            }
            else if(result == MessageBoxResult.No)
            {
               
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {

            if(title == true)
            {
                Generate();
            }

            blockMove = false;
            title = false;
            Canvas.SetTop(TitleImg, 2000);
          
            Panel.SetZIndex(Title,-2);
            Exit = ExitBtn;
            Play = PlayBtn;

            MyCan.Children.Remove(PlayBtn);
            MyCan.Children.Remove(ExitBtn);
            MyCan.Children.Remove(SeedBtn);








        }

        private void Seed_Click(object sender, RoutedEventArgs e)
        {
            SeedW x = new SeedW();
            x.Show();


            x.Closing += (s, e) =>
            {
                Seed = x.seeed;

              

            };

        }
    }
}

