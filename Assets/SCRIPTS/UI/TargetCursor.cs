using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCursor : MonoBehaviour
{
    #region ComponentVariables

    #endregion
    #region InitializeVariables
    public Camera camera;
    public GameObject AimLeft;
    public GameObject AimRight;
    public float GlowTime;
    #endregion
    #region NormalVariables
    private Vector3 MousePos;
    private bool LeftGlowing;
    private bool RightGlowing;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        LeftGlowing = false;
        RightGlowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Change position of target to the same position as cursor
        MousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        MousePos.z = 0;
        transform.position = MousePos;
        // Check Mouse Input Left Right To Glow Up the right part of cursor
        // Case Left Button Press
        if (Input.GetMouseButtonDown(0))
        {
            // If the previous glow not found/has complete -> glow up from 0 to 1 and down from 1 to 0
            if (!LeftGlowing)
            {
                LeftGlowing = true;
                StartCoroutine(GlowCursor(AimLeft));
                LeftGlowing = false;
            }
            // If not: glow up the current to max and then down
            else
            {
                StartCoroutine(GlowUpWhileGlowing(AimLeft));
                LeftGlowing = false;
            }
        }
        // Case Left Button Hold
        // Case Right Button Press
        if (Input.GetMouseButtonDown(1))
        {
            // If the previous glow not found/has complete -> glow up from 0 to 1 and down from 1 to 0
            if (!RightGlowing)
            {
                RightGlowing = true;
                StartCoroutine(GlowCursor(AimRight));
                RightGlowing = false;
            }
            // If not: glow up the current to max and then down
            else
            {
                StartCoroutine(GlowUpWhileGlowing(AimRight));
                RightGlowing = false;
            }
        }
        // Case Right Button Hold
    }
    #endregion
    #region Glow Cursor
    // Glow Up/Down Cursor by increase transparent of the object
    IEnumerator GlowCursor(GameObject cursor)
    {
        for (int i=0; i<10; i++)
        {
            Color c = cursor.GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            cursor.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(GlowTime/10f);
        }
        for (int i = 0; i < 10; i++)
        {
            Color c = cursor.GetComponent<SpriteRenderer>().color;
            c.a -= 0.1f;
            cursor.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(GlowTime / 10f);
        }
    }
    // Glow Up Cursor While Previous Glowing Duration Still On
    IEnumerator GlowUpWhileGlowing(GameObject cursor)
    {
        Color color = cursor.GetComponent<SpriteRenderer>().color;
        int a = (int)(1 - color.a) * 10;
        for (int i=0; i<a; i++)
        {
            Color c = cursor.GetComponent<SpriteRenderer>().color;
            c.a += 0.1f;
            cursor.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(GlowTime / 10f);
        }
        for (int i = 0; i < 10; i++)
        {
            Color c = cursor.GetComponent<SpriteRenderer>().color;
            c.a -= 0.1f;
            cursor.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(GlowTime / 10f);
        }
    }
    #endregion
}
