using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basalin__Lab
{
    public interface IMethod
    {
        //for organised interpretation of an expression
        //void ToInterpreteExp();  

        //for calculations
        void ToCalculate(); 

        //for formalised output of each method in a console, add values to write in arguments
        void ToWriteConsole(double h, int Nctr, double err);

        //мне пришлось вклинивать для пары переменных
        List<double> Get_H();
        List<int> Get_N();
        List<double> Get_Err();
    }
}
