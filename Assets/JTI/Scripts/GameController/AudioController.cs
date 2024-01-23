using System;
using System.Collections;
using System.Collections.Generic;
using Assets.JTI.Scripts.Events.Game;
using JTI.Scripts.Common;
using JTI.Scripts.Events;
using JTI.Scripts.Localization.Data;
using JTI.Scripts.Managers;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JTI.Scripts.GameControllers
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioController : GameController
    {
        [System.Serializable]
        public class AudioControllerSettings : GameControllerSettings
        {
            public AudioData Data;

            public bool PreLoad;
            public Dictionary<int, float> GroupVolume;
            public float VolumeMultiply;

            public AudioControllerSettings(AudioData d)
            {
                Data = d;
                GroupVolume = new Dictionary<int, float>()
                {
                    { -1, 1 }
                };
            }
        }

        [System.Serializable]
        public class AudioTrack
        {
            public int Group { get; private set; }
            public string Id { get; private set; }
            public AudioSource Source { get; private set; }
            public bool IsDestroyed => Source == null || _isDestroyed;
            public bool IsPlaying => Source != null && Source.isPlaying;
            public float Multiply { get; private set; }
            public bool DoNotDestroy { get; private set; }
            public AudioClipData Data { get; private set; }

            public AudioController Controller { get; private set; }

            private bool _isDestroyed;
            public void PlayOneShot(AudioClip clip, float volumeMultiply = 1)
            {
                if (IsDestroyed) return;

                Source.PlayOneShot(clip, volumeMultiply);
            }
            public AudioTrack(AudioController c, string id, AudioSource source, AudioClipData data, int group = 1, bool doNotDestroy = false, float multiply = 1)
            {
                Group = group;
                Id = id;
                Source = source;
                Controller = c;
                DoNotDestroy = doNotDestroy;
                Multiply = multiply;
                Data = data;
            }

            internal void DestroyInternal()
            {
                if (!IsDestroyed) return;

                if (Source != null)
                    Object.Destroy(Source);

                _isDestroyed = true;
            }
            public void Destroy()
            {
                if (!IsDestroyed) return;

                Controller.RemoveTrack(this);
            }

            public void RefreshVolume(float getVolumeByGroup)
            {
                if (IsDestroyed) return;

                Source.volume = getVolumeByGroup * Multiply * (Data?.Volume ?? 1);
            }
        }

        private Dictionary<string, AudioClip> _catch;

        private Coroutine _sequenceCoroutine;

        private List<AudioTrack> _trackLists;

        private AudioControllerSettings _settings;

        private EventSubscriberLocal<GameEvent> _eventSubscriberLocal;

        private AudioTrack _commonAudioTrack;


        public AudioController(AudioControllerSettings settings) : base(settings)
        {
            _settings = settings;

            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(GameManager.Instance.GameEvents);

            if (_settings == null || _settings.Data == null)
            {
                Debug.LogError("No audio data was set !");
                return;
            }
        }
        protected override void CreateWrapper()
        {
            View = new GameObject(nameof(AudioControllerView))
                .AddComponent<AudioControllerView>();
        }

        protected override void OnInitialize()
        {
            _commonAudioTrack = CreateTrack("AudioSource_" + Guid.NewGuid(),-1,true);

            _trackLists = new List<AudioTrack>();
            _catch = new Dictionary<string, AudioClip>();

            Subscribe();

            PreLoad();
            UpdateSettings();
        }

        private void PreLoad()
        {
            View.StartCoroutine(_loadAsync());
        }

        private IEnumerator _loadAsync()
        {
            yield return null;

             var allFiles = Utils.GetAllFilesInInnerDictionary($"Assets/Resources/{_settings.Data.ClipFolder}");

             if (_settings.PreLoad)
             {
                 for (var index = 0; index < allFiles.Count; index++)
                 {
                     var file = allFiles[index];
                     if (file.Contains(".meta"))
                         continue;

                     var a = Resources.LoadAsync<AudioClip>($"{_settings.Data.ClipFolder}{file}");
                     yield return new WaitUntil(() => a.isDone);
                     var au = a.asset as AudioClip;

                     if (au != null)
                     {
                         if (!_catch.ContainsKey(au.name))
                             _catch.Add(au.name, au);
                     }
                 }
             }
             else
             {
                 for (var index = 0; index < allFiles.Count; index++)
                 {
                     var file = allFiles[index];
                     if (file.Contains(".meta"))
                         continue;

                     var a = Resources.Load<AudioClip>($"{_settings.Data.ClipFolder}{file}");

                     if (a != null)
                     {
                         if (!_catch.ContainsKey(a.name))
                             _catch.Add(a.name, a);
                     }
                 }
             }
        }

        protected override void OnOnDestroy()
        {
            UnSubscribe();
        }


        private void Subscribe()
        {
            _eventSubscriberLocal.Subscribe<ChangeSoundSettingsEvent>(OnChangeAudioSettings);
        }

        private void OnChangeAudioSettings(ChangeSoundSettingsEvent data)
        {
            if (data.GroupOnChangeVolume == null) return;

            foreach (var f in data.GroupOnChangeVolume)
            {
                if (_settings.GroupVolume.ContainsKey(f.Key))
                {
                    _settings.GroupVolume[f.Key] = f.Value;
                }
            }

            UpdateSettings();
        }

        private void UpdateSettings()
        {
            foreach (var audioTrack in _trackLists)
            {
                audioTrack.RefreshVolume(GetVolumeByGroup(audioTrack.Group) / _settings.VolumeMultiply);
            }
        }
        private void UnSubscribe()
        {
            _eventSubscriberLocal?.Destroy();
        }

        private void OnSceneChange(string prevSceneName, string sceneName)
        {
            for (var index = 0; index < _trackLists.Count; index++)
            {
                var audioTrack = _trackLists[index];

                if (audioTrack.IsDestroyed)
                {
                    RemoveTrack(audioTrack);
                    index--;
                }
            }

            _trackLists.Clear();
        }

        public AudioClip GetClip(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (_catch.ContainsKey(id))
                return _catch[id];

            var clip = Resources.Load<AudioClip>($"{_settings.Data.ClipFolder}{id}");

            _catch.Add(id, clip);

            return clip;
        }

        public void SetVolumeMultiply(float a)
        {
            _settings.VolumeMultiply = a;

            UpdateSettings();
        }
        public AudioClipData GetAudioData(string id)
        {
            return _settings.Data.GetClipData(id);
        }

        public AudioClip GetClip(AudioClip clip)
        {
            if (clip == null)
                return null;

            if (_catch.ContainsKey(clip.name))
                return clip;

            _catch.Add(clip.name, clip);

            return clip;
        }
        private IEnumerator PlayTrackSequenceCor(List<string> random, int group, bool loop, string start, string final)
        {
            var track = CreateTrack(random.RandomElement(), group);

            yield return null;

            var pList = new List<string>();
            var rRand = new List<string>(random);
            while (rRand.Count > 0)
            {
                var p = rRand.RandomElement();
                pList.Add(p);
                rRand.Remove(p);
            }

            if(!string.IsNullOrEmpty(final)) pList.Add(final);
            if (!string.IsNullOrEmpty(start)) pList.Insert(0,start);


            for (var index = 0; index < pList.Count; index++)
            {
                var a = pList[index];
                var clip = GetClip(a);
                if (clip != null)
                {
                    track.PlayOneShot(clip);
                    yield return new WaitForSeconds(clip.length);
                }

                if (loop && index >= pList.Count - 1)
                {
                    yield return null;
                    index = 0;
                }
            }
        }


        private IEnumerator DelayUnscaledPlayOneShot(AudioClip clip, float multiply, bool unscaled, float delay)
        {
            if (unscaled)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);

                _commonAudioTrack.PlayOneShot(clip, multiply);
            }
        }

        public void AddAndPlayTrackSequence(List<string> random, int group = 0, bool loop = true, string start = "", string final = "")
        {
            if (_sequenceCoroutine != null)
                View.StopCoroutine(_sequenceCoroutine);

            _sequenceCoroutine = View.StartCoroutine(PlayTrackSequenceCor(random, group, loop, start, final));
        }

        public void PlayOneShot3D(string id, Vector3 pos)
        {
            var clip = GetClip(id);
            var data = GetAudioData(id);

            if (clip != null)
            {
                var go = new GameObject("AudioClip" + id)
                {
                    transform =
                    {
                        position = pos
                    }
                };
                var source = go.AddComponent<AudioSource>();
                source.volume = GetVolumeByGroup(-1);
                source.clip = clip;
                source.spatialBlend = 1;
                source.dopplerLevel = 0;
                source.spread = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = data == null || data.Range == -1 ? 1.1f : data.Range;
                source.volume = GetVolumeByGroup(-1) *
                (data == null
                        ? 1f
                        : data.Volume);
                source.Play();
                Object.Destroy(go, clip.length);
            }
        }

        public void PlayOneShot3D(string id, Vector3 pos, float areaSize)
        {
            var clip = GetClip(id);
            var data = GetAudioData(id);

            if (clip != null)
            {
                var go = new GameObject("AudioClip" + id);
                var source = go.AddComponent<AudioSource>();
                go.transform.position = pos;
                source.volume = GetVolumeByGroup(-1) *
                                (data == null
                                    ? 1f
                                    : data.Volume);
                source.clip = clip;
                source.spatialBlend = 1;
                source.dopplerLevel = 0;
                source.spread = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = areaSize <= 1 ? 1.1f : areaSize;
                source.Play(1);
                Object.Destroy(go, clip.length);
            }
        }

        public void PlayOneShot(string[] id, float multiply)
        {
            if (id == null || id.Length == 0) return;

            var clip = GetClip(id.RandomElement());
            var data = GetAudioData(clip?.name);

            _commonAudioTrack.PlayOneShot(clip, multiply * (data?.Volume ?? 1));
        }
        public void PlayOneShot(string id, float multiply)
        {
            var clip = GetClip(id);
            var data = GetAudioData(clip?.name);

            if (clip != null)
            {
                _commonAudioTrack.PlayOneShot(clip, multiply * (data?.Volume ?? 1));
            }
        }
        public void PlayOneShotDelay(string[] id, float delay = 0, float multiply = 1, bool unscaled = false)
        {
            var clip = GetClip(id.RandomElement());
            var data = GetAudioData(clip?.name);

            if (clip != null)
            {
                View.StartCoroutine(DelayUnscaledPlayOneShot(clip, multiply, unscaled, delay));
            }
        }

        public void PlayOneShotDelay(string id, float delay = 0, float multiply = 1, bool unscaled = false)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                View.StartCoroutine(DelayUnscaledPlayOneShot(clip, multiply, unscaled, delay));
            }
        }

        public float GetVolumeByGroup(int a)
        {
            if (_settings.GroupVolume.ContainsKey(a))
            {
                return _settings.GroupVolume[a];
            }
            else return 1;
        }

        public AudioTrack CreateTrack(string id, int group, bool loop = false, bool donNotDestroy = false)
        {
            var source = View.AddComponent<AudioSource>();

            var data = GetAudioData(id);

            source.clip = GetClip(id);
            source.volume = GetVolumeByGroup(group) * _settings.VolumeMultiply * (data?.Volume ?? 1);
            source.loop = loop;

            var tr = new AudioTrack(this, id, source, data, group, donNotDestroy);

            _trackLists.Add(tr);

            return tr;
        }


        public void PlayTrack(AudioTrack a, bool rebuild = true)
        {
            if (a == null) return;

            if (rebuild)
            {
                a.Source.Play();
            }
            else if (!a.Source.isPlaying)
            {
                a.Source.Play();
            }
        }

        public void StopTrack(AudioTrack a)
        {
            if (a == null) return;

            a.Destroy();
        }

        public void RemoveTrack(AudioTrack track)
        {
            if (track == null)
                return;

            track.DestroyInternal();

            _trackLists.Remove(track);
        }


        public void RemoveListener()
        {
            var l = View.GetComponent<AudioListener>();
            if (l != null)
            {
                Object.Destroy(l);
            }
        }

        public void AddListener()
        {
            var l = View.GetComponent<AudioListener>();
            if (l == null)
            {
                View.gameObject.AddComponent<AudioListener>();
            }
        }
    }
}
