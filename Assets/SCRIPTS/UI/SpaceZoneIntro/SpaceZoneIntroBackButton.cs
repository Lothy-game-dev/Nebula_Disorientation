using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneIntroBackButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Board;
    public GameObject[] EnableColliders;
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
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        Board.SetActive(false);
        foreach (var col in EnableColliders)
        {
            if (col.GetComponent<Collider2D>())
            col.GetComponent<Collider2D>().enabled = true;
        }
    }
    #endregion
}
