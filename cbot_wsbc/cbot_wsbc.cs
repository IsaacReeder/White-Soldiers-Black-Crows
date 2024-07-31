using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None, AddIndicators = true)]
    public class cbot_wsbc : Robot
    {
        [Parameter("Volume (in units)", DefaultValue = 0.01, MinValue = 0.01, Step = 0.01)]
        public double Volume { get; set; }

        [Parameter("Take Profit (pips)", DefaultValue = 10, MinValue = 1)]
        public int TakeProfitPips { get; set; }

        [Parameter("Stop Loss (pips)", DefaultValue = 10, MinValue = 1)]
        public int StopLossPips { get; set; }

        [Parameter("Risk Percentage", DefaultValue = 1, MinValue = 0.1, MaxValue = 10, Step = 0.1)]
        public double RiskPercentage { get; set; }

        private double stopLossDistance;
        private double takeProfitDistance;

        protected override void OnStart()
        {
            Print("Started");
        }

        protected override void OnTick()
        {
            // Handle price updates here if needed
        }

        protected override void OnBar()
        {
            // Calculate dynamic StopLoss and TakeProfit in terms of pips
            stopLossDistance = StopLossPips * Symbol.PipSize;
            takeProfitDistance = TakeProfitPips * Symbol.PipSize;

            // White Soldiers: last 3 bars are bullish
            if (IsWhiteSoldiers())
            {
                Print("White Soldiers Formation Detected");

                if (IsTradeAllowed(TradeType.Buy))
                {
                    ExecuteMarketOrder(TradeType.Buy, Symbol.Name, CalculateVolume(), "White Soldiers", stopLossDistance, takeProfitDistance);
                }
            }

            // Black Crows: last 3 bars are bearish
            if (IsBlackCrows())
            {
                Print("Black Crows Formation Detected");

                if (IsTradeAllowed(TradeType.Sell))
                {
                    ExecuteMarketOrder(TradeType.Sell, Symbol.Name, CalculateVolume(), "Black Crows", stopLossDistance, takeProfitDistance);
                }
            }
        }

        private bool IsWhiteSoldiers()
        {
            return Bars.ClosePrices.Last(1) > Bars.OpenPrices.Last(1) &&
                   Bars.ClosePrices.Last(2) > Bars.OpenPrices.Last(2) &&
                   Bars.ClosePrices.Last(3) > Bars.OpenPrices.Last(3);
        }

        private bool IsBlackCrows()
        {
            return Bars.ClosePrices.Last(1) < Bars.OpenPrices.Last(1) &&
                   Bars.ClosePrices.Last(2) < Bars.OpenPrices.Last(2) &&
                   Bars.ClosePrices.Last(3) < Bars.OpenPrices.Last(3);
        }

        private bool IsTradeAllowed(TradeType tradeType)
        {
            // Check if there's an open position already
            foreach (var position in Positions)
            {
                if (position.SymbolName == Symbol.Name && position.TradeType == tradeType)
                {
                    Print("Position already open: {0}", position.TradeType);
                    return false;
                }
            }

            return true;
        }

        private double CalculateVolume()
        {
            // Calculate volume based on risk percentage and stop loss distance
            double accountRisk = Account.Balance * (RiskPercentage / 100);
            double riskPerTrade = stopLossDistance * Symbol.PipValue * Volume;
            return Math.Min(accountRisk / riskPerTrade, Volume);
        }

        protected override void OnStop()
        {
            // Handle cBot stop here
        }
    }
}
