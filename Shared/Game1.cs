using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shared
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphicsDeviceManager.PreferredBackBufferWidth = 256 * 4;
            _graphicsDeviceManager.PreferredBackBufferHeight = 192 * 4;
            _graphicsDeviceManager.PreferredBackBufferFormat = SurfaceFormat.Color;
            _graphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.Depth24;
            _graphicsDeviceManager.HardwareModeSwitch = false;
            _graphicsDeviceManager.PreferMultiSampling = false;
            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.ApplyChanges();
            
            SharedSceneManager.Instance.RenderContext.GraphicsDevice = GraphicsDevice;
            
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            SharedSceneManager.Instance.Add(new DummyScene("dummy", this));
            
            SharedSceneManager.Instance.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // ReSharper disable once HeapView.ObjectAllocation.Evident
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            SharedSceneManager.Instance.RenderContext.SpriteBatch = _spriteBatch;
            
            SharedSceneManager.Instance.Stage("dummy");
            SharedSceneManager.Instance.Grab();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            SharedSceneManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SharedSceneManager.Instance.Draw();

            base.Draw(gameTime);
        }
    }
}
