using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    class Method_3_EndDiff : Method
    {
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };
        double[] eps = { 0.1, 0.001, 0.00001 };
        double initZ = 0;

        public Method_3_EndDiff(List<string> exp, double Zlen) : base(exp) { initZ = Zlen; }

        public override void ToCalculate()
        {

            for (int k = 0; k < eps.Length; k++)
            {
                Console.WriteLine("\nEpsilon:{0}", eps[k]);
                for (int j = 0; j < hstep.Length; j++)
                {
                    //start conditions
                    double X0 = 0.1;
                    List<double> Y0 = new List<double>() { 1, -2 };
                    double[] range = new double[] { 0.1, 10 };

                    //for counting
                    int Nctr = 0; double maxerr = 0, err = 0;

                    //1//
                    List<double> Xnodes = new List<double>();
                    //h=-tau0;
                    for (int i = -3; i <= 0; i++) { Xnodes.Add(X0 + hstep[j] * i); }

                    List<List<double>> Ynodes = new List<List<double>>();  //starting conditions
                    Ynodes.Add(Y0);
                    for (int i = 2; i >= 0; i--)
                    {
                        //ycalc is a func that was taken from previous Euler method
                        Ynodes.Insert(0, Ycalc(Ynodes[0], Xnodes[i], -hstep[j]));

                        err = 0;
                        for (int l = 0; l < Ynodes[0].Count; l++) { err += Math.Pow(Ynodes[0][l] - Ethalon(Xnodes[i])[l], 2); }
                        if (err > maxerr) maxerr = err;

                    }
                    Nctr = Ynodes.Count;

                    //h=z
                    double ZnodeX = X0 + initZ;
                    List<double> ZnodeY = Ycalc(Y0, X0, initZ);

                    //2//
                    //max of 4th derivatives
                    double K = InterPolyLagrDeriv(Xnodes, ZnodeX, Ynodes, ZnodeY);

                    //3//
                    while (Xnodes[Xnodes.Count - 1] >= range[0] && Xnodes[Xnodes.Count - 1] <= range[1]) //stopcond tomake
                    {
                        //4//
                        double taui_1 = Xnodes[Xnodes.Count - 1] - Xnodes[Xnodes.Count - 2];
                        double delta = 2 * eps[k] / K;
                        //approximative calculation due to 5th grade equation doesnt have accurate decision, accuracy here is 1e-6
                        double taui = Fi_calc(taui_1, delta);
                        //if (taui == 0) break;

                        //5//
                        for (int i = 0; i < 2; i++)
                        {
                            List<double> buf = Ycalc(Ynodes[Ynodes.Count - 1], Xnodes[Xnodes.Count - 1] + taui, taui);
                            Ynodes.Add(GaussLinearDecision(Xnodes, Ynodes, taui));

                            Xnodes.Add(Xnodes[Xnodes.Count - 1] + taui);
                            Nctr++;

                            err = 0;
                            for (int l = 0; l < Ynodes[Ynodes.Count - 1].Count; l++) { err += Math.Pow(Ynodes[Ynodes.Count - 1][l] - Ethalon(Xnodes[Xnodes.Count - 1])[l], 2); }
                            err = Math.Sqrt(err);
                            if (err > maxerr) maxerr = err;
                        }

                        //6//
                        initZ = Math.Max(initZ, taui);

                        if (Xnodes[Xnodes.Count - 1] > ZnodeX)
                        {
                            ZnodeY = Ycalc(Ynodes[Ynodes.Count - 1], Xnodes[Xnodes.Count - 1], initZ);
                            ZnodeX = Xnodes[Xnodes.Count - 1] + initZ;
                            K = InterPolyLagrDeriv(Xnodes, ZnodeX, Ynodes, ZnodeY);
                        }
                    }
                    ADDING_DATA_FOR_SAVE(hstep[j], Nctr, maxerr);
                    ToWriteConsole(hstep[j], Nctr, maxerr);
                }
            }
        }

        List<double> GaussLinearDecision(List<double> Xnodes, List<List<double>> Ynodes, double taui)
        {
            double hi = taui, hi_1 = Xnodes[Xnodes.Count - 1] - Xnodes[Xnodes.Count - 2], hi_2 = Xnodes[Xnodes.Count - 2] - Xnodes[Xnodes.Count - 3];
            double psii = hi + hi_1 + hi_2;

            double ai = (hi * hi + hi_1 * hi) * psii / (hi * (hi_1 * hi_1 + hi_1 * hi_2 + 2 * hi * hi_2 + 3 * hi * hi + 4 * hi_1 * hi));
            double ai_1 = (hi + hi_1) * (hi + hi_1) * psii * psii / (hi_1 * (hi_2 + hi_1) * (hi_1 * hi_1 + hi_1 * hi_2 + 2 * hi * hi_2 + 3 * hi * hi + 4 * hi_1 * hi));
            double ai_2 = -hi * hi * psii * psii / (hi_1 * hi_2 * (hi_1 * hi_1 + hi_1 * hi_2 + 2 * hi * hi_2 + 3 * hi * hi + 4 * hi_1 * hi));
            double ai_3 = 1 - ai_2 - ai_1;

            List<List<double>> B = new List<List<double>>(); List<double> b = new List<double>();
            LinearComponentsCalc(ref B, ref b, Xnodes[Xnodes.Count - 1] + hi);

            List<List<double>> E = getEmatrix(B.Count);

            List<List<double>> C = new List<List<double>>();
            for (int i = 0; i < B.Count; i++)
            {
                C.Add(new List<double>());
                for (int j = 0; j < B.Count; j++)
                {
                    C[i].Add(E[i][j] - hi * ai * B[i][j]);
                }
            }

            List<double> c = new List<double>();
            for (int i = 0; i < B.Count; i++)
            {
                double value = ai_3 * Ynodes[Ynodes.Count - 3][i] + ai_2 * Ynodes[Ynodes.Count - 2][i] + ai_1 * Ynodes[Ynodes.Count - 1][i] + hi * ai * b[i];
                c.Add(value);
            }

            //
            for (int i = 0; i < C.Count; i++)
            {
                for (int j = 0; j < C.Count; j++)
                {
                    if (i != j)
                    {
                        double coef = C[j][i] / C[i][i];
                        for (int k = 0; k < C.Count; k++)
                        {
                            C[j][k] -= C[i][k] * coef;
                        }
                        c[j] -= c[i] * coef;

                        if (C[j][j] > 0 && C[j][j] != 1)
                        {
                            coef = C[j][j];
                            for (int k = 0; k < C.Count; k++)
                            {
                                C[j][k] /= coef;
                            }
                            c[j] /= coef;
                        }
                    }
                }
            }

            return c;
        }

        List<List<double>> getEmatrix(int len)
        {
            List<List<double>> E = new List<List<double>>();
            for (int i = 0; i < len; i++)
            {
                E.Add(new List<double>());
                for (int j = 0; j < len; j++)
                {
                    if (i == j) E[i].Add(1);
                    else E[i].Add(0);
                }
            }
            return E;
        }

        void LinearComponentsCalc(ref List<List<double>> B, ref List<double> b, double Xi)
        {
            List<List<double>> Bbuf = new List<List<double>>(); List<double> bbuf = new List<double>();
            foreach (List<string> el in expression) { bbuf.Add(0); }

            List<int> Yinds = new List<int>();

            for (int i = 0; i < expression.Count; i++)
            {
                double val = 1;
                List<double> Bi = new List<double>();
                foreach (string el in expression[i])
                {
                    switch (el)
                    {
                        case "-": val *= -1; break;
                        case "*": Bi.Add(val); val = 1; break;
                        case "y1": break;
                        case "y2": break;
                        case "+": break;
                        default: val *= Convert.ToDouble(el); break;
                    }
                    if (el == "y2") { Yinds.Add(expression[i].IndexOf(el)); break; }
                }
                Bbuf.Add(Bi);
            }

            List<List<string>> subexp = new List<List<string>>();
            for (int i = 0; i < expression.Count; i++)
            {
                subexp.Add(new List<string>());
                for (int j = Yinds[i] + 1; j < expression[i].Count; j++) { subexp[i].Add(expression[i][j]); }
            }
            bbuf = ToInterpreteExp(Xi, new List<double>() { 0, 0 }, subexp);

            B = Bbuf; b = bbuf;

        }

        List<double> Ycalc(List<double> Yi, double Xi, double step) //Euler Y quick 1-step calculation
        {
            List<double> F = ToInterpreteExp(Xi, Yi, expression);
            List<double> Ynew = new List<double>();//
            for (int i = 0; i < Yi.Count; i++)
            {
                Ynew.Add(Yi[i] + step * F[i]);
            }
            return Ynew;
        }

        double InterPolyLagrDeriv(List<double> Xnodes, double ZnodeX, List<List<double>> Ynodes, List<double> ZnodeY)
        {
            //we want a derivative 4th grade of an interpolation Lagrange's polynome 
            //with 5 points we only need koefs at 4th power of Xs
            List<double> result = new List<double>();
            for (int i = 0; i < Ynodes[0].Count; i++) { result.Add(0); }

            for (int i = 0; i < Ynodes[0].Count; i++)
            {
                double basispoly;
                for (int j = Ynodes.Count - 1; j > Ynodes.Count - 5; j--)
                {
                    //we build a basis polynome value excluding Znode
                    basispoly = 1;
                    for (int k = Xnodes.Count - 1; k > Xnodes.Count - 5; k--)
                    {
                        if (j != k) basispoly *= (double)1 / (Xnodes[j] - Xnodes[k]);
                    }
                    basispoly *= (double)1 / (Xnodes[j] - ZnodeX);
                    //and add a multiplication with an Yj corresponding
                    result[i] += Ynodes[j][i] * basispoly;
                }
                //here we adding Znode separately
                basispoly = 1;
                for (int k = Xnodes.Count - 1; k > Xnodes.Count - 5; k--) { basispoly *= (double)1 / (ZnodeX - Xnodes[k]); }
                result[i] += ZnodeY[i] * basispoly;
            }

            //search for max value of ones we got
            double maxval = double.MinValue;
            foreach (double el in result) { if (maxval < el) maxval = el; }
            //4th derivative is multiplied by 4 factorial
            return maxval * 24;
        }

        double Fi_calc(double taui_1, double delta)
        {
            //made it (approximative) for fi-func calculation due to given polynome is a 5th grade one and doesnt have an exact decision method
            bool tostart = true, wasbelowzero = false;
            double fistep = 0.1, curr = 0;
            char prev = ' ', cur = ' ';

            while (Math.Abs(fistep) > 0.000000001)
            {
                double res = 14 * Math.Pow(curr, 5) + 12 * taui_1 * Math.Pow(curr, 4) + 3 * Math.Pow(taui_1, 2) * Math.Pow(curr, 3) - 8 * delta * curr - 3 * delta * taui_1;
                if (res >= 0) cur = '+'; else cur = '-';
                if (tostart) { prev = cur; tostart = false; }

                if (!wasbelowzero && res <= 0) wasbelowzero = true;

                if (prev != cur) { fistep /= -10; }
                curr += fistep;
                prev = cur;
                if (!wasbelowzero)
                {
                    break;
                }
            }
            return curr;
        }

        public override void ToWriteConsole(double h, int Nctr, double err) //пока не вынес в абстрактный класс поскольку не знаю какой вывод понадобится для 3 метода
        {
            Console.WriteLine("h={0}   \tN={1}    \terr={2:f6}", h, Nctr, err);
        }


    }
}
