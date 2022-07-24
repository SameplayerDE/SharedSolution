using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Shared
{
    public class SharedSprite3D : SharedObject3D
    {
        public VertexBuffer VertexBuffer; //VertexBuffer
        public IndexBuffer IndexBuffer; //IndexBuffer
        
        public Vector2 Size;
        public Texture2D Texture2D;
        public Effect Effect;
        public Vector2 Tiling = Vector2.One;
        public bool EnsureOcclusion = true;
        
        public string TextureFile;
        public string EffectFile;
        
        private VertexPositionTexture[] _vertices;
        private int[] _indices;
        
        public SharedSprite3D(string textureFile, string effectFile, Vector2 size)
        {
            Size = size;
            TextureFile = textureFile;
            EffectFile = effectFile;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            Texture2D = contentManager.Load<Texture2D>(TextureFile);
            Effect = contentManager.Load<Effect>(EffectFile);
            base.LoadContent(contentManager);
        }
        
        public override void Initialize()
        {
            GenerateGeometry();
            base.Initialize();
        }
        
        private void GenerateGeometry()
        {
            // Create vertex and index arrays
            _vertices = new VertexPositionTexture[4];
            _indices = new int[6];
            var x = 0;
            // For each billboard...
            for (var i = 0; i < 4; i += 4)
            {
                // Add 4 vertices at the billboard's position
                _vertices[i + 0] = new VertexPositionTexture(new Vector3(-0.5f * Size.X, +0.5f * Size.Y, 0.0f),
                    new Vector2(0, 0) * Tiling);
                _vertices[i + 1] = new VertexPositionTexture(new Vector3(-0.5f * Size.X, -0.5f * Size.Y, 0.0f),
                    new Vector2(0, 1) * Tiling);
                _vertices[i + 2] = new VertexPositionTexture(new Vector3(+0.5f * Size.X, -0.5f * Size.Y, 0.0f),
                    new Vector2(1, 1) * Tiling);
                _vertices[i + 3] = new VertexPositionTexture(new Vector3(+0.5f * Size.X, +0.5f * Size.Y, 0.0f),
                    new Vector2(1, 0) * Tiling);
                // Add 6 indices to form two triangles
                _indices[x++] = i + 0;
                _indices[x++] = i + 3;
                _indices[x++] = i + 2;
                _indices[x++] = i + 2;
                _indices[x++] = i + 1;
                _indices[x++] = i + 0;
            }

            // Create and set the vertex buffer
            VertexBuffer = new VertexBuffer(Scene.Game.GraphicsDevice,
                typeof(VertexPositionTexture),
                4, BufferUsage.WriteOnly);
            VertexBuffer.SetData(_vertices);
            // Create and set the index buffer
            IndexBuffer = new IndexBuffer(Scene.Game.GraphicsDevice,
                IndexElementSize.ThirtyTwoBits,
                6, BufferUsage.WriteOnly);
            IndexBuffer.SetData(_indices);
        }
        
        private void DrawSprite(SharedRenderContext renderContext)
        {
            Effect.CurrentTechnique.Passes[0].Apply();
            renderContext.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
        
        private void DrawOpaquePixels(SharedRenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Effect.Parameters["AlphaTest"]?.SetValue(true);
            Effect.Parameters["AlphaTestGreater"]?.SetValue(true);
            DrawSprite(renderContext);
        }

        private void DrawTransparentPixels(SharedRenderContext renderContext)
        {
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            Effect.Parameters["AlphaTest"]?.SetValue(true);
            Effect.Parameters["AlphaTestGreater"]?.SetValue(false);
            DrawSprite(renderContext);
        }

        public override void Draw(SharedRenderContext renderContext)
        {
            /*var samplerState = new SamplerState();
            samplerState.AddressU = U;
            samplerState.AddressV = V;
            renderContext.GraphicsDevice.SamplerStates[0] = samplerState;*/
            // Set the vertex and index buffer to the graphics card
            renderContext.GraphicsDevice.SetVertexBuffer(VertexBuffer);
            renderContext.GraphicsDevice.Indices = IndexBuffer;
            renderContext.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            Effect.Parameters["Texture"]?.SetValue(Texture2D);
            Effect.Parameters["World"]?.SetValue(WorldMatrix);
            Effect.Parameters["View"]?.SetValue(renderContext.Camera.View);
            Effect.Parameters["Projection"]?.SetValue(renderContext.Camera.Projection);
            if (EnsureOcclusion)
            {
                DrawOpaquePixels(renderContext);
                DrawTransparentPixels(renderContext);
            }
            else
            {
                renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                Effect.Parameters["AlphaTest"]?.SetValue(false);
                DrawSprite(renderContext);
            }

            // Reset render states
            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;
            renderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // Un-set the vertex and index buffer
            renderContext.GraphicsDevice.SetVertexBuffer(null);
            renderContext.GraphicsDevice.Indices = null;
            base.Draw(renderContext);
        }
    }
}