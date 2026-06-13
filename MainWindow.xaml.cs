
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FINALPOE
{ //  Represents an ingredient with name, calories, and food group
    public class Ingredient
    {
        public string Name { get; set; }
        public int Calories { get; set; }
        public string FoodGroup { get; set; }
    } 
    //  Represents a recipe with name, list of ingredients, and total calories calculation

    public class Recipe
    {
        public string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public int TotalCalories => Ingredients.Sum(i => i.Calories);
//  Method to add an ingredient to the recipe
        public void AddIngredient(Ingredient ingredient)
        {
            Ingredients.Add(ingredient);
        }
    }

    public partial class MainWindow : Window
    {
        private ObservableCollection<Recipe> recipes = new ObservableCollection<Recipe>();
        private Recipe selectedRecipe;

        public MainWindow()
        {
            InitializeComponent();
            LoadRecipes();
            RecipesDataGrid.ItemsSource = recipes;
//   Initialize food group combo box with predefined values
            if (FoodGroupComboBox != null)
            {
                FoodGroupComboBox.ItemsSource = new List<string> { "Vegetables", "Fruits", "Proteins", "Grains" };
            }
            else
            {
                MessageBox.Show("FoodGroupComboBox was not initialized correctly.");
            }
        }
// Load initial recipes into the collection
        private void LoadRecipes()
        {
            recipes.Add(new Recipe
            {
                Name = "Recipe 1",
                Ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "Ingredient A", Calories = 100, FoodGroup = "Vegetables" },
                    new Ingredient { Name = "Ingredient B", Calories = 200, FoodGroup = "Proteins" }
                }
            });

            recipes.Add(new Recipe
            {
                Name = "Recipe 2",
                Ingredients = new List<Ingredient>
                {
                    new Ingredient { Name = "Ingredient C", Calories = 150, FoodGroup = "Fruits" },
                    new Ingredient { Name = "Ingredient D", Calories = 300, FoodGroup = "Grains" }
                }
            });
        }
// Event handler for adding a new recipe
        private void AddRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            var newRecipe = new Recipe { Name = NewRecipeNameTextBox.Text };
            recipes.Add(newRecipe);
                   // Sort recipes alphabetically and update data grid

            var sortedRecipes = new ObservableCollection<Recipe>(recipes.OrderBy(r => r.Name));
            recipes.Clear();
            foreach (var recipe in sortedRecipes)
            {
                recipes.Add(recipe);
            }

            // Clear input text box after adding a recipe
            NewRecipeNameTextBox.Text = "";
        }
// Event handler for adding a new ingredient to a selected recipe
        private void AddIngredientButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe == null)
            {
                MessageBox.Show("Please select a recipe to add ingredients to.");
                return;
            }
       // Validate and add new ingredient to the selected recipe
            if (int.TryParse(NewIngredientCaloriesTextBox.Text, out int calories))
            {
                var newIngredient = new Ingredient
                {
                    Name = NewIngredientNameTextBox.Text,
                    Calories = calories,
                    FoodGroup = NewIngredientFoodGroupTextBox.Text
                };
                selectedRecipe.AddIngredient(newIngredient);

                // Clear input fields after adding ingredient
                NewIngredientNameTextBox.Text = "";
                NewIngredientCaloriesTextBox.Text = "";
                NewIngredientFoodGroupTextBox.Text = "";
// Update ingredients data grid and display total calories
                IngredientsDataGrid.ItemsSource = new ObservableCollection<Ingredient>(selectedRecipe.Ingredients);

                UpdateTotalCalories();
                // Display warning if total calories exceed 300

                if (selectedRecipe.TotalCalories > 300)
                {
                    MessageBox.Show("Warning: Total calories exceed 300!");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid number for calories.");
            }
        }
// Event handler for selecting a recipe in the data grid
        private void RecipesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRecipe = RecipesDataGrid.SelectedItem as Recipe;
            if (selectedRecipe != null)
            {
                IngredientsDataGrid.ItemsSource = new ObservableCollection<Ingredient>(selectedRecipe.Ingredients);
                UpdateTotalCalories();
            }
        }
// Update ingredients data grid and display total calories
        private void UpdateTotalCalories()
        {
            TotalCaloriesTextBlock.Text = $"Total Calories: {selectedRecipe?.TotalCalories}";
        }
        // Update total calories text block with current selected recipe's total calories

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filteredRecipes = recipes;
                        // Event handler for filtering recipes based on ingredient name, food group, and maximum calories


                if (!string.IsNullOrWhiteSpace(IngredientTextBox.Text))
                {
                    filteredRecipes = new ObservableCollection<Recipe>(filteredRecipes.Where(r =>
                        r.Ingredients.Any(i => i.Name.Contains(IngredientTextBox.Text))));
                }

                if (FoodGroupComboBox.SelectedItem != null)
                                // Filter recipes based on input criteria

                {
                    var selectedFoodGroup = FoodGroupComboBox.SelectedItem.ToString();
                    filteredRecipes = new ObservableCollection<Recipe>(filteredRecipes.Where(r => r.Ingredients
                        .Any(i => i.FoodGroup.Equals(selectedFoodGroup))));
                }

                filteredRecipes = new ObservableCollection<Recipe>(filteredRecipes
                    .Where(r => r.TotalCalories <= (int)CaloriesSlider.Value));

                RecipesDataGrid.ItemsSource = filteredRecipes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IngredientTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                FilterButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewRecipeButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRecipe != null)
            {
                MessageBox.Show($"Recipe: {selectedRecipe.Name}\nTotal Calories: {selectedRecipe.TotalCalories}\nIngredients:\n" +
                                string.Join("\n", selectedRecipe.Ingredients.Select(i => $"{i.Name} - {i.Calories} calories ({i.FoodGroup})")));
            }
            else
            {
                MessageBox.Show("Please select a recipe to view details.");
            }
        }

        private void NewRecipeNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NewRecipeNameTextBox.Clear();
        }

        private void NewIngredientNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NewIngredientNameTextBox.Clear();
        }

        private void NewIngredientCaloriesTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NewIngredientCaloriesTextBox.Clear();
        }

        private void NewIngredientFoodGroupTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            NewIngredientFoodGroupTextBox.Clear();
        }
    }
}
