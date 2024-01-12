using System;
using System.Collections.Generic;
using System.Linq;
using JTI.Scripts.Common;
using JTI.Scripts.Localization;
using JTI.Scripts.Localization.Data;
using TMPro;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class LocalizationManagerLocal : MonoBehaviour
    {
        public LocalizationData LocalizationData;

        public Dictionary<SystemLanguage, LocalizationLanguage> Dictionary { get; private set; }

        public SystemLanguage Language { get; private set; } = SystemLanguage.English;

        public bool IsInitialized;

        public LocalizationLanguage GetItem(SystemLanguage language)
        {
            if (!IsInitialized)
            {
                Initialize();
            }

            if (Dictionary.ContainsKey(language))
                return Dictionary[language];
            return Dictionary.FirstOrDefault().Value;
        }

        private void Initialize()
        {
            if(IsInitialized) return; 

            IsInitialized = true;
            Dictionary = new Dictionary<SystemLanguage, LocalizationLanguage>();

            for (var index = 0; index < LocalizationData.Values.Count; index++)
            {
                var localization = LocalizationData.Values[index];
                Dictionary.Add(localization.Language, localization);
            }

            if (PlayerPreferencesHelper.IsExist(PlayerPreferencesHelper.PlayerPreferencesType.Language))
            {
                try
                {
                    var lang = PlayerPreferencesHelper.Load<SystemLanguage>(PlayerPreferencesHelper.PlayerPreferencesType.Language);

                    var langData = LocalizationData.Values.Find(x => x.Language == lang);
                    if (langData != null)
                    {
                        ChangeLanguage(langData.Language, true);
                    }
                    else
                    {
                        ChangeLanguage(SystemLanguage.English, true);
                    }
                }
                catch (Exception e)
                {
                    ChangeLanguage(SetSystemLanguage());
                }

            }
            else
            {
                ChangeLanguage(SetSystemLanguage());
            }

        }

        protected void Awake()
        {
            Initialize();
        }

        public void ChangeLanguage(SystemLanguage language, bool force = false)
        {
            if (Language == language && !force)
                return;

            if (!Dictionary.ContainsKey(Language))
            {
                Debug.Log("Language not exist , so set default " + Language);
                Language = SystemLanguage.English;
            }

            Language = language;

            var allLocalizers = FindObjectsOfType<LocalizationTextUiLocalizer>();
            var allLocalizersRow = FindObjectsOfType<LocalizationTextUiLocalizerRow>();

            foreach (var textMeshLocalizer in allLocalizers)
            {
                textMeshLocalizer.Refresh();
            }
            foreach (var textMeshLocalizer in allLocalizersRow)
            {
                textMeshLocalizer.Refresh();
            }

            Language = PlayerPreferencesHelper.LoadDefault(PlayerPreferencesHelper.PlayerPreferencesType.Language, language);

        }
        public string GetLocale(string key)
        {
            return GetLocale(null, key);
        }

        public string GetLocale(TMP_Text title, string key, bool outline = true)
        {
            if(!IsInitialized)
                Initialize();

            var current = GetCurrent();

            if (current?.Font != null)
            {
                title.font = current.Font;

                try
                {
                    if (title?.material != null)
                    {
                        title.fontMaterial = outline ? current.PresetOutline : current.PresetNormal;
                    }

                }
                catch (Exception e)
                {
                }
            }


            if (string.IsNullOrEmpty(key))
            {
                title.text = "Invalid localization key";
                return "Invalid localization key";
            }

            key = key.ToLower();
            var text = "";


            try
            {
                var localizationString = GetItem(Language).Value.Find(l => l.Key.ToLower() == key);
                if (localizationString != null && !string.IsNullOrEmpty(localizationString.Text))
                {
                    text = localizationString.Text;
                    text = text.Replace("\\n", "\n");

                }
                else
                {
                  
                    var englishLoc = GetItem(SystemLanguage.English)?.Value.Find(l => l.Key.ToLower() == key);
                    if (Application.isEditor)
                    {
                        return string.Format("No localization for key: " + key);
                    }
                    else
                    {
                        Debug.LogError(Language + " No localization for key: " + key);
                        text = englishLoc != null ? englishLoc.Text : "[No localization]";
                        text = text.Replace("\\n", "\n");
                    }
                }
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError(e);
#endif
                // ignored
            }

            return text;
        }


        public string GetLocaleWithoutFont(string key)
        {
            if (!IsInitialized)
                Initialize();

            if (string.IsNullOrEmpty(key))
                return "Invalid localization key";

            key = key.ToLower();

            var localizationString = GetItem(Language).Value.Find(l => l.Key.ToLower() == key);

            if (localizationString == null)
                return string.Format("No localization for key: " + key);

            var text = localizationString.Text;
            text = text.Replace("\\n", "\n");
            return text;
        }

        public bool IsLocaleExist(string key)
        {
            if (!IsInitialized)
                Initialize();

            if (string.IsNullOrEmpty(key))
                return false;

            key = key.ToLower();
            var localizationString = GetItem(Language).Value.Find(l => l.Key.ToLower() == key);

            if (localizationString == null)
                return false;

            return true;
        }


        public SystemLanguage SetSystemLanguage()
        {
            var systemLanguage = Application.systemLanguage;

            switch (systemLanguage)
            {
                case SystemLanguage.Ukrainian:
                    systemLanguage = SystemLanguage.Russian;
                    break;
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    systemLanguage = SystemLanguage.Chinese;
                    break;
            }


            var langData = LocalizationData.Values.Find(x => x.Language == systemLanguage);
            if (langData != null)
            {
                return langData.Language;
            }

            return SystemLanguage.English;
        }

        public LocalizationLanguage GetCurrent()
        {
            if (!IsInitialized)
                Initialize();

            return LocalizationData.Values.Find(x => x.Language == Language);
        }
    }
}

