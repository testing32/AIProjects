using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HomeworkTwo
{
    public class Workspace
    {
        #region Constants
        static Random rnd = new Random();
        const ushort CACHE_STORAGE_SIZE = 1;
        #endregion

        static int counter = 0;

        #region Member Variables
        Queue<List<Node>> _pathCache;
        double _cacheAcceptanceDistance;
        #endregion
        
        #region Properties

        private double XMin { get; set; }
        private double XMax { get; set; }
        private double YMin { get; set; }
        private double YMax { get; set; }

        public Obstacles Obstacles { get; set; }
        public List<Node> Milestones { get; private set; }

        public double Width { get { return XMax - XMin; } }
        public double Height { get { return YMax - YMin; } }

        #endregion

        #region Constructor
        public Workspace(double xMin, double xMax, double yMin, double yMax)
        {
            XMin = xMin;
            XMax = xMax;
            YMin = yMin;
            YMax = yMax;
            Obstacles = new Obstacles();
            Milestones = new List<Node>();
            _pathCache = new Queue<List<Node>>();

            // The distance we are willing to accept using the cached path
            _cacheAcceptanceDistance = Math.Min(Height, Width) / 40.0;
        }

        #endregion

        #region Public Methods

        public void GenerateEvenGraph(UInt16 numberOfPointsAlongX, UInt16 numberOfPointsAlongY)
        {
            double xIncrementValue = (XMax - XMin) / (double)(numberOfPointsAlongX - 1);
            double yIncrementValue = (YMax - YMin) / (double)(numberOfPointsAlongY - 1);
            
            List<Node> newNodes = new List<Node>();

            for(UInt16 i = 0; i < numberOfPointsAlongX; i++)
                for (UInt16 j = 0; j < numberOfPointsAlongY; j++)
                {
                    Node node = new Node(i * xIncrementValue, j * yIncrementValue);
                    if (IsNodeValid(node, null))
                        newNodes.Add(node);
                }

            Milestones = newNodes;
            GenerateEvenConnections();
        }

        public void GenerateGraph(UInt16 numberOfMilestones, UInt16 numberOfNeighborsToConnect)
        {
            List<Node> newNodes = new List<Node>();

            while (newNodes.Count < numberOfMilestones + 2)
                newNodes.Add(GenerateValidNode(newNodes));
            GenerateGraph(newNodes, numberOfNeighborsToConnect);
        }

        public void GenerateGraph(List<Node> nodes, UInt16 numberOfNeighborsToConnect)
        {
            Milestones = nodes;
            GenerateConnections(numberOfNeighborsToConnect);
        }

        public List<Node> GetShortestPath(Point startPoint, Point endPoint)
        {
            if (startPoint == null || endPoint == null)
                return null;

            // We create and place the start a goal node into the graph
            Node startNode = new Node(startPoint);
            Node endNode = new Node(endPoint);

            List<Node> path = null;

            if (false && startPoint.Distance(endPoint) > _cacheAcceptanceDistance)
            {

                // Check to see if this path is cached
                List<Node> cachedPath = GetCachedPath(startPoint, endPoint);
                if (cachedPath != null)
                {
                    // We have a close match, build the shortest path using the cached path
                    path = DStarLite(startNode, endNode, cachedPath);
                }
                else
                {
                    // No close match, use A*
                    path = AStar(startNode, endNode);
                }
            }
            else
            {
                // The points are close, just use A*
                path = AStar(startNode, endNode);
            }

            AddPathToShortestPathCache(path);

            // Return the path if there is one
            return (path != null && path.Contains(startNode))
                ? SmoothPath(path) 
                : path;
        }

        public List<Node> GetShortestLineOfSight(Point startPoint, Point endPoint)
        {
            if (startPoint == null || endPoint == null)
                return null;

            // We create and place the start a goal node into the graph
            Node startNode = new Node(startPoint);
            Node endNode = new Node(endPoint);

            // if the end node is in an obstacle, return null
            if (!endNode.Clear(Obstacles))
                return null;

            // Make the start node connect
            ForceConnection(startNode);

            // Set the initial node values
            startNode.Value = 0;
            foreach (Node milestone in Milestones)
                milestone.Value = Double.MaxValue;

            startNode.SetConnectedLineOfSightValues(Obstacles, endNode);

            List<Node> path;
            if (startNode.HasLineOfSight(Obstacles, endNode))
            {
                // We have line of sight so we have a path of the start and end node
                path = new List<Node>() { startNode, endNode };
            }
            else // Get the shortest path   
                path = endNode.ShortestPath();

            // Remove the start and end connections
            startNode.RemoveConnections();
            endNode.RemoveConnections();

            // Return the path if there is one
            return (path != null && path.Contains(startNode))
                ? SmoothPath(path)
                : path;
        }

        /// <summary>
        /// Returns the number of steps it will take from a start point
        /// to get to a line of sight with a goal
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public double ShortestStepsToLineOfSight(Point startPoint, Point endPoint)
        {
            List<Node> path = GetShortestLineOfSight(startPoint, endPoint);
            return path == null ? 0 : path.Last().Value;
        }

        public double ShortestPathLength(Point startPoint, Point endPoint)
        {
            List<Node> path = GetShortestPath(startPoint, endPoint);
            return path == null ? 0 : path.Last().Value;
        }

        /// <summary>
        /// Gets the node in the workspace at a given point location
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Node GetNode(Point p)
        {
            foreach (Node node in Milestones)
                if (node.X == p.X && node.Y == p.Y)
                    return node;

            return null;
        }
        
        /// <summary>
        /// Returns all of the nodes withing a given radius of a point
        /// </summary>
        /// <param name="p"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public List<Node> GetNodes(Point p, float radius)
        {
            List<Node> nodes = new List<Node>();

            // Only snag points which can be within the radius
            foreach(Node node in Milestones.
                Where(o => (o.X >= p.X - radius && o.X <= p.X + radius) 
                    && (o.Y >= p.Y - radius && o.Y <= p.Y + radius)))
            {
                // If the node is within the radius then add it to the list
                if(IsWithinCircle(p, radius, node))
                    nodes.Add(node);
            }

            // return the list
            return nodes;
        }

        /// <summary>
        /// Generates a valid node inside the workspace area
        /// </summary>
        /// <returns>The generated point</returns>
        public Node GenerateValidNode(List<Node> previousCreatedNodes)
        {
            Random rndm = new Random();
            Node node = null;
            do
            {
                node = new Node(rndm.NextDouble() * (XMax - XMin) + XMin, rndm.NextDouble() * (YMax - YMin) + YMin);
            } while (!IsNodeValid(node, previousCreatedNodes));
            return node;
        }

        /// <summary>
        /// This is a faster way of getting a random point on the graph
        /// If you try and generate a random point on a map with a lot of
        /// obstacles you could run into performance issues
        /// </summary>
        /// <returns></returns>
        public Node GetRandomMilestone()
        {
            if(Milestones == null || Milestones.Count == 0)
                return null;

            return Milestones[rnd.Next(Milestones.Count)];
        }

        /// <summary>
        /// Returns the first valid node in a list of nodes
        /// if a valid node exists
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Point GetValidNode(List<Node> list)
        {
            if(list != null)
                foreach (Node node in list)
                    if (IsNodeValid(node, null))
                        return node;

            return null;
        }

        /// <summary>
        /// Checks to see if a point is a valid location
        /// in this workspace
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointValid(Point point)
        {
            Node node = new Node(point);
            return IsNodeValid(node, null);
        }
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Checks to see if a point is within a cirlce by using
        /// determinants and orientation tests
        /// </summary>
        /// <param name="centerOfCircle"></param>
        /// <param name="radius"></param>
        /// <param name="testPoint"></param>
        /// <returns></returns>
        private bool IsWithinCircle(Point centerOfCircle, float radius, Point testPoint)
        {
            float adx, ady, bdx, bdy, cdx, cdy;
            float abdet, bcdet, cadet;
            float alift, blift, clift;

            // Circle test point a will be x - radius, y
            // Circle test point b will be x + radius, y
            // Circle test point c will be x, y + radius

            // test point d is the point we are trying to locate

            adx = centerOfCircle.X - radius - testPoint.X; // Ax - Dx
            ady = centerOfCircle.Y - testPoint.Y; // Ay - Dy

            bdx = centerOfCircle.X + radius - testPoint.X; // Bx - Dx
            bdy = centerOfCircle.Y - testPoint.Y; // By - Dy

            cdx = centerOfCircle.X - testPoint.X; // Cx - Dx
            cdy = centerOfCircle.Y + radius - testPoint.Y; // Cy - Dy

            // Calculating the determinant
            abdet = adx * bdy - bdx * ady;
            bcdet = bdx * cdy - cdx * bdy;
            cadet = cdx * ady - adx * cdy;
            alift = adx * adx + ady * ady;
            blift = bdx * bdx + bdy * bdy;
            clift = cdx * cdx + cdy * cdy;

            // If this value is greater than or equal to one then it's on or inside the circle
            return (alift * bcdet + blift * cadet + clift * abdet) >= 0;
        }

        /// <summary>
        /// Smooths out a node path so that we go from node line of sight
        /// to node line of sight rather than node to node
        /// This method greatly improves tracking quality
        /// </summary>
        /// <param name="path">Path of nodes that needs to be converted</param>
        /// <returns></returns>
        private List<Node> SmoothPath(List<Node> path)
        {
            // Our start node has to be in our path
            List<Node> newPath = new List<Node>() { path.First() };

            // We are starting at 2 because we don't want to consider the first item
            // since it's already in the list
            // The second item is the closest node to the first one but is isn't
            // nessisarilly in the correct direction to our goal
            // This was leading to weird behavior around obstacles where an update
            // would happen and the start node would be in an obstacle
            // The path would then backtrack to try anf fix itself.
            // By using the 3rd node as the 2nd node we get a smoother and more
            // natural path
            // The cost of using the 3rd node is that you might spend more time in
            // the obstacle or follow a path that leads you through an obstacle a little
            // bit. I can live with that as long as it looks nice.
            for (UInt16 i = 2; i < path.Count - 1; i++)
            {
                if (!newPath.Last().HasLineOfSight(Obstacles, path[i + 1]))
                    newPath.Add(path[i]);
            }

            // Our end node has to be in our path
            newPath.Add(path.Last());

            return newPath;
        }

        /// <summary>
        /// 
        /// </summary>
        private void GenerateEvenConnections()
        {
            List<Node> previousColumn = new List<Node>();
            List<Node> currentColumn = new List<Node>();

            // Make connections along the X coordinate
            foreach (Node node in Milestones.OrderBy(o => o.Y).OrderBy(o => o.X))
            {
                // There are other nodes that we can make connections to
                if (currentColumn.Count != 0)
                {
                    // Now we figure out what near by nodes we need to connect to

                    // If the node is starting a new column
                    if (currentColumn.Last().Y >= node.Y)
                    {
                        // place the "currentColumn" as the previous column and
                        // create a new current column
                        previousColumn = currentColumn;
                        currentColumn = new List<Node>();
                    }
                    else // we are continueing along the same column
                    {
                        // create an edge to the previous node since we are on the same column
                        Edge edge = new Edge(node, currentColumn.Last());
                        if (edge.Link(Obstacles))
                            edge.MakeConnection();
                    }

                    // Make connections to the nodes from the previous column
                    foreach (Node previousNode in previousColumn.
                        Where(o => o.Y == node.Y - 1 
                            || o.Y == node.Y 
                            || o.Y == node.Y + 1))
                    {
                        Edge previousColumnEdge = new Edge(node, previousNode);
                        if (previousColumnEdge.Link(Obstacles))
                            previousColumnEdge.MakeConnection();
                    }
                }
                // Add the node to the current column list
                currentColumn.Add(node);
            }
        }

        /// <summary>
        /// Generates edges between points
        /// </summary>
        /// <param name="maxNumberOfConnections">Maximum allowable connections between neighbors</param>
        private void GenerateConnections(UInt16 maxNumberOfConnections)
        {
            List<Edge> potentialEdgeList = new List<Edge>();

            // Get a list of all the potential connections
            foreach (Node mileStone in Milestones)
                potentialEdgeList.AddRange(GetPotentialEdges(mileStone));

            // Connect all of the nodes starting with the closest ones
            foreach (Edge edge in potentialEdgeList.Where(o => o.Link(Obstacles)).OrderBy(o => o.Length))
                if (edge.CanMakeConnection(maxNumberOfConnections))
                    edge.MakeConnection();
        }

        /// <summary>
        /// Get potential edges to a given node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<Edge> GetPotentialEdges(Node node)
        {
            List<Edge> potentialEdgeList = new List<Edge>();

            foreach (Node potentialConnectionNode in Milestones.Where(o => o != node))
                potentialEdgeList.Add(new Edge(node, potentialConnectionNode));

            return potentialEdgeList;
        }

        /// <summary>
        /// Forces a node to connect to its closest node. Useful for connecting a starting
        /// node which may be inside of an obstacle
        /// </summary>
        /// <param name="node"></param>
        private void ForceConnection(Node node)
        {
            IEnumerable<Edge> potentialEdges = GetPotentialEdges(node).OrderBy(o => o.Length).Where(o => o.CanMakeConnection());
            if (potentialEdges.Count() > 0)
                potentialEdges.First().MakeConnection();
        }

        /// <summary>
        /// Connects a node to it's closest neighbor
        /// </summary>
        /// <param name="node"></param>
        private bool MakeConnection(Node node)
        {
            IEnumerable<Edge> potentialEdges = GetPotentialEdges(node).OrderBy(o => o.Length).Where(o => o.CanMakeConnection() && o.Link(Obstacles));
            if (potentialEdges.Count() > 0)
            {
                potentialEdges.First().MakeConnection();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Connects a given node to the closest X nodes
        /// </summary>
        /// <param name="node">The node that needs to be connected</param>
        /// <param name="numberOfConnections">The limit on the number of connections that 
        /// a given node can be connected to</param>
        private void MakeConnection(Node node, UInt16 numberOfConnections)
        {
            foreach (Node possibleConnectionNode in Milestones.
                Select(o => o).
                Where(o => o != node && o.Edges.Count < numberOfConnections).
                OrderBy(o => o.Distance(node)))
            {
                Edge edge = new Edge(node, possibleConnectionNode);
                if (edge.Link(Obstacles))
                {
                    possibleConnectionNode.Edges.Add(edge);
                    node.Edges.Add(edge);
                    if (node.Edges.Count >= numberOfConnections)
                        break;
                }
            }
        }

        /// <summary>
        /// Checks to see if the point is unique, not null, don't collide with
        /// any obstacles, doesn't equal the start or goal node and is within the workspace
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsNodeValid(Node node, List<Node> previousCreatedNodes)
        {
            return node !=  null &&
                node.Clear(Obstacles) &&
                (previousCreatedNodes == null || !previousCreatedNodes.Contains(node)) &&
                node.X > XMin &&
                node.X < XMax &&
                node.Y > YMin &&
                node.Y < YMax;
        }

        /// <summary>
        /// Adds a path to the Path Cache
        /// </summary>
        /// <param name="path"></param>
        private void AddPathToShortestPathCache(List<Node> path)
        {
            if (path == null || path.Count == 0)
                return;

            if (_pathCache.Count >= CACHE_STORAGE_SIZE)
                _pathCache.Dequeue();

            _pathCache.Enqueue(new List<Node>(path));
        }

        /// <summary>
        /// Returns a path if it's close enough to the requested path
        /// </summary>
        /// <param name="firstPoint">The first point we are trying to find a path from</param>
        /// <param name="secondPoint">The second point we are trying to find a path to</param>
        /// <returns></returns>
        private List<Node> GetCachedPath(Point firstPoint, Point secondPoint)
        {

            // Search through each cached path to see if there is a close match
            foreach (List<Node> path in _pathCache)
            {
                // Get the first and last point in the path
                Node firstPathNode = path.First();
                Node lastPathNode = path.Last();

                // Check to see if the path is close to the request
                if ((firstPathNode.Distance(firstPoint) < _cacheAcceptanceDistance && lastPathNode.Distance(secondPoint) < _cacheAcceptanceDistance) ||
                    (firstPathNode.Distance(secondPoint) < _cacheAcceptanceDistance && lastPathNode.Distance(firstPoint) < _cacheAcceptanceDistance))
                {
                    // Make sure the start and end of the path are returned correctly
                    if (firstPathNode.Distance(secondPoint) < firstPathNode.Distance(firstPoint))
                    {
                        path.Reverse();
                    }

                    // return the cached path
                    return path;
                }
            }

            // There isn't a good match, return null
            return null;
        }

        /// <summary>
        /// Uses A* to find the shortest path between two nodes
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <returns></returns>
        private List<Node> AStar(Node startNode, Node endNode)
        {
            ForceConnection(startNode);

            // If the goal node can't create a link then return null
            if (!MakeConnection(endNode))
            {
                startNode.RemoveConnections();
                return null;
            }

            // Set the initial node values
            startNode.Value = 0;
            endNode.Value = Double.MaxValue;
            foreach (Node milestone in Milestones)
                milestone.Value = Double.MaxValue;

            // Add the values going out from the start node
            startNode.SetConnectedNodeValues(endNode);

            // Get the shortest path
            List<Node> path = endNode.ShortestPath();

            // Remove the start and end connections
            startNode.RemoveConnections();
            endNode.RemoveConnections();

            return path;
        }


        /// <summary>
        /// Uses a cached path to try and find the shortest path faster
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="endNode"></param>
        /// <param name="cachedPath"></param>
        /// <returns></returns>
        private List<Node> DStarLite(Node startNode, Node endNode, List<Node> cachedPath)
        {
            Debug.Print("Using Cached Pathfinding" + counter++);
            int middleIndex = cachedPath.Count() / 2;

            List<Node> startPath;
            int startIndex = 1;
            if(startNode.HasLineOfSight(Obstacles, cachedPath.First()))
            {
                startPath = new List<Node>() { startNode };
                while (startIndex + 1< middleIndex && cachedPath[startIndex + 1].HasLineOfSight(Obstacles, startNode))
                    startIndex++;
                
            }
            else
            {
                // First we get the first part of the path, we are going to connect this to the start node
                startIndex = middleIndex;
                while (cachedPath.First().Distance(cachedPath[startIndex]) > _cacheAcceptanceDistance)
                    startIndex--;

                // Use A* to get the start of the path
                startPath = AStar(startNode, new Node(cachedPath[startIndex]));
                if (startPath == null)
                    return null;
            }


            int endIndex = cachedPath.Count - 1;
            List<Node> endPath;
            if (endNode.HasLineOfSight(Obstacles, cachedPath.Last()))
            {
                endPath = new List<Node>() { endNode };
                while (endIndex - 1 > middleIndex && cachedPath[endIndex - 1].HasLineOfSight(Obstacles, endNode))
                    endIndex--;
                
            }
            else
            {
                // Next we get the last part of the path
                endIndex = middleIndex;
                while (cachedPath.Last().Distance(cachedPath[endIndex]) > _cacheAcceptanceDistance)
                    endIndex++;

                // Use A* to get the end of the path
                endPath = AStar(new Node(cachedPath[endIndex]), endNode);
                if (endPath == null)
                    return null;
            }

            // Now we build the new path
            List<Node> path = startPath;

            for (int i = startIndex; i <= endIndex; i++)
                path.Add(cachedPath[i]);

            path.AddRange(endPath);

            return path;
        }

        #endregion


        public float PercentageInLineOfSight(Point point, List<Node> nodes)
        {
            Node node = new Node(point);
            return nodes.Where(o => o.HasLineOfSight(Obstacles, node)).Count() / (float)nodes.Count();

        }
    }
}
