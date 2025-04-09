using System;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace FinalosProjektos
{
    public partial class MainWindow : Window
    {
        public static double gold = 0;
        public double goldIncome = 0;
        public static double increasedGold = 0.01;
        public int goldInterval = 1;
        bool badExit = true;
        public static bool notPause = true;

        private readonly FileManager _fileManager;
        
        public MainWindow()
        {
            InitializeComponent();
            MainControl.Content = new GoldClick(goldPerSec, increasedGold, goldCount);

            //Timer pro pasivní příjem goldů
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(goldInterval);
            timer.Tick += Timer_Tick;
            timer.Start();


            //Timer pro UI update
            DispatcherTimer updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            updateTimer.Tick += Update;
            updateTimer.Start();

            _fileManager = new FileManager("progress.json");

            if (_fileManager.ExitFlagExists())
            {
                MessageBox.Show("Unintended Exit: Please Avoid Using Alt + F4", "FORCED EXIT!");
                _fileManager.DeleteExitFlag();
            }
            LoadProgress();
            this.Closing += MainWindow_Closing;


            //Timer pro Quotu
            DispatcherTimer quotaTime = new DispatcherTimer();
            quotaTime.Interval = TimeSpan.FromMinutes(10);
            quotaTime.Tick += Quota.QuotaTime;
            quotaTime.Start();
        }

        public void LoadProgress()
        {
            GameState gameState = _fileManager.LoadProgress();

            increasedGold = gameState.GoldIncrease;
            GoldClick.goldOnClick = gameState.GoldOnClick;
            GoldClick.clickLevel = gameState.ClickLevel;
            GoldClick.clickPrice = gameState.ClickPrice;    
            GoldClick.autoLevel = gameState.AutoLevel;
            GoldClick.autoPrice = gameState.AutoPrice;
            gold = gameState.Gold;
            Quota.quotaAmount = gameState.QuotaAmount;
            Quota.quotaDay = gameState.QuotaDay;
            Quota.quotaLevel = gameState.QuotaLevel;
        }

        private void SaveProgress()
        {
            GameState gameState = new GameState
            {
                GoldIncrease = increasedGold,
                GoldOnClick = GoldClick.goldOnClick,
                ClickPrice = GoldClick.clickPrice,
                AutoPrice = GoldClick.autoPrice,
                ClickLevel = GoldClick.clickLevel,
                AutoLevel = GoldClick.autoLevel,
                Gold = gold,
                QuotaAmount = Quota.quotaAmount,
                QuotaDay = Quota.quotaDay,
                QuotaLevel = Quota.quotaLevel

            };

            _fileManager.SaveProgress(gameState);
        }
        private void Update(object sender, EventArgs e)
        {      
            if(notPause)
            {
                goldIncome = increasedGold;
                goldCount.Text = gold.ToString("0.00").PadLeft(3);
                goldPerSec.Content = goldIncome.ToString("0.00").PadLeft(3);
                GoldClick.GoldUpdate(this);
                Quota.DisplayUI();
                Quota.QuotaPass(loseBackground, loseMenu);

                if (GoldClick.Instance != null)
                {
                    var animationClickButton = GoldClick.Instance.AnimationClickButton;
                    GoldClick.animationCheck(animationClickButton);
                }
            }
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            if (notPause)
            {
                gold += increasedGold;
            }        
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavPanel.Visibility == Visibility.Visible)
            {
                NavPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                NavPanel.Visibility = Visibility.Visible;
            }
        }

        private void Mines_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Content = new Mines(this);
            NavPanel.Visibility = Visibility.Collapsed;
        }
        private void Quota_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Content = new Quota(this);
            NavPanel.Visibility = Visibility.Collapsed;
        }

        private void Spin_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Content = new Spin(this);
            NavPanel.Visibility = Visibility.Collapsed;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            NavPanel.Visibility = Visibility.Collapsed;
            SaveProgress();
            _fileManager.DeleteExitFlag();
            badExit = false;
            this.Close();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Content = new GoldClick(goldPerSec, increasedGold, goldCount);
            NavPanel.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveProgress();
            if (badExit)
            {
                _fileManager.CreateExitFlag();
            }
        }

        private void loadGame_Click(object sender, RoutedEventArgs e)
        {

            loseMenu.Visibility = Visibility.Hidden;
            loseBackground.Visibility = Visibility.Hidden;            
            LoadProgress();
            notPause = true;
        }
        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            MainControl.Content = new GoldClick(goldPerSec, increasedGold, goldCount);
            loseMenu.Visibility = Visibility.Hidden;
            loseBackground.Visibility = Visibility.Hidden;
            _fileManager.NewGame();
            LoadProgress();
            MenuButton.IsEnabled = true;
            notPause = true;
        }
    }
}
