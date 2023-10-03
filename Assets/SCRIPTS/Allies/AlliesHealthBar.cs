using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlliesHealthBar : MonoBehaviour
{
    #region ComponentVariables
    public Slider slider;
    #endregion
    #region InitializeVariables
    public GameObject Position;
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
        PositionSlider = Position.transform.position - transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        slider.gameObject.SetActive(CurrentValue < MaxValue);
        slider.value = CurrentValue;
        slider.maxValue = MaxValue;
        slider.gameObject.transform.position = new Vector3(transform.parent.position.x + PositionSlider.x, transform.parent.position.y + PositionSlider.y, transform.position.z);
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
