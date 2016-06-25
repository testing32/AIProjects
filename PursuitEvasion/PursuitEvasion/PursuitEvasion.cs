using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using HomeworkTwo;

namespace PursuitEvasion
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PursuitEvasion : Microsoft.Xna.Framework.Game
    {
        #region Constants
        const UInt16 X_MIN = 0;
        const UInt16 X_MAX = 40;
        UInt16 X_RANGE = X_MAX - X_MIN;

        const UInt16 Y_MIN = 0;
        const UInt16 Y_MAX = 40;
        UInt16 Y_RANGE = Y_MAX - Y_MIN;

        const float MAX_SPEED = 10.0f;
        const float TIME_STEP_LENGTH = 1.0f;
        
        const float RADIUS = 0.0f;
        const float MASS = 1.0f;
        const float TURN_RATE = 1.0f;
        const float MAX_FORCE = 4.0f;

        const float TIME_HORIZON = 1.0f;
        const float VISIBLITY_CRITERIA = 1.0f;
        #endregion

        #region Drawing Stuff
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Vector2 circlePosition = new Vector2(0, 0);
        Vector2 buildingPositionOne = new Vector2(0, 0);
        Vector2 buildingPositionTwo = new Vector2(500, 200);

        String backgroundName;
        Texture2D backgroundTexture;
        Texture2D circleTexture;
        Texture2D buildingTexture;
        #endregion

        #region Member Variables
        RobotGroup _robotGroup;
        Workspace _workspace;
        KeyboardState _keyboardState;
        KeyboardState _oldKeyboardState;

        // 1=CITY, 2=BRIDGE, 3=FARM
        int _simulation = 1;
        bool _showLabels;
        bool _showBackground;
        bool _showObstacles;
        #endregion

        #region XNA STUFF

        public PursuitEvasion()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 800;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            GenerateCity();
            _simulation = 1;

            _showLabels = false;
            _showBackground = true;
            _showObstacles = false;

            _keyboardState = new KeyboardState();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            circleTexture = Content.Load<Texture2D>("Circle");
            buildingTexture = Content.Load<Texture2D>("Building");

            font = Content.Load<SpriteFont>("AgentActivityFont");
            LoadBackground();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            _oldKeyboardState = _keyboardState;
            _keyboardState = Keyboard.GetState();

            if(_oldKeyboardState.IsKeyDown(Keys.NumPad1) && _keyboardState.IsKeyUp(Keys.NumPad1) && _simulation != 1)
            {
                GenerateCity();
                _simulation = 1;
                LoadBackground();
            }
            else if (_oldKeyboardState.IsKeyDown(Keys.NumPad2) && _keyboardState.IsKeyUp(Keys.NumPad2) && _simulation != 2) 
            {
                GenerateBridge();
                _simulation = 2;
                LoadBackground();
            }
            else if (_oldKeyboardState.IsKeyDown(Keys.NumPad3) && _keyboardState.IsKeyUp(Keys.NumPad3) && _simulation != 3) 
            {
                GenerateFarm();
                _simulation = 3;
                LoadBackground();
            }
            else if (_oldKeyboardState.IsKeyDown(Keys.L) && _keyboardState.IsKeyUp(Keys.L))
            {
                _showLabels = !_showLabels;
            }
            else if (_oldKeyboardState.IsKeyDown(Keys.B) && _keyboardState.IsKeyUp(Keys.B))
            {
                _showBackground = !_showBackground;
            }
            else if (_oldKeyboardState.IsKeyDown(Keys.O) && _keyboardState.IsKeyUp(Keys.O))
            {
                _showObstacles = !_showObstacles;
            }

            if(_robotGroup != null)
                _robotGroup.Update((UInt16)gameTime.ElapsedGameTime.Milliseconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            // Draw the background first
            Rectangle screenRectangle = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            if(backgroundTexture != null && _showBackground)
                spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);

            if (_workspace != null && _showObstacles)
                foreach (Obstacle obstacle in _workspace.Obstacles)
                    spriteBatch.Draw(CreateRectangle(obstacle), ConvertObstacleToVector2(obstacle), Color.White);

            if (_robotGroup != null)
            {
                foreach (RobotBase pursuer in _robotGroup.PursuitRobots)
                {
                    spriteBatch.Draw(
                        circleTexture,
                        ConvertPointToVector2(pursuer.Location),
                        null,
                        pursuer is GroundPursuer ? Color.Red : Color.Purple,
                        0f,
                        Vector2.Zero,
                        .2f,
                        SpriteEffects.None,
                        0);

                    if (_showLabels)
                    {
                        // Find the center of the string
                        Vector2 FontOrigin = font.MeasureString(pursuer.CurrentTask.ToString()) / 2;

                        // Draw the string
                        spriteBatch.DrawString(font, pursuer.CurrentTask.ToString(), ConvertPointToVector2(pursuer.Location), Color.LightGreen,
                            0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                    }
                }
                foreach (RobotBase target in _robotGroup.TargetRobots)
                {
                    spriteBatch.Draw(
                        circleTexture,
                        ConvertPointToVector2(target.Location),
                        null,
                        Color.Blue,
                        0f,
                        Vector2.Zero,
                        .2f,
                        SpriteEffects.None,
                        0);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D CreateRectangle(Obstacle obstacle)
        {
            UInt16 width = (UInt16)(obstacle.Width * (GraphicsDevice.Viewport.Width / X_RANGE));
            UInt16 height = (UInt16)(obstacle.Height * (GraphicsDevice.Viewport.Height / Y_RANGE));

            Texture2D rectangleTexture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            Color[] color = new Color[width * height] ;//set the color to the amount of pixels in the textures

            for (int i = 0; i < color.Length; i++)//loop through all the colors setting them to whatever values we want
                color[i] = Color.Black;

            rectangleTexture.SetData(color);//set the color data on the texture
            return rectangleTexture;//return the texture
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Generates all the city stuff
        /// </summary>
        private void GenerateCity()
        {
            backgroundName = "City";
            Obstacles obstacles = new Obstacles();

            // Create obstacles
            for (double i = 3.5; i <= 38; i = i + 5.5)
                for (double j = 25.5; j <= 38; j = j + 5.5)
                    obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(j,i), 4, 4));

            for (double i = 25.5; i <= 38; i = i + 5.5)
                for (double j = 3.5; j <= 22; j = j + 5.5)
                    obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(j,i), 4, 4));

            // Set up workspace and generate nodes
            _workspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { Obstacles = obstacles };
            _workspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));


            Workspace airWorkspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { };
            airWorkspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));

            List<Node> groundPatrolPath = new List<Node>() {
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 20)),
                airWorkspace.GetNode(new HomeworkTwo.Point(20, 20)),
                airWorkspace.GetNode(new HomeworkTwo.Point(20, 1)),
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 1))};

            List<Node> airPatrolPath = new List<Node>() {
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 39)),
                airWorkspace.GetNode(new HomeworkTwo.Point(39, 39)),
                airWorkspace.GetNode(new HomeworkTwo.Point(39, 1)),
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 1))};

            // Create our agents
            List<RobotBase> pursuerRobots = new List<RobotBase>() {
                new GroundPursuer(GroundPursuerPatrol.TheInstance,
                    new HomeworkTwo.Point(1,1), // location
                    RADIUS, // radius
                    new Vector2D(), // intial velocity
                    MAX_SPEED * .8f, // max speed
                    new Vector2D(), // intial heading
                    MASS, // mass
                    new Vector2D(), // scale
                    TURN_RATE, // turn rate
                    MAX_FORCE, // max force
                    groundPatrolPath, // patrol path
                    _workspace, // traversability map
                    _workspace), // visibility map

                new AerialPursuer(AerialPursuerPatrol.TheInstance,
                    new HomeworkTwo.Point(1,15), // location
                    RADIUS, // radius
                    new Vector2D(), // intial velocity
                    MAX_SPEED, // max speed
                    new Vector2D(), // intial heading
                    MASS, // mass
                    new Vector2D(), // scale
                    TURN_RATE, // turn rate
                    MAX_FORCE, // max force
                    airPatrolPath, // patrol path
                    airWorkspace, // traversability map
                    airWorkspace) }; // visibility map

            List<Target> targetRobots = new List<Target>() { new Target(TargetPatrol.TheInstance,
                new HomeworkTwo.Point(30,39), // location
                RADIUS, // radius
                new Vector2D(), // intial velocity
                MAX_SPEED, // max speed
                new Vector2D(), // intial heading
                MASS, // mass
                new Vector2D(), // scale
                TURN_RATE, // turn rate
                MAX_FORCE, // max force
                new List<Node>(), // patrol path
                _workspace, // traversability map
                _workspace)}; // visibility map
            
            // Create our robot group with our agents
            _robotGroup = new RobotGroup(pursuerRobots, targetRobots, TIME_STEP_LENGTH, TIME_HORIZON, VISIBLITY_CRITERIA);
        }

        /// <summary>
        /// Generates all the bridge stuff
        /// </summary>
        private void GenerateBridge()
        {
            backgroundName = "Bridge";

            Workspace targetWorkspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { Obstacles = GenerateBaseBridgeObstacles() };
            targetWorkspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));
            
            _workspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { Obstacles = GenerateBaseBridgeObstacles() };
            _workspace.Obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(23, 19), 5, 38));
            _workspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));
            
            List<Node> groundPatrolPath = new List<Node>() {
                _workspace.GetNode(new HomeworkTwo.Point(18, 2)),
                _workspace.GetNode(new HomeworkTwo.Point(18, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 2)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(18, 35))};

            // Create our robot group with our agents
            _robotGroup = new RobotGroup(
                GeneratePursuers(new HomeworkTwo.Point(25, 38), _workspace, _workspace, groundPatrolPath),
                GenerateTargets(new HomeworkTwo.Point(30, 39), targetWorkspace, targetWorkspace),
                TIME_STEP_LENGTH,
                TIME_HORIZON,
                VISIBLITY_CRITERIA);
        }

        /// <summary>
        /// Generates all the farm stuff
        /// </summary>
        private void GenerateFarm()
        {
            backgroundName = "Farm";

            _workspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { Obstacles = GenerateBaseFarmObstacles() };
            _workspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));

            List<Node> groundPatrolPath = new List<Node>() {
                _workspace.GetNode(new HomeworkTwo.Point(18, 10)),
                _workspace.GetNode(new HomeworkTwo.Point(18, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 2)),
                _workspace.GetNode(new HomeworkTwo.Point(28, 35)),
                _workspace.GetNode(new HomeworkTwo.Point(18, 35))};

            // Create our robot group with our agents
            _robotGroup = new RobotGroup(
                GeneratePursuers(new HomeworkTwo.Point(25, 38), _workspace, _workspace, groundPatrolPath),
                GenerateTargets(new HomeworkTwo.Point(30, 39), _workspace, _workspace),
                TIME_STEP_LENGTH,
                TIME_HORIZON,
                VISIBLITY_CRITERIA);
        }

        private Obstacles GenerateBaseBridgeObstacles()
        {
            Obstacles obstacles = new Obstacles();

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(3.6, 6.3), 6.2, 11.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(3.6, 18.7), 6.2, 12.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(3.6, 31), 6.2, 11.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(3.2, 38.7), 7, 3));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(12.5, 6.3), 10, 11));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(12.5, 18.7), 10, 12.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(12.5, 31), 10, 11.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(12.5, 38.7), 10, 2.7));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34, 1), 10.5, 2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34, 7.4), 10.5, 9.7));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34, 18), 10.5, 10));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34, 27.5), 10.5, 7.5));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34, 35), 10.5, 6.5));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(33, 39.5), 14, 1));

            return obstacles;
        }

        private Obstacles GenerateBaseFarmObstacles()
        {
            Obstacles obstacles = new Obstacles();

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(3.8, 3.8), 7.6, 7.6));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(1.8, 11.5), 3.6, 6));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(16.5, 3.8), 10.5, 7.6));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(21, 10), 1, 10));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(9, 12), 6, 6));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(9, 20), 2, 4));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(18.8, 24), 7.5, 2.5));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(20, 26), 5, 1.5));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(28.5, 16.5), 4, 3));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(26.5, 18.9), 1, 2.2));

            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(38, 16), 4, 10));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(28.1, 21.2), 7.5, 1.2));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(32.3, 22.5), 1, 10));
            obstacles.Add(new RectangleObstacle(new HomeworkTwo.Point(34.8, 24), 4, 4));

            return obstacles;
        }

        private List<Target> GenerateTargets(HomeworkTwo.Point point, Workspace traversabilityMap, Workspace visibilityMap)
        {
            // Create our agents
            return new List<Target>() { new Target(TargetPatrol.TheInstance,
                point, // location
                RADIUS, // radius
                new Vector2D(), // intial velocity
                MAX_SPEED * .7f, // max speed
                new Vector2D(), // intial heading
                MASS, // mass
                new Vector2D(1.0, 1.0), // scale
                TURN_RATE, // turn rate
                MAX_FORCE, // max force
                new List<Node>(), // patrol path
                traversabilityMap, // traversability map
                visibilityMap)}; // visibility map
        }

        private List<RobotBase> GeneratePursuers(HomeworkTwo.Point point, Workspace traversabilityMap, 
            Workspace visibilityMap, List<Node> groundPatrolPath)
        {
            Workspace airWorkspace = new Workspace(X_MIN, X_MAX, Y_MIN, Y_MAX) { };
            airWorkspace.GenerateEvenGraph((UInt16)(X_RANGE + 1), (UInt16)(Y_RANGE + 1));

            List<Node> airPatrolPath = new List<Node>() {
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 39)),
                airWorkspace.GetNode(new HomeworkTwo.Point(39, 39)),
                airWorkspace.GetNode(new HomeworkTwo.Point(39, 1)),
                airWorkspace.GetNode(new HomeworkTwo.Point(1, 1))};

            return new List<RobotBase>() { new GroundPursuer(GroundPursuerPatrol.TheInstance,
                point, // location
                RADIUS, // radius
                new Vector2D(), // intial velocity
                MAX_SPEED * .6f, // max speed
                new Vector2D(), // intial heading
                MASS, // mass
                new Vector2D(1, 1), // scale
                TURN_RATE, // turn rate
                MAX_FORCE, // max force
                groundPatrolPath, // patrol path
                traversabilityMap, // traversability map
                visibilityMap),// visibility map
            
                new AerialPursuer(AerialPursuerPatrol.TheInstance,
                    new HomeworkTwo.Point(1,15), // location
                    RADIUS, // radius
                    new Vector2D(), // intial velocity
                    MAX_SPEED * .75f, // max speed
                    new Vector2D(), // intial heading
                    MASS, // mass
                    new Vector2D(), // scale
                    TURN_RATE, // turn rate
                    MAX_FORCE, // max force
                    airPatrolPath, // patrol path
                    airWorkspace, // traversability map
                    airWorkspace) }; // visibility map
        }

        /// <summary>
        /// Converts an obstacle into a 2d vector so we can render it
        /// This contains the starting location of where do draw the obstacle
        /// </summary>
        /// <param name="obstacle"></param>
        /// <returns></returns>
        private Vector2 ConvertObstacleToVector2(Obstacle obstacle)
        {
            return new Vector2(
                (float)((obstacle.Location.X - obstacle.Width / 2.0) * (GraphicsDevice.Viewport.Width / X_RANGE)),
                (float)((obstacle.Location.Y - obstacle.Height / 2.0) * (GraphicsDevice.Viewport.Height / Y_RANGE)));
        }

        private Vector2 ConvertPointToVector2(HomeworkTwo.Point point)
        {
            return new Vector2(point.X * (GraphicsDevice.Viewport.Width / X_RANGE), 
                point.Y * (GraphicsDevice.Viewport.Height / Y_RANGE));
        }

        private void LoadBackground()
        {
            if (!String.IsNullOrEmpty(backgroundName))
                backgroundTexture = Content.Load<Texture2D>(backgroundName);
        }
        #endregion
    }
}
