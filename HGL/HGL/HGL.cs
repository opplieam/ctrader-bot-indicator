using System;
using System.Linq;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.NorthAsiaStandardTime, AccessRights = AccessRights.None)]
    public class HGL : Robot
    {
        #region Class variable
        public enum TradeSignal
        {
            Buy,
            Sell,
            Skip
        }

        public enum BaseLineOption
        {
            KijuSen
        }

        public enum KijuSenMode
        {
            Close,
            HighLow
        }

        public enum C1Option
        {
            SSL
        }

        public enum C2Option
        {
            ECO,
            Manual
        }

        public enum ExitOption
        {
            QQE
        }

        // HIDE VOLUME

        private double _takeProfitPip;

        private bool _takeHalfProfitTriggered = false;
        #endregion

        #region Optimize Parameter
        public enum BaseLineParameter
        {
            // 24
            Fast,
            // 50
            Medium,
            // 72
            Slow,
            // 98
            Slowest
        }

        public enum C1Parameter
        {
            // 8
            Fastest,
            // 16
            Fast,
            // 24
            Medium,
            // 32
            Slow,
            // 40
            Slowest
        }

        public enum C2Parameter
        {
            // 12
            Fastest,
            // 18
            Fast,
            // 30
            Medium,
            // 42
            Slow,
            // 48
            Slowest
        }

        public enum ExitParameter
        {
            // 4
            Fastest,
            // 8
            Fast,
            // 14
            Medium,
            // 22
            Slow
        }

        public enum VolumeFParameter
        {
            // 8
            Fast,
            // 13
            Medium,
            // 18
            Slow
        }

        public enum VolumeSParameter
        {
            // 30
            Fast,
            // 50
            Medium,
            // 70
            Slow
        }
        public enum VolumeTParameter
        {
            // 1.1
            Loose,
            // 1.3
            Medium,
            // 1.5
            Strict
        }
        #endregion

        #region Parameters Algorithm Setting
        [Parameter("Instance name", Group = "Management", DefaultValue = "Bot name")]
        public string InstanceName { get; set; }

        [Parameter("Risk %", Group = "Management", DefaultValue = 2)]
        public double RiskPercentage { get; set; }

        [Parameter("Take Profit Multiplyer", Group = "Management", DefaultValue = 1)]
        public double TakeProfitMultiplyer { get; set; }

        [Parameter("Stop loss Multiplyer", Group = "Management", DefaultValue = 1.5)]
        public double StopLossMultiplyer { get; set; }

        [Parameter("Max ATR Lookback", Group = "Management", DefaultValue = 1)]
        public int ATRMax { get; set; }

        [Parameter("One ATR Rule", Group = "Management", DefaultValue = false)]
        public bool OneATRRule { get; set; }

        [Parameter("Use Slope", Group = "Management", DefaultValue = false)]
        public bool UseSlope { get; set; }

        [Parameter("Slope Thresold", Group = "Management", DefaultValue = 0.3)]
        public double SlopeThresold { get; set; }

        [Parameter("Baseline", Group = "Select Indicator", DefaultValue = BaseLineOption.KijuSen)]
        public BaseLineOption SelectBaseline { get; set; }

        [Parameter("C1", Group = "Select Indicator", DefaultValue = C1Option.SSL)]
        public C1Option SelectC1 { get; set; }

        [Parameter("C2", Group = "Select Indicator", DefaultValue = C2Option.ECO)]
        public C2Option SelectC2 { get; set; }

        [Parameter("Volume", Group = "Select Indicator", DefaultValue = VolumeOption.DamianVolume)]
        public VolumeOption SelectVolume { get; set; }

        [Parameter("Exit", Group = "Select Indicator", DefaultValue = ExitOption.QQE)]
        public ExitOption SelectExit { get; set; }
        #endregion

        #region ATR parameter
        [Parameter("Periods", Group = "ATR", DefaultValue = 14)]
        public int ATRPeriods { get; set; }
        #endregion

        #region Kiju-Sen parameter
        [Parameter("Periods", Group = "Kiju-Sen", DefaultValue = BaseLineParameter.Fast)]
        public BaseLineParameter KijuSenPeriod { get; set; }

        [Parameter("Shift", Group = "Kiju-Sen", DefaultValue = -1)]
        public int KijuSenshift { get; set; }

        [Parameter("Mode", Group = "Kiju-Sen", DefaultValue = KijuSenMode.HighLow)]
        public KijuSenMode KijusenMode { get; set; }
        #endregion

        #region QQE Parameter
        [Parameter("Periods", Group = "QQE", DefaultValue = ExitParameter.Medium)]
        public ExitParameter QQEPeriod { get; set; }
        #endregion

        #region SSL Parameter
        [Parameter("Period", Group = "SSL", DefaultValue = C1Parameter.Fastest)]
        public C1Parameter SSLPeriod { get; set; }
        #endregion

        #region Ergodic Candle Parameter
        [Parameter("Interval", Group = "Ergodic Candlestick", DefaultValue = C2Parameter.Fastest)]
        public C2Parameter ECOInterval { get; set; }
        #endregion

        #region Manual parameter
        [Parameter("Signal", Group = "Manual Signal")]
        public TradeSignal ManualSignal { get; set; }
        #endregion

        #region Damian Volume Parameter
        [Parameter("Short Period", Group = "Damian Volume", DefaultValue = VolumeFParameter.Medium)]
        public VolumeFParameter DamianViscosity { get; set; }

        [Parameter("Long Period", Group = "Damian Volume", DefaultValue = VolumeSParameter.Medium)]
        public VolumeSParameter DamianSedimentation { get; set; }

        [Parameter("Threshold", Group = "Damian Volume", DefaultValue = VolumeTParameter.Medium)]
        public VolumeTParameter DamianThreshold { get; set; }

        [Parameter("Lag supressor", Group = "Damian Volume", DefaultValue = false)]
        public bool Damianlag_supressor { get; set; }

        [Parameter("Source", Group = "Damian Volume")]
        public DataSeries DamianSource { get; set; }
        #endregion

        #region Indicator private variable
        private AverageTrueRange _atr;
        private KAMASlope _kamaSlope;

        // Baseline
        private KijunSen _kijusen;

        // C1
        private SSLIndicator _ssl;

        // C2
        // HIDE C2

        // Exit
        private QualitativeQuantitativeE _qqe;

        // volume
        // HIDE VOLUME

        #endregion

        #region Onstart
        protected override void OnStart()
        {
            _atr = Indicators.AverageTrueRange(ATRPeriods, MovingAverageType.Exponential);

            if (UseSlope)
            {
                _kamaSlope = Indicators.GetIndicator<KAMASlope>(10, 1, 0.5);
            }

            switch (SelectBaseline)
            {
                case BaseLineOption.KijuSen:
                    Dictionary<BaseLineParameter, int> baseLineSetting = new Dictionary<BaseLineParameter, int>();
                    baseLineSetting.Add(BaseLineParameter.Fast, 24);
                    baseLineSetting.Add(BaseLineParameter.Medium, 50);
                    baseLineSetting.Add(BaseLineParameter.Slow, 72);
                    baseLineSetting.Add(BaseLineParameter.Slowest, 98);


                    _kijusen = Indicators.GetIndicator<KijunSen>(baseLineSetting[KijuSenPeriod], KijuSenshift, KijusenMode);
                    break;
            }

            switch (SelectC1)
            {
                case C1Option.SSL:
                    Dictionary<C1Parameter, int> c1Setting = new Dictionary<C1Parameter, int>();
                    c1Setting.Add(C1Parameter.Fastest, 8);
                    c1Setting.Add(C1Parameter.Fast, 16);
                    c1Setting.Add(C1Parameter.Medium, 24);
                    c1Setting.Add(C1Parameter.Slow, 32);
                    c1Setting.Add(C1Parameter.Slowest, 90);

                    _ssl = Indicators.GetIndicator<SSLIndicator>(c1Setting[SSLPeriod]);
                    break;
            }

            // HIDE C2

            switch (SelectExit)
            {
                case ExitOption.QQE:
                    Dictionary<ExitParameter, int> exitSetting = new Dictionary<ExitParameter, int>();
                    exitSetting.Add(ExitParameter.Fastest, 4);
                    exitSetting.Add(ExitParameter.Fast, 8);
                    exitSetting.Add(ExitParameter.Medium, 14);
                    exitSetting.Add(ExitParameter.Slow, 22);

                    _qqe = Indicators.GetIndicator<QualitativeQuantitativeE>(exitSetting[QQEPeriod], exitSetting[QQEPeriod]);
                    break;
            }

            // HIDE VOLUME

        }
        #endregion

        #region OnTick
        protected override void OnTick()
        {
            Position currentPosition = Positions.Find(InstanceName);
            if (currentPosition == null)
            {
                return;
            }
            // Take half profit and break even
            if (currentPosition.Pips >= this._takeProfitPip && !this._takeHalfProfitTriggered)
            {
                // Take half profit
                this._takeHalfProfitTriggered = true;
                TradeResult result = ClosePosition(currentPosition, currentPosition.VolumeInUnits / 2);
                if (!result.IsSuccessful)
                {
                    Print(result.Error);
                }
                // Break even with +2 pips for spread
                result = currentPosition.ModifyStopLossPips(-2);
                //result = currentPosition.ModifyStopLossPips(-1 * this._takeProfitPip / 2);
                if (!result.IsSuccessful)
                {
                    Print(result.Error);
                }
            }
        }
        #endregion

        #region OnBar
        protected override void OnBar()
        {
            //Print(Server.Time.Date.CompareTo(DateTime.Parse("02/24/2021").Date));
            //if (Server.Time.Date.CompareTo(DateTime.Parse("02/24/2021").Date) == -1)
            //{
            //    return;
            //}
            // Open trade one per Instance, Check Exit position If have opened
            Position currentOpenPos = Positions.Find(InstanceName);
            if (currentOpenPos != null)
            {
                // Check Exit indicator
                if (IsExitPosition(currentOpenPos))
                {
                    TradeResult result = currentOpenPos.Close();
                    if (!result.IsSuccessful)
                    {
                        Print(result.Error);
                    }
                }
                return;
            }

            this._takeHalfProfitTriggered = false;

            // Sideway detect
            if (UseSlope)
            {
                if (Math.Abs(_kamaSlope.Result.Last(1)) < SlopeThresold)
                {
                    return;
                }
            }


            // One ATR Rule
            if (OneATRRule && !IsOneATR())
            {
                return;
            }

            // HIDE VOLUME

            TradeSignal c1Signal = CheckC1Signal();
            // C1 Give signal. Check baseline agree.
            if (c1Signal != TradeSignal.Skip && IsBaselineConfirm(c1Signal) && IsC2Confirm(c1Signal))
            {
                MarketOrder(c1Signal);
                return;
            }
            // Baseline give signal. Check C1 agree.
            TradeSignal baseLineSignal = CheckBaselineSignal();
            if (baseLineSignal != TradeSignal.Skip && IsC1Confirm(baseLineSignal) && IsC2Confirm(baseLineSignal))
            {
                MarketOrder(baseLineSignal);
                return;
            }
        }
        #endregion

        #region OnStop
        protected override void OnStop()
        {

        }
        #endregion

        #region Risk Management
        public bool IsOneATR()
        {
            // HIDE ONE ATR
        }
        public double CalculateLotSize(double stopLossPip)
        {
            double tradeVolume = (Account.Balance * (RiskPercentage / 100)) / (stopLossPip * Symbol.PipValue);
            return Symbol.NormalizeVolumeInUnits(tradeVolume);
        }

        public void MarketOrder(TradeSignal signal)
        {
            // Looking for maximum ATR value for given period
            double[] atrArray = new double[ATRMax];
            for (int i = 0; i < ATRMax; i++)
            {
                atrArray[i] = _atr.Result.Last(i + 1);
            }
            double atrValue = atrArray.Max();
            double stopLossPip = StopLossMultiplyer * atrValue / Symbol.PipSize;
            this._takeProfitPip = TakeProfitMultiplyer * atrValue / Symbol.PipSize;
            double tradeVolume = CalculateLotSize(stopLossPip);

            if (signal == TradeSignal.Buy)
            {
                TradeResult result = ExecuteMarketOrder(TradeType.Buy, SymbolName, tradeVolume, InstanceName, stopLossPip, null);
                if (!result.IsSuccessful)
                {
                    Print(result.Error);
                }
            }
            if (signal == TradeSignal.Sell)
            {
                TradeResult result = ExecuteMarketOrder(TradeType.Sell, SymbolName, tradeVolume, InstanceName, stopLossPip, null);
                if (!result.IsSuccessful)
                {
                    Print(result.Error);
                }
            }

        }
        #endregion

        #region Baseline Indicator
        public TradeSignal CheckBaselineSignal()
        {
            double openPrice = Bars.OpenPrices.Last(1);
            double closePrice = Bars.ClosePrices.Last(1);
            double baseLinePrice = 0.0;
            switch (SelectBaseline)
            {
                case BaseLineOption.KijuSen:
                    baseLinePrice = _kijusen.MiddleResult.Last(1);
                    break;
            }

            if ((openPrice > baseLinePrice && baseLinePrice > closePrice) || (closePrice > baseLinePrice && baseLinePrice > openPrice))
            {
                return closePrice > baseLinePrice ? TradeSignal.Buy : TradeSignal.Sell;
            }
            return TradeSignal.Skip;
        }

        public bool IsBaselineConfirm(TradeSignal signal)
        {
            double result = 0.0;
            switch (SelectBaseline)
            {
                case BaseLineOption.KijuSen:
                    result = _kijusen.MiddleResult.Last(1);
                    break;
            }
            if (signal == TradeSignal.Sell)
            {
                return result > Bars.ClosePrices.Last(1);
            }
            if (signal == TradeSignal.Buy)
            {
                return result < Bars.ClosePrices.Last(1);
            }
            return false;
        }
        #endregion

        #region C1 Indicator
        public TradeSignal CheckC1Signal()
        {
            double mainPoint = 0.0;
            double subPoint = 0.0;
            double prevMainPoint = 0.0;
            double prevSubPoint = 0.0;

            double ocrMainPoint = 0.0;
            double ocrSubPoint = 0.0;

            TradeSignal ocrSignal = TradeSignal.Skip;

            switch (SelectC1)
            {
                case C1Option.SSL:
                    mainPoint = _ssl.High.Last(1);
                    subPoint = _ssl.Low.Last(1);
                    prevMainPoint = _ssl.High.Last(2);
                    prevSubPoint = _ssl.Low.Last(2);

                    ocrMainPoint = _ssl.High.Last(3);
                    ocrSubPoint = _ssl.Low.Last(3);
                    break;
            }
            // One candle rule with pull back
            if (ocrSubPoint > ocrMainPoint && prevSubPoint < prevMainPoint && subPoint < mainPoint && Bars.ClosePrices.Last(1) <= Bars.ClosePrices.Last(2))
            {
                ocrSignal = TradeSignal.Buy;
            }

            if (ocrSubPoint < ocrMainPoint && prevSubPoint > prevMainPoint && subPoint > mainPoint && Bars.ClosePrices.Last(1) >= Bars.ClosePrices.Last(2))
            {
                ocrSignal = TradeSignal.Sell;
            }
            // Overall Signal
            if ((prevSubPoint > prevMainPoint && subPoint < mainPoint) || ocrSignal == TradeSignal.Buy)
            {
                return TradeSignal.Buy;
            }
            if ((prevSubPoint < prevMainPoint && subPoint > mainPoint) || ocrSignal == TradeSignal.Sell)
            {
                return TradeSignal.Sell;
            }

            return TradeSignal.Skip;
        }

        public bool IsC1Confirm(TradeSignal signal)
        {
            bool result = false;
            // Check if C1 signal is within 6 candle
            for (int i = 1; i < 7; i++)
            {
                double mainPoint = 0.0;
                double subPoint = 0.0;
                double prevMainPoint = 0.0;
                double prevSubPoint = 0.0;
                double currentMainPoint = 0.0;
                double currentSubPoint = 0.0;

                switch (SelectC1)
                {
                    case C1Option.SSL:
                        currentMainPoint = _ssl.High.Last(1);
                        currentSubPoint = _ssl.Low.Last(1);
                        mainPoint = _ssl.High.Last(i);
                        subPoint = _ssl.Low.Last(i);
                        prevMainPoint = _ssl.High.Last(i + 1);
                        prevSubPoint = _ssl.Low.Last(i + 1);
                        break;
                }
                // Red point must above green point
                if (signal == TradeSignal.Sell && prevSubPoint < prevMainPoint && subPoint > mainPoint)
                {
                    result = true && currentMainPoint < currentSubPoint;
                    break;
                }
                // Green point must above red point
                if (signal == TradeSignal.Buy && prevSubPoint > prevMainPoint && subPoint < mainPoint)
                {
                    result = true && currentMainPoint > currentSubPoint;
                    break;
                }
            }

            return result;
        }
        #endregion

        #region C2 Indicator
        public bool IsC2Confirm(TradeSignal signal)
        {
            // HIDE C2
        }
        #endregion

        #region Exit Indicator
        public bool IsExitPosition(Position pos)
        {
            // HIDE EXIT
        }
        #endregion

        #region Volume Indicator
        public bool IsVolumeConfirm()
        {
            // HIDE VOLUME
        }
        #endregion
    }
}
