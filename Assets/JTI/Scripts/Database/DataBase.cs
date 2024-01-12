using UnityEngine;

namespace Assets.JTI.Scripts.Database
{
    public class DataBase : ScriptableObject
    {
        public void Init()
        {
            OnInit();
        }

        public virtual void OnInit()
        {

        }
    }
}