using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    interface IMethod
    {

       //void ToInterpreteExp();  //for organised interpretation of an expression

        void ToCalculate(); //for calculations

        void ToWriteConsole(double h, int Nctr, double err);  //for formalised output of each method in a console, add values to write in arguments

    }
}
