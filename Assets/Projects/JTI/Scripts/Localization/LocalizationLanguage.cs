using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JTI.Scripts.Localization
{
    [CreateAssetMenu(fileName = "Language", menuName = "JTI/Localization/Language", order = 1)]
    public class LocalizationLanguage : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private Material _presetNormal;
        [SerializeField] private Material _presetOutline;
        [SerializeField] private SystemLanguage _language;
        [SerializeField] private List<LocalizationString> _values;

        public TMP_FontAsset Font => _font;
        public Material PresetNormal => _presetNormal;
        public Material PresetOutline => _presetOutline;
        public SystemLanguage Language => _language;
        public List<LocalizationString> Value => _values;

        public void Clear()
        {
            Value.Clear();
        }

        public void SetValue(string key, string val)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is empty, val " + val);
                return;
            }

            Value.Add(new LocalizationString(key.ToLower(), val));
        }

        public LocalizationLanguage()
        {
            _language = SystemLanguage.English;
            _values = new List<LocalizationString>();
        }
    }
}