using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Reflection.Metadata.BlobBuilder;

namespace FinalosProjektos
{
    public partial class Spin : UserControl
    {
        int randomAngleNum;
        double spinSpeed;
        int realAngleNum;
        public Dictionary<string, int> uhly;
        public Canvas wheel;
        private MainWindow _mainWindow;
        string finalBarva;

        Dictionary<string, int> fourWayUhel = new Dictionary<string, int>()
        {
            {"cerna",0}, {"cervena",90 }, {"zelena",180 }, {"modra", 270}
        };

        Dictionary<string, int> twoWayUhel = new Dictionary<string, int>()
        {
            {"cerna",-90}, {"cervena",90 }
        };

        Dictionary<string, int> eightWayUhel = new Dictionary<string, int>()
        {
            {"hneda",0}, {"cerna",45 }, {"cervena",90 }, {"fialova", 135}, {"zelena", 180}, {"oranzova", 225}, {"modra", 270}, {"zluta", 315}
        };
        public Spin(MainWindow mainWindow)
        {
            InitializeComponent();
            DisableColorButtons("eightInputs");
            EnableColorButtons("fourInputs");
            wheel = WheelGrid1;
            uhly = fourWayUhel;
            _mainWindow = mainWindow;
        }


        private void SpinPaths()
        {
            int completedAnimations = 0;
            int totalAnimations = wheel.Children.OfType<Path>().Count();

            foreach (var child in wheel.Children)
            {
                if (child is Path path)
                {

                    RotateTransform rotateTransform = new RotateTransform();
                    path.RenderTransform = rotateTransform;
                    DoubleAnimation rotateAnimation = new DoubleAnimation
                    {
                        To = realAngleNum,
                        Duration = TimeSpan.FromSeconds(spinSpeed),
                        AccelerationRatio = 0.3,
                        DecelerationRatio = 0.7
                    };
                    rotateAnimation.Completed += (s, e) =>
                    {
                        completedAnimations++;
                        if (completedAnimations == totalAnimations)
                        {
                            WinResult();
                            for (int i = 0; i < colorIndexer; i++)
                            {
                                selectedColors.Add(wayInputsNames[i]);
                            }
                            EnableColorButtons(selectedColors.ToArray());
                            selectedColors.Clear();
                            _mainWindow.MenuButton.IsEnabled = true;
                            foreach (var kvp in wayButtonsHash)
                            {
                                    var wayButton = (Button)FindName(kvp.Key);
                                    wayButton.IsEnabled = true;
                            }
                        }
                    };                    
                    rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
                }
                finalBarva = BarvaResult(realAngleNum % 360);
            }
        }

        private string BarvaResult(int uhel)
        {

            if (uhly == twoWayUhel)
            {
                var result = uhly.FirstOrDefault(x => 270 < ((x.Value + uhel) % 360) || ((x.Value + uhel) % 360) <= 90);
                return result.Key;
            }
            else if (uhly == eightWayUhel)
            {
                var result = uhly.FirstOrDefault(x => 45 < ((x.Value + uhel) % 360) && ((x.Value + uhel) % 360) <= 90);
                return result.Key;
            }
            else
            {
                var result = uhly.FirstOrDefault(x => 0 <((x.Value + uhel) % 360) && ((x.Value + uhel) % 360) <= 90);
                return result.Key;
            }
        }

        private void ResetWheelRotation()
        {
            foreach (var child in wheel.Children)
            {
                if (child is Path path)
                {
                    path.RenderTransform = new RotateTransform(0);
                }
            }
        }

        private void EnableColorButtonsInGrid(Grid parentGrid)
        {
            foreach (UIElement child in parentGrid.Children)
            {
                if (child is Grid grid)
                {
                    EnableColorButtonsInGrid(grid);
                }
                else if (child is Button button)
                {
                    button.IsEnabled = true;
                }
            }
        }

        private void EnableColorButtons(params string[] gridNames)
        {
            Grid allColors = (Grid)this.FindName("allColors");
            if (allColors != null)
            {
                foreach (UIElement child in allColors.Children)
                {
                    if (child is Grid grid)
                    {
                        if (gridNames.Contains(grid.Name))
                        {
                            EnableColorButtonsInGrid(grid);
                        }
                    }
                }
            }
        }

