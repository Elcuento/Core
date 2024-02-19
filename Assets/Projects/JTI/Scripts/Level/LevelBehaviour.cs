using JTI.Scripts.Level;
using UnityEngine;

public class LevelBehaviour : MonoBehaviour
{
    public LevelMaster Master { get; private set; }

    private void Awake()
    {
        Master = transform.root.GetComponent<LevelMaster>();

        if (Master == null)
        {
            Debug.LogError("master is not a root item!");
            return;
        }

        Master.AddLevelBehaviour(this);

        OnAwake();
    }

    public T GetMaster<T>() where T : LevelMaster
    {
        return Master as T;
    }

    private void OnDestroy()
    {
        Master.RemoveLevelBehaviour(this);

        OnOnDestroy();
    }

    public virtual void ChangeState(LevelMaster.LevelState state)
    {

    }

    protected virtual void OnOnDestroy()
    {

    }
    protected virtual void OnAwake()
    {

    }
    public void Setup(LevelMaster m)
    {
        Master = m;
    }

    public virtual void SetPause(bool isPause)
    {
     
    }
}
