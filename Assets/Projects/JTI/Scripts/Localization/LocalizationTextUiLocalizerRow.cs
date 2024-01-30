using System;
using JTI.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace JTI.Scripts.Localization
{
    public class LocalizationTextUiLocalizerRow : MonoBehaviour
    {
        [System.Serializable]
        private class Loc
        {
            public string Key;
            public string After;
            public string Before;
            public bool NoLocalizate;
        }
        [SerializeField] private Loc[] _keys = Array.Empty<Loc>();

        [SerializeField] private TMP_Text _text;

        [SerializeField] private bool _outlined = true;

        private void OnEnable()
        {
            Refresh();
        }

        private void OnValidate()
        {
            _text = GetComponent<TMP_Text>();
        }

        public void Refresh()
        {

            if (LocalizationManager.Instance != null && _keys != null)
            {
                _text.text = "";

                foreach (var a in _keys)
                {
                    if (a.NoLocalizate)
                    {
                        _text.text += " " +
                                      $"{a.Before}{a.Key}{a.After}";
                    }
                    else
                    {
                        _text.text += " " +
                                      $"{a.Before}{LocalizationManager.Instance.GetLocale(_text, a.Key, _outlined)}{a.After}";
                    }
                }

            }
        }

    }
}