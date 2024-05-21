using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
namespace AvaloniaApplication1
{
    public partial class MainWindow : Window
    {
        private int[] _generatedArray;

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
            var firstFiveTextBlock = this.FindControl<TextBlock>("FirstFiveTextBlock");
            var searchTextBox = this.FindControl<TextBox>("SearchTextBox");
            var searchButton = this.FindControl<Button>("SearchButton");
            var resultTextBlock = this.FindControl<TextBlock>("ResultTextBlock");
            var searchMethodComboBox = this.FindControl<ComboBox>("SearchMethodComboBox");
            

            searchButton.Click += (sender, e) =>
            {
                try
                {
                    int searchValue = int.Parse(searchTextBox.Text);

                    if (_generatedArray == null)
                        _generatedArray = GenerateArray(100);

                    Array.Sort(_generatedArray);

                    int[] firstFive = new int[Math.Min(5, _generatedArray.Length)];
                    Array.Copy(_generatedArray, firstFive, firstFive.Length);
                    var firstFiveBuilder = new StringBuilder();
                    for (int i = 0; i < firstFive.Length; i++)
                    {
                        firstFiveBuilder.Append(firstFive[i]);
                        if (i < firstFive.Length - 1)
                            firstFiveBuilder.Append(", ");
                    }

                    firstFiveTextBlock.Text = $"First five elements: {firstFiveBuilder.ToString()}";

                    string selectedSearchMethod = ((ComboBoxItem)searchMethodComboBox.SelectedItem).Content.ToString();
                    int searchIndex = -1;

                    switch (selectedSearchMethod)
                    {
                        case "Sequential Search":
                            searchIndex = SequentialSearch(_generatedArray, searchValue);
                            break;
                        case "Fibonacci Search":
                            searchIndex = FibonacciSearch(_generatedArray, searchValue);
                            break;
                        case "Interpolation Search":
                            searchIndex = InterpolationSearch(_generatedArray, searchValue);
                            break;
                        case "Hash Search":
                            searchIndex = HashSearch(_generatedArray, searchValue);
                            break;
                        default:
                            resultTextBlock.Text = "Invalid search method selected";
                            break;
                    }

                    resultTextBlock.Text = $"Search method: {selectedSearchMethod}, Result: {(searchIndex != -1 ? $"Found at index {searchIndex}" : "Not found")}";
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
            resultTextBlock.Text = "Result will be displayed here";
        }

        private int[] GenerateArray(int size)
        {
            Random rand = new Random();
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = rand.Next(1, 100000);
            }

            return array;
        }

        private int SequentialSearch(int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                    return i;
            }

            return -1;
        }

        private int FibonacciSearch(int[] array, int value)
        {
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
                else return i;
            }

            if (fibMMm1 == 1 && array[offset + 1] == value)
                return offset + 1;

            return -1;
        }

        private int InterpolationSearch(int[] array, int value)
        {
            int low = 0, high = array.Length - 1;

            while (low <= high && value >= array[low] && value <= array[high])
            {
                int pos = low + ((value - array[low]) * (high - low) / (array[high] - array[low]));

                if (array[pos] == value)
                    return pos;
                else if (array[pos] < value)
                    low = pos + 1;
                else
                    high = pos - 1;
            }

            return -1;
        }

        private int HashSearch(int[] array, int value)
        {
            int hash = value % array.Length;

            while (array[hash] != value)
            {
                if (array[hash] == 0)
                    return -1; // Value not found
                hash = (hash + 1) % array.Length;
            }

            return hash;
        }
    }
}
