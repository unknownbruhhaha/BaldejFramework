using GameEngineAPI.Components;

namespace GameEngineAPI
{
    public class Scene
    {
        public List<GameObject> gameObjects = new List<GameObject>();
        
        public GameObject NewGameObject(String Name, String Preset = "Base")
        {
            GameObject gameObject = new GameObject(Name, Preset);
            gameObject.parrent = null;
            gameObjects.Add(gameObject);
            return gameObject;
        }
    }
}
