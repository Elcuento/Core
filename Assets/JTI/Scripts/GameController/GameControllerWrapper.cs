using UnityEngine;

namespace JTI.Scripts.GameControllers
{
    public class GameControllerWrapper : MonoBehaviour
    {
        public virtual void Initialize<T, TU>(TU settings) where TU : GameControllerSettings where T : GameController<TU>
        {
         
        }
    }
}

