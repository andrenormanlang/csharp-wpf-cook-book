using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CookBook
{
    public partial class MainWindow : Window
    {
        // Constants as per requirements
        private const int MaxNumOfIngredients = 50;
        private const int MaxNumOfRecipes = 200;

        // Class level variables
        private Recipe currRecipe;
        private readonly RecipeManager recipeManager;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the recipe manager with max capacity
            recipeManager = new RecipeManager(MaxNumOfRecipes);

            // Initialize current recipe
            currRecipe = new Recipe(MaxNumOfIngredients);

            // Populate the category combo box with enum values
            cmbCategory.ItemsSource = Enum.GetValues(typeof(FoodCategory));
            cmbCategory.SelectedIndex = 0;

            // Set the ListBox's DisplayMemberPath to show recipe names
            lstRecipes.DisplayMemberPath = "Name";
        }

        private void AddIngredientsButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate recipe name
            if (string.IsNullOrWhiteSpace(txtRecipeName.Text))
            {
                MessageBox.Show("Please enter a recipe name first.", "Input Required",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Update current recipe with form data
            currRecipe.Name = txtRecipeName.Text;
            currRecipe.Category = (FoodCategory)cmbCategory.SelectedItem;

            // Create and show the ingredients form
            var detailsForm = new FormRecipeDetails(currRecipe);

            if (detailsForm.ShowDialog() == true)
            {
                // Update current recipe with the modified recipe from details form
                currRecipe = detailsForm.Recipe;
            }
        }

        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This event is handled automatically populated by the ComboBox in constructor
        }

        private void txtRecipeName_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Add validation here if needed
        }

        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currRecipe.Name))
            {
                MessageBox.Show("Please enter a recipe name.", "Input Required",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!currRecipe.HasIngredients())
            {
                MessageBox.Show("Please add ingredients to the recipe.", "Input Required",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Add the recipe to the manager
            if (recipeManager.AddRecipe(currRecipe))
            {
                // Update the ListBox
                RefreshRecipeList();

                // Clear the form and create new current recipe
                ClearForm();
                currRecipe = new Recipe(MaxNumOfIngredients);
            }
            else
            {
                MessageBox.Show("Cannot add more recipes. Maximum limit reached.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RecipeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRecipes.SelectedIndex >= 0)
            {
                var selectedRecipe = recipeManager.GetRecipe(lstRecipes.SelectedIndex);
                if (selectedRecipe != null)
                {
                    txtInstructions.Text = $"INGREDIENTS\n" +
                                           $"{string.Join(", ", selectedRecipe.GetIngredients())}\n\n" +
                                           $"INSTRUCTIONS\n" +
                                           $"{selectedRecipe.Instructions}";
                }
            }
        }


        private void ChangeRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (lstRecipes.SelectedIndex >= 0)
            {
                var selectedRecipe = recipeManager.GetRecipe(lstRecipes.SelectedIndex);
                if (selectedRecipe != null)
                {
                    // Load the selected recipe into the form
                    currRecipe = new Recipe(selectedRecipe); // Use copy constructor
                    txtRecipeName.Text = currRecipe.Name;
                    cmbCategory.SelectedItem = currRecipe.Category;
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe to edit.", "Selection Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (lstRecipes.SelectedIndex >= 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this recipe?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    recipeManager.RemoveRecipe(lstRecipes.SelectedIndex);
                    RefreshRecipeList();
                    ClearForm();
                }
            }
            else
            {
                MessageBox.Show("Please select a recipe to delete.", "Selection Required",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearInputButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            currRecipe = new Recipe(MaxNumOfIngredients);
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Optional: Implement image handling if required
            MessageBox.Show("Image handling functionality can be implemented here.",
                "Feature Notice", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ClearForm()
        {
            txtRecipeName.Clear();
            cmbCategory.SelectedIndex = 0;
            txtInstructions.Text = "Instructions will be displayed here...";
            lstRecipes.SelectedIndex = -1;
        }

        private void RefreshRecipeList()
        {
            lstRecipes.Items.Clear();
            for (int i = 0; i < recipeManager.NumOfRecipes; i++)
            {
                var recipe = recipeManager.GetRecipe(i);
                if (recipe != null)
                {
                    lstRecipes.Items.Add(recipe);
                }
            }
        }
    }

    public enum FoodCategory
    {
        Meat,
        Pasta,
        Pizza,
        Fish,
        Seafood,
        Soups,
        Stew,
        Vegan,
        Vegetarian,
        Other
    }
}

