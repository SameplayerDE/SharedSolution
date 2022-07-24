using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shared
{
    public class SharedSceneManager
    {
        
        public Dictionary<string, SharedScene> Resources = new Dictionary<string, SharedScene>();
        
        public static SharedSceneManager Instance { get; } = new SharedSceneManager();
        public SharedRenderContext RenderContext;
        
        public SharedScene Previous;
        public SharedScene Current;
        public SharedScene Next;
        
        public string PreviousKey;
        public string CurrentKey;
        public string NextKey;

        static SharedSceneManager()
        {
        }
        
        private SharedSceneManager()
        {
            RenderContext = new SharedRenderContext();
        }

        public void Initialize()
        {
            foreach (var scene in Resources.Values)
            {
                scene.Initialize();
            }
        }
        
        public bool Has(string name)
        {
            return Resources.ContainsKey(name);
        }
        
        public SharedScene Find(string name)
        {
            if (Has(name))
            {
                return Resources[name];
            }
            throw new NullReferenceException();
        }

        public void Add(SharedScene value, bool overwrite = false)
        {
            if (overwrite)
            {
                Resources[value.SceneName] = value;
            }
            else
            {
                Resources.Add(value.SceneName, value);
            }
        }
        
        public void Grab()
        {
            if (Next == null) return;
            Previous?.UnloadContent();
            if (Current != null)
            {
                Previous = Current;
                PreviousKey = CurrentKey;
            }

            CurrentKey = NextKey;
            Current = Next;
            Next = null;
            NextKey = string.Empty;
        }
        
        public void Stage(string key)
        {
            if (!Has(key)) return;
            if (NextKey == key) return;
            if (CurrentKey == key) return;
            var value = Find(key);
            if (PreviousKey != key)
            {
                Next?.UnloadContent();
                NextKey = key;
                Next = value;
                Next.LoadContent();
            }
            else
            {
                NextKey = PreviousKey;
                Next = Previous;
                Previous = null;
                PreviousKey = string.Empty;
            }
        }
        
        public void UnStage(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            Previous = value;
            Previous.UnloadContent();
        }
        
        public void Load(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            value.LoadContent();
        }
        
        public void UnLoad(string key)
        {
            if (!Has(key)) return;
            var value = Find(key);
            value.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            RenderContext.GameTime = gameTime;
            Current?.Update(gameTime);
        }

        public void Draw()
        {
            if (Current == null)
                return;
            
           RenderContext.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
           Current.Draw2D(RenderContext);
           RenderContext.SpriteBatch.End();
            
            var rasterizerState = new RasterizerState();
            rasterizerState.CullMode = CullMode.None;
            RenderContext.GraphicsDevice.RasterizerState = rasterizerState;
            RenderContext.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            Current.Draw3D(RenderContext);
            
            RenderContext.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
            Current.Draw2D(RenderContext, true);
            RenderContext.SpriteBatch.End();
            
        }
        
    }
}