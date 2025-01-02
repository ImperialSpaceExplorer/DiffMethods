using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            button4.Enabled = false;
            // Создаем таймер для периодической проверки наличия файлов
            Timer timer = new Timer();

            timer.Interval = 50; // Проверять каждые 1 секунду (можно изменить интервал)
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start(); // Запускаем таймер

            Method_2_RungeKutta method = new Method_2_RungeKutta(new List<string> { exp1, exp2 });

            double[] hstep = { 0.01, 0.001, 0.0001, 0.00001 };
            double Xi = 0.1;
            List<double> Y = new List<double>();
            Y.Add(1); Y.Add(-2);
            List<double> result = method.Ycalc(Y, Xi, hstep[0]);
            double numb1 = 1.1798794256986063;
            double numb2 = -2.2698191385479096;
            List<double> check = new List<double>() { numb1, numb2 };
            bool falg = false;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            string directoryPath = "C:.\\..\\..\\..\\";
            // Проверяем наличие .txt файлов в директории
            string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");

            // Обновляем состояние кнопки
            this.button5.Enabled = (txtFiles.Length > 0); // Включаем кнопку, если есть хотя бы один .txt файл
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

            Draw(50f);

            draw.Visual_Method();

            Name_File = "Method_Euler1" + Spot_plus_txt;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            IsMethod3 = false;

            method = new Method_2_RungeKutta(new System.Collections.Generic.List<string>() { exp1, exp2 });

            IsCalculatingElectrosystem(method);

            Scenario(1);

            Draw(50f);

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

            Draw(7.2f);

            draw.Visual_Method();

            Name_File = "Method_EndDiff" + Spot_plus_txt;
        }


        private void Draw(float Scale)
        {
            List<TextBox> txtes = this.Controls.OfType<TextBox>().ToList();

            if (dr == DialogResult.Yes)
            {
                List<double> list_params = method.GetParameters_CR1R2LJ();

                txtes[1].Text = "Параметры электрической системы:";
                txtes[1].Width = (txtes[1].Text.Length - 2) * 10;

                txtes[2].Text = $"C = {list_params[0]} | L = {list_params[3]} | J = {list_params[4]}";
                txtes[2].Width = (txtes[2].Text.Length - 2) * 8;

                txtes[3].Text = $"R1 = {list_params[1]} | R2 = {list_params[2]}";
                txtes[3].Width = (txtes[3].Text.Length - 2) * 9;
                draw = new DrawNewSystemGraphics(method, panel1);
            }
            else
            {
                txtes[1].Text = "Текущая система уравнений:";
                txtes[1].Width = (txtes[1].Text.Length - 2) * 10;

                txtes[2].Text = "y1' = " + exp1 + "  ";
                txtes[2].Width = (txtes[2].Text.Length - 2) * 6;

                txtes[3].Text = "y2' = " + exp2 + "  ";
                txtes[3].Width = (txtes[3].Text.Length - 2) * 6;

                draw = new DrawGraphics_H_and_Errors(method, panel1, IsMethod3);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            WARNING();

            if (dr == DialogResult.Yes)
            {
                new Calculations_Saving(method).Save();
            }
            else
            {
                new Class_Saves(Name_File, method.Get_H(), method.Get_N(), method.Get_Err(), IsMethod3).Save();
            }
        }

        DialogResult dr;
        public void IsCalculatingElectrosystem(IMethod m)
        {
            dr = MessageBox.Show
                ("Расчёт электрической системы?" +
                "\n\nДа - электрическая система" +
                "\n\nНет - старая тестовая система", "Выбор режима", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes) m.ToSetChoice(2);
        }

        bool IsMethod3;
        private void button5_Click(object sender, EventArgs e)
        {
            string directoryPath = "C:.\\..\\..\\..\\"; // Путь к папке, где находятся файлы
            try
            {
                // Получаем список всех файлов с расширением ".txt" в указанной директории
                string[] txtFiles = Directory.GetFiles(directoryPath, "*.txt");

                if (txtFiles.Length == 0)
                {
                    MessageBox.Show("В проекте нет сохранённых результатов.");
                    return;
                }

                // Запрашиваем подтверждение на удаление всех файлов
                DialogResult result = MessageBox.Show(
                    $"Хотите удалить все файлы\nрезультатов в директории?",
                    "Подтверждение удаления",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    foreach (string filePath in txtFiles)
                    {
                        string PathNameFile = Path.GetFileName(filePath);
                        try
                        {

                            File.Delete(filePath);
                            MessageBox.Show($"Файл '{PathNameFile}' удален.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Ошибка при удалении файла '{PathNameFile}':\n{ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при попытке найти файлы результатов в проекте");
            }
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
            panel1.Location = new Point(10, 10);
            panel1.Width = this.Width * 7 / 10;
            panel1.Height = this.Height - (panel1.Location.Y * 6);
            panel1.BackColor = Color.White;
        }

        readonly string[] Names_operations =
        {
            "Метод Эйлера",
            "Метод Рунге-Кутты",
            "Метод Гаусса",
            "Сохранить использованный \nметод в txt",
            "Удалить все сохранённые txt\nс вычислениями"
        };

        public void Setting_TextBox()
        {
            CreateAndAddTextBox("Методы решений для задачи Коши", 0, 10, 1);
            CreateAndAddTextBox("Текущая система уравнений:", 50, 10, 2);
            CreateAndAddTextBox("y1' = " + exp1 + "  ", 100, 6, 3);
            CreateAndAddTextBox("y2' = " + exp2 + "  ", 150, 6, 4);

            this.ResumeLayout(false);
        }

        private void CreateAndAddTextBox(string text, int yOffset, int koef_width, int index)
        {
            TextBox textBox = new TextBox
            {
                Name = index.ToString(),
                Visible = true,
                Enabled = false,
                BackColor = Color.White,
                Font = new Font("TimeNewRoman", 12),
                Text = text,
                Width = (text.Length - 2) * koef_width,
                Location = new Point(panel1.Width + 50, panel1.Location.Y + yOffset)
            };

            Controls.Add(textBox);
        }

        List<Button> buttons = new List<Button>();
        public void Setting_Buttons()
        {

            buttons = this.Controls.OfType<Button>().Reverse().ToList();

            int coefficient = 50;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Width = (buttons[i].Text.Length - 2) * 10;
                buttons[i].Font = new Font("TimeNewRoman", 12);
                buttons[i].Text = Names_operations[i];
                buttons[i].AutoSize = true;
                //int X0 = textBox.Location.X;
                int X0 = panel1.Width + 50;
                if (i == 0)
                {
                    TextBox LastTextbox = this.Controls.Find(4.ToString(), true).FirstOrDefault() as TextBox;
                    buttons[i].Location = new Point(X0, LastTextbox.Location.Y + coefficient);
                }
                else if (i == buttons.Count - 1)
                {
                    buttons[i].Location = new Point(X0, buttons[i - 1].Location.Y + coefficient + 20);
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

            if (dr == DialogResult.Yes)
            {
                Name_File = method.Name;
            }
            else
            {
                Name_File = Names_operations[Numb_Oper];
            }
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
            if (dr == DialogResult.Yes)
            {
                MessageBox.Show
                 ($"Подсчитан {method.Name}",
                 "Успех!",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Information
                 );
            }
            else
            {
                MessageBox.Show
                 ($"Подсчитан {Name_File}",
                 "Успех!",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Information
                 );
            }
        }

        public void WARNING()
        {

            if (dr == DialogResult.Yes)
            {
                MessageBox.Show
                (
                    $"Результаты вычислений будут сохранены\nв текстовом файле {method.Name}.txt",
                    "Информация!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show
                (
                    $"Результаты вычислений будут сохранены\nв текстовом файле {Name_File}",
                    "Информация!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            //Если panel не добавить в форму (и дважды щёлкнуть по ней)
            //то не запустится отрисовка на панели
        }
    }
}
