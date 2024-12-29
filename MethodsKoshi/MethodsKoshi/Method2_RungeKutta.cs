using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    class Method_2_RungeKutta : Method
    {
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };

        public Method_2_RungeKutta(List<string> exp) : base(exp) { met_name = "Method of Runge-Kutta"; }

        public override void ToCalculate()
        {
            if (choice == 1) ToCalculateExpressionFromString();

            else ToCalculateElectrosystem();
        }


        public void ToCalculateElectrosystem()
        {

            double step = hstep[0];
            double t = 0, tmax = 10;
            List<double> X = new List<double>() { J * R2, 0 };
            List<double> Y = Y_expression(X);   //starting conditions

            resultsX.Add(X); resultsY.Add(Y);

            int Nctr = 0;

            //ConsoleElectrosystemResult(X, Y, Nctr);

            while (t < tmax)
            {
                Nctr++;
                t += step;
                X = Electrosystem_Calculation_Runge_X(X, step);
                Y = Electrosystem_Calculation_Runge_Y(X, step);
                //ConsoleElectrosystemResult(X, Y, Nctr);

                resultsX.Add(X); resultsY.Add(Y);
            }

        }
        List<double> Electrosystem_Calculation_Runge_X(List<double> Xi, double step)
        {

            List<double> k1 = X_expression(Xi);

            List<double> buf = new List<double>();
            for (int j = 0; j < Xi.Count; j++) { buf.Add(Xi[j] + step * k1[j] / 2); }
            List<double> k2 = X_expression(buf);

            buf.Clear(); for (int j = 0; j < Xi.Count; j++) { buf.Add(Xi[j] + step * k2[j] / 2); }
            List<double> k3 = X_expression(buf);

            buf.Clear(); for (int j = 0; j < Xi.Count; j++) { buf.Add(Xi[j] + step * k3[j]); }
            List<double> k4 = X_expression(buf);


            List<double> Xnew = new List<double>();//
            for (int i = 0; i < Xi.Count; i++)
            {
                Xnew.Add(Xi[i] + step / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]));
            }
            return Xnew;
        }

        List<double> Electrosystem_Calculation_Runge_Y(List<double> Xi, double step)
        {
            return Y_expression(Xi);
        }

        void ToCalculateExpressionFromString()
        {
            double[] range = new double[] { 0.1, 10 };

            for (int j = 0; j < hstep.Length; j++)
            {
                double Xi = 0.1, err = 0;
                List<double> Y = new List<double>();   //starting conditions
                Y.Add(0.543657082); Y.Add(-1.31548562239629);
                double maxerr = 0;
                int Nctr = 0;

                while (Xi >= range[0] && Xi <= range[1])
                {

                    Y = Ycalc(Y, Xi, hstep[j]);
                    Xi += hstep[j];

                    err = 0;
                    for (int i = 0; i < Y.Count; i++) { if (err < Y[i] - Ethalon(Xi)[i]) err = Y[i] - Ethalon(Xi)[i]; }
                    if (err > maxerr) maxerr = err;
                    Nctr++;
                }
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        List<double> Ycalc(List<double> Yi, double Xi, double step)
        {
            List<double> k1 = ToInterpreteExp(Xi, Yi, expression);

            List<double> buf = new List<double>();
            for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + step * k1[j] / 2); }
            List<double> k2 = ToInterpreteExp(Xi + step / 2, buf, expression);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + step * k2[j] / 2); }
            List<double> k3 = ToInterpreteExp(Xi + step / 2, buf, expression);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + step * k3[j]); }
            List<double> k4 = ToInterpreteExp(Xi + step, buf, expression);

            List<double> Ynew = new List<double>();//
            for (int i = 0; i < Yi.Count; i++)
            {
                Ynew.Add(Yi[i] + step / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]));
            }
            return Ynew;
        }

        public override void ToWriteConsole(double h, int Nctr, double err) //
        {
            Console.WriteLine("h={0}   N={1}    err={2:f9}", h, Nctr, err);
        }
    }
}
