using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mario1
{

    public partial class Form1 : Form
    {
        // Игровые ресурсы
        Image Level_1_bg = Properties.Resources.bg4;
        Image Image_LuckyBlock = Properties.Resources.Question_Block__1_;
        Image Image_Bricks = Properties.Resources.Bricks;
        Image Image_Iron = Properties.Resources.EmptyBlock;
        Image Image_Pipe = Properties.Resources.Pipe;
        Image Image_Goomba = Properties.Resources.Goomba;
        Image Image_mushroom_bonus = Properties.Resources.Super_Mushroom;

        // Гглавный игровой таймер
        private System.Windows.Forms.Timer gameTimer;

        // Переменные
        private int b1 = 0, b2 = 328, b3 = 656, b4 = 984, b5 = 1312, b6 = 1640, b7 = 1968, b8 = 2296;//картинки кучи кирпичей
        private long spawn = 0;//отвечает когда спавнить что либо
        private int g = 1;//ускорение свободного падения
        private sbyte spaceG = 25;//замедление во время прыжка
        public int top = 845;//высота пола, на котором стоит марио
        private short u;//переменная для падай таймера
        private short speed = 1;//
        private short run = 0;//для картинок бега (прибавляеться каждый цикл таймера и обнуляеться при остановке)
        private short runIf = 0;//для картинок бега (что-то типо флажков только не 2 варианта, а больше)(0-первая картинка бега 1-вторая картинка и т.д.)
        private bool stopForm1_KeyDown = false;
        private bool deadPadeniye = false;
        private int sravnenieTverdoeCosanieL;
        private int sravnenieTverdoeCosanieR;
        private string MarioMode = "ordinary";


        // Флаги для общего таймера
        private bool TimerRidht = false;
        private bool TimerLeft = false;
        private bool TimerSpace = false;
        private bool TimerGravity = false;
        private bool PadayTimer = false;
        private bool SluchayTimer = true;
        private bool TimerSpace_creatures = false;
        private bool Dvizeniye_creaturesTimer = true;
        private bool TimerGravity_creatures = true;
        private bool PadayTimer_creatures = true;

        // Списки (у каждого блока или существа есть свой определённый элемент в списках, при чом всегда один и тот же во всех списках)
        //блоки
        public List<short> spawnX_block = new List<short>();
        List<short> spawnY_block = new List<short>();
        List<string> spawnBlock_block = new List<string>();//название блока
        List<int> bottomBlock_block = new List<int>();//высота блока
        List<int> rightBlock_block = new List<int>();//длинна блока
        List<string> property_block = new List<string>();//св-ва блока
        //задний план блоки
        List<short> spawnX_zp = new List<short>();
        List<short> spawnY_zp = new List<short>();
        List<string> spawnBlock_zp = new List<string>();//название блока
        //существа
        List<int> spawnX_creatures = new List<int>();
        List<int> spawnY_creatures = new List<int>();
        List<bool> direction_creatures = new List<bool>();//направление движения
        List<string> spawnBlock_creatures = new List<string>();//название существа
        List<int> bottomBlock_creatures = new List<int>();//высота существа
        List<int> rightBlock_creatures = new List<int>();//длинна существа
        List<string> property_creatures = new List<string>();//св-ва существа
        List<string> condition_creatures = new List<string>();//состояние существа
        List<int> top_creatures = new List<int>();//нижний пол для существа
        List<int> g_creatures = new List<int>();//ускорение свободного падения для существа
        List<int> spaceG_creatures = new List<int>();//замедление во время прыжка для существа

        List<int[]> list_u = new List<int[]>();//список массивов для отслеживания на какие блока приземляються существа

        List<int> nam = new List<int>();//для сравненя двух блоков, которые бьёт головой марио
        List<int[]> sluchay = new List<int[]>();//для таймера случаев хранящий за одно старое значение spawnY_block, чтобы небыло ошибок при задевании головой блоков и они вернулись на место, если нужно

        public Form1()
        {
            InitializeComponent();

            // Настройка формы для производительной отрисовки (Double buffering)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.KeyPreview = true; // Позволяет форме перехватывать события клавиатуры

            // Инициализация игрового таймера
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 5; // Примерно 250 FPS (1000мс / 250 = ~5мс)
            gameTimer.Tick += GameTimer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Запускаем главный игровой цикл
            gameTimer.Start();

            Start_Load(sender, e);
        }

        private void Start_Load(object sender, EventArgs e)
        {
            Mario.Location = new Point(0, 762);
            Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario.gif");
        }

        // Главный игровой цикл - все обновления происходят здесь
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (TimerLeft)
            {
                if (Mario.Left > 0)
                {
                    for (short y = 0; y < 10; y++)
                    {
                        Mario.Left -= speed;
                        for (short i = 0; i < spawnBlock_block.Count; i++)
                        {
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[i], spawnX_block[i] + rightBlock_block[i], spawnY_block[i], spawnY_block[i] + bottomBlock_block[i] }))
                            {
                                Mario.Left = spawnX_block[i] + rightBlock_block[i];
                            }
                        }
                    }
                    if (Mario.Bottom == top)
                    {
                        if (run % 10 == 0)
                        {
                            if (runIf == 0)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk1.gif");
                                runIf = 1;
                            }
                            else if (runIf == 1)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk2.gif");
                                runIf = 2;
                            }
                            else
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk3.gif");
                                runIf = 0;
                            }
                        }
                    }
                    run += 1;
                }
            }

            if (TimerRidht)
            {
                if (Mario.Left < 800)
                {
                    for (short y = 0; y < 10; y++)
                    {
                        Mario.Left += speed;
                        for (short i = 0; i < spawnBlock_block.Count; i++)
                        {
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[i], spawnX_block[i] + rightBlock_block[i], spawnY_block[i], spawnY_block[i] + bottomBlock_block[i] }) == true)
                            {
                                Mario.Left = (spawnX_block[i] - 83);
                            }
                        }
                    }
                    if (Mario.Bottom == top)
                    {
                        if (run % 10 == 0)
                        {
                            if (runIf == 0)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk1.gif");
                                runIf = 1;
                            }
                            else if (runIf == 1)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk2.gif");
                                runIf = 2;
                            }
                            else
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk3.gif");
                                runIf = 0;
                            }
                        }
                    }
                    run += 1;
                }
                else
                {
                    b1 -= speed * 10;
                    b2 -= speed * 10;
                    b3 -= speed * 10;
                    b4 -= speed * 10;
                    b5 -= speed * 10;
                    b6 -= speed * 10;
                    b7 -= speed * 10;
                    b8 -= speed * 10;


                    for (short i = 0; i < spawnX_creatures.Count; i++)
                    {
                        for (short y = 0; y < 10; y++)
                        {
                            spawnX_creatures[i] -= speed;
                        }
                        if (spawnX_creatures[i] < -500)
                        {
                            spawnX_creatures.RemoveAt(i);
                            spawnY_creatures.RemoveAt(i);
                            direction_creatures.RemoveAt(i);
                            spawnBlock_creatures.RemoveAt(i);
                            rightBlock_creatures.RemoveAt(i);
                            bottomBlock_creatures.RemoveAt(i);
                            condition_creatures.RemoveAt(i);
                            top_creatures.RemoveAt(i);
                            g_creatures.RemoveAt(i);
                            i--;
                        }
                    }
                    for (short i = 0; i < spawnX_block.Count; i++)
                    {
                        for (short y = 0; y < 10; y++)
                        {
                            spawnX_block[i] -= speed;
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[i], spawnX_block[i] + rightBlock_block[i], spawnY_block[i], spawnY_block[i] + bottomBlock_block[i] }) == true)
                            {
                                Mario.Left = (spawnX_block[i] - 83);
                            }
                        }

                        if (spawnX_block[i] < -200)
                        {
                            spawnX_block.RemoveAt(i);
                            spawnY_block.RemoveAt(i);
                            spawnBlock_block.RemoveAt(i);
                            bottomBlock_block.RemoveAt(i);
                            rightBlock_block.RemoveAt(i);
                            property_block.RemoveAt(i);
                            i--;
                        }
                    }

                    if (b1 <= -328)
                    {
                        b1 = b8 + 328;
                    }
                    if (b2 <= -328)
                    {
                        b2 = b1 + 328;
                    }
                    if (b3 <= -328)
                    {
                        b3 = b2 + 328;
                    }
                    if (b4 <= -328)
                    {
                        b4 = b3 + 328;
                    }
                    if (b5 <= -328)
                    {
                        b5 = b4 + 328;
                    }
                    if (b6 <= -328)
                    {
                        b6 = b5 + 328;
                    }
                    if (b7 <= -328)
                    {
                        b7 = b6 + 328;
                    }
                    if (b8 <= -328)
                    {
                        b8 = b7 + 328;
                    }
                    spawn += 1;
                    Spawn_Load(sender, e);
                    if (Mario.Bottom == top)
                    {
                        if (spawn % 10 == 0)
                        {
                            if (runIf == 0)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk1.gif");
                                runIf = 1;
                            }
                            else if (runIf == 1)
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk2.gif");
                                runIf = 2;
                            }
                            else
                            {
                                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Walk3.gif");
                                runIf = 0;
                            }
                        }
                    }
                }
            }

            if (TimerSpace)
            {
                if (spaceG > 0)
                {
                    
                    if (!deadPadeniye)
                    {
                        Mario.Top -= spaceG;
                        for (short i = 0; i < spawnBlock_block.Count; i++)
                        {
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[i], spawnX_block[i] + rightBlock_block[i], spawnY_block[i], spawnY_block[i] + bottomBlock_block[i] }) == true)
                            {
                                
                                if (proverka_sovp_dvuh_perem_spiskov( sluchay.Count, spawnY_block[i]) == true)
                                {
                                    nam.Add(i);
                                    int[] Ar_y = new int[2];
                                    Ar_y[0] = 0;
                                    Ar_y[1] = spawnY_block[i];
                                    sluchay.Add(Ar_y);
                                    sravnenieTverdoeCosanieL = spawnX_block[i] + rightBlock_block[i] - Mario.Left;
                                    for (int m = i + 1; m < spawnBlock_block.Count; m++)
                                    {
                                        if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[m], spawnX_block[m] + rightBlock_block[m], spawnY_block[m], spawnY_block[m] + bottomBlock_block[m] }) == true)
                                        {
                                            sravnenieTverdoeCosanieR = Mario.Right - spawnX_block[m];
                                            if (sravnenieTverdoeCosanieR > sravnenieTverdoeCosanieL)
                                            {
                                                nam[nam.Count - 1] = m;
                                            }
                                            break;
                                        }
                                    }
                                    Mario.Top = spawnY_block[nam[nam.Count - 1]] + bottomBlock_block[nam[nam.Count - 1]];
                                    spaceG = 0;
                                    break;
                                }
                                else
                                {
                                    Mario.Top = spawnY_block[i] + bottomBlock_block[i];
                                    spaceG = 0;
                                    break;
                                }
                            }
                        }
                    }
                    spaceG -= 1;
                }
                else
                {
                    spaceG = 25;
                    TimerSpace = false;
                    TimerGravity = true;
                }
            }

            if (TimerGravity)
            {
                if (Mario.Bottom < top)
                {
                    Mario.Top += g;
                    g += 1;
                    if (!deadPadeniye)
                    {
                        for (short i = 0; i < spawnBlock_block.Count; i++)
                        {
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_block[i], spawnX_block[i] + rightBlock_block[i], spawnY_block[i], spawnY_block[i] + bottomBlock_block[i] }) == true)
                            {
                                Mario.Top = (spawnY_block[i] - 83);
                                g = 1;
                                top = spawnY_block[i];
                                u = i;
                                PadayTimer = true;
                            }
                        }
                        for (short i = 0; i < spawnBlock_creatures.Count; i++)
                        {
                            if (condition_creatures[i] == "dead") { continue; }
                            if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                            {
                                if (property_creatures[i] == "bonus")
                                {

                                }
                                else if (property_creatures[i] == "")
                                {
                                    g = 1;
                                    g_creatures[i] = 1;
                                    top_creatures[i] = 1500;
                                    TimerGravity = false;
                                    condition_creatures[i] = "dead";
                                    TimerSpace = true;
                                }
                            }
                        }
                    }
                    if (Mario.Bottom > top)
                    {
                        Mario.Top = top - 83;
                    }
                }
                else
                {
                    Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario.gif");
                    g = 1;
                    TimerGravity = false;
                }
            }

            if (PadayTimer)
            {
                if (Mario.Right <= spawnX_block[u])
                {
                    if (TimerGravity)
                    {
                        top = 845;
                        PadayTimer = false;
                    }
                    if (Mario.Bottom >= top)
                    {
                        top = 845;
                        TimerGravity = true;
                        PadayTimer = false;
                    }
                }
                if (Mario.Left >= (spawnX_block[u] + rightBlock_block[u]))
                {
                    if (TimerGravity)
                    {
                        top = 845;
                        PadayTimer = false;
                    }
                    if (Mario.Bottom >= top)
                    {
                        top = 845;
                        TimerGravity = true;
                        PadayTimer = false;
                    }
                }
            }

            if (SluchayTimer)
            {
                for (int i = 0; i < nam.Count; i++)
                {
                    switch (spawnBlock_block[nam[i]])
                    {
                        case "Bricks":
                            if (sluchay[i][0] >= 0 & sluchay[i][0] < 7)
                            {
                                spawnY_block[nam[i]] -= 2;
                                Knocking_out_enemy_creatures_with_a_block(sender, e, nam[i]);
                                sluchay[i][0] += 1;
                            }
                            else if (sluchay[i][0] >= 7 & sluchay[i][0] < 14)
                            {
                                spawnY_block[nam[i]] += 2;
                                sluchay[i][0] += 1;
                            }
                            else
                            {
                                if (spawnY_block[nam[i]] < sluchay[i][1])
                                {
                                    spawnY_block[nam[i]] += 1;
                                }
                                else if(spawnY_block[nam[i]] < sluchay[i][1])
                                {
                                    spawnY_block[nam[i]] -= 1;
                                }
                                else
                                {
                                    nam.RemoveAt(i);
                                    sluchay.RemoveAt(i);
                                    i--;
                                }
                            }
                            break;

                        case "LuckyBlock":
                            if (sluchay[i][0] == 0)
                            {
                                spawnBlock_block[nam[i]] = "Iron";
                                spawnY_block[nam[i]] -= 2;
                                Knocking_out_enemy_creatures_with_a_block(sender, e, nam[i]);
                                sluchay[i][0] += 1;
                            }
                            break;
                        
                        case "Pipe":
                            if (sluchay[i][0] < 83)
                            {
                                Mario.Size = new Size(83, ((Mario.Bottom - Mario.Top) - 1));
                                Mario.Top += 1;
                                sluchay[i][0] += 1;
                            }
                            else
                            {
                                if (spawnY_block[nam[i]] < sluchay[i][1])
                                {

                                }
                                else if (spawnY_block[nam[i]] < sluchay[i][1])
                                {

                                }
                                else
                                {
                                    nam.RemoveAt(i);
                                    sluchay.RemoveAt(i);
                                    i--;
                                }
                            }
                            break;

                        case "Iron":
                            if (sluchay[i][0] >= 1 & sluchay[i][0] < 7)
                            {
                                spawnY_block[nam[i]] -= 2;
                                Knocking_out_enemy_creatures_with_a_block(sender, e, nam[i]);
                                if (sluchay[i][0] == 6) 
                                {
                                    if (property_block[nam[i]] == "mushroom/flower bonus")
                                    {
                                        if (MarioMode == "ordinary")
                                        {
                                            spawnX_creatures.Add(spawnX_block[nam[i]]);
                                            spawnY_creatures.Add(spawnY_block[nam[i]]);
                                            direction_creatures.Add(true);
                                            spawnBlock_creatures.Add("mushroom bonus");
                                            rightBlock_creatures.Add(0);
                                            bottomBlock_creatures.Add(83);
                                            property_creatures.Add("bonus");
                                            condition_creatures.Add("stands not for long");
                                            top_creatures.Add(845);
                                            g_creatures.Add(1);
                                        }
                                        else if (MarioMode == "big ordinary")
                                        {
                                            spawnX_block.Add(spawnX_block[nam[i]]);
                                            spawnY_block.Add(spawnY_block[nam[i]]);
                                            spawnBlock_block.Add("flower bonus");
                                            bottomBlock_block.Add(0);
                                            rightBlock_block.Add(83);
                                            property_block.Add("bonus");
                                        }
                                    }
                                    else if (property_block[nam[i]] == "money")
                                    {

                                    }
                                    // добавлить бонус в лист list_u
                                }
                                sluchay[i][0] += 1;
                            }
                            else if (sluchay[i][0] >= 7 & sluchay[i][0] < 14)
                            {
                                spawnY_block[nam[i]] += 2;
                                sluchay[i][0] += 1;
                            }
                            else
                            {
                                nam.RemoveAt(i);
                                sluchay.RemoveAt(i);
                                i--;
                            }
                            break;
                    }
                }
            }

            if (spawnBlock_creatures.Count != 0)
            {
                if (Dvizeniye_creaturesTimer)
                {
                    for (short i = 0; i < spawnX_creatures.Count; i++)
                    {
                        if (condition_creatures[i] == "dead") { continue; }
                        if (direction_creatures[i])
                        {
                            for (short y = 0; y < 5; y++)
                            {
                                if (condition_creatures[i] != "stands not for long") 
                                {
                                    spawnX_creatures[i] += 1;
                                }
                                for (short r = 0; r < spawnBlock_block.Count; r++)
                                {
                                    if (condition_creatures[i] != "stands not for long")
                                    {
                                        if (Сheck(new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }, new int[] { spawnX_block[r], spawnX_block[r] + rightBlock_block[r], spawnY_block[r], spawnY_block[r] + bottomBlock_block[r] }) == true)
                                        {
                                            spawnX_creatures[i] = (spawnX_block[r] - 83);
                                            direction_creatures[i] = false;
                                        }
                                    }
                                }
                                if (condition_creatures[i] != "stands not for long")
                                {
                                    for (short r = 0; r < spawnX_creatures.Count; r++)
                                    {
                                        if (r == i || condition_creatures[r] != "stands not for long") { continue; }
                                        if (Сheck(new int[] { spawnX_creatures[r], spawnX_creatures[r] + rightBlock_creatures[r], spawnY_creatures[r], spawnY_creatures[r] + bottomBlock_creatures[r] }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                                        {
                                            direction_creatures[i] = false;
                                            direction_creatures[r] = true;
                                            y = 5;
                                            break;
                                        }
                                    }
                                }
                                if (!deadPadeniye)
                                {
                                    if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                                    {
                                        if (property_creatures[i] == "bonus")
                                        {

                                        }
                                        else if (property_creatures[i] == "")
                                        {
                                            Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Dead (1).gif");
                                            deadPadeniye = true;
                                            stopForm1_KeyDown = true;
                                            TimerRidht = false;
                                            TimerLeft = false;
                                            TimerSpace = false;
                                            top = 1500;
                                            g = -20;
                                            TimerGravity = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            for (short y = 0; y < 5; y++)
                            {
                                if (condition_creatures[i] != "stands not for long")
                                {
                                    spawnX_creatures[i] -= 1;
                                }
                                for (short r = 0; r < spawnBlock_block.Count; r++)
                                {
                                    if (condition_creatures[i] != "stands not for long")
                                    {
                                        if (Сheck(new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }, new int[] { spawnX_block[r], spawnX_block[r] + rightBlock_block[r], spawnY_block[r], spawnY_block[r] + bottomBlock_block[r] }) == true)
                                        {
                                            spawnX_creatures[i] = (spawnX_block[r] + rightBlock_block[r]);
                                            direction_creatures[i] = true;
                                        }
                                    }
                                }
                                if (condition_creatures[i] != "stands not for long")
                                {
                                    for (short r = 0; r < spawnX_creatures.Count; r++)
                                    {
                                        if (r == i || condition_creatures[r] != "stands not for long") { continue; }
                                        if (Сheck(new int[] { spawnX_creatures[r], spawnX_creatures[r] + rightBlock_creatures[r], spawnY_creatures[r], spawnY_creatures[r] + bottomBlock_creatures[r] }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                                        {
                                            direction_creatures[r] = false;
                                            direction_creatures[i] = true;
                                            y = 5;
                                            break;
                                        }
                                    }
                                }
                                if (!deadPadeniye)
                                {
                                    if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                                    {
                                        if (property_creatures[i] == "bonus")
                                        {

                                        }
                                        else if (property_creatures[i] == "")
                                        {
                                            Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Dead (1).gif");
                                            deadPadeniye = true;
                                            stopForm1_KeyDown = true;
                                            TimerRidht = false;
                                            TimerLeft = false;
                                            TimerSpace = false;
                                            top = 1500;
                                            g = -20;
                                            TimerGravity = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //переделывать
            if (TimerSpace_creatures)
            {
                for (short i = 0; i < spawnBlock_creatures.Count; i++)
                {
                    if (spaceG > 0)
                    {
                        spawnY_creatures[i] -= spaceG_creatures[i];
                        spaceG_creatures[i] -= 1;
                        if (condition_creatures[i] != "stands not for long")
                        {
                            for (short r = 0; r < spawnBlock_block.Count; r++)
                            {
                                if (Сheck(new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }, new int[] { spawnX_block[r], spawnX_block[r] + rightBlock_block[r], spawnY_block[r], spawnY_block[r] + bottomBlock_block[r] }) == true)
                                {
                                    spawnY_creatures[r] = spawnY_block[i] + bottomBlock_block[i];
                                    spaceG = 0;
                                }
                            }
                        }
                        if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                        {
                            if (property_creatures[i] == "bonus")
                            {

                            }
                            else if (property_creatures[i] == "")
                            {
                                stopForm1_KeyDown = true;
                                TimerRidht = false;
                                TimerLeft = false;
                                TimerSpace = false;
                                TimerGravity = false;
                            }
                        }
                    }
                    else
                    {
                        spaceG_creatures[i] = 25;
                        TimerSpace_creatures = false;
                    }
                }
            }

            if (TimerGravity_creatures)
            {
                for (short i = 0; i < spawnBlock_creatures.Count; i++)
                {
                    if (condition_creatures[i] == "dead")
                    {
                        if (spawnY_creatures[i] + bottomBlock_creatures[i] < top_creatures[i])
                        {
                            spawnY_creatures[i] += g_creatures[i];
                            g_creatures[i] += 1;
                        }
                        if (spawnY_creatures[i] + bottomBlock_creatures[i] >= top_creatures[i])
                        {
                            if (g_creatures[i] != 1)
                            {
                                g_creatures[i] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (spawnY_creatures[i] + bottomBlock_creatures[i] < top_creatures[i])
                        {
                            if (condition_creatures[i] != "stands not for long")
                            {
                                spawnY_creatures[i] += g_creatures[i];
                                g_creatures[i] += 1;
                                for (short r = 0; r < spawnBlock_block.Count; r++)
                                {
                                    if (Сheck(new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }, new int[] { spawnX_block[r], spawnX_block[r] + rightBlock_block[r], spawnY_block[r], spawnY_block[r] + bottomBlock_block[r] }) == true)
                                    {
                                        spawnY_creatures[i] = (spawnY_block[r] - bottomBlock_creatures[i]);
                                        g_creatures[i] = 1;

                                        int[] Ar = new int[2];
                                        Ar[0] = i;
                                        Ar[1] = r;
                                        list_u.Add(Ar);

                                        top_creatures[i] = spawnY_block[r];
                                    }
                                }
                                if (spawnY_creatures[i] + bottomBlock_creatures[i] > top_creatures[i])
                                {
                                    spawnY_creatures[i] = top_creatures[i] - bottomBlock_creatures[i];
                                }

                                if (!deadPadeniye)
                                {
                                    if (Сheck(new int[] { Mario.Left, Mario.Right, Mario.Top, Mario.Bottom }, new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }) == true)
                                    {
                                        if (property_creatures[i] == "bonus")
                                        {

                                        }
                                        else if (property_creatures[i] == "")
                                        {
                                            Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Dead (1).gif");
                                            deadPadeniye = true;
                                            stopForm1_KeyDown = true;
                                            TimerRidht = false;
                                            TimerLeft = false;
                                            TimerSpace = false;
                                            top = 1500;
                                            g = -20;
                                            TimerGravity = true;
                                        }
                                    }
                                }
                            }
                        }
                        if (spawnY_creatures[i] + bottomBlock_creatures[i] >= top_creatures[i])
                        {
                            if (g_creatures[i] != 1)
                            {
                                g_creatures[i] = 1;
                            }
                        }
                    }
                }
            }

            if (PadayTimer_creatures)
            {
                for(int o = 0; o < list_u.Count; o++)
                {
                    if ((spawnX_creatures[list_u[o][0]] + rightBlock_creatures[list_u[o][0]]) <= spawnX_block[list_u[o][1]])
                    {
                        top_creatures[list_u[o][0]] = 845;
                        list_u.RemoveAt(o);
                        o--;
                    }
                    else if (spawnX_creatures[list_u[o][0]] >= (spawnX_block[list_u[o][1]] + rightBlock_block[list_u[o][1]]))
                    {
                        top_creatures[list_u[o][0]] = 845;
                        list_u.RemoveAt(o);
                        o--;
                    }
                }
                
            }
            this.Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Level_1_bg, b1, 846);
            e.Graphics.DrawImage(Level_1_bg, b2, 846);
            e.Graphics.DrawImage(Level_1_bg, b3, 846);
            e.Graphics.DrawImage(Level_1_bg, b4, 846);
            e.Graphics.DrawImage(Level_1_bg, b5, 846);
            e.Graphics.DrawImage(Level_1_bg, b6, 846);
            e.Graphics.DrawImage(Level_1_bg, b7, 846);
            e.Graphics.DrawImage(Level_1_bg, b8, 846);

            for (short i = 0; i < spawnY_block.Count; i++)
            {
                //if (spawnBlock_block[i] == "flower bonus") { e.Graphics.DrawImage(Image_flower_bonus, spawnX_block[i], spawnY_block[i]); }
                if (spawnBlock_block[i] == "LuckyBlock") { e.Graphics.DrawImage(Image_LuckyBlock, spawnX_block[i], spawnY_block[i]); }
                if (spawnBlock_block[i] == "Iron") { e.Graphics.DrawImage(Image_Iron, spawnX_block[i], spawnY_block[i]); }
                if (spawnBlock_block[i] == "Bricks") { e.Graphics.DrawImage(Image_Bricks, spawnX_block[i], spawnY_block[i]); }
                if (spawnBlock_block[i] == "Pipe") { e.Graphics.DrawImage(Image_Pipe, spawnX_block[i], spawnY_block[i]); }
            }
            for (short i = 0; i < spawnY_creatures.Count; i++)
            {
                if (spawnBlock_creatures[i] == "Image_Goomba") { e.Graphics.DrawImage(Image_Goomba, spawnX_creatures[i], spawnY_creatures[i]); }
                if (spawnBlock_creatures[i] == "mushroom bonus") { e.Graphics.DrawImage(Image_mushroom_bonus, spawnX_creatures[i], spawnY_creatures[i]); }
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                run = 0;
                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario.gif");
                TimerLeft = false;
                TimerRidht = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                run = 0;
                Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario.gif");
                TimerLeft = false;
                TimerRidht = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!stopForm1_KeyDown)
            {
                if (e.KeyCode == Keys.Left)
                {
                    TimerLeft = true;
                }

                if (e.KeyCode == Keys.Right)
                {
                    TimerRidht = true;
                }

                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Up)
                {
                    if (!TimerGravity)
                    {
                        Mario.Image = Image.FromFile("C:\\Users\\ser99\\Downloads\\Mario1\\Mario1\\gif mario\\Mario - Jump.gif");
                        TimerSpace = true;
                    }
                }

                if (e.KeyCode == Keys.Down)
                {
                    for (int i = 0; i < spawnBlock_block.Count; i++)
                    {
                        if (Mario.Bottom == spawnY_block[i] & spawnBlock_block[i] == "Pipe" & property_block[i] == "down")
                        {
                            if (Mario.Left > spawnX_block[i] & Mario.Right < spawnX_block[i] + rightBlock_block[i])
                            {
                                stopForm1_KeyDown = true;
                                nam.Add(i);
                                int[] Ar_y = new int[2];
                                Ar_y[0] = 0;
                                Ar_y[1] = spawnY_block[i];
                                sluchay.Add(Ar_y);
                            }
                        }
                    }
                }
            }
        }
        private void Knocking_out_enemy_creatures_with_a_block(object sender, EventArgs e, int nam)
        {
            for (int i = 0; i < condition_creatures.Count; i++)
            {
                if (Сheck(new int[] { spawnX_creatures[i], spawnX_creatures[i] + rightBlock_creatures[i], spawnY_creatures[i], spawnY_creatures[i] + bottomBlock_creatures[i] }, new int[] { spawnX_block[nam], spawnX_block[nam] + rightBlock_block[nam], spawnY_block[nam], spawnY_block[nam] + bottomBlock_block[nam]}) == true)
                {
                    g_creatures[i] = 1;
                    top_creatures[i] = 1500;
                    condition_creatures[i] = "dead";
                    for (int r = 0; r < list_u.Count; r++)
                    {
                        if (list_u[r][0] == i)
                        {
                            list_u.RemoveAt(r);
                            break;
                        }
                    }
                }
            }
        }

        public bool Сheck(int[] object_one, int[] object_two)
        {
            if (object_one[1] > object_two[0] & object_one[2] < object_two[3] & object_one[3] > object_two[2] & object_one[0] < object_two[1])
            {
                return true;
            }
            return false;
        }

        public bool proverka_sovp_dvuh_perem_spiskov(int long_pervoe, int vtoroe)
        {
            for (int r = 0; r < long_pervoe; r++)
            {
                if (sluchay[r][1] == vtoroe) { break; return false; }
                else
                {
                    return true;
                }
            }
            return true;
        }

        // Вспомогательные методы для добавления блока
        private void AddBlock(short x, short y, string type, int height, int width, string property)
        {
            spawnX_block.Add(x);
            spawnY_block.Add(y);
            spawnBlock_block.Add(type);
            bottomBlock_block.Add(height);
            rightBlock_block.Add(width);
            property_block.Add(property);
        }

        private void Zp(short x, short y, string type)
        {
            spawnX_zp.Add(x);
            spawnY_zp.Add(y);
            spawnBlock_zp.Add(type);
        }

        private void Creatures(short x, short y, bool direction, string type, short width, short height, string property, string condition)
        {
            spawnX_creatures.Add(x);
            spawnY_creatures.Add(y);
            direction_creatures.Add(direction);
            spawnBlock_creatures.Add(type);
            rightBlock_creatures.Add(width);
            bottomBlock_creatures.Add(height);
            property_creatures.Add(property);
            condition_creatures.Add(condition);
            top_creatures.Add(845);
            g_creatures.Add(1);
        }

        // Спавн 
        private void Spawn_Load(object sender, EventArgs e)
        {
            label1.Text = spawn.ToString();
            switch (spawn)
            {
                case 10:
                    AddBlock(2000, 550, "LuckyBlock", 83, 83, "money");
                    break;
                case 50:
                    AddBlock(2000, 550, "Bricks", 83, 83, "");
                    AddBlock(2083, 550, "LuckyBlock", 83, 83, "mushroom/flower bonus");
                    AddBlock(2000 + (83 * 2), 550, "Bricks", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 300, "LuckyBlock", 83, 83, "money");
                    AddBlock(2000 + (83 * 3), 550, "LuckyBlock", 83, 83, "money");
                    AddBlock(2000 + (83 * 4), 550, "Bricks", 83, 83, "");
                    break;
                case 110:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    Creatures(1900, 300, false, "Image_Goomba", 83, 83, "", "bazovoye");
                    Creatures(1800, 300, false, "Image_Goomba", 83, 83, "", "bazovoye");
                    break;
                case 170:
                    Creatures(1800, 762, false, "Image_Goomba", 83, 83, "", "bazovoye");
                    Creatures(2000, 762, true, "Image_Goomba", 83, 83, "", "bazovoye");
                    AddBlock(2100, 680, "Pipe", 166, 166, "");
                    break;
                case 210:
                    Creatures(1900, 762, true, "Image_Goomba", 83, 83, "", "bazovoye");
                    Creatures(2000, 762, false, "Image_Goomba", 83, 83, "", "bazovoye");
                    AddBlock(2500, 680, "Pipe", 166, 166, "down");
                    break;
                case 270:
                    Creatures(2000, 762, false, "Image_Goomba", 83, 83, "", "bazovoye");
                    AddBlock(2500, 680, "Pipe", 166, 166, "");
                    break;
                case 310:
                    AddBlock(2000, 550, "LuckyBlock", 83, 83, "");
                    break;
                case 350:
                    AddBlock(2000, 550, "Bricks", 83, 83, "");
                    AddBlock(2083, 550, "LuckyBlock", 83, 83, "money");
                    AddBlock(2000 + (83 * 2), 550, "Bricks", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 300, "LuckyBlock", 83, 83, "money");
                    AddBlock(2000 + (83 * 3), 550, "LuckyBlock", 83, 83, "money");
                    AddBlock(2000 + (83 * 4), 550, "Bricks", 83, 83, "");
                    break;
                case 410:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 500:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 550:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 600:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 610:
                    AddBlock(2000, 550, "LuckyBlock", 83, 83, "money");
                    break;
                case 650:
                    AddBlock(2000, 550, "Bricks", 83, 83, "");
                    AddBlock(2083, 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 550, "Bricks", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 300, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 3), 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 4), 550, "Bricks", 83, 83, "");
                    break;
                case 710:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 800:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 850:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 900:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 910:
                    AddBlock(2000, 550, "LuckyBlock", 83, 83, "");
                    break;
                case 950:
                    AddBlock(2000, 550, "Bricks", 83, 83, "");
                    AddBlock(2083, 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 550, "Bricks", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 300, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 3), 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 4), 550, "Bricks", 83, 83, "");
                    break;
                case 1010:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1100:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1150:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1200:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1210:
                    AddBlock(2000, 550, "LuckyBlock", 83, 83, "");
                    break;
                case 1250:
                    AddBlock(2000, 550, "Bricks", 83, 83, "");
                    AddBlock(2083, 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 550, "Bricks", 83, 83, "");
                    AddBlock(2000 + (83 * 2), 300, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 3), 550, "LuckyBlock", 83, 83, "");
                    AddBlock(2000 + (83 * 4), 550, "Bricks", 83, 83, "");
                    break;
                case 1310:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1400:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1450:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
                case 1500:
                    AddBlock(2000, 680, "Pipe", 166, 166, "");
                    break;
            }
        }
    }
}