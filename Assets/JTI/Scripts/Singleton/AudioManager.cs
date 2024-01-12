using System;
using System.Collections;
using System.Collections.Generic;
using Assets.JTI.Scripts.Database;
using DG.Tweening;
using JTI.Scripts.Common;
using JTI.Scripts.Localization.Data;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    [RequireComponent(typeof(AudioListener))]
    public class AudioManager : SingletonMono<AudioManager>
    {
        [SerializeField] private AudioData _audioData;

        [System.Serializable]
        public class AudioTrack
        {
            public string Id;
            public AudioSource Source;
            public bool IsDestroyed => Source == null;
            public bool IsPlaying => Source != null && Source.isPlaying;

            public void DestroyTrack()
            {
                Destroy(Source);
            }
        }

        public string Folder;
        public bool IsMusicPlaying => _musicSource.clip != null && _musicSource.isPlaying;
        public float SoundVolume => _soundVolume;
        public float MusicVolume => _musicVolume;

        private AudioSource _musicSource;
        private AudioSource _soundSource;

        private Dictionary<string, AudioClip> _catch;

        private Tweener _musicTweener;

        private float _musicVolume;
        private float _soundVolume;

        private float _musicMultiplierVolume = 1;
        private float _soundMultiplierVolume = 1;

        private Coroutine _sequenceCoroutine;

        private List<AudioTrack> _trackLists;

        protected override void OnAwaken()
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _soundSource = gameObject.AddComponent<AudioSource>();

            _trackLists = new List<AudioTrack>();
            _catch = new Dictionary<string, AudioClip>();

            _musicSource.loop = true;
            
            _soundVolume = PlayerPreferencesHelper.LoadDefault(PlayerPreferencesHelper.PlayerPreferencesType.SoundVolume, 1f);
            _musicVolume = 1;

            PreLoad();
            CheckSettings();
        }


        public void SetSound(float a)
        {
            PlayerPreferencesHelper.Save(PlayerPreferencesHelper.PlayerPreferencesType.SoundVolume, a);
            CheckSettings();
        }
        private void PreLoad()
        {
            StartCoroutine(_loadAsync());
        }


        public void SetFolder(string s)
        {
            Folder = s == "" ? "" : s + "/";
        }

        private IEnumerator _loadAsync()
        {
            yield return null;

            // var allFiles = Common.GetAllFilesInInnerDictionary($"Assets/Resources/{Paths.AudioFolder}{Folder}");

            /* if (ProviderManager.Instance.Data.SettingsData.LoadAudioOnStartAsync)
             {
                 for (var index = 0; index < allFiles.Count; index++)
                 {
                     var file = allFiles[index];
                     if (file.Contains(".meta"))
                         continue;

                     var a = Resources.LoadAsync<AudioClip>($"{Paths.ResourcesAudio}{file}");
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

                     var a = Resources.Load<AudioClip>($"{Paths.ResourcesAudio}{file}");

                     if (a != null)
                     {
                         if (!_catch.ContainsKey(a.name))
                             _catch.Add(a.name, a);
                     }
                 }
             }*/
        }

        protected override void OnStart()
        {
            base.OnStart();
            Subscribe();
        }


        public AudioClipData GetAudioData(string id)
        {
            return _audioData.GetClipData(id);
        }
        private void OnDestroy()
        {
            UnSubscribe();
        }

        private void Subscribe()
        {
            // EventManager.Instance.Subscribe<SettingsAudioChangeEvent>(OnChangeAudio);
            // SceneManager.Instance.OnLoadEvent += OnSceneChange;
        }

        private void UnSubscribe()
        {
            /*  if (EventManager.Instance == null)
                  return;

              EventManager.Instance.Unsubscribe<SettingsAudioChangeEvent>(OnChangeAudio);

              if (SceneManager.Instance == null)
                  return;

              SceneManager.Instance.OnLoadEvent -= OnSceneChange;*/
        }

        private void CheckSettings()
        {
            // _musicVolume = GameMaster.Instance.Profile.LoadDefault(Profile.ProfileVariableType.MusicOn, true) ? 1 : 0;
            _soundVolume = PlayerPreferencesHelper.LoadDefault(PlayerPreferencesHelper.PlayerPreferencesType.SoundVolume, 1);

            _musicSource.DOKill();
            _soundSource.DOKill();

            _musicSource.volume = _musicVolume;// * ProviderManager.Instance.Data.Settings.MusicVolumeMultiplier
                                              //   * _musicMultiplierVolume;
            _soundSource.volume = _soundVolume;// ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;

        }

        public void OnChangeAudio()
        {
            CheckSettings();

            if (_musicTweener != null) // TODO CHANGE CURRENT
            {
                // _musicTweener.ChangeEndValue()
            }

            foreach (var track in _trackLists)
            {
                if (track != null)
                {
                    track.Source.volume = _soundVolume;// * ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
                }
            }
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

            var clip = Resources.Load<AudioClip>($"{"Game/Audio/"}{Folder}{id}");

            _catch.Add(id, clip);

            return clip;
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

        public void StopMusic()
        {
            _musicSource.Stop();
        }


        public void PlayMusic(string id)
        {
            if (_sequenceCoroutine != null)
                StopCoroutine(_sequenceCoroutine);

            var clip = GetClip(id);

            if (_musicSource.clip == clip && _musicSource.isPlaying)
                return;

            _musicTweener?.Kill();

            _musicTweener = _musicSource.DOFade(0, _musicSource.volume / 1 * 0.5f).OnComplete(() =>
            {
                _musicSource.clip = clip;
                _musicSource.Play();
                /*  _musicSource.DOFade(_musicVolume * ProviderManager.Instance.Data.Settings.MusicVolumeMultiplier
                                                   * _musicMultiplierVolume,
                      0.5f);*/
            });
        }


        public void PlayMusicSequence(List<string> random, string last)
        {
            if (_sequenceCoroutine != null)
                StopCoroutine(_sequenceCoroutine);

            _sequenceCoroutine = StartCoroutine(PlayMusicSequenceCor(random, last));
        }

        public IEnumerator PlayMusicSequenceCor(List<string> random, string last)
        {
            yield return null;

            var pList = new List<string>();
            var rRand = new List<string>(random);
            while (rRand.Count > 0)
            {
                var p = rRand.RandomElement();
                pList.Add(p);
                rRand.Remove(p);
            }

            pList.Add(last);

            for (var index = 0; index < pList.Count; index++)
            {
                var a = pList[index];
                var clip = GetClip(a);
                if (clip != null)
                {
                    PlayMusic(clip.name);
                    yield return new WaitForSeconds(clip.length);
                }
            }

            PlayMusicSequence(random, last);
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
                source.volume = _soundVolume;
                source.clip = clip;
                source.spatialBlend = 1;
                source.dopplerLevel = 0;
                source.spread = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = data == null || data.Range == -1 ? 1.1f : data.Range;
                source.volume =
                    _soundVolume *
                    (data == null
                        ? 1f
                        : data.Volume);
                source.Play();
                Destroy(go, clip.length);
            }
        }

        public void PlayOneShot3D(string id, Vector3 pos, float areaSize)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                var go = new GameObject("AudioClip" + id);
                var source = go.AddComponent<AudioSource>();
                go.transform.position = pos;
                source.volume = _soundVolume;
                source.clip = clip;
                source.spatialBlend = 1;
                source.dopplerLevel = 0;
                source.spread = 1;
                source.rolloffMode = AudioRolloffMode.Linear;
                source.maxDistance = areaSize <= 1 ? 1.1f : areaSize;
                source.volume = _soundVolume; // * ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
                source.Play(1);
                Destroy(go, clip.length);
            }
        }
        public void PlayOneShot(string id)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                _soundSource.PlayOneShot(clip);
                _soundSource.volume = _soundVolume; // * ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
            }
        }

        public void PlayOneShot(string[] id)
        {
            var clip = GetClip(id.RandomElement());

            if (clip != null)
            {
                _soundSource.PlayOneShot(clip);
                _soundSource.volume = _soundVolume; // * ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
            }
        }
        public void PlayOneShotDelay(string id, float delay = 0)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                StartCoroutine(DelayPlay(clip, delay));
            }
        }

        public void PlayOneShotDelayUnscaled(string id, float delay = 0)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                StartCoroutine(DelayUnscaledPlay(clip, delay));
            }
        }

        public AudioTrack PlayTrack(AudioClip c)
        {
            var clip = GetClip(c);

            if (clip != null)
            {
                var track = new AudioTrack
                {
                    Id = clip.name,
                    Source = gameObject.AddComponent<AudioSource>()
                };

                _trackLists.Add(track);

                track.Source.clip = clip;
                track.Source.volume = _soundVolume;//* ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
                track.Source.Play();

                return track;
            }
            else
            {
                Debug.LogError("Not exist clip " + c?.name);
            }

            return null;
        }

        public AudioTrack LoadTrack(string id, bool loop = false)
        {
            var clip = GetClip(id);

            if (clip != null)
            {
                var track = new AudioTrack
                {
                    Id = id,
                    Source = gameObject.AddComponent<AudioSource>()
                };

                _trackLists.Add(track);

                track.Source.clip = clip;
                track.Source.volume = _soundVolume;// * ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
                track.Source.loop = loop;
                track.Source.Play();

                return track;
            }
            else
            {
                Debug.LogError("Not exist clip " + id);
            }

            return null;
        }

        public void StopTrack(AudioTrack track)
        {
            try
            {
                track?.Source?.Stop();
            }
            catch (Exception e)
            {
                
            }

        }

        public void RemoveTrack(AudioTrack track)
        {
            if (track == null)
                return;

            if (track.Source != null)
                Destroy(track.Source);

            _trackLists.Remove(track);
        }

        public void PlayTrack(AudioTrack track, bool rebind = false, bool loop = false)
        {
            if (track != null && track.Source != null)
            {
                if (track.Source.isPlaying && !rebind)
                {
                    track.Source.loop = loop;
                    return;
                }

                track.Source.loop = loop;
                track.Source.volume = _soundVolume;//* ProviderManager.Instance.Data.Settings.SoundVolumeMultiplier;
                track.Source.Play();
            }
        }

        private IEnumerator DelayUnscaledPlay(AudioClip clip, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            _soundSource.PlayOneShot(clip);
        }
        private IEnumerator DelayPlay(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            _soundSource.PlayOneShot(clip);
        }
        private void Load(string s)
        {
            GetClip(s);
        }

        public void SetMusicVolumeMultiplier(float f)
        {
            _musicMultiplierVolume = f;

            _musicSource.DOKill();
            _musicSource.volume = 1;
            /*_musicVolume * ProviderManager.Instance.Data.Settings.MusicVolumeMultiplier *
                                  _musicMultiplierVolume;*/
        }

        public void RemoveListener()
        {
            var l = GetComponent<AudioListener>();
            if (l != null)
            {
                Destroy(l);
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
