using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GameUtility;

namespace PASS3V4
{
    public class Weapon
    {
        protected const int WEAPON_SET_IMG_WIDTH = 4;

        protected float rotationSpeed;

        protected int weaponID;


        protected Texture2D img;
        protected Rectangle imgSourceRec; // source rectangle used to crop the img set
        protected Rectangle sourceRec; // source rectangle used to rotate the img

        protected Vector2 position; // should start from the center of the player
        protected Vector2 origin;
        protected float angle; // angle should be in radian

        protected Rectangle hitBox;
        protected int hitBoxWidth;
        protected int hitBoxHeight;
        protected Vector2 hitBoxOffset;
        
        protected GameRectangle degbugHitBox;

        public Weapon(GraphicsDevice graphicsDevice, Texture2D weaponSetImg, Rectangle imgSourceRec, Vector2 position, Vector2 origin, float rotationSpeed)
        {
            this.imgSourceRec = imgSourceRec;
            this.position = position;
            this.origin = origin;
            this.rotationSpeed = rotationSpeed;

            sourceRec = new Rectangle(0, 0, imgSourceRec.Width, imgSourceRec.Height);

            SetWeaponImg(graphicsDevice, weaponSetImg);
        }

        protected void InitializeHitBox(GraphicsDevice graphicsDevice, int hitBoxWidth, int hitBoxHeight, Vector2 hitBoxOffset)
        {
            this.hitBoxWidth = hitBoxWidth;
            this.hitBoxHeight = hitBoxHeight;
            this.hitBoxOffset = hitBoxOffset;

            hitBox = new Rectangle((int)(hitBoxOffset.X + position.X), (int)(hitBoxOffset.Y + position.Y), hitBoxWidth, hitBoxHeight);
            degbugHitBox = new GameRectangle(graphicsDevice, hitBox);
        }
       

        /// <summary>
        /// set the weapon images from the larger weapon image set
        /// </summary>
        /// <param name="graphicsDevice"></param>
        /// <param name="weaponSetImg"></param>
        public void SetWeaponImg(GraphicsDevice graphicsDevice, Texture2D weaponSetImg)
        {
            // set the weapon image
            img = new Texture2D(graphicsDevice, imgSourceRec.Width, imgSourceRec.Height);
            Color[] data = new Color[imgSourceRec.Width * imgSourceRec.Height];
            weaponSetImg.GetData(0, imgSourceRec, data, 0, data.Length);
            img.SetData(data);
        }

        /// <summary>
        /// update the weapon position, angle and hitbox
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="position"></param>
        public virtual void Update(GameTime gameTime, Vector2 position)
        {
            // update the angle 
            angle += rotationSpeed;
            
            // update the position of the weapon
            this.position = position;
            

            // update the hitbox of the weapon


        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(img, rec, Color.White);
            spriteBatch.Draw(img, position, sourceRec, Color.White, angle, origin, 1, SpriteEffects.None, 0);
        }

    }
}
