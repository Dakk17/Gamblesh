using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FinalosProjektos
{
    public partial class Quota : UserControl
    {
        public static int quotaDay = 3;
        public static double quotaAmount = 1200;
        public static int quotaLevel = 1;

        public static Label quotaLevelStatic;
        public static Label dayRemainStatic;
        public static Label quotaPriceStatic;

        private MainWindow _mainWindow;
        public Quota(MainWindow mainWindow)
        {
            InitializeComponent();

            quotaLevelStatic = quotaLevelLabel;
            dayRemainStatic = dayRemain;
            quotaPriceStatic = quotaPrice;
            _mainWindow = mainWindow;

        }

        public static void QuotaTime(object sender, EventArgs e)
        {
            if (MainWindow.notPause)
            {
                if (quotaDay != 0)
                {
                    quotaDay--;
                }
            }
        }

        public static void QuotaPass(Label loseBackgroundStatic, Grid loseMenuStatic) 
        {
            if (quotaDay == 0)
            {
                if (quotaAmount <= MainWindow.gold)
                {
                    MainWindow.gold -= quotaAmount;
                    quotaLevel++;
                    quotaAmount = Math.Ceiling((1250 * Math.Pow(1.6, quotaLevel)) * 100) / 100;
                    quotaDay = 3;
                }
                else
                {
                    MainWindow.notPause = false;
                    loseBackgroundStatic.Visibility = Visibility.Visible;
                    loseMenuStatic.Visibility = Visibility.Visible;
                }
            }
        }

        public void LoadClickButton(object sender, RoutedEventArgs e)
        {
            _mainWindow.LoadProgress();
        }

        public static void DisplayUI()
        {
            if (quotaLevelStatic != null && dayRemainStatic != null && quotaPriceStatic != null)
            {
                quotaLevelStatic.Content = quotaLevel;
                dayRemainStatic.Content = quotaDay;
                quotaPriceStatic.Content = quotaAmount.ToString().PadLeft(3);
            }
        }
    }
}
