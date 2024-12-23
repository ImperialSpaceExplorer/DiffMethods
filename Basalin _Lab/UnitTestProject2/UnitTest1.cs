using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Basalin__Lab;
using System.Collections.Generic;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string exp1 = "- 4 * y1 - 2 * y2 + 2 / ( e ^ x - 1 )";
            string exp2 = "6 * y1 + 3 * y2 - 3 / ( e ^ x - 1 )";

            Method_1_Euler method = new Method_1_Euler(new List<string> { exp1, exp2 });

            double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };
            double Xi = 0.1;
            List<double> Y = new List<double>();
            Y.Add(1); Y.Add(-2);
            List<double> result = method.Ycalc(Y, Xi, hstep[0]);
            double numb1 = 1.1901666388955008;
            double numb2 = -2.2852499583432513;
            List<double> check = new List<double>() { numb1, numb2};
            CollectionAssert.AreEqual(result, check);
        }


        [TestMethod]
        public void TestMethod2()
        {
            string exp1 = "- 4 * y1 - 2 * y2 + 2 / ( e ^ x - 1 )";
            string exp2 = "6 * y1 + 3 * y2 - 3 / ( e ^ x - 1 )";

            Method_2_RungeKutta method = new Method_2_RungeKutta(new List<string> { exp1, exp2 });

            double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };
            double Xi = 0.1;
            List<double> Y = new List<double>();
            Y.Add(1); Y.Add(-2);
            List<double> result = method.Ycalc(Y, Xi, hstep[0]);
            double numb1 = 1.1798794256986063;
            double numb2 = -2.2698191385479096;
            List<double> check = new List<double>() { numb1, numb2 };
            CollectionAssert.AreEqual(result, check);
        }

        [TestMethod]
        public void TestMethod3()
        {
            string exp1 = "- 4 * y1 - 2 * y2 + 2 / ( e ^ x - 1 )";
            string exp2 = "6 * y1 + 3 * y2 - 3 / ( e ^ x - 1 )";

            double z = 0.1;
            Method_3_EndDiff method = new Method_3_EndDiff(new List<string>() { exp1, exp2 }, z);

            double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };
            double Xi = 0.1;
            List<double> Y = new List<double>();
            Y.Add(1); Y.Add(-2);
            List<double> result = method.Ycalc(Y, Xi, hstep[0]);
            double numb1 = 1.1901666388955008;
            double numb2 = -2.2852499583432513;
            List<double> check = new List<double>() { numb1, numb2 };
            CollectionAssert.AreEqual(result, check);
        }
    }
}
