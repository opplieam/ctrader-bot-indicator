using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class KijunSen : Indicator
    {

        [Parameter("Period", DefaultValue = 26)]
        public int Period { get; set; }

        [Parameter("Timeframe")]
        public TimeFrame TF { get; set; }

        [Output("Middle", LineColor = "Gray", PlotType = PlotType.Line, LineStyle = LineStyle.LinesDots, Thickness = 1)]
        public IndicatorDataSeries MiddleResult { get; set; }

        private IchimokuKinkoHyo _ichimoku;

        private Bars TFBar;

        protected override void Initialize()
        {
            TFBar = MarketData.GetBars(TF);
            _ichimoku = Indicators.IchimokuKinkoHyo(TFBar, 9, Period, 52);
        }

        public override void Calculate(int index)
        {
            int relativeIndex = TFBar.OpenTimes.GetIndexByTime(Bars.OpenTimes[index]);
            MiddleResult[index] = _ichimoku.KijunSen[relativeIndex];
        }

    }

}
