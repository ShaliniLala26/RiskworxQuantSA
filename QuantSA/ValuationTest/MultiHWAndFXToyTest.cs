﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantSA.Valuation;
using QuantSA.Valuation.Models;
using QuantSA.General;
using Accord.Math;
using System.Linq;
using System.Collections.Generic;
using Accord.Statistics;

namespace ValuationTest
{
    [TestClass]
    public class MultiHWAndFXToyTest
    {

        [TestMethod]
        public void TestMultiHWAndFXToyForwards()
        {
            Date valueDate = new Date(2016, 9, 17);
            HullWhite1F zarRatesSim = new HullWhite1F(Currency.ZAR, 0.05, 0.01, 0.07, 0.07, valueDate);
            zarRatesSim.AddForecast(FloatingIndex.JIBAR3M);
            HullWhite1F usdRatesSim = new HullWhite1F(Currency.USD, 0.05, 0.0000001, 0.01, 0.01, valueDate);
            usdRatesSim.AddForecast(FloatingIndex.LIBOR3M);
            HullWhite1F eurRatesSim = new HullWhite1F(Currency.EUR, 0.05, 0.0000001, 0.005, 0.005, valueDate);
            eurRatesSim.AddForecast(FloatingIndex.EURIBOR3M);

            CurrencyPair[] currencyPairs = new CurrencyPair[] { new CurrencyPair(Currency.USD, Currency.ZAR), new CurrencyPair(Currency.EUR, Currency.ZAR) };
            double[] spots = new double[] { 13.6, 15.0 };
            double[] vols = new double[] { 0.15, 0.15 };
            double[,] correlations = new double[,] { { 1.0, 0.0 },
                                                     { 0.0, 1.0 } };
            MultiHWAndFXToy model = new MultiHWAndFXToy(valueDate, Currency.ZAR, new HullWhite1F[] { zarRatesSim, usdRatesSim, eurRatesSim },
                                    currencyPairs, spots, vols, correlations);

            List<Date> simDates = new List<Date>();
            simDates.Add(valueDate.AddMonths(24));
            model.Reset();
            model.SetNumeraireDates(simDates);
            model.SetRequiredDates(currencyPairs[0], simDates); // Will simulate both currency pairs
            model.Prepare();

            int N = 100;
            double[,] fwdSpotValues = Matrix.Zeros(N, 5);
            
            for (int i = 0; i < N; i++)
            {
                model.RunSimulation(i);
                fwdSpotValues[i, 0] = model.GetIndices(currencyPairs[0], simDates)[0];
                fwdSpotValues[i, 1] = model.GetIndices(currencyPairs[1], simDates)[0];
                fwdSpotValues[i, 2] = model.GetIndices(FloatingIndex.JIBAR3M, simDates)[0];
                fwdSpotValues[i, 3] = model.GetIndices(FloatingIndex.LIBOR3M, simDates)[0];
                fwdSpotValues[i, 4] = model.GetIndices(FloatingIndex.EURIBOR3M, simDates)[0];
            }
            double meanUSDZAR = fwdSpotValues.GetColumn(0).Mean();
            double meanEURZAR = fwdSpotValues.GetColumn(1).Mean();


        }


        private FloatLeg CreateFloatingLeg(Currency ccy, Date startDate, double notional, FloatingIndex index, int tenorYears)
        {
            int quarters = tenorYears / 4;
            Date[] paymentDates = Enumerable.Range(1, quarters).Select(i => startDate.AddMonths(3 * i)).ToArray();
            Date[] resetDates = Enumerable.Range(0, quarters-1).Select(i => startDate.AddMonths(3 * i)).ToArray();
            double[] notionals = Vector.Ones(quarters).Multiply(notional);
            double[] spreads = Vector.Zeros(quarters);
            double[] accrualFractions = Vector.Ones(quarters).Multiply(0.25);
            FloatingIndex[] floatingIndices = Enumerable.Range(1, quarters).Select(i => index).ToArray();
            FloatLeg leg = new FloatLeg(ccy, paymentDates, notionals, resetDates, floatingIndices, spreads, accrualFractions);
            return leg;
        }

        /// <summary>
        /// Tests the <see cref="MultiHWAndFXToy"/> with respect to generating PFEs on a portfolio of CCIRSs
        /// </summary>
        [TestMethod]
        public void TestMultiHWAndFXToyCCIRS()
        {
            Date valueDate = new Date(2016, 9, 17);
            HullWhite1F zarRatesSim = new HullWhite1F(Currency.ZAR, 0.05, 0.01, 0.07, 0.07, valueDate);
            zarRatesSim.AddForecast(FloatingIndex.JIBAR3M);
            HullWhite1F usdRatesSim = new HullWhite1F(Currency.USD, 0.05, 0.01, 0.01, 0.01, valueDate);
            usdRatesSim.AddForecast(FloatingIndex.LIBOR3M);
            HullWhite1F eurRatesSim = new HullWhite1F(Currency.EUR, 0.05, 0.01, 0.005, 0.005, valueDate);
            eurRatesSim.AddForecast(FloatingIndex.EURIBOR3M);

            CurrencyPair[] currencyPairs = new CurrencyPair[] { new CurrencyPair(Currency.USD, Currency.ZAR), new CurrencyPair(Currency.EUR, Currency.ZAR) };
            double[] spots = new double[] { 13.6,  15.0 };
            double[] vols = new double[] { 0.15, 0.15 };
            double[,] correlations = new double[,] { { 1.0, 0.0 }, 
                                                     { 0.0, 1.0 } };
            MultiHWAndFXToy model = new MultiHWAndFXToy(valueDate, Currency.ZAR, new HullWhite1F[] { zarRatesSim, usdRatesSim, eurRatesSim },
                                    currencyPairs, spots, vols, correlations);

            List<Product> portfolio = new List<Product>();
            portfolio.Add(CreateFloatingLeg(Currency.ZAR, valueDate, 15e6, FloatingIndex.JIBAR3M, 7));
            portfolio.Add(CreateFloatingLeg(Currency.EUR, valueDate, -1e6, FloatingIndex.EURIBOR3M, 7));
            portfolio.Add(CreateFloatingLeg(Currency.ZAR, valueDate, 13e6, FloatingIndex.JIBAR3M, 13));
            portfolio.Add(CreateFloatingLeg(Currency.USD, valueDate, -1e6, FloatingIndex.EURIBOR3M, 13));
            portfolio.Add(IRSwap.CreateZARSwap(0.07, true, 20e6, valueDate, Tenor.Years(4)));


        }
    }
}