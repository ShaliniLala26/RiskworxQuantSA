﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantSA.Excel.Addin.Functions;

namespace QuantSA.Excel.Addin.Tests.Addin
{
    public class CustomTypeForTests { }
    [TestClass]
    public class ExcelTypeConverterTests
    {
        [TestMethod]
        public void ExcelTypeConverter_ExcelTypeConverter_object2D()
        {
            var o2 = new object[1, 1];
            Assert.IsFalse(ExcelTypeConverter.ShouldUseReference(o2.GetType()));
        }

        [TestMethod]
        public void ExcelTypeConverter_ExcelTypeConverter_Custom2D()
        {
            var o2 = new CustomTypeForTests[1, 1];
            Assert.IsTrue(ExcelTypeConverter.ShouldUseReference(o2.GetType()));
        }
    }
}
