﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>
    /// Interakční logika pro RecipesW.xaml
    /// </summary>
    public partial class RecipesW : Window
    {
        public List<Recipe> Recipes;
        public List<Item> InventoryCraft;
        public List<Item> Crafted = new List<Item>();
        public RecipesW(List<Recipe> R, List<Item> i)
        {
           

            InitializeComponent();
            Recipes = R;
            RecList.ItemsSource = Recipes;
            InventoryCraft = i;
            

        }


        

        private void Craft_Click(object sender, RoutedEventArgs e)
        {
            Recipe Tocraft = null; 
            
            foreach (Recipe recipe in Recipes) // picks which recipe you want to use
            {
                if (recipe.Name == ((sender as Button).Tag as string))
                {
                    Tocraft = recipe;
                    
                    break;
                }
                
            }
            
            // prepares required resources and amounts
            string r1 = Tocraft.Res1;
            int a1 = Tocraft.Amount1;


            string r2 = Tocraft.Res2;
            int? a2 = Tocraft.Amount2;

            string r3 = Tocraft.Res3;
            int? a3 = Tocraft.Amount3;

           


            //bools to check if you have the required resources
            bool i1 = false;
            bool i2 = false;
            bool i3 = false;


            string x = "";
            foreach (Item i in InventoryCraft)
            {
                x += " " + i.Name.ToLower();
                if ((r1 == i.Name.ToLower()) && (a1 <= i.Amount)) //resource one check
                {
                    i1 = true;
                  
                }
                


                if (r2 != null) //resource two check
                {
                   
                    if ((r2 == i.Name.ToLower()) && (a2 <= i.Amount))
                    {
                        i2 = true;
                    }
                    
                }
                else
                {
                    i2 = true;
                }


                if (r3 != null)// resource three check
                {
                    if ((r3 == i.Name.ToLower()) && (a3 <= i.Amount))
                    {
                        i3 = true;
                    }
                    
                }
                else
                {
                    i3 = true;
                }


            }

            //MessageBox.Show(r1 + " " + r2 + " " + r3 + "\n" + x);
         
            List<Item> toremove = new List<Item>();
            if ((i1) && (i3) && (i2)) // if you have all resources
            {
               
                foreach(Item i in InventoryCraft)
                {
                    if(i.Name.ToLower() == r1)
                    {
                        
                        i.Amount -= a1;
                        if(i.Amount == 0)
                        {
                            toremove.Add(i);
                        }
                    }

                    if (i.Name.ToLower() == r2)
                    {
                        
                        i.Amount -= (int)a2;
                        if (i.Amount == 0)
                        {
                            toremove.Add(i);
                        }
                    }

                    if (i.Name.ToLower() == r3)
                    {
                        i.Amount -= (int)a3;
                        if (i.Amount == 0)
                        {
                            toremove.Add(i);
                        }
                    }
                }

                string t = "";
                foreach(Item c in toremove) //removes items from inventory if their amount after crafting reaches 0
                {
                    t += " " + c.Name;
                    InventoryCraft.Remove(c);
                }
               // MessageBox.Show(t);



                Crafted.Add(Tocraft.Output);
                
                
            }





        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.C)|| (e.Key == Key.Escape))
            {
                this.Close();
            }
        }
    }
}
