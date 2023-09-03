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
            SectionName.SetActive(true);
            SectionDesc.SetActive(true);
            isChoose = false;
       }    
    }
    #endregion

    #region

    public void WordFade()
    {
        StartCoroutine(TextHiddentoUnhidden());
    }
    private IEnumerator TextHiddentoUnhidden()
    {       
        for (int i = 0; i < 10; i++)
        {           
            Color c = SectionDesc.GetComponent<TMP_Text>().color;
            c.a += 0.2f;
            SectionDesc.GetComponent<TMP_Text>().color = c;
            yield return new WaitForSeconds(0.15f);
        }      
    }
    #endregion

}
