using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Basalin__Lab
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Settings_PFTB_Parameters();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button5.Enabled = false;
        }

        IMethod method;

        public string exp1 = "- 4 * y1 - 2 * y2 + 2 / ( e ^ x - 1 )";
        public string exp2 = "6 * y1 + 3 * y2 - 3 / ( e ^ x - 1 )";
        IDraw draw;

        private void button1_Click(object sender, EventArgs e)
        {
            
            IsMethod3 = false;

            method = new Method_1_Euler(new System.Collections.Generic.List<string>() { exp1, exp2 });

             IsCalculatingElectrosystem(method);

            Scenario(0);
            draw = new DrawGraphics(method, panel1, IsMethod3);
            draw.Visual_Method();

            Name_File = "Method_Euler1" + Spot_plus_txt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsMethod3 = false;

            method = new Method_2_RungeKutta(new System.Collections.Generic.List<string>() { exp1, exp2 });

             IsCalculatingElectrosystem(method);

            Scenario(1);

            draw = new DrawGraphics(method, panel1, IsMethod3);
            draw.Visual_Method();

            Name_File = "Method_RungeKutta" + Spot_plus_txt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IsMethod3 = true;

            double z = 0.1;
            method = new Method_3_EndDiff(new System.Collections.Generic.List<string>() { exp1, exp2 }, z);

             IsCalculatingElectrosystem(method);

            Scenario(2);

            draw = new DrawGraphics(method, panel1, IsMethod3);
            draw.Visual_Method();

            Name_File = "Method_EndDiff" + Spot_plus_txt;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //видимо нам расскажут во вторник 24.12.2024
            COMING_SOON();
        }

        bool IsMethod3;
        private void button5_Click(object sender, EventArgs e)
        {
            new Class_Saves(Name_File, method.Get_H(), method.Get_N(), method.Get_Err(), IsMethod3).Save();
            WARNING();
        }

        public void IsCalculatingElectrosystem(IMethod m) {
            DialogResult dr = MessageBox.Show("Расчёт электрической системы?", "Выбор режима", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) m.ToSetChoice(2);
        }

        public void Setting_Form()
        {
            // Получение информации об экране
            Screen screen = Screen.PrimaryScreen;
            // Установка размеров формы
            // Ширина формы будет 90% от ширины рабочего стола
            int width = (int)(screen.WorkingArea.Width * 0.9);
            // Высота формы будет 90% от высоты рабочего стола
            int height = (int)(screen.WorkingArea.Height * 0.9);

            this.Width = width;
            this.Height = height;
            // Центрирование формы на экране
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Wheat;
        }

        public void Setting_Panel()
        {
            panel1.Width = this.Width * 7 / 10;
            panel1.Height = this.Height - (panel1.Location.Y * 6);
            panel1.BackColor = Color.White;
        }

        TextBox textBox = new TextBox();
        TextBox textBox0 = new TextBox();
        TextBox textBox1 = new TextBox();
        TextBox textBox2 = new TextBox();

        readonly string[] Names_operations =
            {
            "Метод Эйлера",
            "Метод Рунге-Кутты",
            "Метод Гаусса",
            "Метод Ньютона",
            "Сохранить использованный \nметод в txt"
            };

        List<Button> buttons = new List<Button>();

        public void Setting_TextBox()
        {
            textBox.Visible = true;
            textBox.Enabled = false;
            textBox.BackColor = Color.White;
            textBox.Font = new Font("TimeNewRoman", 12);
            textBox.Text = "Методы решений для задачи Коши";
            textBox.Width = (textBox.Text.Length - 2) * 10;
            textBox.Location = new Point(panel1.Width + 50, panel1.Location.Y);
            
            textBox0.Visible = true;
            textBox0.Enabled = false;
            textBox0.BackColor = Color.White;
            textBox0.Font = new Font("TimeNewRoman", 12);
            textBox0.Text = "Текущая система уравнений:";
            textBox0.Width = (textBox.Text.Length - 2) * 8;
            textBox0.Location = new Point(panel1.Width + 50, textBox.Location.Y + 50);
            
            textBox1.Visible = true;
            textBox1.Enabled = false;
            textBox1.BackColor = Color.White;
            textBox1.Font = new Font("TimeNewRoman", 12);
            textBox1.Text = "y1' = " + exp1 + "  ";
            textBox1.Width = (textBox1.Text.Length - 2) * 6;
            textBox1.Location = new Point(panel1.Width + 50, textBox0.Location.Y + 50);
            
            textBox2.Visible = true;
            textBox2.Enabled = false;
            textBox2.BackColor = Color.White;
            textBox2.Font = new Font("TimeNewRoman", 12);
            textBox2.Text = "y2' = " + exp2 + "  ";
            textBox2.Width = (textBox2.Text.Length - 2) * 6;
            textBox2.Location = new Point(panel1.Width + 50, textBox1.Location.Y + 50);
            
            Controls.Add(textBox);
            Controls.Add(textBox0);
            Controls.Add(textBox1);
            Controls.Add(textBox2);
        }

        public void Setting_Buttons()
        {

            buttons.Add(button1);
            buttons.Add(button2);
            buttons.Add(button3);
            buttons.Add(button4);
            buttons.Add(button5);

            int coefficient = 50;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Width = (buttons[i].Text.Length - 2) * 10;
                buttons[i].Font = new Font("TimeNewRoman", 12);
                buttons[i].Text = Names_operations[i];
                buttons[i].AutoSize = true;
                int X0 = textBox.Location.X; 
                if (i == 0)
                {
                    buttons[i].Location = new Point(X0, textBox2.Location.Y + coefficient);
                }
                else
                {
                    buttons[i].Location = new Point(X0, buttons[i - 1].Location.Y + coefficient);
                    buttons[i].Text = Names_operations[i];
                }
            }
        }

        // Придумал это аббревиатуру для нашего удобства
        // PFTB - Panel(s), Form(s), TextBox(s), Button(s)
        public void Settings_PFTB_Parameters()
        {
            Setting_Form();
            Setting_Panel();
            Setting_TextBox();
            Setting_Buttons();
        }

        public static string Name_File;
        string Spot_plus_txt = ".txt";
        bool flag = false;

        public void Scenario(int Numb_Oper)
        {
            panel1.Refresh();
            Console.Clear();
            foreach (var button in buttons)
            {
                button.Enabled = false;
            }
            method.ToCalculate();
            Name_File = Names_operations[Numb_Oper];
            SUCCESS();
            foreach (var button in buttons)
            {
                button.Enabled = true;
            }
        }

        public void SUCCESS()
        {
            if (!flag)
            {
                button5.Enabled = true;
                buttons.Add(button5);
            }
            buttons.Add(button5);
            MessageBox.Show
                ($"Подсчитан {Name_File}",
                "Успех!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }

        public void WARNING()
        {
            MessageBox.Show
                ($"Результаты вычислений будут сохранены\nв текстовом файле {Name_File}",
                "Предупреждение!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
                );
        }

        public void COMING_SOON()
        {
            MessageBox.Show
                (
                "Coming soon",
                "Ингформация",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
                );
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }
    }
}
