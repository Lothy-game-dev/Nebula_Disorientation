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
    public GameObject StatusBoard;
    public GameObject ArsenalCategory;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int Section;
    public bool isShowAgain;
    private Dictionary<string, object> PlayerInformation;
    private int PlayerID;
    private bool isNew;
    public bool isStart;
    public bool isInTutorial;
    private bool IsShow;
    public bool isInGameplay;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (FindAnyObjectByType<UECMainMenuController>() != null)
        {
            PlayerID = FindAnyObjectByType<UECMainMenuController>().PlayerId;
        } else
        {
            PlayerID = PlayerPrefs.GetInt("PlayerID");
        }
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerID);

        string[] UpdatedProgress = PlayerInformation["NewPilotTutorial"].ToString().Split("|");
        List<string> ProgressList = new List<string>(UpdatedProgress);
        
        if (ProgressList.IndexOf(Screen) != -1)
        {
            isNew = true;
            ProgressList.RemoveAt(ProgressList.IndexOf(Screen));
            string newProgress = string.Join("|", ProgressList);
            FindAnyObjectByType<AccessDatabase>().UpdateTutorialProgress(PlayerID, newProgress);
        }

        
        Section = 1;
        Tutorial();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Section++;
                Tutorial();
            }

            if (IsShow && Textbox.Count > Section - 1 && BlackBG.Count > Section - 1)
            {
                if (Textbox[Section - 1].GetComponent<SpriteRenderer>().color.a < 1 && Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color.a < 1)
                {
                    Color c = Textbox[Section - 1].GetComponent<SpriteRenderer>().color;
                    c.a += 1/60f;
                    Textbox[Section - 1].GetComponent<SpriteRenderer>().color = c;
                    Color c1 = Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color;
                    c1.a += 1 / 60f;
                    Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
                } 
            }
        }
    }
    #endregion
    #region Show tutorial
    // Group all function that serve the same algorithm
    public void Tutorial()
    {
        if (isNew || isShowAgain)
        {
            GetComponent<BoxCollider2D>().enabled = true;
            // turn off all textbox and black background
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
        
            //Start tutorial
            if (!isInGameplay)
            {
                StopAllCoroutines();
            }
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (Textbox.Count > Section - 1 && BlackBG.Count > Section - 1)
        {
            Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().text = Text[Section - 1];
            Textbox[Section - 1].SetActive(true);
            BlackBG[Section - 1].SetActive(true);
            if (!isInGameplay)
            {
                if (ArsenalCategory != null)
                {
                    ArsenalCategory.GetComponent<ArsenalButton>().Switch("Power");
                }
                StartCoroutine(StartAnimation());
            } else
            {
                if (StatusBoard != null)
                {
                    StatusBoard.SetActive(true);
                }
                Time.timeScale = 0;
                isInTutorial = true;
                IsShow = true;
            }
        } else
        {
            PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerID);           
            Section = 0;
            isShowAgain = false;
            isNew = false; 
            IsShow = false;
            GetComponent<BoxCollider2D>().enabled = false;
            if (StatusBoard != null)
            {
                StatusBoard.SetActive(false);               
            }
            if (Screen == "Gameplay")
            {
                Time.timeScale = 1;
            }
            isInTutorial = false;
        }
       
    }
    #endregion
    #region Animation
    // Group all function that serve the same algorithm
    private IEnumerator StartAnimation()
    {
        for (int i = 0; i < 10; i++)
        {
            Color c = Textbox[Section - 1].GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            Textbox[Section - 1].GetComponent<SpriteRenderer>().color = c;
            Color c1 = Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color;
            c1.a += 0.1f;
            Textbox[Section - 1].transform.GetChild(0).GetComponent<TextMeshPro>().color = c1;
            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion
}
