using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic; 

namespace CookBook
{
    public partial class FormRecipeDetails : Window
    {
        private Recipe recipe;

        public FormRecipeDetails(Recipe currentRecipe)
        {
            InitializeComponent();

            // Create a copy of the passed recipe
            recipe = new Recipe(currentRecipe);

            // Set the window title with recipe name
            UpdateWindowTitle();

            InitializeFormData();
        }

        private void UpdateWindowTitle()
        {
            if (!string.IsNullOrEmpty(recipe.Name))
            {
                this.Title = $"{recipe.Name} -- add ingredients";
            }
            else
            {
                this.Title = "Add ingredients";
            }
        }

        public Recipe Recipe
        {
            get { return recipe; }
        }

        private void InitializeFormData()
        {
            txtInstructions.Text = recipe.Instructions;
            RefreshIngredientsList();
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void RefreshIngredientsList()
        {
            lstIngredients.Items.Clear();
            foreach (string ingredient in recipe.GetIngredients())
            {
                if (!string.IsNullOrEmpty(ingredient))
                {
                    lstIngredients.Items.Add(ingredient);
                }
            }
            txtIngredientCount.Text = recipe.NumOfIngredients.ToString();
        }

        private void AddIngredient_Click(object sender, RoutedEventArgs e)
        {
            string ingredient = txtIngredient.Text.Trim();

            if (string.IsNullOrWhiteSpace(ingredient))
            {
                MessageBox.Show("Please enter an ingredient.", "Input Required",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (recipe.AddIngredient(ingredient))
            {
                RefreshIngredientsList();
                txtIngredient.Clear();
                txtIngredient.Focus();
            }
            else
            {
                MessageBox.Show("Maximum number of ingredients reached.", "Cannot Add Ingredient",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (lstIngredients.SelectedItem == null)
            {
                MessageBox.Show("Please select an ingredient to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedIngredient = lstIngredients.SelectedItem.ToString();
            string newIngredient = Interaction.InputBox("Edit Ingredient:", "Edit Ingredient", selectedIngredient);

            if (!string.IsNullOrWhiteSpace(newIngredient))
            {
                int selectedIndex = lstIngredients.SelectedIndex;
                if (recipe.UpdateIngredient(selectedIndex, newIngredient))
                {
                    RefreshIngredientsList();
                }
                else
                {
                    MessageBox.Show("Failed to update the ingredient.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Ingredient cannot be empty.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (lstIngredients.SelectedItem == null)
            {
                MessageBox.Show("Please select an ingredient to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string selectedIngredient = lstIngredients.SelectedItem.ToString();
            var result = MessageBox.Show($"Are you sure you want to delete '{selectedIngredient}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                int selectedIndex = lstIngredients.SelectedIndex;
                if (recipe.RemoveIngredient(selectedIndex))
                {
                    RefreshIngredientsList();
                }
                else
                {
                    MessageBox.Show("Failed to delete the ingredient.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void lstIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = lstIngredients.SelectedItem != null;
            btnEdit.IsEnabled = isItemSelected;
            btnDelete.IsEnabled = isItemSelected;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Validate that we have at least one ingredient
            if (!recipe.HasIngredients())
            {
                var result = MessageBox.Show(
                    "No ingredients added. Do you want to continue adding ingredients?",
                    "Missing Ingredients",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    
                    txtIngredient.Focus();
                    return;
                }
            }

            // Save instructions
            recipe.Instructions = txtInstructions.Text;

            
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

