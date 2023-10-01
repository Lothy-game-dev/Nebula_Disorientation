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
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    private float CurrentValue;
    private float MaxValue;
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
