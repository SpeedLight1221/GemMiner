using System;
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
        public RecipesW(List<Recipe> R)
        {
            InitializeComponent();
            Recipes = R;
            RecList.ItemsSource = Recipes;


        }

        private void Craft_Click(object sender, RoutedEventArgs e)
        {
            Recipe Tocraft = null;

            foreach(Recipe recipe in Recipes)
            {
                if(recipe.Name == ((sender as Button).Tag as string)) 
                {
                    Tocraft = recipe;
                    break;
                }
            }

            string r1 = Tocraft.Res1;
            int a1 = Tocraft.Amount1;


            string r2 = Tocraft.Res2;
            int? a2 = Tocraft.Amount2;



        }
    }
}
