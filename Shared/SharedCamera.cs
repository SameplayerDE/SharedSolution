using Microsoft.Xna.Framework;

namespace Shared
{
    public class SharedCamera : SharedObject3D
    {
        public Matrix View;
        public Matrix Projection;
        
        public float NearPlane = 0.1f;
        public float FarPlane = 10f;
        
        public SharedCamera()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(100f), 256f / 192f, NearPlane, FarPlane);
        }

        public virtual void BuildViewMatrix()
        {
            var lookAt = Vector3.Transform(Vector3.Forward, WorldRotation);
            lookAt.Normalize();

            View = Matrix.CreateLookAt(WorldPosition, WorldPosition + lookAt, Vector3.Up);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            BuildViewMatrix();
        }
    }
}