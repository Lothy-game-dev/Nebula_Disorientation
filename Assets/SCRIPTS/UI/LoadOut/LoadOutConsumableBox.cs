using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOutConsumableBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject StackText;
    public GameObject SelectedText;
    public GameObject MinusButton;
    public GameObject Scene;
    #endregion
    #region NormalVariables
    public GameObject PopUp;
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
    #region mouse check
    private void OnMouseDown()
    {
        if (int.Parse(SelectedText.GetComponent<TextMeshProUGUI>().text)
            < int.Parse(StackText.GetComponent<TextMeshProUGUI>().text))
        {
            PopUp.GetComponent<LoadOutConsumablePopUp>().ShowClickItem(gameObject);
            PopUp.GetComponent<LoadOutConsumablePopUp>().CheckIncreaseClickItem(gameObject);
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "You have run out of this items!\nPlease get some more!", 5f);
        }
    }

    public void SetStackText(int n)
    {
        StackText.GetComponent<TextMeshProUGUI>().text = n.ToString();
    }

    public void SetChosenAmount(int n)
    {
        if (n > 0)
        {
            if (!MinusButton.activeSelf)
            {
                MinusButton.SetActive(true);
            }
            if (!SelectedText.activeSelf)
            {
                SelectedText.SetActive(true);
            }
            SelectedText.GetComponent<TextMeshProUGUI>().text = n.ToString();
        }
        else
        {
            if (MinusButton.activeSelf)
            {
                MinusButton.SetActive(false);
            }
            if (SelectedText.activeSelf)
            {
                SelectedText.SetActive(false);
            }
            SelectedText.GetComponent<TextMeshProUGUI>().text = "0";
        }
    }
    #endregion
}
