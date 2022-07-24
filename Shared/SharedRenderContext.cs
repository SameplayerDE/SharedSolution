using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared
{
    public class SharedRenderContext
    {
        public SpriteBatch SpriteBatch;
        public GraphicsDevice GraphicsDevice;
        public SharedCamera Camera;
        
        public GameTime GameTime;
        public Matrix View;
        public Matrix Projection;
        public Vector3 Up;
        public Vector3 Right;
    }
}