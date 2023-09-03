using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject ItemType;
    public GameObject TutorialBoard;
    public GameObject Text;
    public GameObject SectionDesc;
    public GameObject SectionName;
    #endregion
    #region NormalVariables
    private TutorialMenu TutorialMenu;
    private Vector3 InitScale;
    private float ExpectedScale;
    private bool alreadySelect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        InitScale = transform.localScale;
        ExpectedScale = transform.localScale.x * 1.1f;
        TutorialMenu = TutorialBoard.GetComponent<TutorialMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check mouse
    private void OnMouseDown()
    {
        if (TutorialMenu.SectionDesc != null && TutorialMenu.SectionName != null)
        {
            TutorialMenu.SectionDesc.SetActive(false);
            TutorialMenu.SectionName.SetActive(false);
        }
        if ("BackButton".Equals(gameObject.name))
        {
            FindObjectOfType<MainMenuCameraController>().BackToMainMenu();
        }
        else
        {
            TutorialMenu.SectionDesc = SectionDesc;
            TutorialMenu.SectionName = SectionName;
            TutorialMenu.ItemChoosen = gameObject.name;
            TutorialMenu.isChoose = true;
        }
    }
    private void OnMouseEnter()
    {
        alreadySelect = false;
    }
    private void OnMouseOver()
    {       
        if ("BackButton".Equals(gameObject.name))
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r -= 0.01f;
            c.b -= 0.01f;
            GetComponent<SpriteRenderer>().color = c;
            GetComponent<SpriteRenderer>().sortingOrder = 3;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.r -= 0.01f;
            c2.g -= 0.01f;
            c2.b -= 0.01f;
            Text.GetComponent<SpriteRenderer>().color = c2;
            Text.GetComponent<SpriteRenderer>().sortingOrder = 4;
            if (transform.localScale.x < ExpectedScale)
            {
                transform.localScale = new Vector3
                    (transform.localScale.x + InitScale.x * 0.002f,
                    transform.localScale.y + InitScale.y * 0.002f,
                    transform.localScale.z + InitScale.z * 0.002f);
            }
        }       
    }
    private void OnMouseExit()
    {
        if (!alreadySelect && "BackButton".Equals(gameObject.name))
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            GetComponent<SpriteRenderer>().color = c;
            GetComponent<SpriteRenderer>().sortingOrder = 3;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.r = 1;
            c2.g = 1;
            c2.b = 1;
            Text.GetComponent<SpriteRenderer>().color = c2;
            Text.GetComponent<SpriteRenderer>().sortingOrder = 4;
            transform.localScale = new Vector3(InitScale.x, InitScale.y, InitScale.z);
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
