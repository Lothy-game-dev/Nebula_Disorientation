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
    public string Noti;
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
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (Type=="Reroll")
        {
            if (Noti!=null && Noti!="")
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    Noti, 5f);
            } else
            {
                GetComponent<BoxCollider2D>().enabled = false;
                Scene.GetComponent<LOTWScene>().RegenerateCard();
            }
        } 
        else if (Type=="Pick")
        {
            Scene.GetComponent<LOTWScene>().ConfirmSelect();
        }
        else if (Type == "ShowOwn")
        {
            Scene.GetComponent<LOTWScene>().OpenAllCardsOwnedPopup();
        }
        else if (Type == "ShowAll")
        {
            Scene.GetComponent<LOTWScene>().OpenAllCardsPopup();
        } else if (Type== "ShowAllBack")
        {
            Scene.GetComponent<LOTWScene>().CloseAllCardsPopup();
        }
        else if (Type == "ShowOwnBack")
        {
            Scene.GetComponent<LOTWScene>().CloseAllCardsOwnedPopup();
        }
    }
    #endregion
}
