using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Shared
{
    public class SharedObject2D : SharedObject
    {
        #region Stuff1

        public Vector2 LocalPosition;
        public Vector2 WorldPosition { get; private set; }
        
        public Vector2 LocalScale;
        public Vector2 WorldScale { get; private set; }
        
        public float LocalRotation;
        public float WorldRotation { get; private set; }
        
        public Vector2 PivotPoint;
        
        public Matrix WorldMatrix;

        #endregion

        #region Stuff2

        public SharedObject2D Parent;

        public List<SharedObject2D> Children;

        #endregion
        
        #region Stuff3

        public bool CanDraw;

        public bool DrawInFrontOf3D;

        #endregion

        #region Stuff4

        public SharedScene Scene
        {
            get => _scene ?? Parent?.Scene;
            set => _scene = value;
        }

        #endregion
        
        public SharedObject2D()
        {
            LocalScale = WorldScale = Vector2.One;
            Children = new List<SharedObject2D>();

            DrawInFrontOf3D = true;
            CanDraw = true;
        }

        public void AddChild(SharedObject2D child)
        {
            if (Children.Contains(child)) return;
            Children.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(SharedObject2D child)
        {
            if (Children.Remove(child))
                child.Parent = null;
        }

        public void Rotate(float rotation)
        {
            LocalRotation = rotation;
        }

        public void Translate(float posX, float posY)
        {
            Translate(new Vector2(posX, posY));
        }

        public void Translate(Vector2 position)
        {
            LocalPosition = position;
        }

        public void Scale(float scaleX, float scaleY)
        {
            Scale(new Vector2(scaleX, scaleY));
        }

        public void Scale(Vector2 scale)
        {
            LocalScale = scale;
        }

        public virtual void Initialize()
        {
            Children.ForEach(child => child.Initialize());
        }

        public virtual void LoadContent(ContentManager contentManager)
        {
            Children.ForEach(child => child.LoadContent(contentManager));
        }

        public virtual void UnloadContent()
        {
            Children.ForEach(child => child.UnloadContent());
        }
        
        public virtual void Update(GameTime gameTime)
        {
            WorldMatrix =
                Matrix.CreateTranslation(new Vector3(-PivotPoint, 0)) *
                Matrix.CreateScale(new Vector3(LocalScale, 1)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(LocalRotation)) *
                Matrix.CreateTranslation(new Vector3(LocalPosition, 0));

            if (Parent != null)
            {
                WorldMatrix = Matrix.Multiply(WorldMatrix, Matrix.CreateTranslation(new Vector3(Parent.PivotPoint, 0)));
                WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);
            }
            
            if (!WorldMatrix.Decompose(out var scale, out var rot, out var pos))
            {
                Debug.WriteLine("Object2D Decompose World Matrix FAILED!");
            }

            var (x, y) = Vector2.Transform(Vector2.UnitX, rot);
            WorldRotation = (float)Math.Atan2(y, x);
            WorldRotation = float.IsNaN(WorldRotation) ? 0 : MathHelper.ToDegrees(WorldRotation);

            WorldPosition = new Vector2(pos.X, pos.Y);
            WorldScale = new Vector2(scale.X, scale.Y);

            Children.ForEach(child => child.Update(gameTime));
        }
        
        public virtual void Draw(SharedRenderContext renderContext)
        {
            if (CanDraw)
                Children.ForEach(child => child.Draw(renderContext));
        }
        
    }
}