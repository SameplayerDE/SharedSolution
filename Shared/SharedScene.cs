using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Shared
{
    public class SharedScene
    {
        public string SceneName { get; private set; }
        protected ContentManager Content;
        public Game Game;
        public List<SharedObject2D> SceneObjects2D { get; private set; }
        public List<SharedObject3D> SceneObjects3D { get; private set; }

        public SharedScene(string name, Game game)
        {
            SceneName = name;
            Game = game;
            SceneObjects2D = new List<SharedObject2D>();
            SceneObjects3D = new List<SharedObject3D>();
            Content = new ContentManager(Game.Services, "Content");
        }

        public override bool Equals(object obj)
        {
            if (obj is SharedScene)
            {
                return SceneName.Equals((obj as SharedScene).SceneName);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AddSceneObject(SharedObject2D sceneObject)
        {
            if (!SceneObjects2D.Contains(sceneObject))
            {
                sceneObject.Scene = this;
                SceneObjects2D.Add(sceneObject);
            }
        }

        public void RemoveSceneObject(SharedObject2D sceneObject)
        {
            if (SceneObjects2D.Remove(sceneObject))
            {
                sceneObject.Scene = null;
            }
        }
        
        public void AddSceneObject(SharedObject3D sceneObject)
        {
            if (SceneObjects3D.Contains(sceneObject))
                return;
            sceneObject.Scene = this;
            SceneObjects3D.Add(sceneObject);
        }

        public void RemoveSceneObject(SharedObject3D sceneObject)
        {
            if (!SceneObjects3D.Remove(sceneObject))
            {
                return;
            }
            sceneObject.Scene = null;
        }

        public virtual void Activated(){}
        public virtual void Deactivated(){}

        public virtual void Initialize()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.Initialize());
            SceneObjects3D.ForEach(sceneObject => sceneObject.Initialize());
        }

        public virtual void LoadContent()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.LoadContent(Content));
            SceneObjects3D.ForEach(sceneObject => sceneObject.LoadContent(Content));
        }
        
        public virtual void UnloadContent()
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.UnloadContent());
            SceneObjects3D.ForEach(sceneObject => sceneObject.UnloadContent());
        }

        public virtual void Update(GameTime gameTime)
        {
            SceneObjects2D.ForEach(sceneObject => sceneObject.Update(gameTime));
            SceneObjects3D.ForEach(sceneObject => sceneObject.Update(gameTime));
        }

        public virtual void Draw2D(SharedRenderContext renderContext, bool drawInFrontOf3D = false)
        {
            SceneObjects2D.ForEach(obj =>
            {
                if (obj.DrawInFrontOf3D == drawInFrontOf3D)
                    obj.Draw(renderContext);
            });
        }
        
        public virtual void Draw3D(SharedRenderContext renderContext)
        {
            SceneObjects3D.ForEach((Action<SharedObject3D>)(sceneObject =>
                sceneObject.Draw(renderContext)));
        }
    }
}