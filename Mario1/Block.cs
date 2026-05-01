namespace Mario1
{
    public class Block : GameObject
    {
        public Block(int x, int y, string name, int height, int width, Image image, string property)
        {
            X = x;
            Y = y;
            this.name = name;
            this.height = height;
            this.width = width;
            this.property = property;
            this.image = image;
            block_we_stand = -1;
            sluchay = null;
        }
        public Block(int x, int y, string name, int height, int width, Image image, string property, int location_transfer) : this(x, y, name, height, width, image, property)
        {
            this.location_transfer = location_transfer;
        }
        
        public int[] sluchay;//для таймера случаев, хранящий за одно старое значение spawnY_block, чтобы небыло ошибок при задевании головой блоков и они вернулись на место, если нужно

        public void Delete_block_we_stand(int i, string sposob)
        {
            if (block_we_stand == i & sposob == "") { block_we_stand = -1; }
            else if (block_we_stand > i & sposob == "dead") { block_we_stand -= 1; }
        }
    }
}
