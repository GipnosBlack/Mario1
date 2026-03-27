namespace Mario1
{
    using im_gl = Properties.Resources;// im_gl = images_global все картинки проект

    public class Creature : GameObject
    {
        public Creature(int x, int y, string name, int height, int width, Image image, bool direction, string property, int top, int g, int spaceG, string condition)
        {
            X = x;
            Y = y;
            this.name = name;
            this.height = height;
            this.width = width;
            this.direction = direction;
            this.property = property;
            this.top = top;
            this.g = g;
            this.spaceG = spaceG;
            this.condition = new List<string>() { condition };
            this.image = image;
            run_animation = 0;
            runIf = 0;
        }
        public Creature(int x, int y, string name, int height, int width, Image image, bool direction, string property, int top, int g, int spaceG, string condition, int proper_height) : this( x,  y,  name,  height,  width,  image,  direction,  property,  top,  g,  spaceG,  condition)
        {
            this.proper_height = proper_height;
        }
        public int top;
        public int g;
        public int spaceG;
        public List<string> condition;
        public int proper_height;

        public List<Block> Check_u(List<Block> blocks, int creatures_i, string sposob)
        {
            for (int r = 0; r < blocks.Count; r++)
            {
                blocks[r].Check_u(creatures_i, sposob);
            }
            return blocks;
        }
        public List<Block> Dead_fall(List<Block> blocks, int creatures_i)
        {
            g = 1;
            top = 1500;
            condition.Add("dead_fall");
            return  Check_u(blocks, creatures_i, "");
        }

        public Rectangle DestRect()
        {
            return new Rectangle(0, 0, width, height);
        }

        public void Animation(string metod)
        {
            run_animation++;
            switch (metod)
            {
                case "Walk":
                    if (name == "SMB_greenkoopatroopa")
                    {
                        if (direction)
                        {
                            if (run_animation % 10 == 0)
                            {
                                if (runIf == 0) { image = im_gl.SMB_greenkoopatroopa1; runIf = 1; }
                                else { image = im_gl.SMB_greenkoopatroopa2; runIf = 0; }
                            }
                        }
                        else
                        {
                            if (run_animation % 10 == 0)
                            {
                                if (runIf == 0) { image = im_gl.SMB_greenkoopatroopa1_invert; runIf = 1; }
                                else { image = im_gl.SMB_greenkoopatroopa2_invert; runIf = 0; }
                            }
                        }
                    }
                    else if (name == "SMB_greenparatrooper")
                    {
                        if (direction)
                        {
                            if (run_animation % 10 == 0)
                            {
                                if (runIf == 0) { image = im_gl.SMB_greenparatrooper1; runIf = 1; }
                                else { image = im_gl.SMB_greenparatrooper2; runIf = 0; }
                            }
                        }
                        else
                        {
                            if (run_animation % 10 == 0)
                            {
                                if (runIf == 0) { image = im_gl.SMB_greenparatrooper1_invert; runIf = 1; }
                                else { image = im_gl.SMB_greenparatrooper2_invert; runIf = 0; }
                            }
                        }
                    }
                    break;
                case "Dead":
                    if (name == "Image_Goomba") { image = im_gl.Goomba___Grey___Stomp; runIf = 0; }
                    if (name == "SMB_greenparatrooper" || name == "SMB_greenkoopatroopa") image = im_gl.SMB_Greenshell__1_1;
                    break;
            }
        }
    }
}