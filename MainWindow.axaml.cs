using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class MainWindow : Window
    {
        private int[] _generatedArray;
        private WrapPanel _arrayVisualizationPanel;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            InitializeUI();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var searchTextBox = this.FindControl<TextBox>("SearchTextBox");
            var searchButton = this.FindControl<Button>("SearchButton");
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
            var searchMethodComboBox = this.FindControl<ComboBox>("SearchMethodComboBox");
            var firstHundredTextBlock = this.FindControl<TextBlock>("FirstHundredTextBlock");
            var inputPromptTextBlock = this.FindControl<TextBlock>("InputPromptTextBlock");
            var numberOfElementsComboBox = this.FindControl<ComboBox>("NumberOfElementsComboBox");
            var generateArrayButton = this.FindControl<Button>("GenerateArrayButton");
            _arrayVisualizationPanel = this.FindControl<WrapPanel>("ArrayVisualizationPanel");

            inputPromptTextBlock.Text = "Enter the number of elements to generate:";

            // Populate the ComboBox with options
            var numberOfElementsOptions = new List<int> { 1, 10, 50, 100, 500, 1000, 5000, 10000, 20000, 50000, 100000 };
            foreach (var option in numberOfElementsOptions)
            {
                numberOfElementsComboBox.Items.Add(option.ToString());
            }

            generateArrayButton.Click += (sender, e) =>
            {
                try
                {
                    // Parse the selected number of elements to generate
                    int numberOfElements = int.Parse((string)numberOfElementsComboBox.SelectedItem);

                    // Generate the array with the specified number of elements
                    _generatedArray = ArrayGenerator.GenerateArray(numberOfElements);
                    Array.Sort(_generatedArray);

                    // Display the first 100 elements of the generated array
                    DisplayArray();
                }
                catch (Exception ex)
                {
                    resultTextBlock.Text = $"Error: {ex.Message}";
                }
            };


            searchButton.Click += async (sender, e) =>
            {
                try
                {
                    int searchValue = int.Parse(searchTextBox.Text);

                    if (_generatedArray == null)
                    {
                        resultTextBlock.Text = "Please generate the array first.";
                        return;
                    }

                    string selectedSearchMethod = ((ComboBoxItem)searchMethodComboBox.SelectedItem).Content.ToString();
                    int searchIndex = -1;
                    int comparisons = 0;

                    switch (selectedSearchMethod)
                    {
                        case "Sequential Search":
                            (searchIndex, comparisons) = await SearchWithProgress(SearchAlgorithms.SequentialSearch, _generatedArray, searchValue);
                            break;
                        case "Fibonacci Search":
                            (searchIndex, comparisons) = await SearchWithProgress(SearchAlgorithms.FibonacciSearch, _generatedArray, searchValue);
                            break;
                        case "Interpolation Search":
                            (searchIndex, comparisons) = await SearchWithProgress(SearchAlgorithms.InterpolationSearch, _generatedArray, searchValue);
                            break;
                        case "Hash Search":
                            (searchIndex, comparisons) = await SearchWithProgress(SearchAlgorithms.HashSearch, _generatedArray, searchValue);
                            break;
                        default:
                            resultTextBlock.Text = "Invalid search method selected";
                            return;
                    }

                    resultTextBlock.Text = $"Search method: {selectedSearchMethod}, Result: {(searchIndex != -1 ? $"Found at index {searchIndex}" : "Not found")}, Comparisons: {comparisons}";
                }
                catch (Exception ex)
                {
                    resultTextBlock.Text = $"Error: {ex.Message}";
                }
            };
        }

        private void InitializeUI()
        {
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
            var firstHundredTextBlock = this.FindControl<TextBlock>("FirstHundredTextBlock");

            resultTextBlock.Text = "Result will be displayed here";
        }

        private async Task<(int, int)> SearchWithProgress(Func<int[], int, Action<int>, Task<(int, int)>> searchMethod, int[] array, int value)
        {
            return await searchMethod(array, value, comparisonIndex =>
            {
                HighlightElement(comparisonIndex);
            });
        }

        private void DisplayArray()
        {
            _arrayVisualizationPanel.Children.Clear();
            foreach (var item in _generatedArray.Take(100))
            {
                var textBlock = new TextBlock
                {
                    Text = item.ToString(),
                    Margin = new Thickness(2),
                    Padding = new Thickness(5),
                    Background = Brushes.LightGray,
                    Foreground = Brushes.Black
                };
                _arrayVisualizationPanel.Children.Add(textBlock);
            }
        }

        private void HighlightElement(int index)
        {
            if (index >= 0 && index < _arrayVisualizationPanel.Children.Count)
            {
                for (int i = 0; i < _arrayVisualizationPanel.Children.Count; i++)
                {
                    var textBlock = _arrayVisualizationPanel.Children[i] as TextBlock;
                    if (i == index)
                    {
                        textBlock.Background = Brushes.Yellow;
                    }
                    else
                    {
                        textBlock.Background = Brushes.LightGray;
                    }
                }
            }
        }
    }

    public static class ArrayGenerator
    {
        public static int[] GenerateArray(int size)
        {
            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = rand.Next(1, 100000);
            }

            return array;
        }
    }

    public static class SearchAlgorithms
    {
        public static async Task<(int, int)> SequentialSearch(int[] array, int value, Action<int> reportProgress)
        {
            int comparisons = 0;
            for (int i = 0; i < array.Length; i++)
            {
                comparisons++;
                reportProgress(i);
                await Task.Delay(50); // Add delay for visualization purposes
                if (array[i] == value)
                    return (i, comparisons);
            }

            return (-1, comparisons);
        }

        public static async Task<(int, int)> FibonacciSearch(int[] array, int value, Action<int> reportProgress)
        {
            int comparisons = 0;
            int n = array.Length;
            int fibMMm2 = 0; // (m-2)'th Fibonacci Number
            int fibMMm1 = 1; // (m-1)'th Fibonacci Number
            int fibM = fibMMm2 + fibMMm1; // m'th Fibonacci

            while (fibM < n)
            {
                fibMMm2 = fibMMm1;
                fibMMm1 = fibM;
                fibM = fibMMm2 + fibMMm1;
            }

            int offset = -1;

            while (fibM > 1)
            {
                int i = Math.Min(offset + fibMMm2, n - 1);
                comparisons++;
                reportProgress(i);
                await Task.Delay(50); // Add delay for visualization purposes

                if (array[i] < value)
                {
                    fibM = fibMMm1;
                    fibMMm1 = fibMMm2;
                    fibMMm2 = fibM - fibMMm1;
                    offset = i;
                }
                else if (array[i] > value)
                {
                    fibM = fibMMm2;
                    fibMMm1 = fibMMm1 - fibMMm2;
                    fibMMm2 = fibM - fibMMm1;
                }
                else return (i, comparisons);
            }

            if (fibMMm1 == 1 && array[offset + 1] == value)
            {
                comparisons++;
                reportProgress(offset + 1);
                await Task.Delay(50); // Add delay for visualization purposes
                return (offset + 1, comparisons);
            }

            return (-1, comparisons);
        }

        public static async Task<(int, int)> InterpolationSearch(int[] array, int value, Action<int> reportProgress)
        {
            int comparisons = 0;
            int low = 0, high = array.Length - 1;

            while (low <= high && value >= array[low] && value <= array[high])
            {
                comparisons++;
                int pos = low + ((value - array[low]) * (high - low) / (array[high] - array[low]));
                reportProgress(pos);
                await Task.Delay(50); // Add delay for visualization purposes

                if (array[pos] == value)
                    return (pos, comparisons);
                else if (array[pos] < value)
                    low = pos + 1;
                else
                    high = pos - 1;
            }

            return (-1, comparisons);
        }

        public static async Task<(int, int)> HashSearch(int[] array, int value, Action<int> reportProgress)
        {
            int comparisons = 0;
            int hash = value % array.Length;
            int startHash = hash; 

            while (true)
            {
                comparisons++;
                reportProgress(hash);
                await Task.Delay(50); // Add delay for visualization purposes

                if (array[hash] == value)
                {
                    return (hash, comparisons); 
                }

                if (array[hash] == 0 || comparisons == array.Length)
                {
                    return (-1, comparisons); 
                }

                hash = (hash + 1) % array.Length;

                if (hash == startHash)
                {
                    return (-1, comparisons); 
                }
            }
        }
    }
}
