using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace S2CCore
{
    class AgentState
    {
        public int id { get; }
        public int row { get; private set; }
        public int col { get; private set; }

        private Dictionary<string, string> attributes;

        public AgentState(int agentId, int startRow, int startCol)
        {
            id = agentId;
            row = startRow;
            col = startCol;
            attributes = new Dictionary<string, string>();
        }

        public void addAttribute(string k, string v)
        {
            this.attributes.Add(k, v);
        }

        public string getAttribute(string k)
        {
            return this.attributes[k];
        }

        public void moveTo(int row, int col)
        {
            this.row = row;
            this.col = col;
        }
    }
}
