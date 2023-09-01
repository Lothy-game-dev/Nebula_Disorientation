using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject AoE;
    public float time;
    #endregion
    #region NormalVariables
    private float AoEInitRange;
    private float currentTransparency;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        AoEInitRange = (AoE.transform.position - AoE.transform.GetChild(0).position).magnitude;
        currentTransparency = AoE.GetComponent<SpriteRenderer>().color.a;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Create Area Of Effect
    public void CreateAreaOfEffect(Vector2 position, float AoERange)
    {
        GameObject go = Instantiate(AoE, position, Quaternion.identity);
        go.transform.localScale = new Vector3(
            go.transform.localScale.x * AoERange / AoEInitRange,
            go.transform.localScale.y * AoERange / AoEInitRange,
            go.transform.localScale.z);
        float EndScale = go.transform.localScale.x;
        go.transform.localScale = new Vector3(
            go.transform.localScale.x/5,
            go.transform.localScale.y/5,
            go.transform.localScale.z);
        Color c = go.GetComponent<SpriteRenderer>().color;
        c.a = currentTransparency / 5;
        go.GetComponent<SpriteRenderer>().color = c;
        go.SetActive(true);
        StartCoroutine(AoEEffect(EndScale, go));
    }

    private IEnumerator AoEEffect(float EndScale, GameObject AoEGO)
    {
        for (int i=0;i<10;i++)
        {
            Color c = AoEGO.GetComponent<SpriteRenderer>().color;
            c.a += currentTransparency * 2/25;
            AoEGO.GetComponent<SpriteRenderer>().color = c;
            AoEGO.transform.localScale = new Vector3(
            AoEGO.transform.localScale.x + EndScale * 2/25,
            AoEGO.transform.localScale.y + EndScale * 2/25,
            AoEGO.transform.localScale.z);
            yield return new WaitForSeconds(time / 10);
        }
        Destroy(AoEGO);
    }
    #endregion
}
