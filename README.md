# CTrader Bot & Indicators

## Table of contents
- [Overview](#overview)
- [Project Structure](#project-structure)

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