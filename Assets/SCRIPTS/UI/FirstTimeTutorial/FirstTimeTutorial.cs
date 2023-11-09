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
    public string Screen;
    public List<GameObject> Textbox;
    public List<GameObject> BlackBG;
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
        Part = 0;
        Tutorial();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (Input.GetMouseButtonDown(0))
        {          
            Tutorial();
        }
    }
    #endregion
    #region Show tutorial
    // Group all function that serve the same algorithm
    public void Tutorial()
    {
        if (/*PlayerPrefs.GetString(Screen) == "T"*/ 1==1)
        {
            Part++;
            for (int i = 0; i < Textbox.Count; i++)
            {
                Color c = Textbox[i].GetComponent<SpriteRenderer>().color;
                c.a = 0;
                Textbox[i].GetComponent<SpriteRenderer>().color = c;

                Color c1 = Textbox[i].transform.GetChild(0).GetComponent<TextMeshPro>().color;
                c1.a = 0;
                Textbox[i].transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
                Textbox[i].SetActive(false);
            }

            for (int i = 0; i < BlackBG.Count; i++)
            {
                BlackBG[i].SetActive(false);
            }
        
            StopAllCoroutines();
        
            switch(Part)
            {
                case 1: StartTutorial(); break;
                case 2: StartTutorial(); break;
                case 3: StartTutorial(); break;
                case 4: StartTutorial(); break;
                default:                   
                    PlayerPrefs.SetString(Screen, "F");
                    Part = 0;
                    break;
            }
        }
    }

    public void StartTutorial()
    {
        Textbox[Part - 1].transform.GetChild(0).GetComponent<TextMeshPro>().text = Text[Part - 1];
        Textbox[Part - 1].SetActive(true);
        BlackBG[Part - 1].SetActive(true);
        StartCoroutine(StartAnimation());
       
    }
    #endregion
    #region Animation
    // Group all function that serve the same algorithm
    private IEnumerator StartAnimation()
    {
        for (int i = 0; i < 10; i++)
        {
            Color c = Textbox[Part - 1].GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            Textbox[Part - 1].GetComponent<SpriteRenderer>().color = c;
            Color c1 = Textbox[Part - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color;
            c1.a += 0.1f;
            Textbox[Part - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion
}
