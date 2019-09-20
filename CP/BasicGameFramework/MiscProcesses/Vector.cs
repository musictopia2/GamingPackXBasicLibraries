namespace BasicGameFramework.MiscProcesses
{
    public struct Vector //it does have to be a structure though.
    {
        public int Column { get; set; }
        public int Row { get; set; }
        public Vector(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}