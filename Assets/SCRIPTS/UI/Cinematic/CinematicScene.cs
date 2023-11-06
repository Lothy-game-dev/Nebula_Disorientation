using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CinematicScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject UEC;
    public GameObject CinematicPart;
    public GameObject BackgroundPart;
    public GameObject SkipButton;
    public string[] DescriptionText;
    public GameObject BlackFade;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int Part;
    private string CurrentText;
    private GameObject Background;
    private GameObject CurrentCinematic;
    public bool isLoading;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Part = 1;
        // Show cinematic if new pilot coming
        if (!UEC.activeSelf && PlayerPrefs.GetString("NewPilot") == "T")
        {
            GenerateBlackFadeOpen(transform.position, 3f);
        } else
        {
            gameObject.SetActive(false);
            UEC.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Cinematic start
    // Group all function that serve the same algorithm
    public void CinematicStart(int part, float duration)
    {
        StartCoroutine(StartBGAnimation(duration));
        StartCoroutine(StartCineAnimation(duration));
        StartCoroutine(ShowSkipButton());
    }
    #endregion
    #region Cinematic Animation
    // Group all function that serve the same algorithm
    private IEnumerator StartCineAnimation(float duration)
    {
        
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < 150; i++)
        {
            CurrentCinematic.transform.localScale = new Vector3(CurrentCinematic.transform.localScale.x + 3/150f, CurrentCinematic.transform.localScale.y, CurrentCinematic.transform.localScale.z);
            yield return new WaitForSeconds(1/300f);
        }
        for (int i = 0; i < CurrentText.Length; i++)
        {
            CurrentCinematic.transform.GetChild(0).GetComponent<TextMeshPro>().text += CurrentText.Substring(i, 1);
            yield return new WaitForSeconds((float)(duration - 2)/ CurrentText.Length);
        }
    }
    private IEnumerator StartBGAnimation(float duration)
    {       
        for (int i = 0; i < 300; i++)
        {
            Background.transform.localScale = new Vector3(Background.transform.localScale.x - 0.2f / 300f, Background.transform.localScale.y - 0.2f / 300f, Background.transform.localScale.z);
            yield return new WaitForSeconds(duration / 300f);
        }
 
    }
    private IEnumerator ShowSkipButton()
    {
        if (Part == 4)
        {
            SkipButton.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Affirmative!";
        }
        yield return new WaitForSeconds(5f);
        SkipButton.SetActive(true);
    }
    #endregion
    #region Black Fade
    public void GenerateBlackFadeOpen(Vector2 pos, float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.transform.SetParent(transform);
        bf.SetActive(true);

        StopAllCoroutines();
        SkipButton.SetActive(false);
        for (int i = 0; i < BackgroundPart.transform.childCount; i++)
        {
            BackgroundPart.transform.GetChild(i).gameObject.SetActive(false);
            CinematicPart.transform.GetChild(i).gameObject.SetActive(false);
        }
        if (CinematicPart.transform.childCount > 0 && BackgroundPart.transform.childCount > 0 && DescriptionText.Length > 0 && Part < 5)
        {
            Background = BackgroundPart.transform.GetChild(Part - 1).gameObject;
            CurrentCinematic = CinematicPart.transform.GetChild(Part - 1).gameObject;
            CurrentText = DescriptionText[Part - 1];
            Background.SetActive(true);
            CurrentCinematic.SetActive(true);
        }
        if (Part > 4)
        {
            Destroy(bf);
            PlayerPrefs.DeleteKey("NewPilot");
            UEC.SetActive(true);
            gameObject.SetActive(false);
        } else
        {
            StartCoroutine(BlackFadeOpen(bf, duration));
        }
    }

    private IEnumerator BlackFadeOpen(GameObject Fade, float duration)
    {
        for (int i = 0; i < 50; i++)
        {
            if (i == 30)
            {
                CinematicStart(Part, 10f);                          
            }                           
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
        isLoading = false;
    }
    public void GenerateBlackFadeClose(float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(transform.position.x, transform.position.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeClose(bf, duration));
    }

    private IEnumerator BlackFadeClose(GameObject Fade, float duration)
    {
        isLoading = true;
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a += 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
        // When the animation done, open the black fade
        GenerateBlackFadeOpen(transform.position, 3f);
    }
    #endregion
}
