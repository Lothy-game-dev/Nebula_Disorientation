using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UECSessionMinusButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Left, Right, Top, Bottom;
    public GameObject Counter;
    public GameObject Bar;
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
        if (!CheckIfMouseOutsideScrollRange())
        {
            transform.parent.GetComponent<Collider2D>().enabled = false;
        } else
        {
            transform.parent.GetComponent<Collider2D>().enabled = true;
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private bool CheckIfMouseOutsideScrollRange()
    {
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > Right.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).x < Left.transform.position.x ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y > Top.transform.position.y ||
            Camera.main.ScreenToWorldPoint(Input.mousePosition).y < Bottom.transform.position.y)
            return true;
        return false;
    }

    private void OnMouseDown()
    {
        if (int.Parse(Counter.GetComponent<TextMeshProUGUI>().text) >= 1)
        {
            string check = Bar.GetComponent<UECSessionConsumableBox>().ReduceItem(transform.parent.gameObject);
            if (check=="Success")
            {
                if (int.Parse(Counter.GetComponent<TextMeshProUGUI>().text)==1)
                {
                    Counter.SetActive(false);
                    gameObject.SetActive(false);
                    transform.parent.GetComponent<Image>().color = Color.white;
                } else
                {
                    Counter.GetComponent<TextMeshProUGUI>().text = (int.Parse(Counter.GetComponent<TextMeshProUGUI>().text) - 1).ToString();
                }
            }
        }
        
    }

    private void OnDisable()
    {
        transform.parent.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
