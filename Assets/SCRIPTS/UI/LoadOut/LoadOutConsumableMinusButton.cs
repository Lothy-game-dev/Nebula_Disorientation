using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutConsumableMinusButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject PopUp;
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        bool check = false;
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.name == name)
                {
                    check = true;
                    break;
                }
            }
        }
        if (check)
        {
            transform.parent.GetComponent<Collider2D>().enabled = false;
        } else
        {
            transform.parent.GetComponent<Collider2D>().enabled = true;
        }
    }
    #endregion
    #region mouse check
    private void OnMouseDown()
    {
        PopUp.GetComponent<LoadOutConsumablePopUp>().ShowClickItem(transform.parent.gameObject);
        PopUp.GetComponent<LoadOutConsumablePopUp>().CheckDecreaseClickItem(transform.parent.gameObject);
    }
    private void OnDisable()
    {
        transform.parent.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
