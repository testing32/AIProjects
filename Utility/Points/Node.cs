using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class Node : Point
    {
        #region Properties
        public List<Edge> Edges { get; set; }
        public double Value { get; set; }

        public List<Node> ConnectedNodes
        {
            get
            {
                List<Node> connectedNodes = new List<Node>();

                foreach (Edge edge in Edges)
                    connectedNodes.Add(edge.GetConnectedNode(this));

                return connectedNodes;
            }
        }
        #endregion

        #region Constructor
        public Node(float x, float y) : base(x, y)
        {
            Edges = new List<Edge>();
            Value = double.MaxValue;
        }
        public Node(double x, double y) : this((float)x, (float)y) { }

        public Node(Point point) : this(point.X, point.Y) { }
        #endregion

        #region Public Methods

        /// <summary>
        /// Sets connected node values while being aware of the current value of goal
        /// This allows the algorithm to stop following a path if the value
        /// is above the current best goal value. This assumes that there are no negative
        /// edges
        /// </summary>
        /// <param name="goalNode">Current goal node with it's best value</param>
        public void SetConnectedNodeValues(Node goalNode)
        {
            // If our current value + the distance to the goal is already greater than the goal
            // we can stop our current path
            if (Value + Distance(goalNode) > goalNode.Value)
                return;

            // For each connected edge traverse it and add the cost of the edge
            // Use a greedy approach where we try the node which is closest to the goal node
            foreach (Edge edge in Edges.OrderBy(o => o.GetConnectedNode(this).Distance(goalNode)))
            {
                Node node = edge.GetConnectedNode(this);
                if (node.Value > edge.Length + Value)
                {
                    node.Value = edge.Length + Value;
                    node.SetConnectedNodeValues(goalNode);
                }
            }
        }

        /// <summary>
        /// Returns the line of sight edge if one exists
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public Edge GetLineOfSightEdge(Obstacles obstacles, Node node)
        {
            // Create an edge to the other node
            Edge edge = new Edge(this, node);

            return edge.Link(obstacles) ? edge : null;
        }

        /// <summary>
        /// Returns true if this node has a line of sight to a given node
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasLineOfSight(Obstacles obstacles, Node node)
        {
            return GetLineOfSightEdge(obstacles, node) != null;
        }

        /// <summary>
        /// Works like the connected values method except that any node can make a 
        /// connection to the goal once it has line of sight
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="goalNode"></param>
        public void SetConnectedLineOfSightValues(Obstacles obstacles, Node goalNode)
        {
            // If our current value is already greater than a line of sight to the goal
            // we can stop our current path
            if (Value > goalNode.Value)
                return;

            Edge edge = GetLineOfSightEdge(obstacles, goalNode);

            // Check to see if there is a direct line of sight to the goal
            if (edge != null)
            {
                // We have a staight line of sight which is the shortest distance
                if (goalNode.Value > Value)
                {
                    goalNode.Value = Value;
                    goalNode.RemoveConnections();
                    edge.MakeConnection();
                }
            }
            else
            {
                // Go to the next connected edge, use a greedy approach where we try the node which is closest to the
                // goal node
                foreach (Edge connectedEdge in Edges.OrderBy(o => o.GetConnectedNode(this).Distance(goalNode)))
                {
                    Node node = connectedEdge.GetConnectedNode(this);
                    if (node.Value > connectedEdge.Length + Value)
                    {
                        node.Value = connectedEdge.Length + Value;
                        node.SetConnectedLineOfSightValues(obstacles, goalNode);
                    }
                }
            }
        }

        public List<Node> ShortestPath()
        {
            if (Value == double.MaxValue)
                return null;

            if (Value == 0)
                return new List<Node>() { this };

            Node minConnectedNode = GetMinimumNode();
            if (minConnectedNode == null)
                return null;

            List<Node> shortestPath = minConnectedNode.ShortestPath();
            if(!shortestPath.Contains(this))
                shortestPath.Add(this);

            return shortestPath;
        }

        /// <summary>
        /// Returns true if this node is already connected to the given node
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsConnected(Node other)
        {
            foreach (Edge edge in Edges)
                if (edge.GetConnectedNode(this) == other)
                    return true;

            return false;
        }
        
        public void RemoveConnections()
        {
            foreach (Edge edge in Edges)
                edge.GetConnectedNode(this).Edges.Remove(edge);

            Edges.Clear();
        }
        #endregion

        #region Private Methods
        private Node GetMinimumNode()
        {
            Node minNode = null;
            double minValue = double.MaxValue;
            foreach (Edge edge in Edges)
            {
                if (minNode == null || edge.GetConnectedNode(this).Value + edge.Length < minValue)
                {
                    minNode = edge.GetConnectedNode(this);
                    minValue = minNode.Value + edge.Length;
                }
            }
            return minNode;
        }
        #endregion

    }
}
