using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Basalin__Lab
{
    public interface IDraw
    {
        void Visual_Method();
    }

    public abstract class AbstractDraw: IDraw
    {
        protected IMethod method;

        public AbstractDraw(IMethod method)
        {
            this.method = method;
        }

        public abstract void Visual_Method();
    }

    public class DrawGraphics : AbstractDraw
    {
        private Panel panel;

        private List<double> X = new List<double>();
        private List<double> X_Copy = new List<double>();
        private List<double> Y = new List<double>();
        private List<double> Y_Copy = new List<double>();

        private bool IsMethod3;

        public DrawGraphics(IMethod method, Panel panel, bool IsMethod3) : base(method)
        {
            this.panel = panel;
            X = new List<double>(method.Get_H());
            X_Copy = new List<double>(X).Distinct().ToList();
            X_Copy.Sort();
            Y = new List<double>(method.Get_Err());
            Y_Copy = new List<double>(Y).Distinct().ToList();
            Y_Copy.Sort();
            this.IsMethod3 = IsMethod3;
        }

        public override void Visual_Method()
        {

            Draw_axes();
            Draw_Graphics_Line();
        }

        //Рисуем сам график
        private void Draw_Graphics_Line()
        {
            int width = panel.ClientSize.Width - 100;
            int height = panel.ClientSize.Height - 40;
            Graphics g = panel.CreateGraphics();

            Point origin = new Point(width, height);

            // Включаем антиалиасинг
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen pen = new Pen(Color.Black, 2); // Черный цвет линий


            /*int koefX = width / (X_Copy.Count + 1);*/
            int koefY = height / (Y_Copy.Count + 1);
            int r = 10;
            Random random = new Random();
            for (int i = 1; i < X.Count; i++)
            {
                
                int index_X1 = X_Copy.IndexOf(X[i-1]);
                
                int index_Y1 = Y_Copy.IndexOf(Y[i-1]);

                int index_X2 = X_Copy.IndexOf(X[i]);
                int index_Y2 = Y_Copy.IndexOf(Y[i]);

                int koefX = width / (2 ^ (X_Copy.Count - index_X1) * 4);
                int x1 = width - (index_X1 + 1) * koefX;
                int y1 = height - (index_Y1 + 1) * koefY;

                koefX = width / (2 ^ (X_Copy.Count - index_X2) * 4);
                int x2 = width - (index_X2 + 1) * koefX;
                int y2 = height - (index_Y2 + 1) * koefY;


                g.FillEllipse(new SolidBrush(pen.Color), x1 - r/2, y1 - r/2, r, r);
                if(i == X.Count - 1)
                {
                    g.FillEllipse(new SolidBrush(pen.Color), x2 - r / 2, y2 - r / 2, r, r);
                }
                if (IsMethod3 && i % 4 != 0)
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                }
                else if(IsMethod3 && i % 4 == 0)
                {

                    int red = random.Next(0, 256);
                    int green = random.Next(0, 256);
                    int blue = random.Next(0, 256);
                    pen.Color = Color.FromArgb(red, green, blue); ;

                }
                if (!IsMethod3)
                {
                    g.DrawLine(pen, x1, y1, x2, y2);
                }

            }

            // Освобождаем ресурсы
            pen.Dispose();
        }

        //Отрисовка осей
        private void Draw_axes()
        {
            Graphics g = panel.CreateGraphics();
            Pen pen = new Pen(Color.Black); // Черный цвет линий
            // Включаем антиалиасинг
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int width = panel.ClientSize.Width - 100;
            int height = panel.ClientSize.Height - 40;

            Point origin = new Point(width, height);

            DrawArrow(g, pen, origin.X, 20, origin.X, 19);   // Стрелка вверх
            DrawArrow(g, pen, 20, origin.Y, 20 - 1, origin.Y); // Стрелка влево

            // Рисуем ось X
            g.DrawLine(pen, 20, height, width, height);
            // Рисуем ось Y
            g.DrawLine(pen, width, 20, width, height);


            // Отмечаем координаты на оси X
            for (int i = 0; i < X_Copy.Count; i++)
            {
                int koefX = width / (2 ^ (X_Copy.Count - i) * 4);
                int x = width - (i + 1) * koefX;

                g.DrawString(X_Copy[i].ToString(), new Font("TimeNewRoman", 12),
                    Brushes.Black, x - 7, height + 15);
                g.DrawLine(pen, x, height - 5, x, height + 5);

                if (i == X_Copy.Count - 1)
                {
                    int x_last = width - (i + 3) * koefX;
                    g.DrawString("Шаг, h".ToString(), new Font("TimeNewRoman", 12),
                    Brushes.Black, x_last, height + 15);
                }
            }

            // Отмечаем координаты на оси Y
            int koefY = height / (Y_Copy.Count + 1);
            for (int j = 0; j < Y_Copy.Count; j++)
            {
                if (j == 0)
                {
                    int y0 = height - (j) * koefY;
                    g.DrawString(0.ToString(), new Font("TimeNewRoman", 12),
                    Brushes.Black, width + 10, y0 + 10);
                }
                int y = height - (j + 1) * koefY;
                g.DrawString(Y_Copy[j].ToString("N7"), new Font("TimeNewRoman", 12),
                    Brushes.Black, width + 10, y - 10);

                g.DrawLine(pen, width - 5, y, width + 5, y);

                if (j == Y_Copy.Count - 1)
                {
                    int y_last = height - (j + 2) * koefY;
                    g.DrawString("Ошибка, ∆", new Font("TimeNewRoman", 12),
                    Brushes.Black, width + 10, y_last + 10);
                }
            }
            // Освобождаем ресурсы
            pen.Dispose();
        }

        //Рисуем стрелки
        private void DrawArrow(Graphics g, Pen pen, float x1, float y1, float x2, float y2)
        {
            g.DrawLine(pen, x1, y1, x2, y2);
            float angle = (float)Math.Atan2(y2 - y1, x2 - x1);
            g.DrawLine(pen, x2, y2, x2 - (float)(Math.Cos(angle + Math.PI / 6) * 7), y2 - (float)(Math.Sin(angle + Math.PI / 6) * 7)); // Первая линия стрелки
            g.DrawLine(pen, x2, y2, x2 - (float)(Math.Cos(angle - Math.PI / 6) * 7), y2 - (float)(Math.Sin(angle - Math.PI / 6) * 7)); // Вторая линия стрелки
        }
    }
}
