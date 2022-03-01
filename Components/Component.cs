namespace GameEngineAPI.Components
{
    public interface Component
    {
        public string componentID { get; }

        public GameObject? owner { set; }


        public void OnUpdate();
    }
}
