using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    #region ComponentVariables
    public Slider slider;
    #endregion
    #region InitializeVariables
    public Vector3 Position;
    #endregion
    #region NormalVariables
    private float CurrentValue;
    private float MaxValue;
    private Vector2 PositionSlider;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        slider.gameObject.SetActive(CurrentValue < MaxValue);
        slider.value = CurrentValue;
        slider.maxValue = MaxValue;
        slider.gameObject.transform.position = transform.parent.position + Position;
    }
    #endregion
    #region Public Set Value
    // Set Value from public
    public void SetValue(float value, float maxValue)
    {
        CurrentValue = value;
        MaxValue = maxValue;
    }
    #endregion
}
