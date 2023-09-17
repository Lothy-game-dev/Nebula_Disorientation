using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewStoryScene : MainMenuSceneShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TMP_InputField NameField;
    public GameObject[] StartAnimOn;
    public GameObject[] StartAnimOff;
    public GameObject Contract;
    #endregion
    #region NormalVariables
    private bool startWriting;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        startWriting = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!startWriting)
            {
                NameField.ActivateInputField();
                startWriting = true;
            }
        }
        if (!NameField.isFocused)
        {
            NameField.DeactivateInputField();
            startWriting = false;
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    public override void StartAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        foreach (var go2 in StartAnimOff)
        {
            go2.SetActive(false);
        }
        PilotContract pc = Contract.GetComponent<PilotContract>();
        if (pc!=null)
        {
            pc.EULAList[pc.currentPages].SetActive(false);
            pc.currentPages = 0;
            pc.EULAList[pc.currentPages].SetActive(true);
        }
        foreach (var go in StartAnimOn)
        {
            go.SetActive(true);
        }
        NameField.text = "";
    }

    public override void EndAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
    }
    #endregion
}
