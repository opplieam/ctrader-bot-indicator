using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.NorthAsiaStandardTime, AccessRights = AccessRights.None)]
    public class KAMASlope : Indicator
    {
        private KAMA _kama;

        [Parameter("KAMA Period", DefaultValue = 26)]
        public int KAMAPeriod { get; set; }

        [Parameter("Slope Back Step", DefaultValue = 2)]
        //it take 3 periods to determine a peak
        public int SlopeBackstep { get; set; }

        [Parameter("Slope Limit", DefaultValue = 0.5)]
        public double SlopeLimit { get; set; }

        [Output("Slope", PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries Result { get; set; }

        [Output("UpperLimit", PlotType = PlotType.Line, LineStyle = LineStyle.DotsRare, Thickness = 1, LineColor = "red")]
        public IndicatorDataSeries UpperLimit { get; set; }

        [Output("Center", LineStyle = LineStyle.DotsRare, LineColor = "white")]
        public IndicatorDataSeries CenterLine { get; set; }

        [Output("LowerLimit", PlotType = PlotType.Line, LineStyle = LineStyle.DotsRare, Thickness = 1, LineColor = "red")]
        public IndicatorDataSeries LowerLimit { get; set; }


        protected override void Initialize()
        {
            _kama = Indicators.GetIndicator<KAMA>(Bars.ClosePrices, 2, 30, KAMAPeriod);
        }

        public override void Calculate(int index)
        {
            if (index < 0)
            {
                return;
            }

            if (double.IsNaN(_kama.kama[index - SlopeBackstep]))
            {
                return;
            }

            decimal MAT0 = (decimal)_kama.kama[index];
            decimal MATB = (decimal)_kama.kama[index - SlopeBackstep];

            decimal MASlope = decimal.Round((MAT0 - MATB) / ((decimal)Symbol.PipSize * SlopeBackstep), 2);

            Result[index] = (double)MASlope;

            UpperLimit[index] = SlopeLimit;
            LowerLimit[index] = -SlopeLimit;
            CenterLine[index] = 0;
        }
    }
}
