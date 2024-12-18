using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    abstract class Method : IMethod
    {

        protected List<List<string>> expression;

        public Method(List<string> expression)
        {
            this.expression = new List<List<string>>();
            foreach (string exp in expression)
            {
                this.expression.Add( exp.Split(" ").ToList());
            }
        }

        public abstract void ToCalculate();

        protected List<double> ToInterpreteExp(double Xi, List<double> Y)   //let exp be = "Y1' = y1 - y2 + e ^ x"
        {
            int listctr = 0;

            List<double> res = new List<double>();

            List<double> values;  //values of an expression

            List<string> operators; //+ - * / ^ ()

            while (listctr < expression.Count)
            {
                values = new List<double>(); operators = new List<string>();
                foreach (string el in expression[listctr])
                {
                    switch (el)
                    {
                        case "y1": values.Add(Y[0]); break;

                        case "y2": values.Add(Y[1]); break;

                        case "e": values.Add(Math.E); break;

                        case "x": values.Add(Xi); break;

                        case "+": operators.Add("+"); break;

                        case "-": operators.Add("-"); break;

                        case "*": operators.Add("*"); break;

                        case "/": operators.Add("/"); break;

                        case "^": operators.Add("^"); break;

                        case "(": operators.Add("("); break;

                        case ")": operators.Add(")"); break;

                        default: values.Add(Convert.ToDouble(el)); break;
                    }
                }

                if(operators.Count>=values.Count && operators[0]=="-"){ values[0] *= -1; operators.RemoveAt(0); }

                
                while (values.Count > 1)//&& operators.Count > 0)
                {
                    int figcount = 0;
                    int[] ctrs = util_find_fig(operators);
                    int begctr = ctrs[0], endctr = ctrs[1];
                    if (ctrs[0] != 0 || ctrs[1] != operators.Count-1) {
                        if (ctrs[1] == ctrs[0] + 1) { operators.RemoveAt(ctrs[0]); operators.RemoveAt(ctrs[1] - 1); endctr -= 2; }
                        else { begctr++; endctr--; figcount++; }
                    }
                    int curr_oper = util_find_max_priority(operators, new int[] { begctr, endctr });
                    switch (operators[curr_oper])
                    {
                        case "^":
                            values[curr_oper-figcount] = Math.Pow(values[curr_oper - figcount], values[curr_oper - figcount + 1]);
                            //values.RemoveAt(curr_oper + 1); operators.RemoveAt(curr_oper);
                            break;
                        case "*":
                            values[curr_oper - figcount] *= values[curr_oper - figcount + 1];
                            //values.RemoveAt(curr_oper + 1);
                            break;
                        case "/":
                            values[curr_oper - figcount] /= values[curr_oper - figcount + 1];
                            //values.RemoveAt(curr_oper + 1);
                            break;
                        case "+":
                            values[curr_oper - figcount] += values[curr_oper - figcount + 1];
                            //values.RemoveAt(curr_oper + 1);
                            break;
                        case "-":
                            values[curr_oper - figcount] -= values[curr_oper - figcount + 1];
                            //values.RemoveAt(curr_oper + 1);
                            break;
                    }
                    values.RemoveAt(curr_oper - figcount + 1); operators.RemoveAt(curr_oper);
                }
                res.Add(values[0]);
                listctr++;
            }
            return res;
        }

        int util_find_max_priority(List<string> opers, int[] range) {
            int ind = 0; string highest = "";
            for(int i=range[0];i<=range[1];i++){
                switch (opers[i]) {
                    case "^": return i;
                    case "*": if (highest != "^" && highest != "*" && highest != "/") { ind = i; highest = "*"; } break;
                    case "/": if (highest != "^" && highest != "*" && highest != "/") { ind = i; highest = "/"; } break;
                    case "+": if (highest == "") { ind = i; highest = "+"; } break;
                    case "-": if (highest == "") { ind = i; highest = "-"; } break;
                }
            }
            return ind;
        }

        int[] util_find_fig(List<string> opers) { 
            int[] inds = { 0, opers.Count-1 };
            for (int i = 0; i < opers.Count; i++) { 
                if (opers[i] == "(") inds[0]=i;
                if (opers[i] == ")") { inds[1] = i; break; }
            }
            return inds;
        }

        public abstract void ToWriteConsole(double h, int Nctr, double err);
    }

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
                    for (int i = 0; i < Y.Count; i++) { err += Math.Pow(Y[i] - Ethalon(Y, Xi)[i], 2); }
                    if (err > maxerr) maxerr = err;

                    Nctr++;
                }
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        List<double> Ycalc(List<double> Yi, double Xi, double step) {
            List<double> F = ToInterpreteExp(Xi, Yi);
            List<double> Ynew = new List<double>();//
            for(int i=0;i<Yi.Count;i++) {
                Ynew.Add(Yi[i] + step*F[i]);
            }
            return Ynew;
        }

        List<double> Ethalon(List<double> Yi, double Xi) {  //можно вынести на пользовательский ввод
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

    class Method_2_RungeKutta : Method {
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };

        public Method_2_RungeKutta(List<string> exp) : base(exp) { }

        public override void ToCalculate()
        {
            double[] range = new double[] { 0.1, 10 };

            for (int j = 0; j < hstep.Length; j++)
            {
                double Xi = 0.1, err = 0;
                List<double> Y = new List<double>();   //starting conditions
                Y.Add(1); Y.Add(-2);
                double maxerr = 0;
                int Nctr = 0;

                while (Xi >= range[0] && Xi <= range[1])
                {
                    Xi += hstep[j];
                    Y = Ycalc(Y, Xi, hstep[j]);

                    err = 0;
                    for (int i = 0; i < Y.Count; i++) { err += Math.Pow(Y[i] - Ethalon(Y, Xi)[i], 2); }
                    if (err > maxerr) maxerr = err;

                    Nctr++;
                }
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        List<double> Ycalc(List<double> Yi, double Xi, double step)
        {
            List<double> k1 = ToInterpreteExp(Xi, Yi);

            List<double> buf = new List<double>();
            for(int j=0;j<Yi.Count;j++) { buf.Add(Yi[j] + k1[j] / 2); }
            List<double> k2 = ToInterpreteExp(Xi+step/2,buf);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + k2[j] / 2); }
            List<double> k3 = ToInterpreteExp(Xi + step / 2, buf);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + k3[j]); }
            List<double> k4 = ToInterpreteExp(Xi + step, buf);

            List<double> Ynew = new List<double>();//
            for (int i = 0; i < Yi.Count; i++)
            {
                Ynew.Add(Yi[i] + step/6 * (k1[i] + 2*k2[i] + 2*k3[i] + k4[i]));
            }
            return Ynew;
        }

        List<double> Ethalon(List<double> Yi, double Xi)    //можно вынести на пользовательский ввод
        {
            List<double> res = new List<double>();
            res.Add(1 + 4 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            res.Add(-2 - 6 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            return res;
        }

        public override void ToWriteConsole(double h, int Nctr, double err) //пока не вынес в абстрактный класс поскольку не знаю какой вывод понадобится для 3 метода
        {
            Console.WriteLine("h={0}   N={1}    err={2:f6}", h, Nctr, err);
        }
    }

    class Method_3_EndDiff : Method
    {
        double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };

        public Method_3_EndDiff(List<string> exp) : base(exp) { }

        public override void ToCalculate()
        {
            double[] range = new double[] { 0.1, 10 };

            for (int j = 0; j < hstep.Length; j++)
            {
                double Xi = 0.1, err = 0;
                List<double> Y = new List<double>();   //starting conditions
                Y.Add(1); Y.Add(-2);
                double maxerr = 0;
                int Nctr = 0;

                while (Xi >= range[0] && Xi <= range[1])
                {
                    Xi += hstep[j];
                    Y = Ycalc(Y, Xi, hstep[j]);

                    err = 0;
                    for (int i = 0; i < Y.Count; i++) { err += Math.Pow(Y[i] - Ethalon(Y, Xi)[i], 2); }
                    if (err > maxerr) maxerr = err;

                    Nctr++;
                }
                ToWriteConsole(hstep[j], Nctr, maxerr);
            }
        }

        List<double> Ycalc(List<double> Yi, double Xi, double step)
        {
            List<double> k1 = ToInterpreteExp(Xi, Yi);

            List<double> buf = new List<double>();
            for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + k1[j] / 2); }
            List<double> k2 = ToInterpreteExp(Xi + step / 2, buf);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + k2[j] / 2); }
            List<double> k3 = ToInterpreteExp(Xi + step / 2, buf);

            buf.Clear(); for (int j = 0; j < Yi.Count; j++) { buf.Add(Yi[j] + k3[j]); }
            List<double> k4 = ToInterpreteExp(Xi + step, buf);

            List<double> Ynew = new List<double>();//
            for (int i = 0; i < Yi.Count; i++)
            {
                Ynew.Add(Yi[i] + step / 6 * (k1[i] + 2 * k2[i] + 2 * k3[i] + k4[i]));
            }
            return Ynew;
        }

        List<double> Ethalon(List<double> Yi, double Xi)    //можно вынести на пользовательский ввод
        {
            List<double> res = new List<double>();
            res.Add(1 + 4 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            res.Add(-2 - 6 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            return res;
        }

        public override void ToWriteConsole(double h, int Nctr, double err) //пока не вынес в абстрактный класс поскольку не знаю какой вывод понадобится для 3 метода
        {
            Console.WriteLine("h={0}   N={1}    err={2:f6}", h, Nctr, err);
        }


    }
}
