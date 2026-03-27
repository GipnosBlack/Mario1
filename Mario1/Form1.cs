namespace Mario1
{
    public partial class Form1 : Form
    {
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        private Button btnStart;

        private int[] brick_floor;//êàðòèíêè êó÷è êèðïè÷åé
        private int spawn;//îòâå÷àåò êîãäà ñïàâíèòü ÷òî ëèáî
        public int lives;
        private List<Creature> ñreatures;
        private List<Block> blocks;
        private List<Background> backgrounds;
        private Mario mario;
        private Navigator nav;
        private Image bg;
        private bool stop_movement_location;//äëÿ êîíöîâêè

        // Ããëàâíûé èãðîâîé òàéìåð
        private System.Windows.Forms.Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            // Íàñòðîéêà ôîðìû äëÿ ïðîèçâîäèòåëüíîé îòðèñîâêè (Double buffering)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            KeyPreview = true; // Ïîçâîëÿåò ôîðìå ïåðåõâàòûâàòü ñîáûòèÿ êëàâèàòóðû
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 5;
            gameTimer.Tick += GameTimer_Tick;
            if (screenWidth == 0) screenWidth = 2000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Menu();
        }

        public void Menu()
        {
            BackColor = Color.Black;
            btnStart = new Button();
            btnStart.Size = new Size(280, 107);
            btnStart.BackgroundImage = Properties.Resources.start;
            btnStart.Location = new Point((screenWidth - btnStart.Size.Width) / 2, screenHeight - screenHeight / 3);
            btnStart.CausesValidation = false;
            Controls.Add(btnStart);
            btnStart.Click += button1_Click;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GameStart();
            btnStart.Dispose();
        }

        public void GameStart()
        {
            stop_movement_location = false;
            brick_floor = [0, 328, 656, 984, 1312, 1640, 1968, 2296];
            spawn = 0;
            lives = 3;
            ñreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            nav = new Navigator();
            mario = new Mario(0, 762, nav.base_height_ordinary("ordinary"), nav.base_width_ordinary);
            BackColor = Color.DodgerBlue;
            Spawn_Load();
            gameTimer.Start();
        }

        // Ãëàâíûé èãðîâîé öèêë
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            Left_Mario();
            Right_Mario();
            Sliding_Mario();
            Jamp_Mario();
            Padenie_Mario();
            Paday_Mario();
            Actions();
            for (short i = 0; i < ñreatures.Count; i++)
            {
                i = Jump_ñreatures(i);
                if (i == -1) break;
                i = Fall_ñreatures(i);
                if (i == -1) break;
                i = Creatures_come_out(i);
                if (i == -1) break;
                i = Movement_creatures_X(i);
                if (i == -1) break;
                if (ñreatures[i].X < -200)
                {
                    blocks = ñreatures[i].Check_u(blocks, i, "dead");
                    ñreatures.RemoveAt(i);
                    i -= 1;
                }
                if (i == -1) break;
            }
            for (int i = 0; i < backgrounds.Count; i++)
            {
                if (mario.X >= backgrounds[i].X) {   
                    if (backgrounds[i].name == "End")
                    {
                        nav.Switching_between_locations(backgrounds[i].location_transfer);
                        Switching_between_locations();
                    }
                }
            }
            mario.Intangible_Mario();
            if (mario.pause_atack_fire_bar > 0) mario.pause_atack_fire_bar -= 1;
            this.Invalidate();
            if (mario.Y + mario.height >= 3000) Dead_mario_restart();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (mario is null) return;
            for (int i = backgrounds.Count - 1; i >= 0; i--)
            {
                e.Graphics.DrawImage(backgrounds[i].image, backgrounds[i].X, backgrounds[i].Y);
            }
            for (int i = 0; i < brick_floor.Length; i++)
            {
                e.Graphics.DrawImage(bg, brick_floor[i], 846);
            }
            if (mario.image is not null) { e.Graphics.DrawImage(mario.image, mario.X - (((83 - nav.base_width_ordinary) / 2)), mario.Y); }
            for (int i = ñreatures.Count - 1; i >= 0; i--)
            {
                if (ñreatures[i].height <= ñreatures[i].proper_height) e.Graphics.DrawImage(ñreatures[i].image, ñreatures[i].X, ñreatures[i].Y, ñreatures[i].DestRect(), GraphicsUnit.Pixel);
                else e.Graphics.DrawImage(ñreatures[i].image, ñreatures[i].X, ñreatures[i].Y);
            }
            for (int i = blocks.Count - 1; i >= 0; i--)
            {
                e.Graphics.DrawImage(blocks[i].image, blocks[i].X, blocks[i].Y);
            }
        }

        private void Left_Mario()
        {
            if (mario.TimerLeft)
            {
                if (mario.X > 0)
                {
                    for (short y = 0; y < mario.speed; y++)
                    {
                        mario.X -= 1;
                        for (short i = 0; i < ñreatures.Count; i++)
                        {
                            if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & ñreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & ñreatures[i].property != "Attack against creatures" & ñreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill")
                            {
                                if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                                {
                                    i = Mario_in_ñreatures(i);
                                }
                            }
                        }
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }))
                            {
                                mario.X = blocks[i].X + blocks[i].width;
                                mario.run_animation = 0;
                                mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                                mario.speed = 1;
                                return;
                            }
                        }
                        if (mario.u != -1)
                        {
                            if (mario.X + mario.width < blocks[mario.u].X)
                            {
                                mario.u = -1;
                                if (mario.TimerGravity)
                                {
                                    mario.top = 845;
                                }
                                if (mario.Y + mario.height >= mario.top)
                                {
                                    mario.top = 845;
                                    mario.TimerGravity = true;
                                }
                                break;
                            }
                        }
                    }
                    if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                    {
                        if (!mario.acceleration) mario.speed++;
                        else mario.speed += 2;
                    }
                    else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                    if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                    {
                        mario.defines_the_image("Walk");
                    }
                    mario.run_animation += 1;
                }
                else
                {
                    mario.run_animation = 0;
                    mario.TimerSliding = false;
                    mario.TimerLeft = false;
                    if (!mario.sits) mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                    mario.speed = 1;
                }
            }
        }

        private void Right_Mario()
        {
            if (mario.TimerRight)
            {
                if (mario.X < screenWidth / 2 - screenWidth / 10 || stop_movement_location)
                {
                    for (short y = 0; y < mario.speed; y++)
                    {
                        mario.X += 1;
                        for (short i = 0; i < ñreatures.Count; i++)
                        {
                            if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & ñreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & ñreatures[i].property != "Attack against creatures" & ñreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill")
                            {
                                if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                                {
                                    i = Mario_in_ñreatures(i);
                                }
                            }
                        }
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                            {
                                if (blocks[i].property == "return_between_locations") 
                                {
                                    Switching_return_between_locations(return_location: blocks[i].location_transfer);
                                    return;
                                }
                                mario.X = blocks[i].X - (mario.X + mario.width - mario.X);
                                mario.run_animation = 0;
                                mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                                mario.speed = 1;
                                return;
                            }
                        }
                        if (mario.u != -1)
                        {
                            if (mario.X > (blocks[mario.u].X + blocks[mario.u].width))
                            {
                                mario.u = -1;
                                if (mario.TimerGravity)
                                {
                                    mario.top = 845;
                                }
                                if (mario.Y + mario.height >= mario.top)
                                {
                                    mario.top = 845;
                                    mario.TimerGravity = true;
                                }
                                break;
                            }
                        }
                    }
                    if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                    {
                        if(!mario.acceleration) mario.speed++;
                        else mario.speed += 2;
                    }
                    else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                    if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                    {
                        mario.defines_the_image("Walk");
                    }
                }
                else
                {
                    for (short y = 0; y < mario.speed; y++)
                    {
                        spawn++;
                        Spawn_Load();
                        for (int i = 0; i < brick_floor.Length; i++)
                        {
                            brick_floor[i] -= 1;
                        }
                        for (short i = 0; i < ñreatures.Count; i++)
                        {
                            ñreatures[i].X -= 1;
                            if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & ñreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & ñreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill")
                            {
                                if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                                {
                                    i = Mario_in_ñreatures(i);
                                }
                            }
                        }
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            blocks[i].X -= 1;
                            if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                            {
                                if (blocks[i].property == "return_between_locations")
                                {
                                    Switching_return_between_locations(return_location: blocks[i].location_transfer);
                                    return;
                                }
                                mario.X = (blocks[i].X - (mario.X + mario.width - mario.X));
                                mario.run_animation = 0;
                                if (!mario.sits) mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                                mario.speed = 1;
                                return;
                            }
                            if (blocks[i].X < -200)
                            {
                                blocks.RemoveAt(i);
                                for (int r = 0; r < mario.nam.Count; r++)
                                {
                                    if (mario.nam[r] == i) { mario.nam.RemoveAt(r); r--; }
                                    else if (mario.nam[r] > i) mario.nam[r] -= 1;
                                }
                                if (mario.u > i) mario.u -= 1;
                                i -= 1;
                            }
                        }
                        for (short i = 0; i < backgrounds.Count; i++) backgrounds[i].X--;
                        if (mario.u != -1)
                        {
                            if (mario.X > (blocks[mario.u].X + blocks[mario.u].width))
                            {
                                mario.u = -1;
                                if (mario.TimerGravity)
                                {
                                    mario.top = 845;
                                }
                                if (mario.Y + mario.height >= mario.top)
                                {
                                    mario.top = 845;
                                    mario.TimerGravity = true;
                                }
                                break;
                            }
                        }
                    }
                    if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                    {
                        if (!mario.acceleration) mario.speed++;
                        else mario.speed += 2;
                    }
                    else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                    for (int i = 0; i < brick_floor.Length; i++)
                    {
                        if (brick_floor[i] <= -328)
                        {
                            if (i == 0) brick_floor[i] = brick_floor[brick_floor.Length - 1] + 328;
                            else brick_floor[i] = brick_floor[i - 1] + 328;
                        }
                    }
                    if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                    {
                        mario.defines_the_image("Walk");
                    }
                }
                mario.run_animation += 1;
            }
        }

        private void Sliding_Mario()
        {
            if (mario.TimerSliding)
            {
                if (mario.run_animation % 10 == 0)
                {
                    if (mario.speed <= 1)
                    {
                        mario.speed = 1;
                        if (!mario.sits) mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                        mario.run_animation = 0;
                        mario.TimerLeft = false;
                        mario.TimerRight = false;
                        mario.TimerSliding = false;
                        mario.braking2 = false;
                    }
                    else
                    {
                        if (mario.braking2)
                        {
                            mario.speed -= 2;
                            if (mario.braking)
                            {
                                if (mario.direction) mario.direction = false;
                                else mario.direction = true;
                                if (!mario.sits) mario.defines_the_image("Skid");
                                mario.braking = false;
                            }
                        }
                        if (mario.speed > 1) mario.speed -= 2;
                    }
                }
            }
        }

        private void Jamp_Mario()
        {
            if (mario.TimerSpace)
            {
                if (mario.spaceG > 0)
                {
                    if (!mario.deadPadeniye)
                    {
                        mario.top = 845;
                        mario.Y -= mario.spaceG;
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                            {
                                int sravnenieTverdoeCosanieL = blocks[i].X + blocks[i].width - mario.X;
                                for (int m = i + 1; m < blocks.Count; m++)
                                {
                                    if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[m].X, blocks[m].X + blocks[m].width, blocks[m].Y, blocks[m].Y + blocks[m].height }) == true)
                                    {
                                        int sravnenieTverdoeCosanieR = mario.X + mario.width - blocks[m].X;
                                        if (sravnenieTverdoeCosanieR > sravnenieTverdoeCosanieL)
                                        {
                                            i = m;
                                        }
                                        break;
                                    }
                                }
                                int t = proverka_sovp_dvuh_perem_spiskov(i);
                                if (t != -1)
                                {
                                    blocks[t].sluchay[0] = 14 - blocks[t].sluchay[0];
                                }
                                else
                                {
                                    mario.nam.Add(i);
                                    blocks[i].sluchay = [0, blocks[i].Y];
                                }
                                mario.Y = blocks[mario.nam[mario.nam.Count - 1]].Y + blocks[mario.nam[mario.nam.Count - 1]].height;
                                mario.spaceG = 0;
                                break;
                            }
                        }
                    }
                    mario.spaceG -= 1;
                }
                else
                {
                    mario.spaceG = 30;
                    mario.TimerSpace = false;
                    mario.TimerGravity = true;
                }
            }
        }

        private void Padenie_Mario()
        {
            if (mario.TimerGravity)
            {
                if (mario.Y + mario.height < mario.top)
                {
                    mario.Y += mario.g;
                    mario.g += 1;
                    if (!mario.deadPadeniye)
                    {
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                            {
                                mario.Y = (blocks[i].Y - (mario.Y + mario.height - mario.Y));
                                mario.g = 1;
                                mario.top = blocks[i].Y;
                                mario.u = i;
                            }
                        }
                        if (mario.mode != "intangible ordinary")
                        {
                            for (short i = 0; i < ñreatures.Count; i++)
                            {
                                if (ñreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall") { continue; }
                                if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                                {
                                    i = Creatures_in_mario(i);
                                }
                            }
                        }
                    }
                    if (mario.Y + mario.height > mario.top)
                    {
                        mario.u = -1;
                        mario.Y = mario.top - (mario.Y + mario.height - mario.Y);
                    }
                }
                else
                {
                    if (mario.sits) mario.defines_the_image("Duck");
                    else mario.defines_the_image("Mario/Super Mario/Fiery Mario");
                    mario.g = 1;
                    mario.TimerGravity = false;
                }
            }
        }

        private void Paday_Mario()
        {
            if (mario.u != -1)
            {
                if (mario.X + mario.width < blocks[mario.u].X)
                {
                    mario.u = -1;
                    if (mario.TimerGravity)
                    {
                        mario.top = 845;
                    }
                    if (mario.Y + mario.height >= mario.top)
                    {
                        mario.top = 845;
                        mario.TimerGravity = true;
                    }
                }
                if (mario.X > (blocks[mario.u].X + blocks[mario.u].width))
                {
                    mario.u = -1;
                    if (mario.TimerGravity)
                    {
                        mario.top = 845;
                    }
                    if (mario.Y + mario.height >= mario.top)
                    {
                        mario.top = 845;
                        mario.TimerGravity = true;
                    }
                }
            }
        }

        private void Actions()
        {
            for (int i = 0; i < mario.nam.Count; i++)
            {
                if (blocks[mario.nam[i]].sluchay is null) continue;
                int r = mario.nam[i];
                switch (blocks[r].name)
                {
                    case "Bricks":
                        if (mario.mode == "ordinary")
                        {
                            if (blocks[r].sluchay[0] >= 0 & blocks[r].sluchay[0] < 7)
                            {
                                blocks[r].Y -= 2;
                                Knocking_out_enemy_creatures_with_a_block(r);
                                blocks[r].sluchay[0] += 1;
                            }
                            else if (blocks[r].sluchay[0] >= 7 & blocks[r].sluchay[0] < 14)
                            {
                                blocks[r].Y += 2;
                                blocks[r].sluchay[0] += 1;
                            }
                            else
                            {
                                if (blocks[r].Y < blocks[r].sluchay[1])
                                {
                                    blocks[r].Y += 1;
                                }
                                else if (blocks[r].Y > blocks[r].sluchay[1])
                                {
                                    blocks[r].Y -= 1;
                                }
                                else
                                {
                                    blocks[r].sluchay = null;
                                    mario.nam.RemoveAt(i);
                                    i -= 1;
                                }
                            }
                        }
                        else if (mario.mode == "big ordinary" || mario.mode == "big shooter")
                        {
                            if (mario.u > r) { mario.u -= 1; }
                            blocks.RemoveAt(r);
                            mario.nam.RemoveAt(i);
                            for (int t = 0; t < mario.nam.Count; t++)
                            {
                                if (mario.nam[t] > r) mario.nam[t] -= 1;
                            }
                            i -= 1;
                        }
                        break;
                        
                    case "LuckyBlock":
                        if (blocks[r].sluchay[0] == 0)
                        {
                            blocks[r].name = "Iron";
                            blocks[r].image = Properties.Resources.EmptyBlock;
                            blocks[r].Y -= 2;
                            Knocking_out_enemy_creatures_with_a_block(r);
                            blocks[r].sluchay[0] += 1;
                        }
                        break;

                    case "Pipe":
                        if (blocks[r].sluchay[1] >= mario.Y)
                        {
                            mario.Y += 1;
                            blocks[r].sluchay[0] += 1;
                            mario.TimerRight = false;
                            mario.TimerLeft = false;
                            mario.TimerGravity = false;
                            mario.TimerSpace = false;
                            mario.TimerSliding = false;
                        }
                        else
                        {
                            nav.Switching_between_locations(blocks[r].location_transfer);
                            Switching_between_locations();
                        }
                        break;

                    case "Iron":
                        if (blocks[r].sluchay[0] >= 1 & blocks[r].sluchay[0] < 7)
                        {
                            blocks[r].Y -= 2;
                            Knocking_out_enemy_creatures_with_a_block(r);
                            if (blocks[r].sluchay[0] == 6)
                            {
                                if (blocks[r].property == "mushroom/flower bonus")
                                {
                                    if (mario.mode == "ordinary") ñreatures.Add(new Creature(x: blocks[r].X, y: blocks[r].sluchay[1], name: "mushroom bonus", height: 0, width: 83, direction: true, property: "bonus", top: 845, g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Super_Mushroom, proper_height: 83));
                                    else if (mario.mode == "big ordinary" || mario.mode == "big shooter") ñreatures.Add(new Creature ( x: blocks[r].X, y: blocks[r].sluchay[1], name: "flower bonus", height: 0, width: 83, direction: true, property: "bonus", top: 845, g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Fire_Flower__1_, proper_height: 83));
                                }
                                //èñêëþ÷åíèå â top!!! (ñþäà ñîõðàíÿåòñÿ çíà÷åíèå sluchay[i][1], ÷òîáû ñðàâíèòü ñ ðåàëüíûì spawnY_creatures äëÿ óäàëåíèÿ ïðè äîñòèæåíèè îïðåäåë¸ííîé âûñîòû)
                                else if (blocks[mario.nam[i]].property == "money") ñreatures.Add(new Creature(x: blocks[r].X + (blocks[r].width / 2) - 16, y: blocks[r].sluchay[1], name: "money bonus", height: 0, width: 32, direction: true, property: "bonus", top: blocks[r].sluchay[1], g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Coin, proper_height: 56));
                            }
                            blocks[r].sluchay[0] += 1;
                        }
                        else if (blocks[r].sluchay[0] >= 7 & blocks[r].sluchay[0] < 14)
                        {
                            blocks[r].Y += 2;
                            blocks[r].sluchay[0] += 1;
                        }
                        else
                        {
                            if (blocks[r].Y < blocks[r].sluchay[1])
                            {
                                blocks[r].Y += 1;
                            }
                            else if (blocks[r].Y > blocks[r].sluchay[1])
                            {
                                blocks[r].Y -= 1;
                            }
                            else
                            {
                                blocks[r].Y = blocks[r].sluchay[1];
                                blocks[r].sluchay = null;
                                mario.nam.RemoveAt(i);
                                i -= 1;
                            }
                        }
                        break;
                }
            }
        }

        private short Creatures_come_out(short i)
        {
            if (ñreatures[i].property == "bonus" & ñreatures[i].condition.Find(x => x == "stands") == "stands")
            {
                if (ñreatures[i].name == "mushroom bonus" || ñreatures[i].name == "flower bonus")
                {
                    if (ñreatures[i].height >= ñreatures[i].proper_height)
                    {
                        if (ñreatures[i].name == "mushroom bonus") ñreatures[i].condition.Remove("stands");
                    }
                    else
                    {
                        ñreatures[i].Y -= 1;
                        ñreatures[i].height += 1;
                    }
                }
                else if (ñreatures[i].name == "money bonus")
                {
                    if (ñreatures[i].height < ñreatures[i].proper_height)
                    {
                        mario.coin += 1;
                        if (mario.coin == 100)
                        {
                            mario.coin = 0;
                            lives++;
                        }
                        ñreatures[i].Y -= 6;
                        ñreatures[i].height += 6;
                    }
                    else
                    {
                        ñreatures[i].Y -= 6;
                        if (ñreatures[i].Y < ñreatures[i].top - 150)// top_creatures[i] èç èñêëþ÷åíèÿ!!! (ââîäèòñÿ ïðè ñîçäàíèè money bonus)
                        {
                            blocks = ñreatures[i].Check_u(blocks, i, "dead");
                            ñreatures.RemoveAt(i);
                            i -= 1;
                        }
                    }
                }
            }
            return i;
        }

        private short Movement_creatures_X(short i)
        {
            if (ñreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall") return i;
            if (ñreatures[i].condition.Find(x => x == "stands") != "stands")
            {
                if (ñreatures[i].direction)
                {
                    ñreatures[i].X += 5;
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (Ñheck(new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            ñreatures[i].X = blocks[r].X - ñreatures[i].width;
                            ñreatures[i].direction = false;
                        }
                        Check_u(i, r);
                    }
                }
                else
                {
                    ñreatures[i].X -= 5;
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (Ñheck(new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            ñreatures[i].X = blocks[r].X + blocks[r].width;
                            ñreatures[i].direction = true;
                        }
                        Check_u(i, r);
                    }
                }
                ñreatures[i].Animation("Walk");
            }
            for (short r = 0; r < ñreatures.Count; r++)
            {
                if (r == i || ñreatures[i].condition.Find(x => x == "stands") == "stands") continue;
                if (Ñheck(new int[] { ñreatures[r].X, ñreatures[r].X + ñreatures[r].width, ñreatures[r].Y, ñreatures[r].Y + ñreatures[r].height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                {
                    if ((ñreatures[r].property == "Attack against creatures" & ñreatures[i].property == "") || (ñreatures[i].property == "Attack against creatures" & ñreatures[r].property == ""))
                    {
                        blocks = ñreatures[i].Check_u(blocks, i, "dead");
                        ñreatures.RemoveAt(i);
                        if (i < r) { r -= 1; i -= 1; }
                        else i -= 2;
                        blocks = ñreatures[r].Check_u(blocks, r, "dead");
                        ñreatures.RemoveAt(r);
                        if (i == -1) return i;
                        break;
                    }
                    else if (ñreatures[r].property != "Attack against creatures" & ñreatures[i].property != "Attack against creatures")
                    {
                        if (ñreatures[i].direction != true) ñreatures[i].direction = true;
                        else ñreatures[i].direction = false;
                        if (ñreatures[r].direction != false) ñreatures[r].direction = false;
                        else ñreatures[r].direction = true;
                        break;
                    }
                }
            }
            if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & ñreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill")
                if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                    i = Mario_in_ñreatures(i);
            return i;
        }

        private short Jump_ñreatures(short i)
        {
            if (ñreatures[i].spaceG > 0)
            {
                ñreatures[i].Y -= ñreatures[i].spaceG;
                ñreatures[i].spaceG -= 1;
                if (ñreatures[i].condition.Find(x => x == "stands") != "stands")
                {
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (Ñheck(new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            ñreatures[i].Y = blocks[r].Y + blocks[i].height;
                            ñreatures[i].spaceG = 0;
                        }
                    }
                }
                if (mario.mode != "intangible ordinary")
                {
                    if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                    {
                        i = Creatures_in_mario(i);
                    }
                }
            }
            else
            {
                if (ñreatures[i].condition.Find(x => x == "jamp") == "jamp")
                {
                    ñreatures[i].spaceG = 25;
                }
            }
            return i;
        }

        private short Fall_ñreatures(short i)
        {
            //if (ñreatures[i].name != "SMB_greenparatrooper" & ñreatures[i].name != "SMB_greenkoopatroopa") return i;
            if (ñreatures[i].Y + ñreatures[i].height < ñreatures[i].top)
            {
                for (int r = 0; r < ñreatures[i].g; r++)
                {
                    ñreatures[i].Y += 1;
                    if (ñreatures[i].Y + ñreatures[i].height >= ñreatures[i].top) break;
                }
                ñreatures[i].g += 1;
                if (ñreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall") return i;
                for (short r = 0; r < blocks.Count; r++)
                {
                    if (Ñheck(new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                    {
                        ñreatures[i].Y = (blocks[r].Y - ñreatures[i].height);
                        blocks[r].u = i;
                        ñreatures[i].top = blocks[r].Y;
                    }
                }
                if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & ñreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill")
                {
                    if (Ñheck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }) == true)
                    {
                        i = Mario_in_ñreatures(i);
                        return i;
                    }
                }
            }
            else
            {
                ñreatures[i].g = 1;
            }
            return i;
        }

        private short Creatures_in_mario(short i)
        {
            if (ñreatures[i].property == "bonus")
            {
                if (mario.mode == "ordinary")
                {
                    mario.Y -= nav.base_height_ordinary(mario.mode); ;
                    mario.mode = "big ordinary";
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);

                    mario.defines_the_image("Super Mario");
                }
                else if (mario.mode == "big ordinary")
                {
                    mario.mode = "big shooter";
                    mario.defines_the_image("Fiery Mario");
                }
                blocks = ñreatures[i].Check_u(blocks, i, "dead");
                ñreatures.RemoveAt(i);
                i -= 1;
                return i;
            }
            else if (ñreatures[i].property == "")
            {
                mario.g = 1;
                mario.spaceG = 15;
                mario.TimerGravity = false;
                mario.TimerSpace = true;
                if (ñreatures[i].name == "SMB_greenparatrooper" || ñreatures[i].name == "SMB_greenkoopatroopa")
                {
                    ñreatures[i].g = 15;
                    ñreatures[i].spaceG = 0;
                    ñreatures[i].height = 38;
                    ñreatures[i].width = 45;
                    ñreatures[i].condition.Remove("jamp");
                    ñreatures[i].condition.Add("stands");
                    ñreatures[i].condition.Add("doesn_t_kill");
                    ñreatures[i].Animation("Dead");
                }
                else if (ñreatures[i].name == "Image_Goomba")
                {
                    ñreatures[i].g = 15;
                    ñreatures[i].height = 35;
                    ñreatures[i].condition.Add("intangible_ordinary");
                    ñreatures[i].condition.Add("stands");
                    ñreatures[i].condition.Add("doesn_t_kill");
                    ñreatures[i].Animation("Dead");
                }
            }
            return i;
        }

        private short Mario_in_ñreatures(short i)
        {
            if (ñreatures[i].property == "bonus")
            {
                if (mario.mode == "ordinary")
                {
                    mario.Y -= nav.base_height_ordinary(mario.mode);
                    mario.mode = "big ordinary";
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.defines_the_image("Super Mario");
                }
                else if (mario.mode == "big ordinary")
                {
                    mario.mode = "big shooter";
                    mario.defines_the_image("Fiery Mario");
                }
                blocks = ñreatures[i].Check_u(blocks, i, "dead");
                ñreatures.RemoveAt(i);
                i -= 1;
            }
            else if (ñreatures[i].property == "")
            {
                if (mario.mode == "ordinary")
                {
                    mario.Mario_Dead();
                }
                else if (mario.mode == "big ordinary" || mario.mode == "big shooter")
                {
                    mario.mode = "intangible ordinary";
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.Y += nav.base_height_ordinary(mario.mode);
                    Get_up_from_your_squats();
                    mario.defines_the_image("Mario");
                }
            }
            return i;
        }

        private void Check_u(int creatures_i, int blocks_i)
        {
            if (blocks[blocks_i].u == -1 || ñreatures[creatures_i].top == 845) return;
            if ((ñreatures[creatures_i].X + ñreatures[creatures_i].width) < blocks[blocks_i].X || ñreatures[creatures_i].X > (blocks[blocks_i].X + blocks[blocks_i].width))
            {
                ñreatures[creatures_i].top = 845;
                blocks[blocks_i].u = -1;
            }
        }

        private void Get_up_from_your_squats()
        {
            if (mario.sits == true)
            {
                if (mario.mode == "intangible ordinary")
                {
                    mario.Y -= nav.base_height_ordinary("big ordinary") - nav.base_height_ordinary("sits");
                }
                else if (mario.mode == "big ordinary")
                {
                    mario.defines_the_image("Super Mario");
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.Y -= nav.base_height_ordinary(mario.mode) - nav.base_height_ordinary("sits");
                }
                else if (mario.mode == "big shooter")
                {
                    mario.defines_the_image("Fiery Mario");
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.Y -= nav.base_height_ordinary(mario.mode) - nav.base_height_ordinary("sits");
                }
                mario.sits = false;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (mario is null) return;
            if (e.KeyCode == Keys.Left & !mario.TimerSliding & mario.TimerLeft)
            {
                mario.TimerSliding = true;
            }
            if (e.KeyCode == Keys.Right & !mario.TimerSliding & mario.TimerRight)
            {
                mario.TimerSliding = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                Get_up_from_your_squats();
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                mario.max_speed = 10;
                mario.acceleration = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (mario is null) return;
            if (!mario.stopForm1_KeyDown)
            {
                if (e.KeyCode == Keys.Left & !mario.sits)
                {
                    if (mario.TimerRight)
                    {
                        mario.TimerSliding = true;
                        mario.braking = true;
                        mario.braking2 = true;
                    }
                    else if ((!mario.TimerLeft & !mario.TimerSliding & !mario.TimerRight) || (mario.TimerLeft & mario.TimerSliding & !mario.TimerRight))
                    {
                        mario.TimerLeft = true;
                        mario.direction = false;
                        mario.TimerSliding = false;
                        if (mario.mode == "big shooter" & mario.pause_atack_fire_bar == 0)
                            Get_up_from_your_squats();
                    }
                }

                if (e.KeyCode == Keys.Right & !mario.sits)
                {
                    if (mario.TimerLeft)
                    {
                        mario.TimerSliding = true;
                        mario.braking = true;
                        mario.braking2 = true;
                    }
                    else if ((!mario.TimerRight & !mario.TimerSliding & !mario.TimerLeft) || (mario.TimerRight & mario.TimerSliding & !mario.TimerLeft))
                    {
                        mario.TimerRight = true;
                        mario.direction = true;
                        mario.TimerSliding = false;
                        if (mario.mode == "big shooter" & mario.pause_atack_fire_bar == 0)
                            Get_up_from_your_squats();
                    }
                }

                if (e.KeyCode == Keys.Space)
                {
                    if (mario.mode == "big shooter" & mario.pause_atack_fire_bar == 0)
                    {
                        if (mario.direction == true) ñreatures.Add(new Creature(x: mario.X + 32, y: mario.Y + 32, direction: true, name: "Fire bar", width: 16, height: 16, condition: "jamp", property: "Attack against creatures", g: 1, spaceG: 25, top: 845, image: Properties.Resources.Fire_bar));
                        else ñreatures.Add(new Creature(x: mario.X + 32, y: mario.Y + 32, direction: false, name: "Fire bar", width: 16, height: 16, condition: "jamp", property: "Attack against creatures", g: 1, spaceG: 25, top: 845, image: Properties.Resources.Fire_bar));
                        mario.pause_atack_fire_bar = 50;
                    }
                }

                if (e.KeyCode == Keys.ShiftKey)
                {
                    mario.max_speed = 14;
                    mario.acceleration = true;
                }

                if(e.KeyCode == Keys.Up)
                {
                    if (!mario.TimerGravity)
                    {
                        mario.defines_the_image("Jump");
                        mario.TimerSpace = true;
                    }
                }

                if(e.KeyCode == Keys.Down)
                {
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (mario.Y + mario.height == blocks[i].Y & blocks[i].name == "Pipe" & blocks[i].property == "between_locations")
                        {
                            if (mario.X > blocks[i].X & mario.X + mario.width < blocks[i].X + blocks[i].width)
                            {
                                mario.stopForm1_KeyDown = true;
                                mario.nam.Add(i);
                                blocks[i].sluchay = [0, blocks[i].Y];
                                break;
                            }
                        }
                        else if (i == blocks.Count - 1 & (mario.mode == "big ordinary" || mario.mode == "big shooter") & mario.sits == false)
                        {

                            mario.sits = true;
                            mario.width = nav.base_width_ordinary;
                            mario.height = nav.base_height_ordinary("sits");
                            mario.Y += nav.base_height_ordinary(mario.mode);
                            mario.defines_the_image("Duck");
                        }
                    }
                }
            }
        }

        private void Knocking_out_enemy_creatures_with_a_block(int nam)
        {
            for (int i = 0; i < ñreatures.Count; i++)
            {
                if (Ñheck(new int[] { ñreatures[i].X, ñreatures[i].X + ñreatures[i].width, ñreatures[i].Y, ñreatures[i].Y + ñreatures[i].height }, new int[] { blocks[nam].X, blocks[nam].X + blocks[nam].width, blocks[nam].Y, blocks[nam].Y + blocks[nam].height }) == true)
                {
                    blocks = ñreatures[i].Dead_fall(blocks, i);
                }
            }
        }

        private bool Ñheck(int[] object_one, int[] object_two)
        {
            if (object_one[1] > object_two[0] & object_one[2] < object_two[3] & object_one[3] > object_two[2] & object_one[0] < object_two[1])
            {
                return true;
            }
            return false;
        }

        private int proverka_sovp_dvuh_perem_spiskov(int i)
        {
            foreach (int t in mario.nam)
            {
                if (t == i) return t;
            }
            return -1;
        }

        private void Switching_return_between_locations(int return_location)
        {
            for (int i = 0; i < nav.navigator[return_location].Count; i++)
            {
                object[] location_object = nav.navigator[return_location][i];
                if ((string)location_object[0] == "Block")
                {
                    if ((string)location_object[7] == "exit_from_the_location") 
                    {
                        if ((int)location_object[8] == nav.current_location)
                        {
                            for (int r = 0; r < i; r++)
                            {
                                if ((string)location_object[0] != "settings")
                                {
                                    nav.navigator[return_location].RemoveAt(r);
                                    i--;
                                }
                                else bg = (Image)nav.navigator[return_location][0][2];
                            }
                            ñreatures = new List<Creature>();
                            blocks = new List<Block>();
                            backgrounds = new List<Background>();
                            mario.nam = new List<int>();
                            brick_floor = [0, 328, 656, 984, 1312, 1640, 1968, 2296];
                            stop_movement_location = false;
                            spawn = (int)location_object[1] - 200;
                            mario.u = -1;
                            mario.X = (int)location_object[1] - spawn;
                            mario.Y = (int)location_object[2] - mario.height;
                            nav.Switching_between_locations(return_location);
                            mario.top = 845;
                            mario.TimerGravity = true;
                            mario.stopForm1_KeyDown = false;
                            Spawn_Load();
                            return;
                        }
                    }
                }
            }
        }

        private void Switching_between_locations()
        {
            int current_location = nav.current_location;
            int current_level = nav.current_level;
            bool navigator_breack = nav.navigator_breack;
            nav = new Navigator() { current_location = current_location, current_level = current_level, navigator_breack = navigator_breack, end_location_X = -1 };
            stop_movement_location = false;
            spawn = 0;
            ñreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            mario.nam = new List<int>();
            brick_floor = [0, 328, 656, 984, 1312, 1640, 1968, 2296];
            mario.u = -1;
            mario.X = 0;
            mario.Y = 845 - mario.height;
            mario.top = 845;
            mario.stopForm1_KeyDown = false;
        }

        private void Dead_mario_restart()
        {
            spawn = 0;
            ñreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            mario.nam = new List<int>();
            brick_floor = [0, 328, 656, 984, 1312, 1640, 1968, 2296];
            int level = nav.current_level;
            nav = new Navigator();
            if (lives != 0)
            {
                mario = new Mario(0, 762, nav.base_height_ordinary("ordinary"), nav.base_width_ordinary);
                lives -= 1;
                nav.current_location = level;
                nav.current_level = level;
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            mario = null;
            gameTimer.Stop();
            Menu();
        }

        public void Spawn_Load()
        {
            for (int i = 0; i < nav.navigator[nav.current_location].Count; i++)
            {
                object[] location_object = nav.navigator[nav.current_location][i];
                if ((int)location_object[1] <= spawn + screenWidth)
                {
                    switch ((string)location_object[0])
                    {
                        case "Block":
                            if ((string)location_object[7] != "between_locations" & (string)location_object[7] != "exit_from_the_location" & (string)location_object[7] != "return_between_locations") 
                                blocks.Add(new Block(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], width: (int)location_object[4], height: (int)location_object[5], image: (Image)location_object[6], property: (string)location_object[7]));
                            else blocks.Add(new Block(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], width: (int)location_object[4], height: (int)location_object[5], image: (Image)location_object[6], property: (string)location_object[7], location_transfer: (int)location_object[8]));
                            break;
                        case "Creature":
                            ñreatures.Add(new Creature(x: (int)location_object[1] - spawn, y: (int)location_object[2], image: (Image)location_object[3], direction: (bool)location_object[4], name: (string)location_object[5], width: (int)location_object[6], height: (int)location_object[7], condition: (string)location_object[8], property: (string)location_object[9], g: (int)location_object[10], spaceG: (int)location_object[11], top: (int)location_object[12]));
                            break;
                        case "Background":
                            if ((string)location_object[3] != "End") backgrounds.Add(new Background(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], image: (Image)location_object[4]));
                            else
                            {
                                backgrounds.Add(new Background(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], image: (Image)location_object[4], location_transfer: (int)location_object[5]));
                                nav.end_location_X = (int)location_object[6];
                            }
                            break;
                        case "settings":
                            bg = (Image)location_object[2];
                            break;
                        default: 
                            break;
                    }
                    nav.navigator_breack = true;
                    if ((string)location_object[0] != "settings") nav.navigator[nav.current_location].RemoveAt(i--);
                }
                else
                {
                    if (nav.navigator_breack == true) { nav.navigator_breack = false; break; }
                }
            }
            if (nav.end_location_X == spawn + screenWidth) stop_movement_location = true;
        }
    }
}