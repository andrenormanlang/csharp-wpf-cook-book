using MySql.Data.MySqlClient;

namespace CookBook
{
    internal class RecipeManager
    {
        private readonly string connectionString = "server=localhost;port=3307;user=root;password=root;database=cookbook_db;";

        public RecipeManager() { }

        /// <summary>
        /// Gets the number of recipes stored in the database.
        /// </summary>
        public int GetNumOfRecipes()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sqlCount = "SELECT COUNT(*) FROM recipes";
                    using (MySqlCommand cmd = new MySqlCommand(sqlCount, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return 0; // Return 0 if there is an error
                }
            }
        }


        public bool AddRecipe(Recipe recipe)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sqlRecipe = "INSERT INTO recipes (name, category, instructions, image) VALUES (@name, @category, @instructions, @image)";
                    using (MySqlCommand cmd = new MySqlCommand(sqlRecipe, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", recipe.Name);
                        cmd.Parameters.AddWithValue("@category", recipe.Category.ToString());
                        cmd.Parameters.AddWithValue("@instructions", recipe.Instructions);
                        cmd.Parameters.AddWithValue("@image", recipe.ImageData);
                        cmd.ExecuteNonQuery();

                        long recipeId = cmd.LastInsertedId;

                        foreach (string ingredient in recipe.GetIngredients())
                        {
                            string sqlIngredient = "INSERT INTO ingredients (recipe_id, ingredient) VALUES (@recipe_id, @ingredient)";
                            using (MySqlCommand ingredientCmd = new MySqlCommand(sqlIngredient, conn))
                            {
                                ingredientCmd.Parameters.AddWithValue("@recipe_id", recipeId);
                                ingredientCmd.Parameters.AddWithValue("@ingredient", ingredient);
                                ingredientCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }

        public Recipe GetRecipe(int index)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Retrieve a recipe using LIMIT and offset
                    string sqlRecipe = "SELECT * FROM recipes LIMIT @offset, 1";
                    using (MySqlCommand cmd = new MySqlCommand(sqlRecipe, conn))
                    {
                        cmd.Parameters.AddWithValue("@offset", index);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Recipe recipe = new Recipe(50)
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Name = reader["name"].ToString(),
                                    Category = (FoodCategory)Enum.Parse(typeof(FoodCategory), reader["category"].ToString()),
                                    Instructions = reader["instructions"].ToString(),
                                    ImageData = reader["image"] != DBNull.Value ? (byte[])reader["image"] : null
                                };

                                reader.Close(); // Close reader to allow for a new query

                                // Load ingredients after recipe is created
                                LoadIngredientsForRecipe(recipe, conn);

                                return recipe;
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }


        /// <summary>
        /// Updates an existing recipe in the database.
        /// </summary>
        /// <param name="recipe">The updated recipe object.</param>
        /// <returns>True if the recipe was updated successfully; otherwise false.</returns>
       public bool UpdateRecipe(Recipe recipe)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Update the recipe
                    string sqlUpdateRecipe = "UPDATE recipes SET name = @name, category = @category, instructions = @instructions, image = @image WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlUpdateRecipe, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", recipe.Name);
                        cmd.Parameters.AddWithValue("@category", recipe.Category.ToString());
                        cmd.Parameters.AddWithValue("@instructions", recipe.Instructions);
                        cmd.Parameters.AddWithValue("@image", recipe.ImageData);
                        cmd.Parameters.AddWithValue("@id", recipe.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Remove old ingredients
                    string sqlDeleteIngredients = "DELETE FROM ingredients WHERE recipe_id = @recipe_id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlDeleteIngredients, conn))
                    {
                        cmd.Parameters.AddWithValue("@recipe_id", recipe.Id);
                        cmd.ExecuteNonQuery();
                    }

                    // Add updated ingredients
                    foreach (string ingredient in recipe.GetIngredients())
                    {
                        string sqlInsertIngredient = "INSERT INTO ingredients (recipe_id, ingredient) VALUES (@recipe_id, @ingredient)";
                        using (MySqlCommand cmd = new MySqlCommand(sqlInsertIngredient, conn))
                        {
                            cmd.Parameters.AddWithValue("@recipe_id", recipe.Id);
                            cmd.Parameters.AddWithValue("@ingredient", ingredient);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        private void LoadIngredientsForRecipe(Recipe recipe, MySqlConnection conn)
        {
            string sqlIngredients = "SELECT ingredient FROM ingredients WHERE recipe_id = @recipe_id";
            using (MySqlCommand cmdIngredients = new MySqlCommand(sqlIngredients, conn))
            {
                cmdIngredients.Parameters.AddWithValue("@recipe_id", recipe.Id);
                using (MySqlDataReader reader = cmdIngredients.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        recipe.AddIngredient(reader["ingredient"].ToString());
                    }
                }
            }
        }

        public void RemoveRecipe(int recipeId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sqlDeleteIngredients = "DELETE FROM ingredients WHERE recipe_id = @recipe_id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlDeleteIngredients, conn))
                    {
                        cmd.Parameters.AddWithValue("@recipe_id", recipeId);
                        cmd.ExecuteNonQuery();
                    }

                    string sqlDeleteRecipe = "DELETE FROM recipes WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlDeleteRecipe, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", recipeId);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}

