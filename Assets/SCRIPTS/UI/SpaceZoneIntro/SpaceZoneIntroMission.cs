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
            StartCoroutine(ChangeCondition(true));
        } else
        {
            IsVictory = true;
            DefeatConditionText.gameObject.SetActive(false);
            DefeatCondition.gameObject.SetActive(false);
            StartCoroutine(ChangeCondition(false));
        }
    }

    private IEnumerator ChangeCondition(bool isVictory)
    {
        for (int i=0; i<18; i++)
        {
            if (i < 9)
            {
                transform.localScale = new Vector3(transform.localScale.x - InitScaleX / 9f, transform.localScale.y, transform.localScale.z);
            } else
            {
                transform.localScale = new Vector3(transform.localScale.x + InitScaleX / 9f, transform.localScale.y, transform.localScale.z);
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
            yield return new WaitForSeconds(1 / 18f);
        }
        GetComponent<Collider2D>().enabled = true;
        if (isVictory)
        {
            DefeatConditionText.gameObject.SetActive(true);
            DefeatCondition.gameObject.SetActive(true);
        } else
        {
            VictoryConditionText.gameObject.SetActive(true);
            VictoryCondition.gameObject.SetActive(true);
        }
    }
    #endregion
}
