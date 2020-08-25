﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantSA.Shared.Conventions.DayCount;
using QuantSA.Shared.Dates;

namespace QuantSA.Shared.Test.Conventions.DayCount
{
    [TestClass]
    public class DayCountTests
    {
        [TestMethod]
        public void TestDayCounts()
        {
            var date1 = new Date(2003, 11, 1);
            var date2 = new Date(2004, 5, 1);

            // Actual/365
            Assert.AreEqual(182.0 / 365, DayCountStore.Actual365Fixed.YearFraction(date1, date2), 1e-9);

            // Actual/360
            Assert.AreEqual(182.0 / 360, DayCountStore.Actual360.YearFraction(date1, date2), 1e-9);

            // Actual/Actual ISDA
            Assert.AreEqual(61.0 / 365 + 121.0 / 366, DayCountStore.ActActISDA.YearFraction(date1, date2), 1e-9);
            Assert.AreEqual(0.497724380567, DayCountStore.ActActISDA.YearFraction(date1, date2), 1e-9); // QuantLib case

            // 30/360
            date1 = new Date(2008, 2, 28);
            date2 = new Date(2008, 3, 31);
            Assert.AreEqual(32.0 / 360, DayCountStore.Thirty360Euro.YearFraction(date1, date2), 1e-9); // QuantLib case

            // Business 252
            var weekendsOnly = new Calendar("Test", new List<Date>());
            var weekendsAndOneHoliday = new Calendar("Test", new List<Date> {new Date(2008, 3, 21)});
            IDayCountConvention business252Weekends = DayCountStore.Business252(weekendsOnly);
            IDayCountConvention business252WeekendsAndHolday = DayCountStore.Business252(weekendsAndOneHoliday);
            Assert.AreEqual(22.0 / 252, business252Weekends.YearFraction(date1, date2), 1e-9);
            Assert.AreEqual(21.0 / 252, business252WeekendsAndHolday.YearFraction(date1, date2), 1e-9);
        }
    }
}