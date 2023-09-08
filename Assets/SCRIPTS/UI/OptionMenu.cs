using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public Slider MasterVolumnSlider;
    public GameObject FPSText;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public bool IsClicked;
    public string FpsCounter;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        MasterVolumnSlider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
        FPSText.GetComponent<TMP_Text>().text = "30";
        FpsCounter = FPSText.GetComponent<TMP_Text>().text;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClicked)
        {
            FPSText.GetComponent<TMP_Text>().text = FpsCounter;
            IsClicked = false;
            Debug.Log("1");
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    public void ValueChangeCheck()
    {
        Debug.Log(1 / Time.deltaTime);
    }

    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
