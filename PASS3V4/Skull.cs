//Author: Colin Wang
//File Name: Skull.cs
//Project Name: PASS3 a dungeon crawler
//Created Date: June 4, 2024
//Modified Date: June 10, 2024
//Description: mob which follows you and (supposed to shoot you and attack you)

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PASS3V4
{
    public class Skull : Mob
    {
        // all diffrent skull movement states
        private const string FRONT_CLOSED = "FRONTCLOSED";
        private const string FRONT_OPEN = "FRONTOPEN";
        private const string FRONT_SIDE_CLOSED = "FRONTSIDECLOSED";
        private const string FRONT_SIDE_OPEN = "FRONTSIDEOPEN";
        private const string SIDE_CLOSED = "SIDECLOSED";
        private const string SIDE_OPEN = "SIDEOPEN";

        // skull properties
        private const int DAMAGE = 5;
        private const int HEALTH = 10;
        private const int SPEED = 2;
        private const int ATTACKDELAY = 1000;

        // skull ranges variables
        private const int HOSTILE_RANGE = 70;
        private const int ATTACK_RANGE = 150;

        // skull hurtbox variables
        private const int HURT_BOX_WIDTH = 45;
        private const int HURT_BOX_HEIGHT = 45;
        private const int HURT_BOX_OFFSET_X = 0;
        private const int HURT_BOX_OFFSET_Y = 0;

        // skull feet variables
        private const int FEET_WIDTH = 45;
        private const int FEET_HEIGHT = 45;
        private const int FEET_OFFSET_X = 0;
        private const int FEET_OFFSET_Y = 0;

        // frame variables
        private const int FRAME_WIDTH = HURT_BOX_WIDTH;
        private const int FRAME_HEIGHT = HURT_BOX_HEIGHT;

        // size of the frame
        private readonly Vector2 frameSize = new Vector2(FRAME_WIDTH, FRAME_HEIGHT);

        // path to the csv
        private const string FILEPATH = "Skull";

        // tempary fireball for attacking
        public FireBall TempFireBall { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Skull"/> class.
        /// </summary>
        /// <param name="content">The content manager.</param>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <param name="spawnPos">The spawn position.</param>
        /// <returns>The initialized <see cref="Skull"/> object.</returns>
        public Skull(ContentManager content, GraphicsDevice graphicsDevice, Vector2 spawnPos)
            : base(content, graphicsDevice, MobType.Skull, DAMAGE, HEALTH, SPEED, HOSTILE_RANGE, ATTACK_RANGE, FILEPATH, spawnPos)
        {
            // Set the position of the skull
            position = spawnPos;

            // Initialize the rectangles for the skull
            InitializeRectangles(
                HURT_BOX_WIDTH, HURT_BOX_HEIGHT, HURT_BOX_OFFSET_X, HURT_BOX_OFFSET_Y,
                FEET_WIDTH, FEET_HEIGHT, FEET_OFFSET_X, FEET_OFFSET_Y,
                frameSize);
        }

        /// <summary>
        /// Updates the state of the skull.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        /// <param name="wallRecs">The array of wall collision rectangles.</param>
        public override void Update(GameTime gameTime, Rectangle[] wallRecs = null)
        {
            // Update the health bar
            UpdateHealthBar();

            // Update the state of the skull
            switch (State)
            {
                case MobState.Spawning:
                    // Change the state to idle after spawning
                    State = MobState.Idle;
                    break;
                case MobState.Idle:
                    // Check the range between the skull and the player
                    CheckRange(Player.breadCrumbs, HostileRange);
                    // Update the frame of the skull
                    UpdateFrame();
                    break;
                case MobState.Walk:
                    // Update the movement of the skull
                    UpdateWalk(wallRecs);
                    break;
            }

            // If the skull is attacking
            if (IsAttack)
            {
                // Update the angle to the player
                UpdateAngleToPlayer(Player.GetPlayerCenterPosition());

                // Create a new fireball to attack the player
                CreateFireBall();
            }

        }

        /// <summary>
        /// create a fire ball
        /// </summary>
        private void CreateFireBall()
        {
            TempFireBall = new FireBall(graphicsDevice, FireBall.BASE_SPEED, AngleToPlayer, centerPosition, FireBall.BASE_DAMAGE);
        }

        /// <summary>
        /// get the fire ball
        /// </summary>
        /// <returns></returns>
        public FireBall GetFireBall()
        {
            if (TempFireBall == null) return null;

            FireBall temp = TempFireBall;
            TempFireBall = null;

            return temp;
        }

        /// <summary>
        /// update the walking of the skull
        /// </summary>
        /// <param name="wallRecs"></param>
        private void UpdateWalk(Rectangle[] wallRecs)
        {
            // follow the player, and check if the player is in the hostile range
            FollowPlayer(wallRecs, CheckRange(Player.breadCrumbs, HostileRange));

            // update the skulls rectangles and frame
            UpdateRecs(false);
            UpdateFrame();
        }

        /// <summary>
        /// update the frame image based on the skull movement
        /// </summary>
        private void UpdateFrame()
        {
            // update the current frame of the skull based on the angle
            // vertical movement

            if (Velocity.X >= 0) direction = RIGHT; // right
            else if (Velocity.X < 0) direction = LEFT; // left

            // update the frame of the mob based on the velocity and angle it makes with the player
            double tempAngle = MathHelper.PiOver2 - Math.Atan2(Math.Abs(Velocity.Y), Math.Abs(Velocity.X));

            if (tempAngle <= MathHelper.PiOver4 / 2 || Velocity.X == 0) // front movement
            {
                if (IsAttack) frames.SetCurrentFrame(FRONT_OPEN); else frames.SetCurrentFrame(FRONT_CLOSED);
            }
            else if (tempAngle <= MathHelper.PiOver4) // front side movement
            {
                if (IsAttack) frames.SetCurrentFrame(FRONT_SIDE_OPEN); else frames.SetCurrentFrame(FRONT_SIDE_CLOSED);
            }
            else if (tempAngle > MathHelper.PiOver4) // side movement
            {
                if (IsAttack) frames.SetCurrentFrame(SIDE_OPEN); else frames.SetCurrentFrame(SIDE_CLOSED);
            }
        }
    }
}
