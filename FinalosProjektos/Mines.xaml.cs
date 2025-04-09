using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FinalosProjektos
{
    public partial class Mines : UserControl
    {
        int tries = 1;
        int indexOfBomb;
        int pokusu = 16;
        int Bombs = 1; //Změna na tlačítko výběru min
        double betNumber;
        private double priceMultiplier;
        bool canPlay = false;
        List<Button> AllButtons;
        List<int> BombIndex = new List<int>();
        List<Button> ClickedButtons = new List<Button>();

        private MainWindow _mainWindow; 

        public Mines(MainWindow mainWindow)
        {
            InitializeComponent();
            AllButtons = FindButtonsPrefix("B");
            foreach (Button button in AllButtons)
            {
                button.IsEnabled = false;
            }
            _mainWindow = mainWindow;
        }
        private List<Button> FindButtonsPrefix(string prefix)
        {
            List<Button> buttons = new List<Button>();
            foreach (UIElement child in MainGrid.Children)
            {
                if (child is Button button && button.Name.StartsWith(prefix))
                {
                    buttons.Add(button);
                }
            }
            return buttons;
        }

        //ZOBRAZENÍ/POVOLENÍ VŠECH TLAČÍTEK
        public void ButtonClear()
        {
            foreach (Button tlacitko in ClickedButtons)
            {
                tlacitko.IsEnabled = true;
            }
        }

        //ZÍSKÁNÍ POZIC BOMB 
        public void BombCount()
        {
            for (int i = 0; i < Bombs; i++)
            {
                Randomos();
                BombIndex.Add(indexOfBomb);
            }
            tries = 0;
        }

        //RANDOM VÝBĚR MIN
        public void Randomos()
        {
            Random random = new Random();
            indexOfBomb = random.Next(0, 16);
            while (BombIndex.Contains(indexOfBomb))
            {
                indexOfBomb = random.Next(0, 16);
            }
        }

        //POUŽÍT POUZE NUMERICKOU KLÁVESNICI 
        private void NumberInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //LIST K TEXTBOXŮM
        List<string> textBoxText = new List<string>()
        {
            "1,00", "1x"
        };

        //PSANÍ DO POLÍČKA 
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Text = string.Empty;
        }

        //ODKLIKNUTÍ Z POLÍČKA 
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "")
            {
                textBox.Text = textBoxText[int.Parse(textBox.Name.Substring(1))];
            }

            if (textBox.Name == "t0")
            {
                betNumber = Convert.ToDouble(textBox.Text);
                textBox.Text = betNumber.ToString("0.00");
                switch (betNumber)
                {
                    case < 1:
                        betNumber = 1;
                        textBox.Text = betNumber.ToString("0.00");
                        break;
                    case > 1000000000:
                        betNumber = 1000000000;
                        textBox.Text = betNumber.ToString("0.00");
                        break;
                }
            }

            else if (textBox.Name == "t1")
            {
                Bombs = (int)double.Parse(textBox.Text.Replace("x", ""));
                switch (Bombs)
                {
                    case >= 16:
                        Bombs = 15;
                        break;

                    case < 1:
                        Bombs = 1;
                        break;
                }
                textBox.Text = Bombs.ToString() + "x";
            }
        }

        //MINY 
        public void Mines0(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            clickedButton.IsEnabled = false; // kliknuté tlačítko je vypnuto
            ClickedButtons.Add(clickedButton);
            Collect.IsEnabled = true;

            if (pokusu >= 1)
            {
                if (tries == 1)
                {
                    foreach (Button button in AllButtons)
                    {
                        button.IsEnabled = true;
                    }
                    pokusu -= Bombs;
                    BombCount();
                }

                if (clickedButton != null)
                {
                    string buttonContent = clickedButton.Name;
                    
                    if (buttonContent != "start" && buttonContent != "Collect")
                    {
                        pokusu--;
                        if (BombIndex.Contains(int.Parse(buttonContent.Substring(1))))
                        {
                            _mainWindow.MenuButton.IsEnabled = true;
                            t0.IsEnabled = true;
                            t1.IsEnabled = true;                            
                            Collect.IsEnabled = false;
                            priceMultiplier = 0;
                            ButtonClear();
                            ClickedButtons.Clear();
                            tries = 1;
                            BombIndex.Clear();
                            pokusu = 16;

                            foreach (Button button in AllButtons)
                            {
                                button.IsEnabled = false;
                            }

                            foreach (int tlacitko in BombIndex)
                            {
                                //Button badButton = FindName("B"+tlacitko.ToString()) as Button;
                                //badButton.Background = new SolidColorBrush(Colors.Red);
                            }
                        }

                        else if (pokusu == 0)
                        {
                            priceMultiplier = (ClickedButtons.Count-1) * double.Parse(t0.Text) * double.Parse(t1.Text.Replace("x", ""))/2;
                            _mainWindow.MenuButton.IsEnabled = true;
                            t0.IsEnabled = true;
                            t1.IsEnabled = true;
                            MainWindow.gold += priceMultiplier;
                            priceMultiplier = 0;
                            Collect.IsEnabled = false;                            
                            tries = 1;
                            BombIndex.Clear();
                            pokusu = 16;
                            ButtonClear();
                            ClickedButtons.Clear();

                            foreach (Button button in AllButtons)
                            {
                                button.IsEnabled = false;
                            }
                        }
                        else
                        {
                            priceMultiplier = (ClickedButtons.Count-1)* double.Parse(t0.Text)*double.Parse(t1.Text.Replace("x",""))/3;
                        }
                    }
                }

            }
            else if (pokusu == 0)
            {
                _mainWindow.MenuButton.IsEnabled = true;
                t0.IsEnabled = true;
                t1.IsEnabled = true;                
                Collect.IsEnabled = false;
                MainWindow.gold += priceMultiplier;
                priceMultiplier = 0;
                tries = 1;
                BombIndex.Clear();
                pokusu = 16;
                ButtonClear();
                ClickedButtons.Clear();

                foreach (Button button in AllButtons)
                {
                    button.IsEnabled = false;
                }
            }
        }

        //NOVÁ HRA - SPUSTÍ MINY
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {

            if (MainWindow.gold >= double.Parse(t0.Text))
            {
                canPlay = true;
                MainWindow.gold -= double.Parse(t0.Text);
            }

            else
            {
                canPlay = false;
            }
            if (canPlay)
            {
                _mainWindow.NavPanel.Visibility = Visibility.Collapsed;
                t0.IsEnabled = false;
                t1.IsEnabled = false;
                Mines0(sender, e);
                _mainWindow.MenuButton.IsEnabled = false;
            }
        }
        private void Collect_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MenuButton.IsEnabled = true;
            t0.IsEnabled = true;
            t1.IsEnabled = true;            
            Collect.IsEnabled = false;
            MainWindow.gold += priceMultiplier;
            priceMultiplier = 0;
            tries = 1;
            BombIndex.Clear();
            pokusu = 16;
            ButtonClear();
            ClickedButtons.Clear();

            foreach (Button button in AllButtons)
            {
                button.IsEnabled = false;
            }
        }
    }
}
