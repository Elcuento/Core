using System.Collections.Generic;
using Assets.JTI.Scripts.Database;
using JTI.Scripts.Common;
using UnityEditor;
using UnityEngine;

namespace JTI.Scripts.Localization.Data
{
    [CreateAssetMenu(fileName = "AudioData", menuName = "JTI/Audio/AudioData", order = 3)]
    public class AudioData : DataBase
    {
        [SerializeField] private List<AudioClipData> _clipsData;
        [SerializeField] private string _clipFolder;
        public string ClipFolder => _clipFolder;


        public AudioClipData GetClipData(string id)
        {
            return _clipsData.Find(x => x.ClipId == id);
        }


#if UNITY_EDITOR
        public void UpdateData()
        {
            _clipsData = Utils.GetTypesInProject<AudioClipData>();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
