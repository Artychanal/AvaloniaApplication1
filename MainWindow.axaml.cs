using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
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
            var inputPromptTextBlock = this.FindControl<TextBlock>("InputPromptTextBlock");
            var numberOfElementsComboBox = this.FindControl<ComboBox>("NumberOfElementsComboBox");
            var generateArrayButton = this.FindControl<Button>("GenerateArrayButton");
            var inputPromptTextBlock2 = this.FindControl<TextBlock>("InputPromptTextBlock2");
            _arrayVisualizationPanel = this.FindControl<WrapPanel>("ArrayVisualizationPanel");
            var exitButton = this.FindControl<Button>("ExitButton");

            inputPromptTextBlock.Text = "Enter the number of elements to generate:";

            var numberOfElementsOptions = new List<int> { 1, 10, 50, 100, 500, 1000, 5000, 10000, 20000, 50000, 100000 };
            foreach (var option in numberOfElementsOptions)
            {
                numberOfElementsComboBox.Items.Add(option.ToString());
            }

            generateArrayButton.Click += (sender, e) =>
            {
                try
                {
                    int numberOfElements = int.Parse((string)numberOfElementsComboBox.SelectedItem);
                    _generatedArray = ArrayGenerator.GenerateArray(numberOfElements);
                    Array.Sort(_generatedArray);
                    DisplayArray();
                }
                catch (Exception ex)
                {
                    resultTextBlock.Text = $"Error: {ex.Message}";
                }
            };
            inputPromptTextBlock2.Text = "Enter element you want to search for:";

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
            exitButton.Click += (sender, e) =>
            {
                Close(); 
            };
        }

        private void InitializeUI()
        {
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
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
            HashSet<int> uniqueNumbers = new HashSet<int>();
            int[] array = new int[size];

            for (int i = 0; i < size; i++)
            {
                int randomNumber;
                do
                {
                    randomNumber = rand.Next(1, 1000000);
                } while (!uniqueNumbers.Add(randomNumber)); 
                array[i] = randomNumber;
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
                if (array.Length == 10 || array.Length == 50 ||array.Length == 100)
                {
                    reportProgress(i);
                    await Task.Delay(50);
                }

                if (array[i] == value)
                    return (i, comparisons);
            }

            return (-1, comparisons);
        }

        public static async Task<(int, int)> FibonacciSearch(int[] array, int value, Action<int> reportProgress)
        {
            int comparisons = 0;
            int n = array.Length;
            int fibMMm2 = 0; 
            int fibMMm1 = 1; 
            int fibM = fibMMm2 + fibMMm1; 

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
                if (array.Length == 10 || array.Length == 50 || array.Length == 100)
                {
                    reportProgress(i);
                    await Task.Delay(50);
                }

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
                if (array.Length == 10 || array.Length == 50 || array.Length == 100)
                {
                    reportProgress(offset + 1);
                    await Task.Delay(50);
                }

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

                if (array[high] == array[low])
                {
                    if (array[low] == value)
                    {
                        reportProgress(low);
                        return (low, comparisons);
                    }
                    break;
                }

                int pos = low + (int)(((double)(value - array[low]) * (high - low)) / (array[high] - array[low]));

                if (pos < 0 || pos >= array.Length)
                {
                    break;
                }

                if (array.Length == 10 || array.Length == 50 ||array.Length == 100)
                {
                    reportProgress(pos);
                    await Task.Delay(50);
                }

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
            int size = array.Length;
            int[] hashTable = new int[size]; 

            for (int i = 0; i < size; i++)
            {
                hashTable[i] = -1;
            }

            int PJWHash(int key)
            {
                int BitsInUnsignedInt = sizeof(uint) * 8;
                int ThreeQuarters = (BitsInUnsignedInt * 3) / 4;
                int OneEighth = BitsInUnsignedInt / 8;
                uint HighBits = (uint)(0xFFFFFFFF) << (BitsInUnsignedInt - OneEighth);
                uint hash = 0;
                uint test = 0;
                foreach (char c in key.ToString())
                {
                    hash = (hash << OneEighth) + c;
                    if ((test = hash & HighBits) != 0)
                    {
                        hash = ((hash ^ (test >> ThreeQuarters)) & (~HighBits));
                    }
                }
                return (int)(hash % size);
            }

            for (int i = 0; i < size; i++)
            {
                int hash = PJWHash(array[i]); 
                while (hashTable[hash] != -1)
                {
                    hash = (hash + 1) % size; 
                }
                hashTable[hash] = i; 
            }

            int hashValue = PJWHash(value);
            while (hashTable[hashValue] != -1 && comparisons < size)
            {
                comparisons++;
                int index = hashTable[hashValue]; 
                if (array.Length == 10 || array.Length == 50 || array.Length == 100)
                {
                    reportProgress(index);
                    await Task.Delay(50); 
                }

                if (array[index] == value)
                {
                    return (index, comparisons);
                }
                hashValue = (hashValue + 1) % size; 
            }

            return (-1, comparisons);
        }
    }
}
