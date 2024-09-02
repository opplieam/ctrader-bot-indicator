# CTrader Bot & Indicators

## Table of contents
- [Overview](#overview)
- [Project Structure](#project-structure)
- [Automate Trading](#automate-trading)
    * [Back Testing Result](#back-testing-result)
- [Indicators](#indicators)
    * [Kiju-sen MTF](#kiju-sen-multi-timeframe)
    * [KAMA && KAMA Slope](#kama--kama-slope)
    * [SSL MTF](#ssl-multi-timeframe)
    * [Quantitative Qualitative Estimation](#quantitative-qualitative-estimation)

## Overview
This repository contains some of my custom indicators and bot (Automate Trading) used in CTrader platform. Written in C#. 

> All indicators are fully released.

> Automate trading is partially released. I have no plan to release full code or reveal full algorithm. This is for demonstrating purpose.

## Project Structure
```
├── HGL                     # Automate Trading directory
│   ├── HGL
│   │   ├── HGL.cs
│   │   ├── HGL.csproj
│   │   ├── Properties
│   │   │   └── AssemblyInfo.cs
│   └── HGL.sln
├── Images
├── KAMA                    # KAMA Indicator directory
│   ├── KAMA
│   │   ├── KAMA.cs
│   │   ├── KAMA.csproj
│   │   ├── Properties
│   │   │   └── AssemblyInfo.cs
│   └── KAMA.sln
├── KAMASlope               # KAMA Slope Indicator directory
│   ├── KAMASlope
│   │   ├── KAMASlope.cs
│   │   ├── KAMASlope.csproj
│   │   ├── Properties
│   │   │   └── AssemblyInfo.cs
│   └── KAMASlope.sln
├── KijusenMTF              # Kiju-sen multi timeframe Indicator directory
│   ├── KijusenMTF
│   │   ├── KijusenMTF.cs
│   │   ├── KijusenMTF.csproj
│   │   ├── Properties
│   │   │   └── AssemblyInfo.cs
│   └── KijusenMTF.sln
├── QualitativeQuantitativeE # QQE Indicator directory
│   ├── QualitativeQuantitativeE
│   │   ├── Properties
│   │   │   └── AssemblyInfo.cs
│   │   ├── QualitativeQuantitativeE.cs
│   │   ├── QualitativeQuantitativeE.csproj
│   └── QualitativeQuantitativeE.sln
└── SSLMTF                  # SSL Multi timeframe indicator directory
    ├── SSLMTF
    │   ├── Properties
    │   │   └── AssemblyInfo.cs
    │   ├── SSLMTF.cs
    │   ├── SSLMTF.csproj
```

## Automate Trading
HGL Stand for "Holy Grail Latest(version)". There are variation like Holy Grail H4 (Design for 4 Hour candles), Holy Grail V2 and so on. The Latest variation is the most powerful bot. It's design for trending pattern but can also use in sideway pattern

The bot is automating calculate a LOT size, When to enter, When to exit, risk management, sideway pattern detection. Base on user input.

The algorithm behind this bot is very complicated. Example. 
- When take a profit threshold reached, Bot is closing half of your current LOT and let it run until the exit indicator trigger. This is translated to "Don't greedy, take half of the profit then ride the trend until your exit indicator told you to leave." So It's guarantee the profit and let the algorithm decide when to fully take profit. Don't use emotional.
- This bot is dynamic calculate how many pips for take profit and stop loss using `average true range` and the multiplier because difference market have difference order flow. So to avoid headache, consistency and make a bot to be use in every markets, Calculating with `average true range` is the best choice.

There are more logic behind the scene.

Some important user input
- Risk % 
    * How much risk are you going to take per trade in percentage 
    * Ex. 2% mean, you are going to lose 2% of your capital if you hit a stop loss per trade.
    * More risk, More profit. Less risk, Less profit
    * Suggestion is 2% per trade. (default)
- Take profit Multiplier / Stop loss Multiplier
    * The multiplier that will be use in the calculation with `average to range`
    * Suggestion is 1 / 1.5 (default) or 1 / 1 
- Use Slope
    * If it enabled, the bot is skipped open trading if the market is sideway.
    * Use in combination with `Slope Threshold`

You can fine tune other parameters base on the market.

### Back Testing result
Market: BTCUSD

Timeframe: 4HR

Date: 01/01/2023 - 31/08/2024

Result: +51%

![back-testing](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/Back-Testing.PNG?raw=true)
![stat](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/Trade-Stat.PNG?raw=true)
![Equity](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/Equity.PNG?raw=true)
![history](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/History.PNG?raw=true)
![param1](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/Param1.PNG?raw=true)
![param2](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/BTCUSDH4/Param2.PNG?raw=true)


Market: XAUUSD

Timeframe: 4HR

Date: 01/01/2023 - 31/08/2024

Result: +183%

![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/Back-Testing.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/Trade-Stat.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/Equity.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/History.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/Param1.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/XAUUSDH4/Param2.PNG?raw=true)

## Indicators
### Kiju-sen Multi Timeframe
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/Kijusen-MTF.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/Kijusen-MTF-Setting.PNG?raw=true)

The custom Kiju-sen Indicator which can draw the Kiju-sen from another timeframe. 

The graph is H1 timeframe, The white dot line is Kiju-sen from H1 timeframe. The yellow dot line is Kiju-sen from H4 timeframe.

### KAMA & KAMA Slope
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/KAMA+Slope.PNG?raw=true)

KAMA (Kaufman’s Adaptive Moving Average) Slope is the custom indicator to calculate slope of KAMA which can be identifying the trend or sideway pattern. The 0 value represent a sideway. +/- are bull/bear. 

### SSL Multi Timeframe
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/SSL-MTF.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/SSL-MTF-Setting.PNG?raw=true)

The custom SSL (Semaphore Signal Level channel) Indicator which can draw the SSL from another timeframe.

The graph is 30Min timeframe. The SSL is from H4 timeframe.

### Quantitative Qualitative Estimation
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/QQE.PNG?raw=true)
![](https://github.com/opplieam/ctrader-bot-indicator/blob/main/Images/QQE-Setting.PNG?raw=true)

The community using a wrong math calculation. So this is my own solution.