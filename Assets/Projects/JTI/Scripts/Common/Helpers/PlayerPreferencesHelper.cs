using System;
using UnityEngine;

namespace JTI.Scripts.Common
{
    public static class PlayerPreferencesHelper
    {
        public enum PlayerPreferencesType
        {
            SoundVolume,
            MusicVolume,
            Language
        }

        public static void Save(PlayerPreferencesType type, object val)
        {
            PlayerPrefs.SetString(type.ToString(), val.ToJson());
            PlayerPrefs.Save();
        }

        public static void Change(PlayerPreferencesType type, int v, int def)
        {
            try
            {
                Save(type, LoadDefault<int>(type, def) + v);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void Change(PlayerPreferencesType type, float v, float def)
        {
            try
            {
                Save(type, LoadDefault<float>(type, def) + v);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void Remove(PlayerPreferencesType type)
        {
            var key = type.ToString();

            if (!PlayerPrefs.HasKey(key))
                return;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public static T Load<T>(PlayerPreferencesType type)
        {
            var json = PlayerPrefs.GetString(type.ToString(), "");

            try
            {
                if (!string.IsNullOrEmpty(json))
                    return json.FromJson<T>();
            }
            catch (Exception e)
            {
                return default(T);
            }


            return default(T);
        }

        public static T LoadDefault<T>(PlayerPreferencesType type, T def)
        {
            var json = PlayerPrefs.GetString(type.ToString(), "");

            try
            {
                if (!string.IsNullOrEmpty(json))
                    return json.FromJson<T>();
            }
            catch (Exception e)
            {
                return def;
            }


            return def;
        }

        public static bool IsExist(PlayerPreferencesType type)
        {
            return PlayerPrefs.HasKey(type.ToString());
        }

        public static void Clear()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}

