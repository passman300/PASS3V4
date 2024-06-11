//Author: Colin Wang
//File Name: Mob.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 4, 2024
//Modified Date: June 10, 2024
//Description: Basic mob class for the game, which is the parent of all other mobs (only have the skull working)

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace PASS3V4
{
    public class Mob : Entity
    {
        /// <summary>
        /// Represents the frames of a mob, providing functionality to read and add frames from a CSV file.
        /// </summary>
        public class Frames
        {
            // The current frame of the mob.
            public int currentFrame { get; set; }

            // The list of images representing the frames of the mob.
            public List<Texture2D> images = new();

            // The dictionary mapping frame names to their corresponding indices in the images list.
            public Dictionary<string, int> frameNameToNum = new();
 
            // The dictionary mapping frame indices to their corresponding names in the images list.
            public Dictionary<int, string> frameNumToName = new();

            /// <summary>
            /// Initializes a new instance of the <see cref="Frames"/> class by reading frames from a CSV file.
            /// </summary>
            /// <param name="sourceImg">The source image from which to crop the frames.</param>
            /// <param name="filePath">The path to the CSV file containing the frame information.</param>
            public Frames(Texture2D sourceImg, string filePath)
            {
                ReadCSV(sourceImg, "FrameSheets/Enemies/" + filePath);
            }

            /// <summary>
            /// Reads frames from a CSV file and adds them to the frames list.
            /// </summary>
            /// <param name="sourceImg">The source image from which to crop the frames.</param>
            /// <param name="csvFilePath">The path to the CSV file containing the frame information.</param>
            public void ReadCSV(Texture2D sourceImg, string csvFilePath)
            {
                try
                {
                    StreamReader r = new StreamReader(csvFilePath);
                    string[] data = r.ReadToEnd().Split("\r\n"); // read the entire file, and split it into lines

                    r.Close();
                    
                    // for each line in the file, read the x, y, width, and height of the frame
                    foreach (string rawLine in data)
                    {
                        if (rawLine == "") continue;

                        // split the line into the frame name, x, y, width, and height
                        string[] line = rawLine.Split(',');
                        string name = line[0].ToString();
                        int.TryParse(line[1].ToString(), out int x);
                        int.TryParse(line[2].ToString(), out int y);
                        int.TryParse(line[3].ToString(), out int width);
                        int.TryParse(line[4].ToString(), out int height);

                        // add the frame to the frames list
                        AddImage(sourceImg, new Rectangle(x, y, width, height), name);
                    }
                }
                catch (System.IO.FileNotFoundException e) // file not found
                {
                    StreamWriter tempSw = File.CreateText(csvFilePath); // create a new file
                    tempSw.Close();
                }
            }

            /// <summary>
            /// Adds a frame to the frames list.
            /// </summary>
            /// <param name="sourceImg">The source image from which to crop the frame.</param>
            /// <param name="sourceRec">The rectangle specifying the region of the source image to crop.</param>
            /// <param name="frameName">The name of the frame.</param>
            public void AddImage(Texture2D sourceImg, Rectangle sourceRec, string frameName)
            {
                // add the frame to the frames list
                images.Add(Util.Crop(sourceImg, sourceRec));

                // add the frame to the frameNameToNum and frameNumToName dictionaries
                frameNameToNum.Add(frameName, images.Count - 1);
                frameNumToName.Add(images.Count - 1, frameName);
            }

            /// <summary>
            /// Sets the current frame to the specified frame name.
            /// </summary>
            /// <param name="frameName">The name of the frame to set as the current frame.</param>
            public void SetCurrentFrame(string frameName)
            {
                currentFrame = frameNameToNum[frameName];
            }
        }

        // Mob types
        public enum MobType
        {
            Skull,
            Zombie,
            Ghost
        }

        // Mob states
        public enum MobState
        {
            Spawning,
            Idle,
            Walk,
            Attack,
            GetHit,
            Dead
        }

        public MobType Type { get; set; } // The type of the mob.
        public MobState State { get; set; } // The state of the mob.
        public int HostileRange { get; set; } // The range of the mob from the player, so it follows the player.
        public int AttackRange { get; set; } // The range of the mob from the player so it attacks.

        protected bool IsAttack { get; set; } // check if the mob is attacking
        protected bool IsMoving { get; set; } // check if the mob is moving
        public float AngleToPlayer { get; set; } // The angle to the player

        public Frames frames; // The frames of the mob.

        // DEBUG, the debug hit range
        protected GameCircle debugHitRange;

        /// <summary>
        /// Constructs a new mob, based on the following parameters.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="graphicsDevice"></param>
        /// <param name="mobType"></param>
        /// <param name="damage"></param>
        /// <param name="health"></param>
        /// <param name="speed"></param>
        /// <param name="hostileRange"></param>
        /// <param name="attackRange"></param>
        /// <param name="csvFilePath"></param>
        /// <param name="spawnPos"></param>
        /// <returns></returns>
        public Mob(ContentManager content, GraphicsDevice graphicsDevice, MobType mobType, int damage, int health, int speed, int hostileRange, int attackRange, string csvFilePath, Vector2 spawnPos) : base(content, graphicsDevice)
        {
            Type = mobType; // Set the mob type

            // Set the mob stats
            Damage = damage; 
            MaxHealth = health;
            CurrentHealth = health;
            Speed = speed;
            HostileRange = hostileRange;
            AttackRange = attackRange;

            // Set the mob state
            State = MobState.Spawning;
            position = spawnPos;

            // Load the mob frames
            frames = new Frames(Assets.skullImg, "Skull.csv");

            // Load the mobs health bar
            HealthBar = new HealthBar(Color.White, MaxHealth, MaxHealth, new Vector2(HealthBar.DEFAULT_HEALTHBAR_WIDTH, HealthBar.DEFAULT_HEALTHBAR_HEIGHT));
        }

        /// <summary>
        /// Initializes the rectangles for collision detection and debugging with the specified parameters.
        /// </summary>
        /// <param name="hurtBoxWidth">The width of the hurtBox.</param>
        /// <param name="hurtBoxHeight">The height of the hurtBox.</param>
        /// <param name="hurtBoxOffsetX">The X offset of the hurtBox relative to the position.</param>
        /// <param name="hurtBoxOffsetY">The Y offset of the hurtBox relative to the position.</param>
        /// <param name="feetWidth">The width of the feetRec.</param>
        /// <param name="feetHeight">The height of the feetRec.</param>
        /// <param name="feetOffsetX">The X offset of the feetRec relative to the position.</param>
        /// <param name="feetOffsetY">The Y offset of the feetRec relative to the position.</param>
        /// <param name="frameSize">The size of the frame.</param>
        public override void InitializeRectangles(int hurtBoxWidth, int hurtBoxHeight, int hurtBoxOffsetX, int hurtBoxOffsetY, int feetWidth, int feetHeight, int feetOffsetX, int feetOffsetY, Vector2 frameSize)
        {
            // Call the base class's InitializeRectangles method
            base.InitializeRectangles(hurtBoxWidth, hurtBoxHeight, hurtBoxOffsetX, hurtBoxOffsetY, feetWidth, feetHeight, feetOffsetX, feetOffsetY, frameSize);

            // Initialize the debugHitRange circle
            debugHitRange = new GameCircle(graphicsDevice, centerPosition, HostileRange);
        }
        
        /// <summary> 
        /// Updates the mob
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Overloaded Update method for mobs that required the use of wallRecs
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="wallRecs"></param>
        public override void Update(GameTime gameTime, Rectangle[] wallRecs)
        {
        }

        /// <summary>
        /// Updates the health bar with the current health and the center position.
        /// </summary>
        protected virtual void UpdateHealthBar()
        {
            // Update the health bar with the current health and the center position
            HealthBar.Update(CurrentHealth, centerPosition);
        }

        /// <summary>
        /// Updates the angle to the player
        /// </summary>
        /// <param name="playerPos"></param>
        protected void UpdateAngleToPlayer(Vector2 playerPos)
        {
            AngleToPlayer = (float)Math.Atan2(playerPos.Y - centerPosition.Y, playerPos.X - centerPosition.X);
        }

        /// <summary>
        /// Checks the range between the current mob and the player.
        /// </summary>
        /// <param name="breadCrumbs">The breadcrumbs that the mob has followed.</param>
        /// <param name="range">The range of the mob.</param>
        /// <returns>The node that is the closest to the player and within the range.</returns>
        protected virtual Node CheckRange(BreadCrumbs breadCrumbs, int range)
        {
            // Initialize the current node to the head of the breadcrumbs
            Node currentNode = breadCrumbs.GetHead();
            Vector2 crumbPos = currentNode.Data;

            // Set the mob to not be moving
            IsMoving = false;

            // Loop through the breadcrumbs until a node is within the range
            while (currentNode != null && currentNode.Next != null && Vector2.DistanceSquared(centerPosition, crumbPos) > Math.Pow(HostileRange, 2))
            {
                // Move to the next node
                currentNode = currentNode.Next;

                // Update the crumb position
                if (currentNode != null)
                {
                    crumbPos = currentNode.Data;
                }

                // If the current node is within the range, set the mob to moving and change the state to walk
                if (Vector2.DistanceSquared(centerPosition, crumbPos) <= Math.Pow(HostileRange, 2))
                {
                    IsMoving = true;
                    State = MobState.Walk;
                }
            }

            // If the current node is the head, set the mob to moving and change the state to walk
            if (currentNode == breadCrumbs.GetHead())
            {
                IsMoving = true;
                State = MobState.Walk;
            }

            // Return the current node
            return currentNode; // The current node will be the node that is the closest to the player and within the range
        }

        /// <summary>
        /// Follow the player by moving towards the breadcrumbs.
        /// </summary>
        /// <param name="wallRecs">The rectangle representing the walls in the game.</param>
        /// <param name="currentNode">The current node in the breadcrumbs.</param>
        protected virtual void FollowPlayer(Rectangle[] wallRecs, Node currentNode)
        {
            Vector2 crumbPos = currentNode.Data;

            // Loop through the breadcrumbs until the mob is not moving
            while (currentNode != null && Vector2.DistanceSquared(centerPosition, crumbPos) <= Math.Pow(HostileRange, 2))
            {
                // Set the mob to be moving
                IsMoving = true;

                // Calculate the movement velocity
                Vector2 delta = crumbPos - centerPosition;
                float distance = (float)Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);

                // If the distance is greater than 0, normalize the movement
                if (distance > 0)
                {
                    float normalizedDeltaX = delta.X / distance;
                    float normalizedDeltaY = delta.Y / distance;

                    // Set the velocity according to the movement direction and speed
                    Velocity = new Vector2((float)(normalizedDeltaX * Speed), (float)(normalizedDeltaY * Speed));
                }

                // Check if the distance is within the attack range
                if (distance <= AttackRange)
                {
                    IsAttack = true;
                }
                else
                {
                    IsAttack = false;
                }

                // Move the mob
                Move(wallRecs, Velocity.X, Velocity.Y);

                // Move to the previous node
                currentNode = currentNode.Prev;

                // Update the crumb position
                if (currentNode != null)
                {
                    crumbPos = currentNode.Data;
                }
            }

            // If the mob is not moving, set the state to idle
            if (!IsMoving)
            {
                State = MobState.Idle;
                Velocity = new Vector2(0, 0);
            }
        }

        /// <summary>
        /// Move the mob based on the velocity, and into its x and y directions.
        /// </summary>
        /// <param name="wallRecs">The rectangle representing the walls in the game.</param>
        /// <param name="x">The x velocity.</param>
        /// <param name="y">The y velocity.</param>
        protected override void Move(Rectangle[] wallRecs, float x, float y)
        {
            // check the x and y movement separately
            if (x != 0 && y != 0)
            {
                // move in the x and y directions separately, as it allows for smooth diagonal movement
                Move(wallRecs, 0, y);
                Move(wallRecs, x, 0);
                return;
            }

            // update the mob's position
            position += CheckWallCollision(wallRecs, x, y).Velocity;

            //DEBUG
            if (true) debugHitRange.TranslateTo((int)centerPosition.X, (int)centerPosition.Y);
        }

        /// <summary>
        /// Draws the mob on the screen.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw on.</param>
        /// <param name="isDebug">Whether or not to draw debug information.</param>
        public override void Draw(SpriteBatch spriteBatch, bool isDebug = false)
        {
            // Draw the health bar
            HealthBar.Draw(spriteBatch, new Vector2(0, 50));

            // Check if the frame image needs to be flipped
            if (direction == LEFT)
            {
                // Draw the frame image flipped horizontally if the mob is facing left
                spriteBatch.Draw(frames.images[frames.currentFrame], imgBox, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
            else
            {
                // Draw the frame image normally if the mob is not facing left
                spriteBatch.Draw(frames.images[frames.currentFrame], imgBox, Color.White);
            }

            // DEBUG
            if (isDebug)
            {
                // Draw the debug hit range if in debug mode
                debugHitRange.Draw(spriteBatch, Color.Red * 0.5f);

                // Draw the center position of the mob in debug mode
                spriteBatch.DrawString(Assets.debugFont, centerPosition.ToString(), centerPosition - new Vector2(10, 10), Color.White);
            }

            
        }
    }
}
