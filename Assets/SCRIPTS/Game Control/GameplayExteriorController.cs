using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayExteriorController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject BlackFade;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GenerateBlackFadeOpen(transform.position, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate Black Fade
    public void GenerateBlackFadeOpen(Vector2 pos, float duration)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf, duration));
    }

    private IEnumerator BlackFadeOpen(GameObject Fade, float duration)
    {
        for (int i=0;i<50;i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1/50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }
    #endregion
}
