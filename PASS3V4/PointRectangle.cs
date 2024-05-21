using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace PASS3V4
{
    internal class PointRectangle
    {
        public Vector2[] Points { get; set; }


        public PointRectangle(Vector2[] points)
        {
            Points = points;
        }

        public void Rotate(float centerX, float centerY, float angle, float width, float height)
        {
            // Calculate the new coordinates of the rectangle's vertices after rotation
            float newX1 = (float)(centerX + (width / 2) * Math.Cos(angle) - (height / 2) * Math.Sin(angle));
            float newY1 = (float)(centerY + (width / 2) * Math.Sin(angle) + (height / 2) * Math.Cos(angle));

            float newX2 = (float)(centerX - (width / 2) * Math.Cos(angle) - (height / 2) * Math.Sin(angle));
            float newY2 = (float)(centerY - (width / 2) * Math.Sin(angle) + (height / 2) * Math.Cos(angle));

            float newX3 = (float)(centerX - (width / 2) * Math.Cos(angle) + (height / 2) * Math.Sin(angle));
            float newY3 = (float)(centerY - (width / 2) * Math.Sin(angle) - (height / 2) * Math.Cos(angle));

            float newX4 = (float)(centerX + (width / 2) * Math.Cos(angle) + (height / 2) * Math.Sin(angle));
            float newY4 = (float)(centerY + (width / 2) * Math.Sin(angle) - (height / 2) * Math.Cos(angle));

            // Set the new coordinates of the rectangle's vertices
            Points[0] = new Vector2(newX1, newY1);
            Points[1] = new Vector2(newX2, newY2);
            Points[2] = new Vector2(newX3, newY3);
            Points[3] = new Vector2(newX4, newY4);    
        }

        public void DrawRectangle(GraphicsDevice graphicsDevice)
        {
            // Create a BasicEffect instance
            BasicEffect basicEffect = new BasicEffect(graphicsDevice);
            basicEffect.VertexColorEnabled = true;

            // Set the world, view, and projection matrices
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreateOrthographic(graphicsDevice..Width, GraphicsDevice.Viewport.Height, 0.0f, 1.0f);

            // Draw the rectangle
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, Points, 0, 2);
            }
        }

    }
}
