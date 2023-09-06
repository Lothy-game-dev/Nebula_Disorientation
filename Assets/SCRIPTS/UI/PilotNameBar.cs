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
    public void SetText(int count, string Name, string Rank)
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text
            = "Pilot " + count.ToString() + " - Name: " + Name + "\nRank: " + Rank;
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
