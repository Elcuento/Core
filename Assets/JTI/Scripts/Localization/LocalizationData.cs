using System;
using System.Collections.Generic;
using JTI.Scripts.Common;
using UnityEditor;
using UnityEngine;

namespace JTI.Scripts.Localization.Data
{
    [CreateAssetMenu(fileName = "Localization", menuName = "JTI/Localization/Localization", order = 3)]
    public class LocalizationData : ScriptableObject
    {
        [SerializeField] private TextAsset _languageData;

        public List<LocalizationLanguage> Values;


        public void UpdateData()
        {
#if UNITY_EDITOR


            var languages = new Dictionary<SystemLanguage, LocalizationLanguage>();

            foreach (var localizationLanguage in Values)
            {
                languages.Add(localizationLanguage.Language, localizationLanguage);
                localizationLanguage.Clear();
            }

            Debug.Log("Load localization data");

            try
            {
                var data = _languageData.text.FromJson<List<Dictionary<string, string>>>();

                Debug.Log("Deserialization Completed");

                for (var index = 0; index < data.Count; index++)
                {
                    var line = data[index];

                    var key = "";
                    var first = false;
                    foreach (var keyIn in line)
                    {
                        if (!first)
                        {
                            key = keyIn.Value;
                            first = true;
                            continue;
                        }

                        if (Enum.TryParse(keyIn.Key, out SystemLanguage s))
                        {
                            if (languages.ContainsKey(s))
                            {
                                languages[s].SetValue(key, keyIn.Value);
                            }
                        }
                        else
                        {
                            Debug.LogError("Unknown language " + keyIn.Key);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("cant parse language data " + e);
                throw;
            }

            foreach (var localizationLanguage in Values)
            {
                EditorUtility.SetDirty(localizationLanguage);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}
