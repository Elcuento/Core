using JTI.Scripts.Managers;
using TMPro;
using UnityEngine;

namespace JTI.Scripts.Localization
{
    public class LocalizationTextUiLocalizer : MonoBehaviour
    {
        [SerializeField] private bool _upper;

        [SerializeField] private string _key;

        [SerializeField] private string _beforeText;

        [SerializeField] private string _afterText;

        [SerializeField] private TMP_Text _text;

        [SerializeField] private bool _outlined = false;

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
            if (LocalizationManager.Instance != null)
            {
                _text.text = $"{_beforeText}{LocalizationManager.Instance.GetLocale(_text, _key, _outlined)}{_afterText}";

                if (_upper)
                {
                    _text.text = _text.text.ToUpper();
                }
            }
        }

        public void SetUpper(bool a)
        {
            _upper = a;

            Refresh();
        }
        public void SetText(string key, string beforeText = "", string afterText = "")
        {
            _key = key;

            if (_upper)
            {
                _beforeText = beforeText.ToUpper();
                _afterText = afterText.ToUpper();
            }
            else
            {
                _beforeText = beforeText;
                _afterText = afterText;
            }

            Refresh();
        }
    }
}