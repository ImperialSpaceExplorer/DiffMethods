using System;

namespace MethodsKoshi
{
    class Program
    {
        static void Main(string[] args)
        {
            string exp1 = "- 4 * y1 - 2 * y2 + 2 / ( e ^ x - 1 )";
            string exp2 = "6 * y1 + 3 * y2 - 3 / ( e ^ x - 1 )";

            Method_1_Euler m1 = new Method_1_Euler(new System.Collections.Generic.List<string>() { exp1, exp2});
            //m1.ToCalculate();

            Method_2_RungeKutta m2 = new Method_2_RungeKutta(new System.Collections.Generic.List<string>() { exp1, exp2 });
            //m2.ToCalculate();

            double z = 0.1;
            Method_3_EndDiff m3 = new Method_3_EndDiff(new System.Collections.Generic.List<string>() { exp1, exp2 }, z);
            m3.ToCalculate();
        }
    }
}
