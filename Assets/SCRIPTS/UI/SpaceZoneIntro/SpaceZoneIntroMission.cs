using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceZoneIntroMission : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TextMeshPro VictoryConditionText;
    public TextMeshPro VictoryCondition;
    public TextMeshPro DefeatConditionText;
    public TextMeshPro DefeatCondition;
    #endregion
    #region NormalVariables
    public bool IsVictory;
    private float InitScaleX;
    private bool ChangeConditionBool;
    private float waiting;
    private bool ChangeConditionVictoryOrNot;
    private int counter;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitScaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (ChangeConditionBool)
        {

            if (transform.localScale.x < InitScaleX)
            {

                ChangeCondition(ChangeConditionVictoryOrNot);
            } else
            {
                Done();
                ChangeConditionBool = false;
            }
        }
    }
    #endregion
    #region Mouse Check
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        GetComponent<Collider2D>().enabled = false;
        if (IsVictory)
        {
            IsVictory = false;
            VictoryConditionText.gameObject.SetActive(false);
            VictoryCondition.gameObject.SetActive(false);
            ChangeConditionVictoryOrNot = true;
            counter = 0;
            ChangeCondition(ChangeConditionVictoryOrNot);
            ChangeConditionBool = true;
        } else
        {
            IsVictory = true;
            DefeatConditionText.gameObject.SetActive(false);
            DefeatCondition.gameObject.SetActive(false);
            ChangeConditionVictoryOrNot = false;
            counter = 0;
            ChangeCondition(ChangeConditionBool);
            ChangeConditionBool = true;
        }
    }

    private void ChangeCondition(bool isVictory)
    {
        if (counter < 30)
        {
            transform.localScale = new Vector3(transform.localScale.x - InitScaleX / 30f, transform.localScale.y, transform.localScale.z);
        } else
        {
            transform.localScale = new Vector3(transform.localScale.x + InitScaleX / 30f, transform.localScale.y, transform.localScale.z);
        }
        if (Mathf.Abs(transform.localScale.x) < 0.01f)
        {
            if (isVictory)
            {
                GetComponent<SpriteRenderer>().color = new Color(1,0,0, 76/255f);
            } else
            {
                GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 76 / 255f);
            }
        }
        counter++;
    }

    private void Done()
    {
        GetComponent<Collider2D>().enabled = true;
        if (ChangeConditionVictoryOrNot)
        {
            DefeatConditionText.gameObject.SetActive(true);
            DefeatCondition.gameObject.SetActive(true);
        }
        else
        {
            VictoryConditionText.gameObject.SetActive(true);
            VictoryCondition.gameObject.SetActive(true);
        }
    }
    #endregion
}
