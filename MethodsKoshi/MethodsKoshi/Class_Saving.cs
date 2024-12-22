using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodsKoshi
{
    public class Class_Saves : ISaving
    {
        private List<double> hstep;
        private List<int> Nctr;
        private List<double> maxerr;
        //Поменять путь Файла
        private string filePath = "..\\..\\..\\";
    
        private bool IsMethod3 = false;
    
        public Class_Saves
            (
            string File_Name,
            List<double> hstep,
            List<int> Nctr,
            List<double> maxerr,
            bool IsMethod3
            )
        {
            filePath = filePath + File_Name;
            this.hstep = hstep;
            this.Nctr = Nctr;
            this.maxerr = maxerr;
            this.IsMethod3 = IsMethod3;
        }
    
        public void Save()
        {
            // Создаем объект StreamWriter для записи в файл
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int n1 = hstep.Select(el => el.ToString().Length).Max();
                int n2 = Nctr.Select(el => el.ToString().Length).Max();
    
                string skip = "              ";
                writer.Write("h");
                for (int i = 0; i < n1; i++)
                {
                    writer.Write(" ");
                    if (i == n1 - 1)
                    {
                        writer.Write(skip);
                    }
                }
                writer.Write("N");
                for (int i = 0; i < n2; i++)
                {
                    writer.Write(" ");
                    if (i == n2 - 1)
                    {
                        writer.Write(skip);
                    }
                }
                writer.Write("err");
                writer.WriteLine();
    
    
    
                for (int i = 0; i < hstep.Count; i++)
                {
                    if (IsMethod3 && i % 4 == 0)
                    {
                        writer.WriteLine();
                    }
    
    
                    writer.Write(hstep[i]);
                    if (hstep[i].ToString().Length < n1)
                    {
                        for (int j = 0; j < n1 - hstep[i].ToString().Length; j++)
                        {
                            writer.Write(" ");
                        }
                    }
                    writer.Write(skip + " ");
                    writer.Write(Nctr[i]);
                    if (Nctr[i].ToString().Length < n2)
                    {
                        for (int j = 0; j < n2 - Nctr[i].ToString().Length; j++)
                        {
                            writer.Write(" ");
                        }
                    }
                    writer.Write(skip + " ");
                    writer.Write(maxerr[i]);
                    writer.WriteLine();
                }
            }
        }
    }
}
