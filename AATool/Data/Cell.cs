
namespace AATool.Data
{
    public struct Cell
    {
        public int Row;
        public int Column;

        public Cell(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public override bool Equals(object obj) => 
            obj is Cell other && this.Row == other.Row && this.Column == other.Column;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + this.Row.GetHashCode();
                hash = (hash * 23) + this.Column.GetHashCode();
                return hash;
            }
        }
    }
}
