using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FinalosProjektos
{
    public class GameState
    {
        public double GoldIncrease { get; set; }
        public double GoldOnClick { get; set; }
        public double ClickPrice { get; set; }
        public double AutoPrice { get; set; }
        public double ClickLevel { get; set; }
        public double AutoLevel { get; set; }

        public double Gold { get; set; }

        public int QuotaDay { get; set; }
        public double QuotaAmount { get; set; }
        public int QuotaLevel { get; set; }

        public GameState()
        {
            GoldIncrease = 0.01;
            GoldOnClick = 20;
            ClickPrice = 1.00;
            AutoPrice = 20.00;
            ClickLevel = 1;
            AutoLevel = 1;
            Gold = 0;
            QuotaDay = 3;
            QuotaAmount = 1200;
            QuotaLevel = 1;
        }
    }
}
