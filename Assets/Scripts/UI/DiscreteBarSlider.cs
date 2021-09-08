using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class DiscreteBarSlider : MonoBehaviour
{
    public int slots = 2;
    public int last_active_slot = -1;
    public int next_slot = -1;
    public float feather_k = 1f;
    public List<UnityEvent> FieldActions = new List<UnityEvent>();

    private float v;
    private float last_pos;
    private bool isDragged;
    private UnityEngine.UI.Slider slider;

    void Start()
    {
        slider = gameObject.GetComponent<UnityEngine.UI.Slider>();
        v = 0f;
        last_pos = 0f;
        isDragged = false;
    }

    void Update()
    {
        if (isDragged)
        {
            isDragged = false;
            return;
        }

        int targ = GetTarget();
        float pos = GetTargetRelativePosition(targ);
        if (pos == slider.normalizedValue)
        {
            v = 0f;
            next_slot = -1;
            if(last_active_slot != targ)
            {
                last_active_slot = targ;
                if(FieldActions.Count > targ)
                    FieldActions[targ].Invoke();
            }
            return;
        }

        v += feather_k * (pos - slider.normalizedValue);
        slider.SetValueWithoutNotify(slider.maxValue * (slider.normalizedValue += v * Time.deltaTime));

        last_pos = slider.normalizedValue;
    }

    private int GetTarget()
    {
        if (next_slot >= 0)
            return next_slot;

        int ret = (int) (slider.normalizedValue * slots);
        ret = ret >= slots ? slots-1 : ret;

        if(ret == last_active_slot && GetTargetRelativePosition(ret) != slider.normalizedValue)
            ret += GetTargetRelativePosition(ret) > slider.normalizedValue ? -1 : 1;

        return ret;
    }

    private float GetTargetPosition(int target)
    {
        return slider.maxValue * GetTargetRelativePosition(target);
    }

    private float GetTargetRelativePosition(int target)
    {
        return 1 / (slots - 1) * target;
    }

    public void HardToggle(int target)
    {
        slider.normalizedValue = GetTargetRelativePosition(target);
    }

    public void SoftToggle(int target)
    {
        next_slot = target;

        last_active_slot = target;
        FieldActions[target].Invoke();
    }

    public void Drag()
    {
        //isDragged = true;
        //v = (last_pos - slider.normalizedValue) / Time.deltaTime;
        last_pos = slider.normalizedValue;
    }
}
