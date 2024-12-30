using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basalin__Lab
{
    class Method_1_Euler : Method
    {
       
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001};

        public Method_1_Euler(List<string> exp) : base(exp) { met_name = "Method of Euler"; }

        public override void ToCalculate()
        {
            if (choice == 1) ToCalculateExpressionFromString();

            else ToCalculateElectrosystem();
        }


         void ToCalculateExpressionFromString()
        {        
            double[] range = new double[]{ 0.1, 10 };

            for (int j = 0; j < hstep.Length; j++)
            {
                double Xi = 0.1, err=0; 
                List<double> Y = new List<double>();   //starting conditions
                 Y.Add(0.543657082); Y.Add(-1.31548562239629);
                double maxerr = 0;
                int Nctr = 0;

                while (Xi >= range[0] && Xi <= range[1])
                {
                    Xi += hstep[j];
                    Y = Ycalc(Y, Xi, hstep[j]);

                    err = 0;
                    for (int i = 0; i < Y.Count; i++) { err += Math.Pow(Y[i] - Ethalon( Xi)[i], 2); }
                    err = Math.Sqrt(err);
                    if (err > maxerr) maxerr = err;

                    Nctr++;
                }
                
                ADDING_DATA_FOR_SAVE(hstep[j], Nctr, maxerr);
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        public override List<double> Ycalc(List<double> Yi, double Xi, double step) {
            List<double> F = ToInterpreteExp(Xi, Yi, expression);
            List<double> Ynew = new List<double>();//
            for(int i=0;i<Yi.Count;i++) {
                Ynew.Add(Yi[i] + step*F[i]);
            }
            return Ynew;
        }

          void ToCalculateElectrosystem()
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
                X = Electrosystem_Calculation_Euler_X(X, step);
                Y = Electrosystem_Calculation_Euler_Y( X, step);
                //ConsoleElectrosystemResult(X, Y, Nctr);


                resultsX.Add(X); resultsY.Add(Y);
            }

        }

        public override void ToWriteConsole(double h, int Nctr, double err)
        {
            Console.WriteLine("h={0}   \tN={1}    \terr={2:f6}", h, Nctr, err);
        }
    }
}
