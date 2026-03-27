namespace Mario1
{
    using im_gl = Properties.Resources;// im_gl = images_global все картинки проект
    public class Navigator
    {
        public int current_location = 0;//Текущая локация
        public int end_location_X = -1;//пиксель / 10


        //Марио
        public short base_height_ordinary(string mode)
        {
            if (mode == "ordinary" || mode == "intangible ordinary") return 83;
            else if (mode == "sits") return 114;
            else return 166;
        }
        public short base_width_ordinary = 65;


        //Уровень - та жэ локация, только "Главная текущая", тоесть если марио умрёт в локации а главный уровень другой, то и возродится он в главной локации(уровне)
        public int current_level = 0;//Текущий уровень
        public int[] levels_id = {0, 2};//Массив возможных уровней
        public bool navigator_breack = false;

        public List<List<object[]>> navigator = 
        [
            [
                //настройки пола(картинка)
                ["settings", 0, im_gl.bg4],

                //наполнение локации
                ["Block", 1317, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 1632, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 1715, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 1798, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 1798, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 1882, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 1965, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 2297, 680, "Pipe", 166, 166, im_gl.Pipe1, "between_locations", 1],
                ["Block", 3125, 680, "Pipe", 166, 166, im_gl.Pipe1, ""],
                ["Creature", 3463, 762, im_gl.SMB_greenkoopatroopa1, false, "SMB_greenkoopatroopa", 83, 83, "", "", 1, 0, 845],
                ["Block", 3801, 680, "Pipe", 166, 166, im_gl.Pipe1, "exit_from_the_location", 2],
                //["Creature", 4244, 762, im_gl.SMB_greenparatrooper1, false, "SMB_greenparatrooper", 83, 83, "jamp", "", 1, 25, 845],
                ["Creature", 4454, 762, im_gl.Goomba, false, "Image_Goomba", 83, 83, "", "", 1, 0, 845],
                ["Block", 4711, 680, "Pipe", 166, 166, im_gl.Pipe1, ""],
                ["Block", 5294, 350, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 6356, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6439, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 6522, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6601, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6684, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6767, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6850, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 6933, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7016, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7099, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7182, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7522, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7605, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7688, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 7771, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, ""],
                ["Block", 7767, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 8266, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 8349, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 8767, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 9017, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 9017, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 9267, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 10000, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10083, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10166, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10249, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10583, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10666, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10666, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, ""],
                ["Block", 10749, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 10749, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, ""],
                ["Block", 10832, 100, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 11077, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11155, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11155, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11233, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11233, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11233, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11311, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11311, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11311, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11311, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11542, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11542, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11542, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11542, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11620, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11620, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11620, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11698, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11698, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 11776, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12250, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12328, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12328, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12406, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12406, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12406, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12484, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12484, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12484, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12484, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12715, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12715, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12715, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12715, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12793, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12793, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12793, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12871, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12871, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 12949, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 13471, 680, "Pipe", 166, 166, im_gl.Pipe1, "exit_from_the_location", 1],
                ["Block", 13878, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 14044, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 14127, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Creature", 14316, 762, im_gl.Goomba, false, "Image_Goomba", 83, 83, "", "", 1, 0, 845],
                ["Creature", 14504, 762, im_gl.Goomba, false, "Image_Goomba", 83, 83, "", "", 1, 0, 845],
                ["Block", 14786, 680, "Pipe", 166, 166, im_gl.Pipe1, ""],
                ["Block", 14952, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15030, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15030, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15108, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15108, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15108, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15186, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15186, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15186, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15186, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15264, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15264, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15264, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15264, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15264, 438, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 438, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15342, 355, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 770, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 687, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 604, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 521, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 438, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                ["Block", 15420, 355, "Iron", 83, 83, im_gl.EmptyBlock, ""],
                
                //финиш [,,,,, на какую локацию след., на каком месте стопать жвиж. блоков относительно марио]
                ["Background", 16520, 470, "End", im_gl.finish1, 2, 17020]
            ],
            [
                ["Block", 2100, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 2500, 680, "Pipe", 166, 166, im_gl.Pipe1, "return_between_locations", 0]
            ],
            [
                ["Block", 2100, 450, "Bricks", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 2429, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 2512, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "mushroom/flower bonus"],
                ["Block", 2596, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 2596, 100, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 2679, 450, "LuckyBlock", 83, 83, im_gl.Question_Block__1_, "money"],
                ["Block", 2762, 450, "Bricks", 83, 83, im_gl.Bricks, ""],
                ["Block", 3080, 680, "Pipe", 166, 166, im_gl.Pipe1, "return_between_locations", 0]
            ]
        ];

        public void Switching_between_locations(int next_location)
        {
            foreach (int level_id in levels_id)
            {
                if (next_location == level_id)
                {
                    current_level = next_location;
                }
            }
            current_location = next_location;
        }
    }
}
