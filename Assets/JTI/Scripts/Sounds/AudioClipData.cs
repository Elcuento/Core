using UnityEngine;

namespace JTI.Scripts.Localization.Data
{
    [CreateAssetMenu(fileName = "AudioClipData", menuName = "JTI/Audio/AudioClipData", order = 3)]
    public class AudioClipData : ScriptableObject
    {
        [SerializeField] private string _clipId;
        [SerializeField] private float _volume = 1;
        [SerializeField] private float _range = -1;

        public float Range => _range;
        public float Volume => _volume;
        public string ClipId => string.IsNullOrEmpty(_clipId) ? name : _clipId;

    }
}
