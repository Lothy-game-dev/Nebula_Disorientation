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
    public GameObject AfterMenuSelectPosition;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string ItemChoosen;
    private GameObject CloneSecName;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        CheckItemChoosen();     
    }
    #endregion
    #region Check what item choosen
    public void CheckItemChoosen()
    {
        if (ItemChoosen != null)
        {
            //turn off all child before choosing
            for (int i = 0; i < SectionName.transform.GetChild(0).transform.childCount; i++)
            {
                SectionName.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
                SectionDesc.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false);
            }
            if ("Control".Equals(ItemChoosen))
            {
                SectionName.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
                SectionDesc.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(true);
            } else
            {
                if ("BackButton".Equals(ItemChoosen))
                {
                    FindObjectOfType<MainMenuCameraController>().ChangeToScene(AfterMenuSelectPosition);
                    /*AfterMenuSelectPosition.transform.Find("TutorialButton").GetComponent<MainMenuButtons>().EnterView();*/
                } else
                {
                    if ("Shopping".Equals(ItemChoosen))
                    {
                        SectionName.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                        SectionDesc.transform.GetChild(0).transform.GetChild(1).gameObject.SetActive(true);
                    }
                }
            }
        } 
    }
    #endregion
    
}
