using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media.Imaging;

namespace CookBook
{
    /// <summary>
    /// The main window of the application, which handles user interactions for managing recipes.
    /// </summary>
    public partial class MainWindow : Window
    {
        // Constants as per requirements
        private const int MaxNumOfIngredients = 50;
        private const int MaxNumOfRecipes = 200;

        // Class-level variables
        private Recipe currRecipe;
        private readonly RecipeManager recipeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class and sets up the user interface.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize the recipe manager
            recipeManager = new RecipeManager();

            // Initialize current recipe
            currRecipe = new Recipe(MaxNumOfIngredients);

            // Populate the category combo box with enum values
            cmbCategory.ItemsSource = Enum.GetValues(typeof(FoodCategory));
            cmbCategory.SelectedIndex = 0;

            // Set the ListBox's DisplayMemberPath to show recipe names
            lstRecipes.DisplayMemberPath = "Name";

            // Load existing recipes into the ListBox
            RefreshRecipeList();
        }


        /// <summary>
        /// Handles the click event for adding ingredients to the current recipe.
        /// </summary>
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

        /// <summary>
        /// Handles category selection changes. This event is auto-handled by the ComboBox in the constructor.
        /// </summary>
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles text changes in the recipe name field.
        /// </summary>
        private void txtRecipeName_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the click event for adding the current recipe to the recipe manager.
        /// </summary>
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

                // Clear the form and create a new current recipe
                ClearForm();
                currRecipe = new Recipe(MaxNumOfIngredients);
            }
            else
            {
                MessageBox.Show("Cannot add more recipes. Maximum limit reached.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the selection change event for the recipe ListBox, updating the display with the selected recipe details.
        /// </summary>
        private void RecipeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstRecipes.SelectedIndex >= 0)
            {
                var selectedRecipe = recipeManager.GetRecipe(lstRecipes.SelectedIndex);
                if (selectedRecipe != null)
                {
                    // Display ingredients and instructions in the text block
                    txtInstructions.Text = $"INGREDIENTS\n" +
                                           $"{string.Join("\n", selectedRecipe.GetIngredients())}\n\n" +
                                           $"INSTRUCTIONS\n" +
                                           $"{selectedRecipe.Instructions}";

                    // Load the image if available
                    if (selectedRecipe.ImageData != null)
                    {
                        using (var ms = new MemoryStream(selectedRecipe.ImageData))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = ms;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            imgRecipeImage.Source = bitmap;
                        }
                    }
                    else
                    {
                        imgRecipeImage.Source = null; // Set to null if no image is present
                    }
                }
            }
        }



        /// <summary>
        /// Handles the click event for changing the selected recipe in the ListBox.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for deleting the selected recipe from the ListBox.
        /// </summary>
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

        /// <summary>
        /// Handles the click event for clearing the form inputs.
        /// </summary>
        private void ClearInputButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            currRecipe = new Recipe(MaxNumOfIngredients);
        }

        /// <summary>
        /// Handles the click event for adding an image to the recipe (currently not implemented).
        /// </summary>
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Convert image to byte[]
                currRecipe.ImageData = File.ReadAllBytes(filePath);

                // Load the image into the UI element
                imgRecipeImage.Source = new BitmapImage(new Uri(filePath));
            }
        }

        /// <summary>
        /// Clears the form input fields.
        /// </summary>
        private void ClearForm()
        {
            txtRecipeName.Clear();
            cmbCategory.SelectedIndex = 0;
            txtInstructions.Text = "Instructions will be displayed here...";
            lstRecipes.SelectedIndex = -1;
        }

        /// <summary>
        /// Refreshes the recipe list by reloading all recipes from the manager into the ListBox.
        /// </summary>
        private void RefreshRecipeList()
        {
            lstRecipes.Items.Clear();
            int numOfRecipes = recipeManager.GetNumOfRecipes(); // Get the number of recipes from the database

            for (int i = 0; i < numOfRecipes; i++)
            {
                var recipe = recipeManager.GetRecipe(i); // Retrieve each recipe by index
                if (recipe != null)
                {
                    lstRecipes.Items.Add(recipe);
                }
            }
        }

    }

    /// <summary>
    /// Enumeration representing different categories of food for recipes.
    /// </summary>
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

