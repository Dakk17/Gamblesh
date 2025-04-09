using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WpfAnimatedGif;
using static System.Net.Mime.MediaTypeNames;

namespace FinalosProjektos
{
    public partial class GoldClick : UserControl
    {
        public static Label goldSec;
        public static TextBlock goldCounts;
        public static double goldIncrease;
        public static double goldOnClick = 0.01;
        private System.Windows.Controls.Image _creditImage; 

        // PROMĚNNÉ PRO FARMY 
        public static double clickPrice = 1.00;
        public static double autoPrice = 20.00;
        public static double clickLevel = 1;
        public static double autoLevel = 1;

        public static TextBlock vylepseniTextBlock;
        public static TextBlock cenaTextBlock;

        public static string vylepseniEquation;
        public static string cenaEquation;
        public static string autoVylepseniEquation;
        public static string autoCenaEquation;

        public static Button StaticClickButton;

        public static GoldClick Instance { get; private set; }
        public System.Windows.Controls.Image AnimationClickButton => animationClickButton;

        public GoldClick(Label goldpersec,double goldincrease, TextBlock GoldCount)
        {
            InitializeComponent();
            Instance = this;
            goldSec = goldpersec;
            goldIncrease = goldincrease;
            goldCounts = GoldCount;
            UpdateVylepseniTextBlock(this);
            StaticClickButton = ClickButton;
        }

        public static void GoldUpdate(DependencyObject parent)
        {

            UpdateVylepseniTextBlock(parent);
            AutoUpdateTextBlock(parent);
            StaticClickButton.Content = goldOnClick.ToString()+" G/Click";
        }

        public static void UpdateVylepseniTextBlock(DependencyObject parent)
        {
            vylepseniTextBlock = FindVisualChild<TextBlock>(parent, "Vylepseni");
            cenaTextBlock = FindVisualChild<TextBlock>(parent, "Cena");
            if (vylepseniTextBlock != null && cenaTextBlock !=null)
            {
                vylepseniEquation = (Math.Ceiling(0.01 * Math.Pow(1.5, clickLevel) * 100) / 100).ToString("0.00") + " G/Click";
                vylepseniTextBlock.Text = vylepseniEquation;
                cenaEquation = (Math.Ceiling((Math.Pow(1.5, clickLevel) - 0.5) * 100) / 100).ToString("0.00") + " G";
                cenaTextBlock.Text = cenaEquation;          
            }           
        }

        public static void AutoUpdateTextBlock(DependencyObject parent)
        {
            TextBlock autoCenaTextBlock = FindVisualChild<TextBlock>(parent, "Cena1");
            TextBlock autoVylepseniTextBlock = FindVisualChild<TextBlock>(parent, "Vylepseni1");
            if (autoCenaTextBlock != null && autoVylepseniTextBlock != null)
            {
                autoCenaTextBlock.Text = (Math.Ceiling((Math.Pow(4, autoLevel/2) + 18) * 100) / 100).ToString("0.00") + " G";
                autoVylepseniTextBlock.Text = (Math.Ceiling(Math.Pow(1.7, autoLevel)/34 * 100) / 100).ToString("0.00") + " G/s";
            }
        }

        public async void Coin_Click(object sender, RoutedEventArgs e)
        {
            var controller = ImageBehavior.GetAnimationController(this.animationClickButton);
            if(controller != null)
            {
                controller.GotoFrame(0);
            }

            var image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri("pack://application:,,,/ClickButtonResized.gif");
            image.EndInit();
            ImageBehavior.SetAnimatedSource(animationClickButton, image);
            MainWindow.gold += goldOnClick;
            goldCounts.Text = MainWindow.gold.ToString("0.00");            
        }
        private async void animationClickButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Coin_Click(sender, e);            
        }

        public static void animationCheck(System.Windows.Controls.Image animationClickButton)
        {
            var controller = ImageBehavior.GetAnimationController(animationClickButton);

            if (controller != null)
            {
                if (controller.IsComplete)
                {
                    controller.Pause();
                    controller.Dispose();
                }
            }
        }


        public void ClickUpgrade_Click(object sender, RoutedEventArgs e)
        {
            goldSec.Content = MainWindow.increasedGold.ToString("0.00");
            if(MainWindow.gold >= Math.Ceiling((Math.Pow(1.5, clickLevel) - 0.5) * 100) / 100)
            {
                MainWindow.gold -= clickPrice;
                clickLevel++;
                goldOnClick = Math.Ceiling(0.01*Math.Pow(1.5,clickLevel-1)*100)/100;
                ClickButton.Content = goldOnClick + " G/Click";             
                clickPrice = Math.Ceiling((Math.Pow(1.5, clickLevel )-0.5) * 100) / 100;
            }
            UpdateVylepseniTextBlock(this);
        }
        private void AutoUpgrade_Click(object sender, RoutedEventArgs e)
        {
            goldSec.Content = MainWindow.increasedGold.ToString("0.00");
            if (MainWindow.gold >= Math.Ceiling((Math.Pow(4, autoLevel/2)+18) * 100) / 100)
            {
                MainWindow.gold -= autoPrice;
                MainWindow.increasedGold = Math.Ceiling(Math.Pow(1.7, autoLevel)/34 * 100) / 100;
                autoPrice = Math.Ceiling((Math.Pow(4, autoLevel/2)+18) * 100) / 100;
                autoLevel++;
            }
            AutoUpdateTextBlock(this);
        }

        private static T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T element && element.Name == name)
                {
                    return element;
                }
                T result = FindVisualChild<T>(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
