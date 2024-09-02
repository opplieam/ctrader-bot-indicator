using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SSLMTF : Indicator
    {
        [Parameter(DefaultValue = 10, MinValue = 1)]
        public int Period { get; set; }

        [Parameter("Timeframe")]
        public TimeFrame TF { get; set; }

        [Output("SSL High", LineColor = "#3BB3E4")]
        public IndicatorDataSeries High { get; set; }

        [Output("SSL Low", LineColor = "#FF006E")]
        public IndicatorDataSeries Low { get; set; }

        private SimpleMovingAverage SMAHigh;
        private SimpleMovingAverage SMALow;

        private Bars TFBar;
        private int minusIndex;

        protected override void Initialize()
        {
            TFBar = MarketData.GetBars(TF);
            minusIndex = (int)(TFBar.OpenTimes[1] - TFBar.OpenTimes[0]).TotalMinutes / (int)(Bars.OpenTimes[1] - Bars.OpenTimes[0]).TotalMinutes;
            SMAHigh = Indicators.SimpleMovingAverage(TFBar.HighPrices, Period);
            SMALow = Indicators.SimpleMovingAverage(TFBar.LowPrices, Period);
        }

        public int GetDirectionAt(int index)
        {
            var high = High[index];
            var low = Low[index];
            return double.IsNaN(high) || double.IsNaN(low) ? 0 : high >= low ? +1 : -1;
        }

        public override void Calculate(int index)
        {
            int relativeIndex = TFBar.OpenTimes.GetIndexByTime(Bars.OpenTimes[index]);

            var smaHigh = SMAHigh.Result[relativeIndex];
            var smaLow = SMALow.Result[relativeIndex];
            var closePrice = TFBar.ClosePrices[relativeIndex];
            var prevDir = index > 0 ? GetDirectionAt(index - minusIndex) : 0;
            var result = closePrice > smaHigh ? +1 : closePrice < smaLow ? -1 : prevDir;

            High[index] = result > 0 ? smaHigh : result < 0 ? smaLow : double.NaN;
            Low[index] = result > 0 ? smaLow : result < 0 ? smaHigh : double.NaN;
        }
    }
}
