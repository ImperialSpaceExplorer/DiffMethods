using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    class Method_1_Euler : Method
    {
       
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001};

        public Method_1_Euler(List<string> exp) : base(exp) { }

        public override void ToCalculate()
        {        
            double[] range = new double[]{ 0.1, 10 };

            for (int j = 0; j < hstep.Length; j++)
            {
                double Xi = 0.1, err=0; 
                List<double> Y = new List<double>();   //starting conditions
                Y.Add(1); Y.Add(-2);
                double maxerr = 0;
                int Nctr = 0;

                while (Xi >= range[0] && Xi <= range[1])
                {
                    Xi += hstep[j];
                    Y = Ycalc(Y, Xi, hstep[j]);

                    err = 0;
                    for (int i = 0; i < Y.Count; i++) { err += Math.Pow(Y[i] - Ethalon( Xi)[i], 2); }
                    if (err > maxerr) maxerr = err;

                    Nctr++;
                }
                
                ADDING_DATA_FOR_SAVE(hstep[j], Nctr, maxerr);
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        List<double> Ycalc(List<double> Yi, double Xi, double step) {
            List<double> F = ToInterpreteExp(Xi, Yi, expression);
            List<double> Ynew = new List<double>();//
            for(int i=0;i<Yi.Count;i++) {
                Ynew.Add(Yi[i] + step*F[i]);
            }
            return Ynew;
        }

        List<double> Ethalon( double Xi) {  //можно вынести на пользовательский ввод
            List<double> res = new List<double>();
            res.Add(1 + 4 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            res.Add( -2 - 6 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            return res;
        }

        public override void ToWriteConsole(double h, int Nctr, double err)
        {
            Console.WriteLine("h={0}   N={1}    err={2:f6}", h, Nctr, err);
        }
    }
}
