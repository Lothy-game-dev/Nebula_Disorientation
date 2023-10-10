using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceZoneBackground : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject BGTemplate;
    #endregion
    #region NormalVariables
    private GameObject TemplateChosen;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
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
    public void ChangeBackground(string name)
    {
        for (int i=0; i < BGTemplate.transform.childCount; i++)
        {
            if (BGTemplate.transform.GetChild(i).name.ToLower().Equals(name.ToLower()))
            {
                SetBackground(i);
                break;
            }
        }
    }
    public void SetBackground(int n)
    {
        if (n < BGTemplate.transform.childCount)
        {
            TemplateChosen = BGTemplate.transform.GetChild(n).GetChild(Random.Range(0, BGTemplate.transform.GetChild(n).childCount)).gameObject;
            for (int j=0;j<transform.childCount;j++)
            {
                GameObject center = transform.GetChild(j).gameObject;
                center.GetComponent<SpriteRenderer>().sprite = TemplateChosen.GetComponent<SpriteRenderer>().sprite;
                for (int i=0;i<center.transform.childCount;i++)
                {
                    center.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = TemplateChosen.GetComponent<SpriteRenderer>().sprite;
                }
            }
        }
    }
    #endregion
}
