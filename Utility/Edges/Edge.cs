using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class Edge : LineSegment
    {
        #region Properties
        public override Point FirstPoint
        {
            get
            {
                return FirstNode;
            }
        }

        public override Point SecondPoint
        {
            get
            {
                return SecondNode;
            }
        }

        public Node FirstNode { get; private set; }
        public Node SecondNode { get; private set; }
        #endregion

        #region Constructor
        public Edge(Node firstNode, Node secondNode) : base()
        {
            FirstNode = firstNode.X < secondNode.X ? firstNode : secondNode;
            SecondNode = firstNode.X >= secondNode.X ? firstNode : secondNode;
        }
        #endregion

        #region Public Methods
        public Node GetConnectedNode(Node node)
        {
            return FirstNode != node ? FirstNode : SecondNode;
        }

        public bool CanMakeConnection()
        {
            return !FirstNode.IsConnected(SecondNode);
        }

        public bool CanMakeConnection(UInt16 maxNumberOfConnections)
        {
            return FirstNode.Edges.Count < maxNumberOfConnections && 
                SecondNode.Edges.Count < maxNumberOfConnections &&
                !FirstNode.IsConnected(SecondNode);
        }

        public void MakeConnection()
        {
            FirstNode.Edges.Add(this);
            SecondNode.Edges.Add(this);
        }

        #endregion
    }
}
