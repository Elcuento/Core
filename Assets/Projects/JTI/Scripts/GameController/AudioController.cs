using System;
using System.Collections;
using System.Collections.Generic;
using Assets.JTI.Scripts.Events.Game;
using JTI.Scripts.Common;
using JTI.Scripts.Events;
using JTI.Scripts.Localization.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JTI.Scripts.GameControllers
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioController : GameControllerMono
    {
        [System.Serializable]
        public class AudioControllerSettings : GameControllerSettings
        {
            [SerializeField] private List<AudioVolume> _volumeByGroup;
            [SerializeField] private AudioData _data;
            [SerializeField] private bool _preLoad;
            [SerializeField] private float _volumeMultiply = 1;

            public AudioData Data => _data;
            public bool PreLoad => _preLoad;
            public float VolumeMultiply => _volumeMultiply;


            [System.Serializable]
            public class AudioVolume
            {
                public float Volume;
                public int Group;
            }

            public void SetVolumeMultiply(float volume)
            {
                _volumeMultiply = volume;
            }
            public AudioVolume GetVolumeByGroup(int group)
            {
                if (_volumeMap.ContainsKey(group))
                {
                    return _volumeMap[group];
                }
                else
                {
                    var v = _volumeByGroup.Find(x => x.Group == group);
                    if (v != null)
                    {
                        _volumeMap.Add(v.Group, v);
                    }
                    else
                    {
                        _volumeMap.Add(group, new AudioVolume() { Volume = 1, Group = group });
                    }

                    return _volumeMap[group];
                }
            }

            private Dictionary<int, AudioVolume> _volumeMap;


            public AudioControllerSettings()
            {
                _volumeByGroup = new List<AudioVolume>();
                _volumeMap = new Dictionary<int, AudioVolume>();
                _volumeMultiply = 1;
            }
            public AudioControllerSettings(AudioData d)
            {
                _data = d;

                _volumeMultiply = 1;
                _volumeMap = new Dictionary<int, AudioVolume>();
                _volumeByGroup = new List<AudioVolume>();
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

            private float _from;
            private float _to;

            private bool _isDestroyed;
            private bool _destroyAfterPlay;
            private bool _controlPlay;

            private Coroutine _playCoroutine;

            public void PlayOneShot(AudioClip clip, float volumeMultiply = 1)
            {
                if (IsDestroyed) return;

                Source.PlayOneShot(clip, volumeMultiply);
            }


            private IEnumerator PlayAudioWithControl()
            {
                Source.time = _from;
                Source.Play();

                while (IsPlaying)
                {
                    yield return null;

                    if (_to != -1)
                    {
                        if (Source.time >= _to)
                        {
                            Stop();
                            yield break;
                        }
                    }
                }

                Stop();
            }


            public void Stop()
            {
                if (_playCoroutine != null)
                {
                    Controller.StopCoroutine(_playCoroutine);
                }

                Source.Stop();

                if (_destroyAfterPlay)
                {
                    Destroy();
                }
            }

            public void Play()
            {
                if (_controlPlay)
                {
                    if (_playCoroutine != null)
                    {
                        Controller.StopCoroutine(_playCoroutine);
                    }

                    _playCoroutine = Controller.StartCoroutine(PlayAudioWithControl());
                }
                else
                {
                    Source.Play();
                }

            }

            public AudioTrack(AudioController c, string id, AudioSource source, AudioClipData data, int group = 1, bool doNotDestroy = false, float multiply = 1,
                float from =0, float to=-1, bool destroyAfterPlay = false)
            {
                Group = group;
                Id = id;
                Source = source;
                Controller = c;
                DoNotDestroy = doNotDestroy;
                Multiply = multiply;
                Data = data;

                _from = from;
                _to = to;

                _destroyAfterPlay = destroyAfterPlay;

                if (_from != 0 || _to != -1 || _destroyAfterPlay)
                {
                    _controlPlay = true;
                }

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

        [SerializeField] private AudioControllerSettings _settings;

        private Action _onUpdateEvent;

        private Dictionary<string, AudioClip> _catch;

        private Coroutine _sequenceCoroutine;

        private List<AudioTrack> _trackLists;

        private EventSubscriberLocal<GameEvent> _eventSubscriberLocal;

        private AudioTrack _commonAudioTrack;

        protected override void OnInstall()
        {
            _trackLists = new List<AudioTrack>();
            _catch = new Dictionary<string, AudioClip>();

            _commonAudioTrack = CreateTrack("AudioSource_" + Guid.NewGuid(), -1, true);

            _eventSubscriberLocal = new EventSubscriberLocal<GameEvent>(Manager.GameEvents);

            Subscribe();

            PreLoad();
            UpdateSettings();
        }

        private void Update()
        {
            _onUpdateEvent?.Invoke();
        }

        private void PreLoad()
        {
            StartCoroutine(_loadAsync());
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
                if (data.GroupOnChangeVolume.ContainsKey(f.Key))
                {
                    data.GroupOnChangeVolume[f.Key] = data.GroupOnChangeVolume[f.Key];
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
            _settings.SetVolumeMultiply(a);

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

        public void PlayCoroutine(Coroutine aCoroutine)
        {
            
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
                StopCoroutine(_sequenceCoroutine);

            _sequenceCoroutine = StartCoroutine(PlayTrackSequenceCor(random, group, loop, start, final));
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

        public void PlayOneShotInOnTime(string id, float multiply, float start =- 1, float end = -1)
        {
            var clip = GetClip(id);
            var data = GetAudioData(clip?.name);

            if (clip != null)
            {
                _commonAudioTrack.PlayOneShot(clip, multiply * (data?.Volume ?? 1));
            }
        }

        public void PlayOneShot3D(string id, Transform tr)
        {

            var clip = GetClip(id);
            var data = GetAudioData(id);

            // Debug.Log(clip +":" + clip?.length);
            if (clip != null)
            {
                var go = new GameObject("AudioClip" + id)
                {
                    transform =
                    {
                        position = tr.position
                    }
                };
                go.transform.SetParent(tr);

                var source = go.AddComponent<AudioSource>();
                source.volume = GetVolumeByGroup(-1) *
                                (data == null
                                    ? 1f
                                    : data.Volume);
                source.clip = clip;
                source.spatialBlend = 1;
                source.dopplerLevel = 0;
                source.spread = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = data == null || data.Range == -1 ? 1.1f : data.Range;
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
                StartCoroutine(DelayUnscaledPlayOneShot(clip, multiply * (data?.Volume ?? 1), unscaled, delay));
            }
        }

        public void PlayOneShotDelay(string id, float delay = 0, float multiply = 1, bool unscaled = false)
        {
            var clip = GetClip(id);
            var data = GetAudioData(clip?.name);

            if (clip != null)
            {
                StartCoroutine(DelayUnscaledPlayOneShot(clip, multiply * (data?.Volume ?? 1), unscaled, delay));
            }
        }

        public float GetVolumeByGroup(int a)
        {
            return _settings.GetVolumeByGroup(a).Volume;
        }

        public AudioTrack CreateTrack(string id, int group, bool loop = false, bool donNotDestroy = false)
        {
            var source = gameObject.AddComponent<AudioSource>();

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
                a.Play();
            }
            else if (!a.Source.isPlaying)
            {
               a.Play();
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
            var l = GetComponent<AudioListener>();
            if (l != null)
            {
                Object.Destroy(l);
            }
        }

        public void AddListener()
        {
            var l = GetComponent<AudioListener>();
            if (l == null)
            {
                gameObject.AddComponent<AudioListener>();
            }
        }
    }
}
