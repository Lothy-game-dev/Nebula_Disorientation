using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuCard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NameText;
    public GameObject EffectText;
    public BonusStatus BonusStatus;
    #endregion
    #region NormalVariables
    public List<string> Effect;
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
    private void OnMouseEnter()
    {
        NameText.SetActive(false);
        EffectText.SetActive(true);
    }

    private void OnMouseDown()
    {
        foreach (var go in BonusStatus.EffectList)
        {
            if (Effect.Contains(go.GetComponent<PauseMenuEffect>().Effect))
            {
                go.GetComponent<PauseMenuEffect>().Highlight();
            } else
            {
                go.GetComponent<PauseMenuEffect>().UnHighlight();
            }
        }
    }

    private void OnMouseExit()
    {
        NameText.SetActive(true);
        EffectText.SetActive(false);
    }
    #endregion
}
