using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    #region ComponentVariables
    private Rigidbody2D rb;
    #endregion
    #region InitializeVariables
    public GameObject Text;
    public GameObject BeforePosition;
    public GameObject AfterPosition;
    public GameObject[] OtherButtons;
    public GameObject AfterMenuSelectPosition;
/*    public GameObject CamPosAfterMove;
    public GameObject CamPosBeforeMove;
    public GameObject MainCam;*/
    #endregion
    #region NormalVariables
    private float InitScale;
    private float ExpectedScale;
    private bool alreadySelect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        InitScale = transform.localScale.x;
        ExpectedScale = transform.localScale.x * 1.1f;
        EnterView();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (transform.position.x > AfterPosition.transform.position.x)
        {
            rb.velocity = new Vector2(0, 0);
            transform.position = AfterPosition.transform.position;
        }
    }
    #endregion
    #region Mouse Check
    private void OnMouseEnter()
    {
    }

    private void OnMouseOver()
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
                (transform.localScale.x + InitScale * 0.002f, 
                transform.localScale.y + InitScale * 0.002f, 
                transform.localScale.z + InitScale * 0.002f);
        }
    }
    private void OnMouseExit()
    {
        if (!alreadySelect)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            GetComponent<SpriteRenderer>().color = c;
            GetComponent<SpriteRenderer>().sortingOrder = 1;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.r = 1;
            c2.g = 1;
            c2.b = 1;
            Text.GetComponent<SpriteRenderer>().color = c2;
            Text.GetComponent<SpriteRenderer>().sortingOrder = 2;
            transform.localScale = new Vector3(InitScale, InitScale, InitScale);
        }
    }

    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (!alreadySelect)
        {
            alreadySelect = true;
            foreach (var but in OtherButtons)
            {
                but.GetComponent<MainMenuButtons>().ExitView();
            }
            if (name== "ExitButton")
            {
                FindObjectOfType<NotificationBoardController>().VoidReturnFunction = QuitGame;
                FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(transform.parent.position, "Exit Already?");
            } else
            StartCoroutine(DelayTransfer());
        }
    }
    #endregion
    #region Enter and Exit
    public void QuitGame()
    {
        Application.Quit();
    }
    private IEnumerator DelayTransfer()
    {
        bool isDown = true;
        for (int i=0;i<20;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            if (c.a >= 1)
            {
                isDown = true;
            } else if (c.a <= 0)
            {
                isDown = false;
            }
            if (isDown)
            {
                c.a -= 0.5f;
                c2.a -= 0.5f;
            } else
            {
                c.a += 0.5f;
                c2.a += 0.5f;
            }
            GetComponent<SpriteRenderer>().color = c;
            Text.GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(0.1f);
        }
        FindObjectOfType<MainMenuCameraController>().ChangeToScene(gameObject.transform.parent.gameObject, AfterMenuSelectPosition);
    }

    public void EnterView()
    {
        alreadySelect = false;
        GetComponent<Collider2D>().enabled = false;
        Color c = GetComponent<SpriteRenderer>().color;
        c.a = 0;
        c.r = 1;
        c.g = 1;
        c.b = 1;
        GetComponent<SpriteRenderer>().color = c;
        Color c2 = Text.GetComponent<SpriteRenderer>().color;
        c2.a = 0;
        c2.r = 1;
        c2.g = 1;
        c2.b = 1;
        Text.GetComponent<SpriteRenderer>().color = c2;
        transform.position = BeforePosition.transform.position;
        transform.localScale = new Vector3(InitScale, InitScale, InitScale);
        StartCoroutine(MoveToAfterPos());
    }

    private IEnumerator MoveToAfterPos()
    {
        if (name.Equals("LoadStoryButton"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        else if (name.Equals("EncyclopediaButton"))
        {
            yield return new WaitForSeconds(0.2f);
        }
        else if (name.Equals("TutorialButton"))
        {
            yield return new WaitForSeconds(0.3f);
        }
        else if (name.Equals("OptionsButton"))
        {
            yield return new WaitForSeconds(0.4f);
        }
        else if (name.Equals("ExitButton"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        rb.velocity = (AfterPosition.transform.position - BeforePosition.transform.position) / 0.5f;
        for (int i = 0; i < 10; i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            GetComponent<SpriteRenderer>().color = c;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.a += 0.1f;
            Text.GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(0.05f);
        }
        rb.velocity = new Vector2(0, 0);
        transform.position = AfterPosition.transform.position;
        if (name.Equals("NewStoryButton"))
        {
            yield return new WaitForSeconds(0.5f);
        }
        else if (name.Equals("LoadStoryButton"))
        {
            yield return new WaitForSeconds(0.4f);
        }
        else if (name.Equals("EncyclopediaButton"))
        {
            yield return new WaitForSeconds(0.3f);
        }
        else if (name.Equals("TutorialButton"))
        {
            yield return new WaitForSeconds(0.2f);
        }
        else if (name.Equals("OptionsButton"))
        {
            yield return new WaitForSeconds(0.1f);
        }
        GetComponent<Collider2D>().enabled = true;
    }

    public void ExitView()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(MoveToBeforePos());
    }

    private IEnumerator MoveToBeforePos()
    {
        rb.velocity = (BeforePosition.transform.position - AfterPosition.transform.position)/4f;
        for (int i = 0; i < 5; i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= 0.2f;
            GetComponent<SpriteRenderer>().color = c;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.a -= 0.2f;
            Text.GetComponent<SpriteRenderer>().color = c2;
            yield return new WaitForSeconds(0.2f);
        }
        rb.velocity = new Vector2(0, 0);
    }


    #endregion
}
