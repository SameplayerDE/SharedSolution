using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Shared
{
    public class SharedObject3D : SharedObject
    {
        #region Stuff1

        public Vector3 LocalPosition;
        public Vector3 WorldPosition;

        public Quaternion LocalRotation;
        public Quaternion WorldRotation;

        public Vector3 LocalScale;
        public Vector3 WorldScale;
        
        public Matrix WorldMatrix;
        
        #endregion

        #region Stuff2

        public SharedObject3D Parent;
        public List<SharedObject3D> Children;

        #endregion

        #region Stuff3

        public SharedScene Scene
        {
            get => _scene ?? Parent?.Scene;
            set => _scene = value;
        }

        #endregion
        
        public SharedObject3D()
        {
            Children = new List<SharedObject3D>();
            LocalScale = WorldScale = Vector3.One;
        }

        public void AddChild(SharedObject3D child)
        {
            if (Children.Contains(child)) return;
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(SharedObject3D child)
        {
            if (Children.Remove(child))
            {
                child.Parent = null;
            }
        }
        
        public void Translate(Vector3 translation)
        {
            LocalPosition = translation;
        }

        public void Translate(float x, float y, float z)
        {
            LocalPosition = new Vector3(x, y, z);
        }

        public void Scale(Vector3 scale)
        {
            LocalScale = scale;
        }

        public void Scale(float x, float y, float z)
        {
            LocalScale = new Vector3(x, y, z);
        }
        
        public void Rotate(Vector3 rotation, RotationType type = RotationType.Degree)
        {
            var (x, y, z) = rotation;
            Rotate(x, y, z, type);
        }
        
        public void Rotate(float x, float y, float z, RotationType type = RotationType.Degree)
        {
            if (type == RotationType.Degree)
            {
                x = MathHelper.ToRadians(x);
                y = MathHelper.ToRadians(y);
                z  = MathHelper.ToRadians(z);
            }
            
            var nRotation =
                Matrix.CreateRotationX(x) *
                Matrix.CreateRotationY(y) *
                Matrix.CreateRotationZ(z);
            LocalRotation = Quaternion.CreateFromRotationMatrix(nRotation);
        }
        
        public void Rotate(Quaternion rotation)
        {
            LocalRotation = rotation;
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
            WorldMatrix = Matrix.CreateFromQuaternion(LocalRotation) *
                          Matrix.CreateScale(LocalScale) *
                          Matrix.CreateTranslation(LocalPosition);
            
            if (Parent != null)
            {
                WorldMatrix = Matrix.Multiply(WorldMatrix, Parent.WorldMatrix);
                
                if (!WorldMatrix.Decompose(out var scale, out var rotation, out var position))
                    Debug.WriteLine("Object3D Decompose World Matrix FAILED!");

                WorldPosition = position;
                WorldScale = scale;
                WorldRotation = rotation;
            }
            else
            {
                WorldPosition = LocalPosition;
                WorldScale = LocalScale;
                WorldRotation = LocalRotation;
            }

            Children.ForEach(child => child.Update(gameTime));
        }

        public virtual void Draw(SharedRenderContext renderContext)
        {
            Children.ForEach(child => child.Draw(renderContext));
        }
        
    }
}