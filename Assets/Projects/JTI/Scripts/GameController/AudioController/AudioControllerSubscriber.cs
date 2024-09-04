using System;
using JTI.Scripts.Events;
using JTI.Scripts.Events.Game;
using JTI.Scripts.GameControllers;
using JTI.Scripts.Managers;
using UnityEngine;


namespace JTI.Scripts.GameControllers
{
    public class AudioControllerSubscriber : MonoBehaviour
    {
        public AudioSource AudioSource => _audioSource;

        [SerializeField] private AudioClip _clip;
        [SerializeField] private int _group = -1;

        private AudioSource _audioSource;
        private AudioController _audioController;
        private EventSubscriberLocal<GameEvent> _eventSubscriberLocal;
        private float _maxVolume = 1f;
        private bool _started;
        private string _id;

        private void Awake()
        {
            if (!gameObject.TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
                _audioSource.spatialBlend = 1f;
            }

            AudioSource.clip = _clip;
            _maxVolume = AudioSource.volume;
        }

        private void Start()
        {
            _started = true;

            _audioController = GameManager.Instance.GetController<AudioController>();
            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(GameManager.Instance.GameEvents);
            _eventSubscriberLocal.Subscribe<ChangeSoundSettingsEvent>(OnChangeSoundSettings);

            ChangeSoundSettings();
            if (AudioSource.playOnAwake) AudioSource.Play();
        }

        public void Init(string id, int group = -1)
        {
            if (!_started) Start();

            _id = id;
            _clip = _clip == null ? _audioController.GetClip(id) : _clip;
            AudioSource.clip = _clip;
            _group = group;
        }

        private void OnChangeSoundSettings(ChangeSoundSettingsEvent a)
        {
            ChangeSoundSettings();
        }

        private void ChangeSoundSettings()
        {
            var data = _audioController.GetAudioData(_id);
            var volume1 = data ? data.Volume : 1;
            var volume2 = _audioController.GetVolumeByGroup(_group);
            AudioSource.volume = _maxVolume * volume1 * volume2;
        }
    }
}