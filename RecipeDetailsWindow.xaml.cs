using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;

namespace CookBook
{
    /// <summary>
    /// Interaction logic for FormRecipeDetails.xaml
    /// </summary>
    public partial class FormRecipeDetails : Window
    {
        private Recipe recipe;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormRecipeDetails"/> class with a copy of the current recipe.
        /// </summary>
        /// <param name="currentRecipe">The recipe to be edited or viewed.</param>
        public FormRecipeDetails(Recipe currentRecipe)
        {
            InitializeComponent();

            // Create a copy of the passed recipe
            recipe = new Recipe(currentRecipe);

            // Set the window title with the recipe name
            UpdateWindowTitle();

            InitializeFormData();
        }

        /// <summary>
        /// Gets the edited recipe from the form.
        /// </summary>
        public Recipe Recipe
        {
            get { return recipe; }
        }

        /// <summary>
        /// Updates the recipe instructions whenever the text changes.
        /// </summary>
        private void txtInstructions_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtInstructions != null)
            {
                recipe.Instructions = txtInstructions.Text;
            }
        }

        /// <summary>
        /// Updates the window title with the recipe name.
        /// </summary>
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

        /// <summary>
        /// Initializes the form data with the current recipe details.
        /// </summary>
        private void InitializeFormData()
        {
            txtInstructions.Text = recipe.Instructions;
            RefreshIngredientsList();
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        /// <summary>
        /// Refreshes the ingredients list with the current recipe ingredients.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for adding a new ingredient to the recipe.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for editing the selected ingredient.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for deleting the selected ingredient from the recipe.
        /// </summary>
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

        /// <summary>
        /// Handles the selection changed event in the ingredients list, enabling or disabling the edit and delete buttons.
        /// </summary>
        private void lstIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool isItemSelected = lstIngredients.SelectedItem != null;
            btnEdit.IsEnabled = isItemSelected;
            btnDelete.IsEnabled = isItemSelected;
        }

        /// <summary>
        /// Handles the click event for confirming the recipe details and saving the instructions.
        /// </summary>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
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

            recipe.Instructions = txtInstructions.Text;
            this.DialogResult = true;
        }

        /// <summary>
        /// Handles the click event for canceling the form and discarding changes.
        /// </summary>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
