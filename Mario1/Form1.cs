namespace Mario1
{
    public partial class Form1 : Form
    {
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        private Button btnStart;

        private int spawn;//ÓÚ‚Â˜‡ÂÚ ÍÓ„‰‡ ÒÔ‡‚ÌËÚ¸ ˜ÚÓ ÎË·Ó
        public int lives;
        private List<Creature> Òreatures;
        private List<Block> blocks;
        private List<Background> backgrounds;
        private Mario mario;
        private Navigator nav;
        internal void StopGameTimer() => gameTimer.Stop();
        internal void StartGameTimer() => gameTimer.Start();

        // √„Î‡‚Ì˚È Ë„Ó‚ÓÈ Ú‡ÈÏÂ
        private System.Windows.Forms.Timer gameTimer;

        public Form1()
        {
            InitializeComponent();
            // Õ‡ÒÚÓÈÍ‡ ÙÓÏ˚ ‰Îˇ ÔÓËÁ‚Ó‰ËÚÂÎ¸ÌÓÈ ÓÚËÒÓ‚ÍË (Double buffering)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            KeyPreview = true; // œÓÁ‚ÓÎˇÂÚ ÙÓÏÂ ÔÂÂı‚‡Ú˚‚‡Ú¸ ÒÓ·˚ÚËˇ ÍÎ‡‚Ë‡ÚÛ˚
            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 5;
            gameTimer.Tick += GameTimer_Tick;
            if (screenWidth == 0) screenWidth = 2000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Menu();
        }

        private void Menu()
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

        private void GameStart()
        {
            spawn = 0;
            lives = 3;
            Òreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            nav = new Navigator();
            mario = new Mario(0, 700, nav.base_height_ordinary("ordinary"), nav.base_width_ordinary);
            BackColor = Color.DodgerBlue;
            Spawn_Load();
            gameTimer.Start();
        }

        // √Î‡‚Ì˚È Ë„Ó‚ÓÈ ˆËÍÎ
        public void GameTimer_Tick(object sender, EventArgs e)
        {
            if (!AnimationManager.IsAnimating)
            {
                Left_Mario();
                Right_Mario();
                Sliding_Mario();
                Jamp_Mario();
                Padenie_Mario();
                Actions();
            }
            for (short i = 0; i < Òreatures.Count; i++)
            {
                if (Òreatures[i].Animation(metod: "", triger: 70) & Òreatures[i].condition.Find(x => x == "intangible") == "intangible")
                    { Òreatures.RemoveAt(i--); if (i == -1) break; }
                i = Jump_Òreatures(i);
                if (i == -1) break;
                i = Fall_Òreatures(i);
                if (i == -1) break;
                i = Creatures_come_out(i);
                if (i == -1) break;
                i = Movement_creatures_X(i);
                if (i == -1) break;
                if (Òreatures[i].X < -200)
                {
                    blocks = Òreatures[i].Check_block_we_stand(blocks, i, "dead");
                    Òreatures.RemoveAt(i--);
                    if (i == -1) break;
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
                if (backgrounds[i].X > 0 - backgrounds[i].width & backgrounds[i].X < screenWidth)
                e.Graphics.DrawImage(backgrounds[i].image, backgrounds[i].X, backgrounds[i].Y);
            }
            for (int i = Òreatures.Count - 1; i >= 0; i--)
            {
                if (Òreatures[i].X > 0 - Òreatures[i].width & Òreatures[i].X < screenWidth)
                {
                    if (Òreatures[i].height <= Òreatures[i].proper_height) e.Graphics.DrawImage(Òreatures[i].image, Òreatures[i].X, Òreatures[i].Y, Òreatures[i].DestRect(), GraphicsUnit.Pixel);
                    else e.Graphics.DrawImage(Òreatures[i].image, Òreatures[i].X, Òreatures[i].Y);
                }
            }
            if (mario.image is not null)
            {
                if (!mario.deadPadeniye)
                {
                    if (mario.Y > 0 - mario.height & mario.Y < screenHeight)
                    {
                        e.Graphics.DrawImage(mario.image, mario.X - (((83 - nav.base_width_ordinary) / 2)), mario.Y);
                    }
                }
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                if ((blocks[i].X > 0 - blocks[i].width & blocks[i].X < screenWidth))
                {
                    e.Graphics.DrawImage(blocks[i].image, blocks[i].X, blocks[i].Y);
                }
            }
            if (mario.image is not null)
            {
                if (mario.deadPadeniye)
                {
                    if (mario.Y > 0 - mario.height & mario.Y < screenHeight)
                    {
                        e.Graphics.DrawImage(mario.image, mario.X - (((83 - nav.base_width_ordinary) / 2)), mario.Y);
                    }
                }
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
                        mario.X--;
                        for (short i = 0; i < Òreatures.Count; i++)
                        {
                            if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & Òreatures[i].property != "Attack against creatures" & Òreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & Òreatures[i].condition.Find(x => x == "intangible") != "intangible")
                            {
                                if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                                {
                                    i = Mario_in_Òreatures(i);
                                }
                            }
                        }
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }))
                            {
                                mario.TimerSliding = false;
                                mario.TimerLeft = false;
                                mario.X = blocks[i].X + blocks[i].width;
                                mario.run_animation = 0;
                                if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                                mario.speed = 1;
                                return;
                            }
                        }
                        if (Paday_Mario(false)) break;
                    }
                    if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                    {
                        if (!mario.acceleration) mario.speed++;
                        else mario.speed += 2;
                    }
                    else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                    if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                    {
                        mario.Defines_the_image("Walk");
                    }
                    mario.run_animation += 1;
                }
                else
                {
                    mario.run_animation = 0;
                    mario.TimerSliding = false;
                    mario.TimerLeft = false;
                    if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                    mario.speed = 1;
                }
            }
        }

        private void Right_Mario()
        {

            if (mario.TimerRight)
            {
                if (mario.X < screenWidth / 2 - screenWidth / 10 || spawn + screenWidth > nav.end_location_X)
                {
                    if (mario.X + mario.width < screenWidth)
                    {
                        for (short y = 0; y < mario.speed; y++)
                        {
                            mario.X++;
                            for (short i = 0; i < Òreatures.Count; i++)
                            {
                                if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & Òreatures[i].property != "Attack against creatures" & Òreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & Òreatures[i].condition.Find(x => x == "intangible") != "intangible")
                                {
                                    if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                                    {
                                        i = Mario_in_Òreatures(i);
                                    }
                                }
                            }
                            for (short i = 0; i < blocks.Count; i++)
                            {
                                if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                                {
                                    if (blocks[i].property == "return_between_locations")
                                    {
                                        Switching_return_between_locations(return_location: blocks[i].location_transfer);
                                        return;
                                    }
                                    else if (blocks[i].property == "between_locations" & blocks[i].name == "Column")
                                    {
                                        mario.block_we_stand = i - 1; //ÚÓ ÂÒÚ¸ ·ÎÓÍ Ì‡ ÍÓÚÓÓÏ ÒÚÓËÚ Column
                                        mario.TimerLeft = false;
                                        mario.TimerRight = false;
                                        mario.TimerGravity = false;
                                        mario.TimerSliding = false;
                                        mario.TimerSpace = false;
                                        mario.g = 1;
                                        mario.spaceG_max = 15;
                                        mario.spaceG = 20;
                                        mario.stopForm1_KeyDown = true;
                                        mario.run_animation = 0;
                                        mario.speed = 1;
                                        AnimationManager.PlayAnimation(
                                            durationMs: 2000,      // 0.8 ÒÂÍÛÌ‰˚
                                            intervalMs: 5,
                                            onFrame: frame =>
                                            {
                                                // ÀÓ„ËÍ‡ ‡ÌËÏ‡ˆËË (Í‡Ê‰˚È ÚËÍ Ú‡ÈÏÂ‡)
                                                Anim_finish();
                                            },
                                            onComplete: () =>
                                            {
                                                Switching_between_locations(i);
                                            }
                                        );
                                        return;
                                    }
                                    else if (blocks[i].property == "between_locations" & blocks[i].name == "Pipe90")
                                    {

                                    }
                                    mario.TimerSliding = false;
                                    mario.TimerRight = false;
                                    mario.X = blocks[i].X - mario.width;
                                    mario.run_animation = 0;
                                    if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                                    mario.speed = 1;
                                    return;
                                }
                            }
                            if (Paday_Mario(true)) break;
                        }
                        if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                        {
                            if (!mario.acceleration) mario.speed++;
                            else mario.speed += 2;
                        }
                        else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                        if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                        {
                            mario.Defines_the_image("Walk");
                        }
                    }
                    else
                    {
                        mario.run_animation = 0;
                        mario.TimerSliding = false;
                        mario.TimerRight = false;
                        if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                        mario.speed = 1;
                    }
                }
                else
                {
                    for (short y = 0; y < mario.speed; y++)
                    {
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (—heck(new int[] { mario.X, mario.X + mario.width + 1, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }) == true)
                            {
                                if (blocks[i].property == "return_between_locations")
                                {
                                    Switching_return_between_locations(return_location: blocks[i].location_transfer);
                                    return;
                                }
                                else if (blocks[i].property == "between_locations" & blocks[i].name != "Pipe")
                                {
                                    Switching_between_locations(i);
                                    return;
                                }
                                mario.TimerSliding = false;
                                mario.TimerRight = false;
                                mario.X = blocks[i].X - mario.width;
                                mario.run_animation = 0;
                                if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                                mario.speed = 1;
                                return;
                            }
                        }
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            blocks[i].X--;
                            if (blocks[i].X < -200)
                            {
                                blocks.RemoveAt(i);
                                for (int r = 0; r < mario.nam.Count; r++)
                                {
                                    if (mario.nam[r] == i) mario.nam.RemoveAt(r--);
                                    else if (mario.nam[r] > i) mario.nam[r]--;
                                }
                                if (mario.block_we_stand > i) mario.block_we_stand--;
                                i--;
                            }
                        }
                        for (short i = 0; i < Òreatures.Count; i++)
                        {
                            if (mario.mode != "intangible ordinary" & Òreatures[i].property != "Attack against creatures" & !mario.deadPadeniye & Òreatures[i].condition.Find(x => x == "dead_fall") != "dead_fall" & Òreatures[i].condition.Find(x => x == "intangible") != "intangible")
                            {
                                if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                                {
                                    i = Mario_in_Òreatures(i);
                                }
                            }
                        }
                        for (short i = 0; i < Òreatures.Count; i++) Òreatures[i].X--;
                        for (short i = 0; i < backgrounds.Count; i++) backgrounds[i].X--;
                        if (Paday_Mario(true)) break;
                        spawn++;
                        Spawn_Load();
                    }
                    if (mario.run_animation % 5 == 0 & mario.speed < mario.max_speed & !mario.TimerSliding)
                    {
                        if (!mario.acceleration) mario.speed++;
                        else mario.speed += 2;
                    }
                    else if (mario.run_animation % 5 == 0 & mario.speed > mario.max_speed & !mario.TimerSliding) mario.speed--;
                    if (mario.Y + mario.height == mario.top & !mario.braking2 & !mario.sits)
                    {
                        mario.Defines_the_image("Walk");
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
                        if (!mario.sits) mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
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
                                if (!mario.sits) mario.Defines_the_image("Skid");
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
                        mario.block_we_stand = -1;
                        mario.top = 3000;
                        mario.Y -= mario.spaceG;
                        if (mario.spaceG_max != 0) mario.spaceG_max--;
                        for (int i = 0; i < blocks.Count; i++)
                        {
                            if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }))
                            {
                                int sravnenieTverdoeCosanieL = blocks[i].X + blocks[i].width - mario.X;
                                for (int m = i + 1; m < blocks.Count; m++)
                                {
                                    if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[m].X, blocks[m].X + blocks[m].width, blocks[m].Y, blocks[m].Y + blocks[m].height }))
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
                                mario.spaceG = 20;
                                mario.spaceG_max = 15;
                                mario.TimerSpace = false;
                                mario.TimerGravity = true;
                                break;
                            }
                        }
                    }
                    if (!mario.spaceG_bool || mario.spaceG_max == 0) mario.spaceG--;
                }
                else
                {
                    mario.spaceG_max = 15;
                    mario.spaceG = 20;
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
                    mario.g++;
                    if (!mario.deadPadeniye)
                    {
                        for (short i = 0; i < blocks.Count; i++)
                        {
                            if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { blocks[i].X, blocks[i].X + blocks[i].width, blocks[i].Y, blocks[i].Y + blocks[i].height }))
                            {
                                mario.Y = (blocks[i].Y - (mario.Y + mario.height - mario.Y));
                                mario.g = 1;
                                mario.top = blocks[i].Y;
                                mario.block_we_stand = i;
                                mario.TimerGravity = false;
                                if (mario.sits) mario.Defines_the_image("Duck");
                                else mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                            }
                        }
                        if (mario.mode != "intangible ordinary")
                        {
                            for (short i = 0; i < Òreatures.Count; i++)
                            {
                                if (Òreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall" || Òreatures[i].condition.Find(x => x == "intangible") == "intangible") continue;
                                if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }))
                                {
                                    i = Creatures_in_mario(i);
                                }
                            }
                        }
                    }
                    if (mario.Y + mario.height > mario.top)
                    {
                        mario.block_we_stand = -1;
                        mario.Y = mario.top - (mario.Y + mario.height - mario.Y);
                    }
                }
                else
                {
                    if (mario.sits) mario.Defines_the_image("Duck");
                    else mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                    mario.g = 1;
                    mario.TimerGravity = false;
                }
            }
        }

        private bool Paday_Mario(bool direction)
        {
            if (mario.block_we_stand != -1 & !mario.TimerSpace)
            {
                if (mario.X + mario.width < blocks[mario.block_we_stand].X || mario.X > (blocks[mario.block_we_stand].X + blocks[mario.block_we_stand].width))
                {
                    for (int i = 0; i < blocks.Count; i++)
                    {
                        if (mario.block_we_stand == i) continue;
                        if(blocks[i].Y == blocks[mario.block_we_stand].Y & ((!direction & blocks[i].X + blocks[i].width > blocks[mario.block_we_stand].X - 15 & blocks[i].X < blocks[mario.block_we_stand].X) || (direction & blocks[i].X < blocks[mario.block_we_stand].X + blocks[mario.block_we_stand].width + 15 & blocks[i].X > blocks[mario.block_we_stand].X))) 
                        { mario.block_we_stand = i; return false; }
                    }
                    mario.block_we_stand = -1;
                    mario.top = 3000;
                    mario.TimerGravity = true;
                    return true;
                }
            }
            return false;
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
                            if (mario.block_we_stand > r) { mario.block_we_stand -= 1; }
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
                            Switching_between_locations(r);
                            return;
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
                                    if (mario.mode == "ordinary") Òreatures.Add(new Creature(x: blocks[r].X, y: blocks[r].sluchay[1], name: "mushroom bonus", height: 0, width: 83, direction: true, property: "bonus", top: 3000, g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Super_Mushroom, proper_height: 83));
                                    else if (mario.mode == "big ordinary" || mario.mode == "big shooter") Òreatures.Add(new Creature ( x: blocks[r].X, y: blocks[r].sluchay[1], name: "flower bonus", height: 0, width: 83, direction: true, property: "bonus", top: 3000, g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Fire_Flower__1_, proper_height: 83));
                                }
                                //ËÒÍÎ˛˜ÂÌËÂ ‚ top!!! (Ò˛‰‡ ÒÓı‡ÌˇÂÚÒˇ ÁÌ‡˜ÂÌËÂ sluchay[i][1], ˜ÚÓ·˚ Ò‡‚ÌËÚ¸ Ò Â‡Î¸Ì˚Ï spawnY_creatures ‰Îˇ Û‰‡ÎÂÌËˇ ÔË ‰ÓÒÚËÊÂÌËË ÓÔÂ‰ÂÎ∏ÌÌÓÈ ‚˚ÒÓÚ˚)
                                else if (blocks[mario.nam[i]].property == "money") Òreatures.Add(new Creature(x: blocks[r].X + (blocks[r].width / 2) - 16, y: blocks[r].sluchay[1], name: "money bonus", height: 0, width: 32, direction: true, property: "bonus", top: blocks[r].sluchay[1], g: 1, spaceG: 0, condition: "stands", image: Properties.Resources.Coin, proper_height: 56));
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
            if (Òreatures[i].property == "bonus" & Òreatures[i].condition.Find(x => x == "stands") == "stands")
            {
                if (Òreatures[i].name == "mushroom bonus" || Òreatures[i].name == "flower bonus")
                {
                    if (Òreatures[i].height >= Òreatures[i].proper_height)
                    {
                        if (Òreatures[i].name == "mushroom bonus") Òreatures[i].condition.Remove("stands");
                    }
                    else
                    {
                        Òreatures[i].Y -= 1;
                        Òreatures[i].height += 1;
                    }
                }
                else if (Òreatures[i].name == "money bonus")
                {
                    if (Òreatures[i].height < Òreatures[i].proper_height)
                    {
                        mario.coin += 1;
                        if (mario.coin == 100)
                        {
                            mario.coin = 0;
                            lives++;
                        }
                        Òreatures[i].Y -= 6;
                        Òreatures[i].height += 6;
                    }
                    else
                    {
                        Òreatures[i].Y -= 6;
                        if (Òreatures[i].Y < Òreatures[i].top - 150)// top_creatures[i] ËÁ ËÒÍÎ˛˜ÂÌËˇ!!! (‚‚Ó‰ËÚÒˇ ÔË ÒÓÁ‰‡ÌËË money bonus)
                        {
                            blocks = Òreatures[i].Check_block_we_stand(blocks, i, "dead");
                            Òreatures.RemoveAt(i--);
                        }
                    }
                }
            }
            return i;
        }

        private short Movement_creatures_X(short i)
        {
            if (Òreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall") return i;
            if (Òreatures[i].condition.Find(x => x == "stands") != "stands")
            {
                if (Òreatures[i].direction)
                {
                    Òreatures[i].X += Òreatures[i].speed;
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (—heck(new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            Òreatures[i].X = blocks[r].X - Òreatures[i].width;
                            Òreatures[i].direction = false;
                        }
                        Check_block_we_stand(i, r);
                    }
                }
                else
                {
                    Òreatures[i].X -= Òreatures[i].speed;
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (—heck(new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            Òreatures[i].X = blocks[r].X + blocks[r].width;
                            Òreatures[i].direction = true;
                        }
                        Check_block_we_stand(i, r);
                    }
                }
                if (Òreatures[i].condition.Find(x => x == "attack_on_everyone") != "attack_on_everyone") Òreatures[i].Animation("Walk");
            }
            for (short r = 0; r < Òreatures.Count; r++)
            {
                if (r == i || Òreatures[r].condition.Find(x => x == "stands") == "stands" || Òreatures[r].condition.Find(x => x == "intangible") == "intangible") continue;
                if (—heck(new int[] { Òreatures[r].X, Òreatures[r].X + Òreatures[r].width, Òreatures[r].Y, Òreatures[r].Y + Òreatures[r].height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                {
                    if ((Òreatures[r].property == "Attack against creatures" & Òreatures[i].property == "") || (Òreatures[i].property == "Attack against creatures" & Òreatures[r].property == ""))
                    {
                        blocks = Òreatures[i].Check_block_we_stand(blocks, i, "dead");
                        Òreatures.RemoveAt(i);
                        if (i < r) { r -= 1; i -= 1; }
                        else i -= 2;
                        blocks = Òreatures[r].Check_block_we_stand(blocks, r, "dead");
                        Òreatures.RemoveAt(r);
                        if (i == -1) return i;
                        if (r == -1) break;
                        //break;
                    }
                    else if (Òreatures[i].condition.Find(x => x == "attack_on_everyone") == "attack_on_everyone" & Òreatures[r].condition.Find(x => x == "attack_on_everyone") != "attack_on_everyone")
                    {
                        blocks = Òreatures[r].Dead_fall(blocks, r);
                    }
                    else if (Òreatures[r].condition.Find(x => x == "attack_on_everyone") == "attack_on_everyone" & Òreatures[i].condition.Find(x => x == "attack_on_everyone") != "attack_on_everyone")
                    {
                        blocks = Òreatures[i].Dead_fall(blocks, i);
                    }
                    else if ((Òreatures[r].property != "Attack against creatures" & Òreatures[i].property != "Attack against creatures") || (Òreatures[i].condition.Find(x => x == "attack_on_everyone") == "attack_on_everyone" & Òreatures[r].condition.Find(x => x == "attack_on_everyone") == "attack_on_everyone"))
                    {
                        if (Òreatures[i].direction != true) Òreatures[i].direction = true;
                        else Òreatures[i].direction = false;
                        if (Òreatures[r].direction != false) Òreatures[r].direction = false;
                        else Òreatures[r].direction = true;
                    }
                }
            }

            if (Òreatures[i].condition.Find(x => x == "waiting_for_Mario_to_exit_to_kill") == "waiting_for_Mario_to_exit_to_kill") 
            {
                if (!—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                {
                    Òreatures[i].condition.Remove("waiting_for_Mario_to_exit_to_kill");
                    Òreatures[i].condition.Remove("doesn_t_kill");
                }
            }
            else if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & Òreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill" & Òreatures[i].condition.Find(x => x == "intangible") != "intangible") 
            {
                if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                    return Mario_in_Òreatures(i);
            }
            return i;
        }

        private short Jump_Òreatures(short i)
        {
            if (Òreatures[i].spaceG > 0)
            {
                Òreatures[i].Y -= Òreatures[i].spaceG;
                Òreatures[i].spaceG--;
                if (Òreatures[i].condition.Find(x => x == "stands") != "stands")
                {
                    for (short r = 0; r < blocks.Count; r++)
                    {
                        if (—heck(new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                        {
                            Òreatures[i].Y = blocks[r].Y + blocks[i].height;
                            Òreatures[i].spaceG = 0;
                        }
                    }
                }
                if (mario.mode != "intangible ordinary" || Òreatures[i].condition.Find(x => x == "intangible") != "intangible")
                {
                    if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                    {
                        return Creatures_in_mario(i);
                    }
                }
            }
            else
            {
                if (Òreatures[i].condition.Find(x => x == "jamp") == "jamp")
                {
                    Òreatures[i].spaceG = 25;
                }
            }
            return i;
        }

        private short Fall_Òreatures(short i)
        {
            if (Òreatures[i].property == "bonus" & Òreatures[i].condition.Find(x => x == "stands") == "stands") return i;
            if (Òreatures[i].Y + Òreatures[i].height < Òreatures[i].top)
            {
                for (int r = 0; r < Òreatures[i].g; r++)
                {
                    Òreatures[i].Y += 1;
                    if (Òreatures[i].Y + Òreatures[i].height >= Òreatures[i].top) break;
                }
                Òreatures[i].g += 1;
                if (Òreatures[i].condition.Find(x => x == "dead_fall") == "dead_fall") return i;
                for (short r = 0; r < blocks.Count; r++)
                {
                    if (—heck(new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }, new int[] { blocks[r].X, blocks[r].X + blocks[r].width, blocks[r].Y, blocks[r].Y + blocks[r].height }) == true)
                    {
                        Òreatures[i].Y = (blocks[r].Y - Òreatures[i].height);
                        blocks[r].block_we_stand = i;
                        Òreatures[i].top = blocks[r].Y;
                 
                    }
                }
                if (mario.mode != "intangible ordinary" & !mario.deadPadeniye & Òreatures[i].condition.Find(x => x == "doesn_t_kill") != "doesn_t_kill" & Òreatures[i].condition.Find(x => x == "intangible") != "intangible")
                {
                    if (—heck(new int[] { mario.X, mario.X + mario.width, mario.Y, mario.Y + mario.height }, new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }) == true)
                    {
                        return Mario_in_Òreatures(i);
                    }
                }
            }
            else
            {
                Òreatures[i].g = 1;
            }
            return i;
        }

        private short Creatures_in_mario(short i)
        {
            if (Òreatures[i].property == "bonus")
            {
                if (mario.mode == "ordinary")
                {
                    mario.Y -= nav.base_height_ordinary(mario.mode); ;
                    mario.mode = "big ordinary";
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);

                    mario.Defines_the_image("Super Mario");
                }
                else if (mario.mode == "big ordinary")
                {
                    mario.mode = "big shooter";
                    mario.Defines_the_image("Fiery Mario");
                }
                blocks = Òreatures[i].Check_block_we_stand(blocks, i, "dead");
                Òreatures.RemoveAt(i--);
                return i;
            }
            else if (Òreatures[i].property == "")
            {
                mario.Y -= mario.g;
                mario.g = 1;
                mario.spaceG = 15;
                mario.TimerGravity = false;
                mario.TimerSpace = true;
                
                if (Òreatures[i].name == "SMB_greenparatrooper" || Òreatures[i].name == "SMB_greenkoopatroopa")
                {
                    if (Òreatures[i].condition.Find(x => x == "doesn_t_kill") == "doesn_t_kill")
                    {
                        Òreatures[i].condition.Remove("stands");
                        Òreatures[i].condition.Add("waiting_for_Mario_to_exit_to_kill");
                        Òreatures[i].condition.Add("attack_on_everyone");
                        Òreatures[i].speed = 15;
                        if (mario.X + (mario.width/2) <= Òreatures[i].X + (Òreatures[i].width / 2)) Òreatures[i].direction = true;
                        else Òreatures[i].direction = false;
                    }
                    else
                    {
                        Òreatures[i].g = 15;
                        Òreatures[i].spaceG = 0;
                        Òreatures[i].height = 38;
                        Òreatures[i].width = 45;
                        Òreatures[i].condition.Remove("jamp");
                        Òreatures[i].condition.Add("stands");
                        Òreatures[i].condition.Add("doesn_t_kill");
                        Òreatures[i].Animation("Dead");
                    }
                        
                }
                else if (Òreatures[i].name == "Image_Goomba")
                {
                    Òreatures[i].g = 15;
                    Òreatures[i].height = 35;
                    Òreatures[i].run_animation = 0;
                    Òreatures[i].condition.Add("intangible");
                    Òreatures[i].condition.Add("stands");
                    Òreatures[i].condition.Add("doesn_t_kill");
                    Òreatures[i].Animation("Dead");
                }
            }
            return i;
        }

        private short Mario_in_Òreatures(short i)
        {
            if (Òreatures[i].condition.Find(x => x == "doesn_t_kill") == "doesn_t_kill" & (Òreatures[i].name == "SMB_greenparatrooper" || Òreatures[i].name == "SMB_greenkoopatroopa"))
            {
                Òreatures[i].condition.Remove("stands");
                Òreatures[i].condition.Add("waiting_for_Mario_to_exit_to_kill");
                Òreatures[i].condition.Add("attack_on_everyone");
                Òreatures[i].speed = 15;
                if (mario.X + (mario.width / 2) <= Òreatures[i].X + (Òreatures[i].width / 2)) Òreatures[i].direction = true;
                else Òreatures[i].direction = false;
            }
            else if (Òreatures[i].property == "bonus")
            {
                if (mario.mode == "ordinary")
                {
                    mario.Y -= nav.base_height_ordinary(mario.mode);
                    mario.mode = "big ordinary";
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.Defines_the_image("Super Mario");
                }
                else if (mario.mode == "big ordinary")
                {
                    mario.mode = "big shooter";
                    mario.Defines_the_image("Fiery Mario");
                }
                blocks = Òreatures[i].Check_block_we_stand(blocks, i, "dead");
                Òreatures.RemoveAt(i--);
            }
            else if (Òreatures[i].property == "")
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
                    mario.Defines_the_image("Mario");
                }
            }
            return i;
        }

        private void Check_block_we_stand(int creatures_i, int blocks_i)
        {
            if (blocks[blocks_i].block_we_stand == -1 || Òreatures[creatures_i].top == 3000) return;
            if ((Òreatures[creatures_i].X + Òreatures[creatures_i].width) < blocks[blocks_i].X || Òreatures[creatures_i].X > (blocks[blocks_i].X + blocks[blocks_i].width))
            {
                Òreatures[creatures_i].top = 3000;
                blocks[blocks_i].block_we_stand = -1;
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
                    mario.Defines_the_image("Super Mario");
                    mario.width = nav.base_width_ordinary;
                    mario.height = nav.base_height_ordinary(mario.mode);
                    mario.Y -= nav.base_height_ordinary(mario.mode) - nav.base_height_ordinary("sits");
                }
                else if (mario.mode == "big shooter")
                {
                    mario.Defines_the_image("Fiery Mario");
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
            if (e.KeyCode == Keys.Up)
            {
                mario.spaceG_bool = false;
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
                        if (mario.direction == true) Òreatures.Add(new Creature(x: mario.X + 32, y: mario.Y + 32, direction: true, name: "Fire bar", width: 16, height: 16, condition: "jamp", property: "Attack against creatures", g: 1, spaceG: 25, top: 3000, image: Properties.Resources.Fire_bar));
                        else Òreatures.Add(new Creature(x: mario.X + 32, y: mario.Y + 32, direction: false, name: "Fire bar", width: 16, height: 16, condition: "jamp", property: "Attack against creatures", g: 1, spaceG: 25, top: 3000, image: Properties.Resources.Fire_bar));
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
                    if (!mario.TimerGravity & mario.block_we_stand != -1 & !mario.TimerSpace)
                    {
                        if (mario.sits) { mario.sits = false; mario.height = nav.base_height_ordinary(""); mario.Y -= nav.base_height_ordinary("") - nav.base_height_ordinary("sits"); }
                        mario.spaceG_bool = true;
                        mario.Defines_the_image("Jump");
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
                            mario.TimerSliding = true;
                            mario.sits = true;
                            mario.width = nav.base_width_ordinary;
                            mario.height = nav.base_height_ordinary("sits");
                            mario.Y += nav.base_height_ordinary(mario.mode) - nav.base_height_ordinary("sits");
                            mario.Defines_the_image("Duck");
                        }
                    }
                }
            }
        }

        private void Knocking_out_enemy_creatures_with_a_block(int nam)
        {
            for (int i = 0; i < Òreatures.Count; i++)
            {
                if (Òreatures[i].property == "bonus") continue;
                if (—heck(new int[] { Òreatures[i].X, Òreatures[i].X + Òreatures[i].width, Òreatures[i].Y, Òreatures[i].Y + Òreatures[i].height }, new int[] { blocks[nam].X, blocks[nam].X + blocks[nam].width, blocks[nam].Y, blocks[nam].Y + blocks[nam].height }) == true)
                {
                    blocks = Òreatures[i].Dead_fall(blocks, i);
                }
            }
        }

        private bool —heck(int[] object_one, int[] object_two)
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

        public void Anim_finish()
        {
            //TimerSpace ‚ ‰‡ÌÌÓÏ ÏÂÚÓ‰Â ËÒÒÔÓÎ¸ÁÛÂÚÒˇ ÌÂ ÔÓ Ì‡ÁÌ‡˜ÂÌË˛ !!! (˜ÚÓ·˚ ÛÔÓˇ‰Ó˜ËÚ¸ ‰ÂÈÒÚ‚Ëˇ Ë ÌÂ ÒÓÁ‰‡‚‡Ú¸ ‰ÓÔ. ÔÂÂÏÂÌÌÓÈ)
            if (mario.Y + mario.height < 762) { mario.Y += 3; mario.TimerSpace = true; }
            else if (mario.TimerSpace) { mario.X += 93; mario.TimerSpace = false; }
            else
            {
                if (!mario.TimerRight) mario.TimerRight = true; 
                Right_Mario();
                Padenie_Mario();
                for (int i = 0; i < backgrounds.Count; i++)
                {
                    if (backgrounds[i].name == "Finish" & mario.X >= backgrounds[i].X + 100) AnimationManager._currentFrame = AnimationManager._maxFrames;
                }
            }
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
                            for (int r = 0; r < i - 5; r++)
                            {
                                if ((string)nav.navigator[return_location][r][0] != "Settings")
                                {
                                    nav.navigator[return_location].RemoveAt(r--);
                                    i--;
                                }
                            }
                            Òreatures = new List<Creature>();
                            blocks = new List<Block>();
                            backgrounds = new List<Background>();
                            mario.nam = new List<int>();
                            spawn = (int)location_object[1] - 200;
                            mario.block_we_stand = -1;
                            mario.X = (int)location_object[1] - spawn + (int)location_object[5] / 2 - mario.width / 2;
                            mario.Y = (int)location_object[2];
                            mario.direction = true;
                            nav.Switching_between_locations(return_location);
                            mario.top = 3000;
                            mario.TimerRight = false;
                            mario.TimerGravity = false;
                            mario.TimerSliding = false;
                            mario.TimerSpace = false;
                            mario.g = 1;
                            mario.spaceG_max = 15;
                            mario.spaceG = 20;
                            mario.stopForm1_KeyDown = true;
                            mario.run_animation = 0;
                            mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
                            mario.speed = 1;
                            Spawn_Load();
                            AnimationManager.PlayAnimation(
                                durationMs: 800,      // 0.8 ÒÂÍÛÌ‰˚
                                intervalMs: 5,
                                onFrame: frame =>
                                {
                                    // ÀÓ„ËÍ‡ ‡ÌËÏ‡ˆËË (Í‡Ê‰˚È ÚËÍ Ú‡ÈÏÂ‡)
                                    if (frame < mario.height / 2 + mario.height % 2)
                                    {
                                        mario.Y -= 2; // œÓ‰·‡Ò˚‚‡ÂÏ ‚‚Âı
                                    }
                                    else 
                                    {
                                        mario.stopForm1_KeyDown = false; // –‡Á·ÎÓÍËÛÂÏ ‚‚Ó‰
                                        mario.TimerGravity = true;
                                        AnimationManager.IsAnimating = false;
                                    }
                                },
                                onComplete: () =>
                                {
                                    
                                }
                            );
                        }
                    }
                }
            }
        }

        private void Switching_between_locations(int t)
        {
            nav.Switching_between_locations(blocks[t].location_transfer);
            int current_location = nav.current_location;
            int current_level = nav.current_level;
            bool navigator_breack = nav.navigator_breack;
            nav = new Navigator() { current_location = current_location, current_level = current_level, navigator_breack = navigator_breack };
            spawn = 0;
            Òreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            mario.nam = new List<int>();
            mario.block_we_stand = -1;
            mario.X = 83;
            mario.Y = 845 - 166;
            mario.direction = true;
            mario.TimerLeft = false;
            mario.TimerRight = false;
            mario.TimerGravity = false;
            mario.TimerSliding = false;
            mario.TimerSpace = false;
            mario.g = 1;
            mario.spaceG_max = 15;
            mario.spaceG = 20;
            mario.stopForm1_KeyDown = true;
            mario.run_animation = 0;
            mario.speed = 1;
            mario.Defines_the_image("Mario/Super Mario/Fiery Mario");
            mario.top = 3000;
            Spawn_Load();
            AnimationManager.PlayAnimation(
                                durationMs: 800,      // 0.8 ÒÂÍÛÌ‰˚
                                intervalMs: 5,
                                onFrame: frame =>
                                {
                                    // ÀÓ„ËÍ‡ ‡ÌËÏ‡ˆËË (Í‡Ê‰˚È ÚËÍ Ú‡ÈÏÂ‡)
                                    if (frame < mario.height / 2 + mario.height % 2)
                                    {
                                        mario.Y -= 2; // œÓ‰·‡Ò˚‚‡ÂÏ ‚‚Âı
                                    }
                                    else
                                    {
                                        mario.stopForm1_KeyDown = false; // –‡Á·ÎÓÍËÛÂÏ ‚‚Ó‰
                                        mario.TimerGravity = true;
                                        AnimationManager.IsAnimating = false;
                                    }
                                },
                                onComplete: () =>
                                {
                                    
                                }
                            );
        }

        private void Dead_mario_restart()
        {
            spawn = 0;
            Òreatures = new List<Creature>();
            blocks = new List<Block>();
            backgrounds = new List<Background>();
            mario.nam = new List<int>();
            int level = nav.current_level;
            nav = new Navigator();
            if (lives != 0)
            {
                mario = new Mario(0, 700, nav.base_height_ordinary("ordinary"), nav.base_width_ordinary);
                lives -= 1;
                nav.current_location = level;
                nav.current_level = level;
                Spawn_Load();
            }
            else GameOver();
        }

        private void GameOver()
        {
            mario = null;
            gameTimer.Stop();
            Menu();
        }

        private void Spawn_Load()
        {
            bool exit = false;
            for (int i = 0; i < nav.navigator[nav.current_location].Count; i++)
            {
                object[] location_object = nav.navigator[nav.current_location][i];
                switch ((string)location_object[0])
                {
                    case "Block":
                        if((int)location_object[1] <= spawn + screenWidth + 700)
                        {
                            if ((string)location_object[7] != "between_locations" & (string)location_object[7] != "exit_from_the_location" & (string)location_object[7] != "return_between_locations")
                                blocks.Add(new Block(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], width: (int)location_object[4], height: (int)location_object[5], image: (Image)location_object[6], property: (string)location_object[7]));
                            else blocks.Add(new Block(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], width: (int)location_object[4], height: (int)location_object[5], image: (Image)location_object[6], property: (string)location_object[7], location_transfer: (int)location_object[8]));
                        }
                        else if (nav.navigator_breack == true) { nav.navigator_breack = false; exit = true; }
                        break;
                    case "Creature":
                        if ((int)location_object[1] <= spawn + screenWidth)
                        {
                            Òreatures.Add(new Creature(x: (int)location_object[1] - spawn, y: (int)location_object[2], image: (Image)location_object[3], direction: (bool)location_object[4], name: (string)location_object[5], width: (int)location_object[6], height: (int)location_object[7], condition: (string)location_object[8], property: (string)location_object[9], g: (int)location_object[10], spaceG: (int)location_object[11], top: (int)location_object[12]));
                        }
                        else if (nav.navigator_breack == true) { nav.navigator_breack = false; exit = true; }
                        break;
                    case "Background":
                        if ((int)location_object[1] <= spawn + screenWidth)
                            backgrounds.Add(new Background(x: (int)location_object[1] - spawn, y: (int)location_object[2], name: (string)location_object[3], image: (Image)location_object[4]));
                        else if (nav.navigator_breack == true) { nav.navigator_breack = false; exit = true; }
                        break;
                    case "Settings":
                        if (nav.floor_parameters_are_met)
                        {
                            BackColor = (Color)location_object[1];
                            nav.end_location_X = (int)location_object[2];
                            nav.floor_parameters_are_met = false;
                        }
                        else if (nav.navigator_breack == true) { nav.navigator_breack = false; exit = true; }
                        break;
                    default: 
                        break;
                }
                if (exit) break;
                nav.navigator_breack = true;
                    if ((string)location_object[0] != "Settings")
                        nav.navigator[nav.current_location].RemoveAt(i--);
            }
        }
    }
}