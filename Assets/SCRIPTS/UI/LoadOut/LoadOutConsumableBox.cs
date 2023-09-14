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
        PopUp.GetComponent<LoadOutConsumablePopUp>().ShowClickItem(gameObject);
        PopUp.GetComponent<LoadOutConsumablePopUp>().CheckIncreaseClickItem(gameObject);
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
            SelectedText.GetComponent<TextMeshProUGUI>().text = "";
        }
    }
    #endregion
}
