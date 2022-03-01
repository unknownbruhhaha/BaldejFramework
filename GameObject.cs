using GameEngineAPI.Components;

namespace GameEngineAPI
{
    public class GameObject
    {
        public String Name;
        public List<Component> components = new List<Component>();
        public GameObject? parrent;

        public GameObject(string name, string preset = "")
        {
            Name = name;
            if (preset == "3D")
            {
                components.Add(new Transform3D());
            }
            SceneManager.currentScene.gameObjects.Add(this);
        }

        public void AddComponent(Component component)
        {
            component.owner = this;
            components.Add(component);
        }

        public void RemoveComponent(Type type)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].GetType() == type)
                {
                    components.RemoveAt(i);
                }
            }
        }

        public Component? GetComponent(string ID)
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i].componentID == ID)
                {
                    return components[i];
                }
            }

            return null;
        }
    }
}
