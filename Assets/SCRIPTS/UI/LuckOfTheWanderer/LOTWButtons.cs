using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOTWButtons : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    public string Type;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
    #region Mouse check
    private void OnMouseDown()
    {
        if (Type=="Reroll")
        {
            GetComponent<BoxCollider2D>().enabled = false;
            Scene.GetComponent<LOTWScene>().RegenerateCard();
        } 
        else if (Type=="Pick")
        {
            Scene.GetComponent<LOTWScene>().ConfirmSelect();
        }
        else if (Type == "ShowOwn")
        {

        }
        else if (Type == "ShowAll")
        {

        }
    }
    #endregion
}
