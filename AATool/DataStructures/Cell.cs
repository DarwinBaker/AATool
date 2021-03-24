
namespace AATool.DataStructures
{
    public struct Cell
    {
        public int Row;
        public int Column;

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Cell))
                return false;

            Cell other = (Cell)obj;
            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Row.GetHashCode();
                hash = hash * 23 + Column.GetHashCode();
                return hash;
            }
        }
    }
}
