using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PilotNameBar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    #endregion
    #region NormalVariables
    public GameObject Scene;
    public string PilotName;
    public string PilotRank;
    public string Session;
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
        // Call function and timer only if possible
    }
    #endregion
    #region Set Text
    public void SetText(int count, string Name, string Rank, string SessionData)
    {
        PilotName = Name;
        PilotRank = Rank;
        Session = SessionData;
        string Text = "Pilot " + count.ToString() + " - Name: " + Name + "\nRank: " + Rank;
        if (SessionData != "None")
        {
            Text += "\n<color=#00ff00>Currently in session</color> - Stage <color=#3bccec>" + SessionData + "</color>";
        }
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
            = Text;
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        LoadStoryScene loadStory = Scene.GetComponent<LoadStoryScene>();
        if (loadStory!=null)
        {
            loadStory.CurrentPressPilot = gameObject;
        }
    }
    #endregion
}
