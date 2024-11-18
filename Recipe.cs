using System;
using System.Linq;

namespace CookBook
{
    /// <summary>
    /// Represents a recipe with a name, category, ingredients, and instructions.
    /// </summary>
    public class Recipe
    {
        // Properties for the Recipe
        public int Id { get; set; }
        private string name;
        private FoodCategory category;
        private string[] ingredients;
        private string instructions;
        private int numOfIngredients;
        private readonly int maxIngredients;
        private string imagePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Recipe"/> class with a specified maximum number of ingredients.
        /// </summary>
        /// <param name="maxIngredients">The maximum number of ingredients allowed for the recipe.</param>
        public Recipe(int maxIngredients)
        {
            this.maxIngredients = maxIngredients;
            ingredients = new string[maxIngredients];
            numOfIngredients = 0;
            instructions = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Recipe"/> class by copying an existing recipe.
        /// </summary>
        /// <param name="other">The recipe to copy.</param>
        public Recipe(Recipe other)
        {
            this.Id = other.Id;
            this.maxIngredients = other.maxIngredients;
            this.ingredients = new string[maxIngredients];
            Array.Copy(other.ingredients, this.ingredients, other.numOfIngredients);
            this.numOfIngredients = other.numOfIngredients;
            this.name = other.name;
            this.category = other.category;
            this.instructions = other.instructions;
            this.ImageData = other.ImageData;
            this.imagePath = other.imagePath;
        }

        /// <summary>
        /// Gets or sets the name of the recipe.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the recipe name is empty or whitespace.</exception>
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Recipe name cannot be empty");
                name = value.Trim();
            }
        }

        /// <summary>
        /// Gets or sets the category of the recipe.
        /// </summary>
        public FoodCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// Gets or sets the instructions for the recipe.
        /// </summary>
        public string Instructions
        {
            get { return instructions; }
            set { instructions = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the binary data of the image associated with the recipe.
        /// </summary>
        public byte[]? ImageData { get; set; }

        /// <summary>
        /// Adds a new ingredient to the recipe.
        /// </summary>
        /// <param name="ingredient">The ingredient to add.</param>
        /// <returns>True if the ingredient was added; false if the recipe is full or the ingredient is invalid.</returns>
        public bool AddIngredient(string ingredient)
        {
            if (numOfIngredients >= maxIngredients)
                return false;

            if (string.IsNullOrWhiteSpace(ingredient))
                return false;

            ingredients[numOfIngredients++] = ingredient.Trim();
            return true;
        }

        /// <summary>
        /// Gets the list of ingredients in the recipe.
        /// </summary>
        /// <returns>An array of ingredients currently in the recipe.</returns>
        public string[] GetIngredients()
        {
            string[] result = new string[numOfIngredients];
            Array.Copy(ingredients, result, numOfIngredients);
            return result;
        }

        /// <summary>
        /// Gets the number of ingredients currently in the recipe.
        /// </summary>
        public int NumOfIngredients => numOfIngredients;

        /// <summary>
        /// Gets the maximum number of ingredients allowed in the recipe.
        /// </summary>
        public int MaxIngredients => maxIngredients;

        /// <summary>
        /// Determines whether the recipe has any ingredients.
        /// </summary>
        /// <returns>True if the recipe has at least one ingredient; otherwise, false.</returns>
        public bool HasIngredients() => numOfIngredients > 0;

        /// <summary>
        /// Clears all ingredients from the recipe.
        /// </summary>
        public void ClearIngredients()
        {
            Array.Clear(ingredients, 0, ingredients.Length);
            numOfIngredients = 0;
        }

        /// <summary>
        /// Removes an ingredient from the recipe by its index.
        /// </summary>
        /// <param name="index">The index of the ingredient to remove.</param>
        /// <returns>True if the ingredient was removed; false if the index is invalid.</returns>
        public bool RemoveIngredient(int index)
        {
            if (index < 0 || index >= numOfIngredients)
                return false;

            for (int i = index; i < numOfIngredients - 1; i++)
            {
                ingredients[i] = ingredients[i + 1];
            }
            ingredients[numOfIngredients - 1] = null;
            numOfIngredients--;
            return true;
        }

        /// <summary>
        /// Updates an ingredient at the specified index with a new value.
        /// </summary>
        /// <param name="index">The index of the ingredient to update.</param>
        /// <param name="newIngredient">The new ingredient value.</param>
        /// <returns>True if the ingredient was updated; false if the index is invalid or the new ingredient is empty.</returns>
        public bool UpdateIngredient(int index, string newIngredient)
        {
            if (index < 0 || index >= numOfIngredients || string.IsNullOrWhiteSpace(newIngredient))
                return false;

            ingredients[index] = newIngredient.Trim();
            return true;
        }

        /// <summary>
        /// Gets or sets the path of the image associated with the recipe.
        /// </summary>
        public string ImagePath
        {
            get { return imagePath; }
            set { imagePath = value; }
        }

        /// <summary>
        /// Generates a string representation of the recipe, including its name and category.
        /// </summary>
        /// <returns>A string representing the recipe's name and category.</returns>
        public override string ToString()
        {
            return $"{Name} ({Category})";
        }
    }
}

