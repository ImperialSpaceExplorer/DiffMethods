using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    class Saver
    {
        Method met;

        public Saver(Method m) { met = m; }

        public void SaveResults() {
            System.IO.FileStream fsWrite = System.IO.File.Open("..\\..\\..\\" + met.Name +"_output.txt", System.IO.FileMode.Create);
            System.IO.StreamWriter sr = new System.IO.StreamWriter(fsWrite);

            sr.WriteLine(met.Name.ToString());

            sr.WriteLine("X vector values:");
            int ctr = 0;
            foreach (List<double> pair in met.Results_X) {
                sr.Write($"({pair[0]}, {pair[1]})"); ctr++;
                if(ctr==10) { sr.WriteLine(); ctr = 0; }
            }

            sr.WriteLine("\nY vector values:");

            foreach (List<double> pair in met.Results_Y)
            {
                sr.Write($"({pair[0]}, {pair[1]})"); ctr++;
                if (ctr == 10) { sr.WriteLine(); ctr = 0; }
            }

            sr.WriteLine("\n--------------------------------------------------------------");

            sr.Close();
        }


    }
}
