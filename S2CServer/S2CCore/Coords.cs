using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class Coords
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public Coords(int row, int col)
        {
            Row = row;
            Column = col;
        }

        public override bool Equals(object obj)
        {
            return obj is Coords coords &&
                   Row == coords.Row &&
                   Column == coords.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}
