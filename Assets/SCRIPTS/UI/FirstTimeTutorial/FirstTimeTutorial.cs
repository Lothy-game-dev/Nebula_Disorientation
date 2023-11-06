using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FirstTimeTutorial : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Textbox;
    public GameObject BlackBG;
    public GameObject FirstSectionPos;
    public GameObject SecondSectionPos;
    public GameObject ThirdSectionPos;
    public GameObject FourthSectionPos;
    public GameObject FirstSectionTextPos;
    public GameObject SecondSectionTextPos;
    public GameObject ThirdSectionTextPos;
    public GameObject FourthSectionTextPos;
    public string[] Text;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int Part;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Part = 1;
        Tutorial();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (Input.GetMouseButtonDown(0))
        {
            Part++;
            Tutorial();
        }
    }
    #endregion
    #region Show tutorial
    // Group all function that serve the same algorithm
    public void Tutorial()
    {
        Color c = Textbox.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        Textbox.GetComponent<SpriteRenderer>().color = c;

        Color c1 = Textbox.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        c1.a = 0;
        Textbox.transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
        
        StopAllCoroutines();
        
        switch(Part)
        {
            case 1: StartTutorial(FirstSectionPos, FirstSectionTextPos); break;
            case 2: StartTutorial(SecondSectionPos, SecondSectionTextPos); break;
            case 3: StartTutorial(ThirdSectionPos, ThirdSectionTextPos); break;
            case 4: StartTutorial(FourthSectionPos, FourthSectionTextPos); break;
            default:
                Textbox.SetActive(false);
                BlackBG.SetActive(false);
                break;
        }
    }

    public void StartTutorial(GameObject BlackBGPos, GameObject TextPos)
    {
        Textbox.transform.position = TextPos.transform.position;
        Textbox.transform.GetChild(0).GetComponent<TextMeshPro>().text = Text[Part - 1];
        Textbox.SetActive(true);
        StartCoroutine(StartAnimation());
        if (Part >= 2)
        {
            BlackBG.transform.position = BlackBGPos.transform.position;
            BlackBG.SetActive(true);
        }
    }
    #endregion
    #region Animation
    // Group all function that serve the same algorithm
    private IEnumerator StartAnimation()
    {
        for (int i = 0; i < 10; i++)
        {
            Color c = Textbox.GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            Textbox.GetComponent<SpriteRenderer>().color = c;
            Color c1 = Textbox.transform.GetChild(0).GetComponent<TextMeshPro>().color;
            c1.a += 0.1f;
            Textbox.transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion
}
