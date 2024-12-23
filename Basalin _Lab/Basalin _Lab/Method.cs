using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basalin__Lab
{
    abstract class Method : IMethod
    {
        //добавляю в начале класса, чтобы тебе быстрее было что исправить у себя
        public List<double> Get_H()
        {
            return Hstep;
        }
        
        public List<int> Get_N()
        {
            return NctR;
        }
        
        public List<double> Get_Err()
        {
            return Maxerr;
        }

                //Добавление функции для сбора данных
        protected void ADDING_DATA_FOR_SAVE(double h, int Nctr, double err)
        {
            Hstep.Add(h);
            NctR.Add(Nctr);
            Maxerr.Add(err);
        }
        
        protected List<double> Hstep = new List<double>();
        protected List<int> NctR = new List<int>();
        protected List<double> Maxerr = new List<double>();
        //**********************************************************************************


        protected List<List<string>> expression;

        public Method(List<string> expression)
        {
            this.expression = new List<List<string>>();
            foreach (string exp in expression)
            {
                this.expression.Add( exp.Split(' ').ToList());
            }
        }

        public abstract void ToCalculate();

        protected List<double> ToInterpreteExp(double Xi, List<double> Y, List<List<string>> expression)   //let exp be = "Y1' = y1 - y2 + e ^ x"
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
                        //may be extended

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

                if (operators.Count >= values.Count) {
                    if (operators[0] == "-"){ values[0] *= -1; operators.RemoveAt(0); } 
                    else if(operators[0] == "+") operators.RemoveAt(0);
                }
                
                while (values.Count > 1)
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
                            break;
                        case "*":
                            values[curr_oper - figcount] *= values[curr_oper - figcount + 1];
                            break;
                        case "/":
                            values[curr_oper - figcount] /= values[curr_oper - figcount + 1];
                            break;
                        case "+":
                            values[curr_oper - figcount] += values[curr_oper - figcount + 1];
                            break;
                        case "-":
                            values[curr_oper - figcount] -= values[curr_oper - figcount + 1];
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

        protected List<double> Ethalon(double Xi)    
        {
            List<double> res = new List<double>();
            //res.Add(1 + 4 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));

            //res.Add(1 - 1.48879 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) + 1));
            res.Add(1 + 4.504340245 * Math.Pow(Math.E, -1 * Xi) + 2 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));

            //res.Add(-2 - 6 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));

            //res.Add(-2 + 2.23319 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) + 1));
            res.Add(-2 - 6.756505364 * Math.Pow(Math.E, -1 * Xi) - 3 * Math.Pow(Math.E, -1 * Xi) * Math.Log(Math.Pow(Math.E, Xi) - 1));
            return res;
        }

        public abstract void ToWriteConsole(double h, int Nctr, double err);
        public abstract List<double> Ycalc(List<double> Yi, double Xi, double step);
    }    
}
