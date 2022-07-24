using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shared
{
    public class DummyScene : SharedScene
    {

        private SharedCamera _camera;
        
        private SharedSprite3D _hero;
        private SharedSprite3D _other;
        
        public DummyScene(string name, Game game) : base(name, game)
        {
        }

        public override void Initialize()
        {

            _camera = new SharedCamera();
            _camera.Translate(0, 0, 1);

            _hero = new SharedSprite3D("tree", "BasicEffect", Vector2.One);
            _hero.Translate(0, 0, 0.5f);
            _hero.EnsureOcclusion = true;
            
            _other = new SharedSprite3D("tree", "BasicEffect", Vector2.One);
            _other.EnsureOcclusion = true;
            
            AddSceneObject(_hero);
            AddSceneObject(_other);
            AddSceneObject(_camera);

            SharedSceneManager.Instance.RenderContext.Camera = _camera;
            
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var direction = Vector3.Zero;
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (keyboardState.IsKeyDown(Keys.A))
            {
                direction.X -= 1f * delta;
            }
            
            if (keyboardState.IsKeyDown(Keys.D))
            {
                direction.X += 1f * delta;
            }
            
            if (keyboardState.IsKeyDown(Keys.W))
            {
                direction.Z -= 1f * delta;
            }
            
            if (keyboardState.IsKeyDown(Keys.S))
            {
                direction.Z += 1f * delta;
            }

            if (direction != Vector3.Zero)
            {
                var translation = _hero.LocalPosition + direction;
                _hero.Translate(translation);
            }

            base.Update(gameTime);
        }
    }
}