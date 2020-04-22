using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class CoordsEdge : IEdge<Coords>
    {

        public CoordsEdge(Coords start, Coords end)
        {
            Source = start;
            Target = end;
        }
        public Coords Source { get; private set; }

        public Coords Target { get; private set; }
    }

    public class GraphBasedVisitOnceAgent : SimpleAgentBase
    {
        private AdjacencyGraph<Coords, CoordsEdge> _Graph;

        public GraphBasedVisitOnceAgent(Dictionary<string, string> args) : base(args)
        {
            _Graph = new AdjacencyGraph<Coords, CoordsEdge>();
        }

        private List<CoordsEdge> GetNeighbours(Coords cell)
        {
            List<CoordsEdge> neighbours = new List<CoordsEdge>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    Coords c = new Coords(cell.Row + i, cell.Column + j);
                    if (c.Row >= 0 && c.Row < SpaceSize.Row
                        && c.Column >= 0 && c.Column < SpaceSize.Column)
                    {
                        var ce = new CoordsEdge(cell, c);
                        neighbours.Add(ce);
                    }
                }
            }
            return neighbours;
        }

        public override IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            if (_Graph.VertexCount == 0)
            {
                // add vertices
                for (int i = 0; i < SpaceSize.Row; i++)
                {
                    for (int j = 0; j < SpaceSize.Column; j++)
                    {
                        var cell = new Coords(i, j);
                        _Graph.AddVertex(cell);
                    }
                }
                for (int i = 0; i < SpaceSize.Row; i++)
                {
                    for (int j = 0; j < SpaceSize.Column; j++)
                    {
                        var cell = new Coords(i, j);
                        _Graph.AddEdgeRange(GetNeighbours(cell));
                    }
                }
            }
            var algo = new DijkstraShortestPathAlgorithm<Coords, CoordsEdge>(_Graph, edge =>
            {
                return 1;
            });
            return new CleanCommand(AgentId, location.Row, location.Column);
        }
    }
}
