using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class cbot_wsbc : Robot
    {
        [Parameter(DefaultValue = 1000)]
        public double Volume { get; set; }

        [Parameter(DefaultValue = 10)]
        public double TakeProfit { get; set; }

        [Parameter(DefaultValue = 10)]
        public double StopLoss { get; set; }

        protected override void OnStart()
        {
            // To learn more about cTrader Automate visit our Help Center:
            // https://help.ctrader.com/ctrader-automate

            Print("Started");
        }

        protected override void OnTick()
        {
            // Handle price updates here
        }

        protected override void OnBar()
        {
            // TWS - If last three candles are bullish, execute market order 
            if(Bars.ClosePrices.Last(1) > Bars.OpenPrices.Last(1) 
            && Bars.ClosePrices.Last(2) > Bars.OpenPrices.Last(2) 
            && Bars.ClosePrices.Last(3) > Bars.OpenPrices.Last(3))
            {
                ExecuteMarketOrder(TradeType.Buy, SymbolName, Volume, "", StopLoss, TakeProfit);
            }

            // TBC
            if(Bars.ClosePrices.Last(1) < Bars.OpenPrices.Last(1) 
            && Bars.ClosePrices.Last(2) < Bars.OpenPrices.Last(2) 
            && Bars.ClosePrices.Last(3) < Bars.OpenPrices.Last(3))
            {
                ExecuteMarketOrder(TradeType.Sell, SymbolName, Volume, "", StopLoss, TakeProfit);
            }
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }
    }
}