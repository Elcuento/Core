using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CustomToggleUi : Toggle
{
    [SerializeField] private UnityEvent OnValueChangeOnEvent;
    [SerializeField] private GameObject Enable;
    [SerializeField] private GameObject Disable;


    public void SetDisable(GameObject mobject)
    {
        Disable = mobject;
    }

    public void SetEnable(GameObject mobject)
    {
        Enable = mobject;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        onValueChanged.AddListener(OnToggle);

        if (Enable != null)
            Enable.SetActive(isOn);

        if (Disable != null)
            Disable.SetActive(!isOn);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        onValueChanged.RemoveListener(OnToggle);

    }

    private void OnToggle(bool state)
    {
        if (Enable != null)
            Enable.SetActive(isOn);

        if (Disable != null)
            Disable.SetActive(!isOn);

        if (state)
        {
            OnValueChangeOnEvent?.Invoke();
        }
    }

}
