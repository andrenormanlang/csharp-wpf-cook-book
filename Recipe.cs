namespace CookBook
{
    public class Recipe
    {
        private string name;
        private FoodCategory category;
        private string[] ingredients;
        private string instructions;
        private int numOfIngredients;
        private readonly int maxIngredients;

        public Recipe(int maxIngredients)
        {
            this.maxIngredients = maxIngredients;
            ingredients = new string[maxIngredients];
            numOfIngredients = 0;
            instructions = string.Empty;
        }

        // Copy constructor
        public Recipe(Recipe other)
        {
            this.maxIngredients = other.maxIngredients;
            this.ingredients = new string[maxIngredients];
            Array.Copy(other.ingredients, this.ingredients, other.ingredients.Length);
            this.numOfIngredients = other.numOfIngredients;
            this.name = other.name;
            this.category = other.category;
            this.instructions = other.instructions;
        }

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

        public FoodCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Instructions
        {
            get { return instructions; }
            set { instructions = value ?? string.Empty; }
        }

        public bool AddIngredient(string ingredient)
        {
            if (numOfIngredients >= maxIngredients)
                return false;

            if (string.IsNullOrWhiteSpace(ingredient))
                return false;

            ingredients[numOfIngredients++] = ingredient.Trim();
            return true;
        }

        public string[] GetIngredients()
        {
            string[] result = new string[numOfIngredients];
            Array.Copy(ingredients, result, numOfIngredients);
            return result;
        }

        public int NumOfIngredients
        {
            get { return numOfIngredients; }
        }

        public int MaxIngredients
        {
            get { return maxIngredients; }
        }

        public bool HasIngredients()
        {
            return numOfIngredients > 0;
        }

        public void ClearIngredients()
        {
            Array.Clear(ingredients, 0, ingredients.Length);
            numOfIngredients = 0;
        }

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

        // Method to update an ingredient by index
        public bool UpdateIngredient(int index, string newIngredient)
        {
            if (index < 0 || index >= numOfIngredients || string.IsNullOrWhiteSpace(newIngredient))
                return false;

            ingredients[index] = newIngredient.Trim();
            return true;
        }
    }

    public class RecipeManager
    {
        private readonly Recipe[] recipeList;
        private int numOfElems;

        public RecipeManager(int maxRecipes)
        {
            recipeList = new Recipe[maxRecipes];
            numOfElems = 0;
        }

        public int NumOfRecipes
        {
            get { return numOfElems; }
        }

        public bool AddRecipe(Recipe recipe)
        {
            if (numOfElems >= recipeList.Length)
                return false;

            if (recipe == null)
                return false;

            recipeList[numOfElems] = new Recipe(recipe); // Create a copy
            numOfElems++;
            return true;
        }

        public Recipe GetRecipe(int index)
        {
            if (index < 0 || index >= numOfElems)
                return null;

            return recipeList[index];
        }

        public void RemoveRecipe(int index)
        {
            if (index < 0 || index >= numOfElems)
                return;

            recipeList[index] = null;
            MoveElementsOneStepToLeft(index);
            numOfElems--;
        }

        private void MoveElementsOneStepToLeft(int index)
        {
            for (int i = index + 1; i < recipeList.Length; i++)
            {
                recipeList[i - 1] = recipeList[i];
                recipeList[i] = null;
            }
        }
    }
}
