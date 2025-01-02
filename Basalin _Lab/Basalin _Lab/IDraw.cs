using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basalin__Lab
{
    public interface IDraw
    {
        void Visual_Method();
    }

    public abstract class AbstractDraw : IDraw
    {
        protected IMethod method;

        public AbstractDraw(IMethod method)
        {
            this.method = method;
        }

        //Рисуем стрелки
        protected void DrawArrow(Graphics g, Pen pen, float x1, float y1, float x2, float y2)
        {
            g.DrawLine(pen, x1, y1, x2, y2);
            float angle = (float)Math.Atan2(y2 - y1, x2 - x1);
            g.DrawLine(pen, x2, y2, x2 - (float)(Math.Cos(angle + Math.PI / 6) * 7), y2 - (float)(Math.Sin(angle + Math.PI / 6) * 7)); // Первая линия стрелки
            g.DrawLine(pen, x2, y2, x2 - (float)(Math.Cos(angle - Math.PI / 6) * 7), y2 - (float)(Math.Sin(angle - Math.PI / 6) * 7)); // Вторая линия стрелки
        }

        protected abstract void Draw_Graphics_Line();
        public abstract void Visual_Method();
    }
}
