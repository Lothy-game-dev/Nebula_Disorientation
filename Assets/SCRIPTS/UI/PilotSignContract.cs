using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PilotSignContract : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TMP_InputField InputField;
    public GameObject SignButton;
    public GameObject[] Warning;
    public GameObject SignWord;
    public GameObject ScenePos;
    #endregion
    #region NormalVariables
    private float InitScaleX;
    private bool alreadySign;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        if (name == "Sign")
        {
            alreadySign = false;
            InitScaleX = transform.localScale.x;
            transform.localScale = new Vector3(transform.localScale.x / 10f, transform.localScale.y, transform.localScale.z);
            GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine(ButtonAnimation());
        }
        else if (name == "SignInputField")
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0f;
            GetComponent<SpriteRenderer>().color = c;
            StartCoroutine(ButtonAnimation());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (name=="SignInputField")
        {
            if (InputField.isFocused)
            {
                if (InputField.text.Length < 6f || InputField.text.Length > 16f)
                {
                    foreach (var warn in Warning)
                    {
                        if (!warn.activeSelf)
                        {
                            warn.SetActive(true);
                        }
                        Color c = warn.GetComponent<SpriteRenderer>().color;
                        c.g = (6 - InputField.text.Length) / 6;
                        c.b = (6 - InputField.text.Length) / 6;
                        warn.GetComponent<SpriteRenderer>().color = c;
                    }
                    if (SignButton.activeSelf) SignButton.SetActive(false);
                } 
                else
                {
                    foreach (var warn in Warning)
                    {
                        if (warn.activeSelf)
                        {
                            warn.SetActive(false);
                            Color c = warn.GetComponent<SpriteRenderer>().color;
                            c.g = 1;
                            c.b = 1;
                            warn.GetComponent<SpriteRenderer>().color = c;
                        }
                    }
                    if (!SignButton.activeSelf) SignButton.SetActive(true);
                }
                
            }
        }
        else if (name=="Sign" && !alreadySign)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                CheckAndSign();
            }
        }
    }
    #endregion
    #region Sign Contract
    private void OnMouseDown()
    {
        if (name == "Sign" && !alreadySign)
        {
            CheckAndSign();
        }
    }

    private void CheckAndSign()
    {
        if (!alreadySign)
        {
            alreadySign = true;
            string check = "";
            AccessDatabase ad = FindObjectOfType<AccessDatabase>();
            if (ad!=null)
            {
                check = ad.CreateNewPlayerProfile(InputField.text.ToUpper());
            }
            if ("Success".Equals(check))
            {
                string check2 = ad.AddPlaySession(InputField.text.ToUpper());
                if ("Success".Equals(check2))
                {
                    FindObjectOfType<NotificationBoardController>().CreateUECMovingNotiBoard(ScenePos.transform.position,
                    "Contract signed successfully!\nEnjoy your adventure in Nebula Disorientation!\n(Auto close in 5 seconds)", 5f);
                } else if ("No Exist".Equals(check2))
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ScenePos.transform.position,
                    "Pilot does not exist!\n Please try again.", 5f);
                    alreadySign = false;
                } else if ("Fail".Equals(check2))
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ScenePos.transform.position,
                    "Play Session Creation Failed!\n Please try again.", 5f);
                    alreadySign = false;
                }
            } else if ("Exist".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ScenePos.transform.position,
                    "Contract signed failed!\nPilot's Name has already existed.", 5f);
                alreadySign = false;
            } else if ("Fail".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ScenePos.transform.position,
                    "Contract signed failed!\nPlease try again.", 5f);
                alreadySign = false;
            }
        }
    }

    private IEnumerator ButtonAnimation()
    {
        if (name=="Sign")
        {
            for (int i = 0; i < 10; i++)
            {
                if (transform.localScale.x < InitScaleX)
                {
                    transform.localScale = new Vector3(transform.localScale.x + InitScaleX / 10f, transform.localScale.y, transform.localScale.z);
                    yield return new WaitForSeconds(0.1f);
                }
                else break;
            }
            SignWord.SetActive(true);
            GetComponent<BoxCollider2D>().enabled = true;
        } else if (name == "SignInputField")
        {
            for (int i = 0; i < 10; i++)
            {
                Color c = GetComponent<SpriteRenderer>().color;
                c.a += 0.1f;
                GetComponent<SpriteRenderer>().color = c;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
    #endregion
}
