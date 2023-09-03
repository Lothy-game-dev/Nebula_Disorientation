using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject SectionDesc;
    public GameObject SectionName;
    public GameObject MainCam;
    public GameObject MainCamBeforeMove;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string ItemChoosen;
    public bool isChoose;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       if (ItemChoosen != null && isChoose)
       {
            CheckItemChoosen();
            isChoose = false;
       }    
    }
    #endregion
    #region Check what item choosen
    public void CheckItemChoosen()
    {
        if (ItemChoosen != null)
        {
            Debug.Log("1");
            //turn off all child before choosing
            SectionName.SetActive(true);
            SectionDesc.SetActive(true);
            
        } 
    }
    #endregion
    
}
