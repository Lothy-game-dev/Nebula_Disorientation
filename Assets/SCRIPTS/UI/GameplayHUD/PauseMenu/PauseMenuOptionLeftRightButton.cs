using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenuOptionLeftRightButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string Type;
    public List<string> Contents;
    public TextMeshPro Text;
    public PauseMenuOption Option;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private bool available;
    private int currentIndex;
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
        if (!available)
        {
            CheckText();
        }

    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void CheckText()
    {
        if (Contents.Contains(Text.text))
        {
            available = true;
            currentIndex = Contents.IndexOf(Text.text);
        }
    }

    private void OnMouseDown()
    {
        if (name.Contains("Left"))
        {
            if (currentIndex == 0)
            {
                currentIndex = Contents.Count - 1;
            } else
            {
                currentIndex--;
            }
        } else if (name.Contains("Right"))
        {
            if (currentIndex == Contents.Count - 1)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
        }
        Text.text = Contents[currentIndex];
        if (Type.Equals("FPS"))
        {
            Option.Fps = Text.text;
        } else if (Type.Equals("Resolution"))
        {
            Option.Resol = Text.text;
        }
    }
    #endregion
}
