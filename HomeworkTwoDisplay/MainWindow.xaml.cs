using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HomeworkTwo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constants
        int X_MIN = 0;
        int X_MAX = 22;
        int Y_MIN = 0;
        int Y_MAX = 22;
        UInt16 NUMBER_OF_MILESTONES = 200;
        UInt16 NUMBER_OF_CONNECTIONS = 10;
        double IMPORTANT_POINT_SIZE = 1.4;
        double NORMAL_POINT_SIZE = 2;
        bool SET_POINTS = false;
        bool LINE_OF_SIGHT = false;
        bool EVEN_POINTS = true;
        Obstacles obstacles = new Obstacles();

        int xRange;
        int yRange;
        double pixelsPerXUnit;
        double pixelsPerYUnit;

        #endregion

        Workspace _workspace;

        public MainWindow()
        {
            InitializeComponent();
            _workspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX);
        }

        #region Events
        private void btnTestCaseOne_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
            obstacles = new Obstacles(){new CircleObstacle(new Point(10, 4), 4),
            new CircleObstacle(new Point(8, 18), 4),
            new CircleObstacle(new Point(18, 14), 4)};

            Point startPoint = new Point(2.1, 20.1);
            Point goalPoint = new Point(14.1, 1.1);

            DrawStuff(startPoint, goalPoint);
        }

        private void btnTestCaseTwo_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
            obstacles = new Obstacles(){new CircleObstacle(new Point(10, 4), 4),
            new CircleObstacle(new Point(10, 16), 6),
            new CircleObstacle(new Point(18, 14), 4)};

            Point startPoint = new Point(2.1, 20.1);
            Point goalPoint = new Point(16.1, 1.1);

            DrawStuff(startPoint, goalPoint);
        }
        #endregion

        #region Private Methods

        private void DrawStuff(Point startPoint, Point goalPoint)
        {
            // Set Constants
            xRange = X_MAX - X_MIN;
            yRange = Y_MAX - Y_MIN;
            pixelsPerXUnit = myCanvas.ActualWidth / (double)xRange;
            pixelsPerYUnit = myCanvas.ActualHeight / (double)yRange;

            GenerateGrid();
            GenerateObstacles();
            GeneratePoint(new Node(startPoint), Brushes.Black, Brushes.Green, IMPORTANT_POINT_SIZE);
            GeneratePoint(new Node(goalPoint), Brushes.Black, Brushes.Red, IMPORTANT_POINT_SIZE);

            //Workspace workspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { Obstacles = obstacles };
            Workspace workspace = _workspace;
            workspace.Obstacles = obstacles;
            List<Node> shortestPath;
            if (SET_POINTS)
            {
                List<Node> setNodes = new List<Node>() {
                new Node(5, 15),
                new Node(5, 12),
                new Node(10, 9),
                new Node(14, 6),
                new Node(15.99, 1.01),
                new Node(14.01, 1.01)};
                /*
                List<Node> setNodes = new List<Node>() {
                new Node() { XCoordinate=12, YCoordinate=0)
                new Node() { XCoordinate=12, YCoordinate=8 }};*/
                workspace.GenerateGraph(setNodes, NUMBER_OF_CONNECTIONS);
                shortestPath = workspace.GetShortestPath(startPoint, goalPoint);
            }
            else if (EVEN_POINTS)
            {
                if (LINE_OF_SIGHT)
                    shortestPath = ProbabilisticRoadmap.TheInstance.ExecuteLineOfSight(
                        obstacles,
                        startPoint,
                        goalPoint,
                        (UInt16)(xRange + 1),
                        (UInt16)(yRange + 1),
                        workspace);
                else
                {
                    workspace.Obstacles = obstacles;
                    workspace.GenerateEvenGraph((UInt16)(xRange + 1), (UInt16)(yRange + 1));
                    shortestPath = workspace.GetShortestPath(startPoint, goalPoint);
                }
            }
            else
            {
                shortestPath = ProbabilisticRoadmap.TheInstance.Execute(
                       obstacles,
                       startPoint,
                       goalPoint,
                       NUMBER_OF_MILESTONES,
                       NUMBER_OF_CONNECTIONS,
                       workspace);
            }
            GenerateMilestones(workspace);
            GenerateEdges(workspace);
            GenerateShortestPath(shortestPath);
            GeneratePathLength(shortestPath);
        }

        private void GenerateGrid()
        {
            Line line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 4,
                X1 = 1,
                X2 = 1,
                Y1 = 1,
                Y2 = myCanvas.ActualHeight
            };
            myCanvas.Children.Add(line);

            line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 4,
                X1 = myCanvas.ActualWidth,
                X2 = myCanvas.ActualWidth,
                Y1 = 1,
                Y2 = myCanvas.ActualHeight
            };
            myCanvas.Children.Add(line);

            line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 4,
                X1 = 1,
                X2 = myCanvas.ActualWidth,
                Y1 = 1,
                Y2 = 1
            };
            myCanvas.Children.Add(line);

            line = new Line()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 4,
                X1 = 1,
                X2 = myCanvas.ActualWidth,
                Y1 = myCanvas.ActualHeight,
                Y2 = myCanvas.ActualHeight
            };
            myCanvas.Children.Add(line);

            for (int i = 1; i < xRange; i++)
            {
                line = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = i * pixelsPerXUnit,
                    X2 = i * pixelsPerXUnit,
                    Y1 = 1,
                    Y2 = myCanvas.ActualHeight
                };
                myCanvas.Children.Add(line);
            }

            for (int i = 1; i < yRange; i++)
            {
                line = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    X1 = 1,
                    X2 = myCanvas.ActualWidth,
                    Y1 = i * pixelsPerYUnit,
                    Y2 = i * pixelsPerYUnit
                };
                myCanvas.Children.Add(line);
            }
        }

        private void GenerateObstacles()
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle is CircleObstacle)
                {
                    CircleObstacle circleObstacle = (CircleObstacle)obstacle;

                    Ellipse ellipse = new Ellipse();

                    ellipse.Width = circleObstacle.Radius * 2 * pixelsPerXUnit;
                    ellipse.Height = circleObstacle.Radius * 2 * pixelsPerYUnit;

                    ellipse.StrokeThickness = 5;
                    ellipse.Stroke = Brushes.Blue;
                    ellipse.Fill = Brushes.Blue;
                    ellipse.Opacity = .6;

                    Canvas.SetTop(ellipse, (obstacle.Location.Y + Y_MIN - circleObstacle.Radius) * pixelsPerYUnit);
                    Canvas.SetLeft(ellipse, (obstacle.Location.X + X_MIN - circleObstacle.Radius) * pixelsPerXUnit);
                    myCanvas.Children.Add(ellipse);
                }
            }
        }

        private void GeneratePoint(Node node, SolidColorBrush outerColor, SolidColorBrush innerColor, double size)
        {
            #if DEBUG
            if (node.Value != double.MaxValue)
            {
                TextBlock tb = new TextBlock();
                tb.Text = node.Value.ToString("F");
                Canvas.SetTop(tb, (node.Y + Y_MIN) * pixelsPerYUnit - tb.ActualHeight / 2.0);
                Canvas.SetLeft(tb, (node.X + X_MIN) * pixelsPerXUnit - tb.ActualWidth / 2.0);
                myCanvas.Children.Add(tb);
            }
            else
            #endif
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = pixelsPerXUnit / size;
                ellipse.Height = pixelsPerYUnit / size;
                ellipse.StrokeThickness = 3;
                ellipse.Stroke = outerColor;
                ellipse.Fill = innerColor;
                ellipse.Opacity = .6;

                Canvas.SetTop(ellipse, (node.Y + Y_MIN) * pixelsPerYUnit - ellipse.Height / 2.0);
                Canvas.SetLeft(ellipse, (node.X + X_MIN) * pixelsPerXUnit - ellipse.Width / 2.0);
                myCanvas.Children.Add(ellipse);
            }
        }

        private void GenerateMilestones(Workspace workspace)
        {
            if (workspace == null || workspace.Milestones == null || workspace.Milestones.Count == 0)
                return;

            foreach (Node milestone in workspace.Milestones)
                GeneratePoint(milestone, Brushes.Gray, Brushes.DarkSlateGray, NORMAL_POINT_SIZE);
        }

        private void GenerateEdges(Workspace workspace)
        {
            if (workspace == null || workspace.Milestones == null || workspace.Milestones.Count == 0)
                return;

            Line line;
            foreach (Node milestone in workspace.Milestones)
            {
                foreach (Edge edge in milestone.Edges)
                {
                    line = new Line()
                    {
                        Stroke = Brushes.Gray,
                        StrokeThickness = 1,
                        X1 = edge.FirstPoint.X * pixelsPerXUnit,
                        X2 = edge.SecondPoint.X * pixelsPerXUnit,
                        Y1 = edge.FirstPoint.Y * pixelsPerYUnit,
                        Y2 = edge.SecondPoint.Y * pixelsPerYUnit
                    };
                    myCanvas.Children.Add(line);
                }
            }
        }

        private void GenerateShortestPath(List<Node> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return;

            Line line;
            for (int i = 1; i < nodes.Count; i++)
            {
                line = new Line()
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 3,
                    X1 = nodes[i - 1].X * pixelsPerXUnit,
                    X2 = nodes[i].X * pixelsPerXUnit,
                    Y1 = nodes[i - 1].Y * pixelsPerYUnit,
                    Y2 = nodes[i].Y * pixelsPerYUnit
                };
                myCanvas.Children.Add(line);
            }
        }

        private void GeneratePathLength(List<Node> shortestPath)
        {
            if (shortestPath == null)
            {
                lbPathLengthValue.Content = string.Empty;
                return;
            }
            double pathLength = 0;
            for (int i = 1; i < shortestPath.Count; i++)
            {
                pathLength += shortestPath[i].Distance(shortestPath[i - 1]);
            }
            lbPathLengthValue.Content = pathLength.ToString("F");
        }

        #endregion
        
    }
}
