
namespace JTI.Scripts.Localization
{
[System.Serializable]
    public class LocalizationString
    {
        public string Key;

        public string Text;

        public LocalizationString(string key, string text)
        {
            Key = key.ToLower();
            Text = text;
        }
    }
}