namespace GameEngineAPI.Assets
{
    public interface Asset
    {
        public string AssetType { get; }

        public string AssetShortName { get; set; }
    }
}
