using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverheatBar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public Slider Slider;
    #endregion
    #region NormalVariables
    public Weapons Weapon;
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
        SetOverheatBar();
    }
    #endregion
    #region Set Overheat Bar
    // Group all function that serve the same algorithm
    public void SetOverheatBar()
    {
        Slider.maxValue = 100;
        Slider.value = Weapon.currentOverheat;
        Slider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(Color.green, Color.red, Slider.normalizedValue);
    }
    #endregion
}
