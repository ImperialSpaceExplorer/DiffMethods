﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Basalin__Lab
{
    public class DrawNewSystemGraphics : AbstractDraw
    {
        private readonly Panel panel;
        private readonly int centerX = 0;
        private readonly int centerY = 0;
        public DrawNewSystemGraphics(IMethod method, Panel panel) : base(method)
        {
            this.method = method;
            this.panel = panel;
            centerX = panel.Width / 2;
            centerY = panel.Height / 2;
        }

        public override void Visual_Method()
        {
            All_Axes();
            Draw_Graphics_Line();
        }

        private void All_Axes()
        {
            Graphics graphics = panel.CreateGraphics();
            Pen pen = new Pen(Color.Red, 3);
            graphics.DrawLine(pen, centerX, 0, centerX, panel.Height); // Вертикальная ось по середине панели
            Axes(centerX / 2, centerY, centerX, centerY * 2, 0, centerX / 2);
            Axes(centerX * 3 / 2, centerY, centerX, centerY * 2, centerX, centerX / 2);
            pen.Dispose();
        }


        private void Axes(float center_X, float center_Y, float width, float height, float Sdvig, float SD)
        {
            // Получаем объект Graphics для рисования
            Graphics g = panel.CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Рисуем оси координат
            Pen pen = new Pen(Color.Black);

            g.DrawLine(pen, Sdvig, center_Y, Sdvig + width, center_Y); // Горизонтальная ось
            DrawArrow(g, pen, Sdvig + width - 1, center_Y, Sdvig + width, center_Y);

            string str1 = " ";
            string str2 = " ";
            if (center_X > width) //Второй график
            {
                str1 = "I2";
                str2 = "I3";
            }
            else //Первый график
            {
                str1 = "Uc";
                str2 = "IL";
            }

            //НАЗВАНИЕ ОСИ X!
            g.DrawString(str1, new Font("TimeNewRoman", 10), Brushes.Purple, center_X + SD - 20, center_Y + 5);

            g.DrawLine(pen, center_X, 0, center_X, height); // Вертикальная ось
            DrawArrow(g, pen, center_X, 1, center_X, 0);

            //НАЗВАНИЕ ОСИ Y!
            g.DrawString(str2, new Font("TimeNewRoman", 10), Brushes.Purple, center_X + 5, 5);

            pen.Dispose();
        }

        protected override void Draw_Graphics_Line()
        {
            Action<Graphics> someAction = LeftDrawing;
            DrawingGraphics(someAction, 0f);
            someAction = RightDrawing;
            DrawingGraphics(someAction, centerX);
        }

        public void DrawingGraphics(Action<Graphics> action, float startX)
        {
            if (action != null)
            {
                using (Graphics g = panel.CreateGraphics())
                {
                    // Рисуем на одной из половин панели
                    RectangleF clipRectLeft = new RectangleF(startX, 0, centerX, centerY * 2);
                    Region regionLeft = new Region(clipRectLeft);
                    g.Clip = regionLeft;

                    action(g); // Выполнение делегата с передачей Graphics

                    g.ResetClip();
                }
            }
        }

        private void LeftDrawing(Graphics g)
        {
            Drawing(g, method.Results_X, centerX / 2, centerY, 0);
        }

        private void RightDrawing(Graphics g)
        {
            Drawing(g, method.Results_Y, centerX * 3 / 2, centerY, centerX);
        }

        public List<double> ReturnList(List<List<double>> Vector, float number, int position_in_list)
        {
            foreach (List<double> list in Vector)
            {
                if ((float)list[position_in_list] == number)
                {
                    return list;
                }
            }

            return new List<double>();
        }

        public static float CompareWithOperator(float a, float b, Func<float, float, bool> comparison)
        {
            if (comparison(a, b) == true)
            {
                return a;
            }
            return b;
        }

        private void Drawing(Graphics g, List<List<double>> Vector, float center_X, float center_Y, float Sdvig)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            // Определение функций сравнения
            bool less(float x, float y) => x < y;
            bool more(float x, float y) => x > y;

            foreach (List<double> point in Vector)
            {
                if (point[0] < minX)
                {
                    minX = (float)point[0];
                }
                minX = CompareWithOperator((float)point[0], minX, less);
                maxX = CompareWithOperator((float)point[0], maxX, more);
                minY = CompareWithOperator((float)point[1], minY, less);
                maxY = CompareWithOperator((float)point[1], maxY, more);
            }

            List<List<double>> extreme_points = new List<List<double>>
            {
                ReturnList(Vector, minX, 0),
                ReturnList(Vector, maxX, 0),
                ReturnList(Vector, minY, 1),
                ReturnList(Vector, maxY, 1)
            };
            extreme_points = new List<List<double>>(extreme_points.Distinct().ToList());
            float rangeX = maxX - minX;
            float rangeY = maxY - minY;

            // Масштабируем координаты точек
            float scaleFactor = 50f;

            List<PointF> points = new List<PointF>();
            foreach (List<double> point in Vector)
            {
                float NotScaledX = (float)point[0];
                float NotScaledY = (float)point[1];
                points.Add(new PointF(NotScaledX, NotScaledY));
            }

            DrawCoordinateExtremePoints(g, extreme_points, center_X, center_Y, scaleFactor, Sdvig);
            Pen pen = new Pen(Color.Green);
            for (int i = 1; i < points.Count; i++)
            {
                float x1 = center_X + points[i - 1].X * scaleFactor;
                float y1 = center_Y - points[i - 1].Y * scaleFactor;

                float x2 = center_X + points[i].X * scaleFactor;
                float y2 = center_Y - points[i].Y * scaleFactor;

                g.DrawLine(pen, x1, y1, x2, y2);

            }

            // Освобождаем ресурсы
            pen.Dispose();
        }


        private void DrawCoordinateExtremePoints(Graphics g, List<List<double>> extreme_points, float center_X, float center_Y, float scaleFactor, float Sdvig)
        {
            g.DrawString("Крайние точки:", new Font("TimeNewRoman", 10), Brushes.Purple, 5 + Sdvig, 5);
            Pen pen = new Pen(Color.Red, 2);
            int index = 1;
            foreach (List<double> point in extreme_points)
            {

                float x_axe = center_X + (float)point[0] * scaleFactor;
                g.DrawLine(pen, x_axe, center_Y - 5, x_axe, center_Y + 5);

                string str = $"{index}) ({point[0]} | {point[1]})";

                g.DrawString(str, new Font("TimeNewRoman", 7), Brushes.Purple, 5 + Sdvig, 5 + index * 20);

                float y_axe = center_Y - (float)point[1] * scaleFactor;
                g.DrawLine(pen, center_X - 5, y_axe, center_X + 5, y_axe);

                index++;
            }
        }
    }
}