        private void DisableColorButtonsInGrid(Grid parentGrid)
        {
            foreach(UIElement child in parentGrid.Children)
            {
                if(child is Grid grid)
                {
                    DisableColorButtonsInGrid(grid);
                }
                else if (child is Button button)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void DisableColorButtons(params string[] gridNames)
        {
            Grid allColors = (Grid)this.FindName("allColors");
            if(allColors != null)
            {
                foreach(UIElement child in allColors.Children)
                {
                    if (child is Grid grid)
                    {
                        if (gridNames.Contains(grid.Name))
                        {
                            DisableColorButtonsInGrid(grid);
                        }
                    }
                }
            }
        }
        
        private void DisableAllColorButtons()
        {
            Grid allColors = (Grid)this.FindName("allColors");
            if(allColors  != null)
            {
                foreach(UIElement child in allColors.Children)
                {
                    if (child is Grid grid)
                    {
                        DisableColorButtonsInGrid(grid);
                    }
                }
            }
        }

        int winMultiplier =4;
        List<string> wayInputsNames = new List<string>()
        {
            "twoInputs", "fourInputs", "eightInputs"
        };

        List<string> wayButtons = new List<string>()
        {
            "twoWays", "eightWays"
        };

        Dictionary<string, int> wayButtonsHash = new Dictionary<string, int>()
        {
            {"twoWays", 2 }, {"fourWays", 4}, {"eightWays", 8}
        };
        private void TwoWayClick(object sender, RoutedEventArgs e)
        {
            if (!wayButtons.Contains("fourWays"))
            {
                wayButtons.Add("fourWays");
            }
            wayButtons.Remove("twoWays");
            colorIndexer = 1;
            winMultiplier = 2;
            multipleNum.Content = "2X";
            DisableColorButtons("fourInputs","eightInputs");
            WheelGrid.Visibility = Visibility.Visible;
            WheelGrid1.Visibility = Visibility.Hidden;
            WheelGrid2.Visibility = Visibility.Hidden;
            wheel = WheelGrid;
            uhly = twoWayUhel;
            ResetWheelRotation();
        }

        private void FourWayClick(object sender, RoutedEventArgs e)
        {
            colorIndexer = 2;
            winMultiplier = 4;
            multipleNum.Content = "4X";
            DisableColorButtons("eightInputs");
            EnableColorButtons("fourInputs");
            wheel = WheelGrid1;
            WheelGrid1.Visibility = Visibility.Visible;
            WheelGrid2.Visibility = Visibility.Hidden;
            WheelGrid.Visibility = Visibility.Hidden;
            uhly = fourWayUhel;
            ResetWheelRotation();
        }

        private void EightWayClick(object sender, RoutedEventArgs e)
        {
            colorIndexer = 3;
            winMultiplier = 8;
            multipleNum.Content = "8X";
            EnableColorButtons("fourInputs", "eightInputs");
            wheel = WheelGrid2;
            WheelGrid2.Visibility = Visibility.Visible;
            WheelGrid1.Visibility = Visibility.Hidden;
            WheelGrid.Visibility = Visibility.Hidden;
            uhly = eightWayUhel;
            ResetWheelRotation();
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
        }

        //ODKLIKNUTÍ Z POLÍČKA 

        double bet;
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                textBox.Text = "1,00";
            }

            bet = Convert.ToDouble(textBox.Text);
            textBox.Text = bet.ToString("0.00");
            switch (bet)
            {
                case < 1:
                    bet = 1;
                    textBox.Text = bet.ToString("0.00");
                    break;
                case > 1000000000:
                    bet = 1000000000;
                    textBox.Text = bet.ToString("0.00");
                    break; 
            }
                        
        }

        List <Button> taggedButton = new List<Button>();
        private void NumberInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void barvaClick(object sender, RoutedEventArgs e)
        {
            Button barvaButton = sender as Button;
            if (barvaButton != null)
            {
                noInput.Visibility = Visibility.Hidden;
                if(taggedButton.Count == 0)
                {
                    taggedButton.Add(barvaButton);
                    barvaButton.BorderThickness = new Thickness(2.5);
                    barvaButton.BorderBrush = Brushes.Lime;
                }
                else
                {
                    taggedButton[0].BorderThickness = new Thickness(1);
                    taggedButton[0].BorderBrush = Brushes.Black;
                    if (taggedButton[0] == barvaButton)
                    {
                        barvaButton.BorderThickness = new Thickness(1);
                        barvaButton.BorderBrush = Brushes.Black;
                        taggedButton.Clear();
                    }
                    else
                    {
                        taggedButton.Clear();
                        barvaButton.BorderThickness = new Thickness(2.5);
                        barvaButton.BorderBrush = Brushes.Lime;
                        taggedButton.Add(barvaButton);
                    }
                }                
            }
        }

        bool canPlay = false;
        int colorIndexer = 2;

        List<string> selectedColors = new List<string>();
        private void startSpin_Click(object sender, RoutedEventArgs e)
        {            
            if(taggedButton.Count ==0)
            {
                noInput.Visibility = Visibility.Visible;
            }
            if (double.Parse(betAmount.Text) <= MainWindow.gold && taggedButton.Count !=0)
            {
                canPlay = true;
                MainWindow.gold -= double.Parse(betAmount.Text);
                _mainWindow.NavPanel.Visibility = Visibility.Collapsed;
            }

            else
            {
                canPlay = false;
            }

            if (canPlay)
            {
                foreach (var kvp in wayButtonsHash)
                {
                    var wayButton = (Button)FindName(kvp.Key);
                    wayButton.IsEnabled = false;
                }
                _mainWindow.MenuButton.IsEnabled = false;
                DisableAllColorButtons();
                startSpin.IsEnabled = false;
                Random random = new Random();
                randomAngleNum = random.Next(2520, 4320);
                while (randomAngleNum % 45 >= 43 || randomAngleNum % 45 <= 2)
                {
                    randomAngleNum = random.Next(2520, 4320);
                }
                realAngleNum = 360 + randomAngleNum;
                spinSpeed = 2 + (random.NextDouble() * (3.5 - 2));
                SpinPaths();
            }
        }

        private void WinResult()
        {
            if (taggedButton[0].Name == finalBarva)
            {
                MainWindow.gold += double.Parse(betAmount.Text) * winMultiplier;
            }
            startSpin.IsEnabled = true;
        }
    }
}