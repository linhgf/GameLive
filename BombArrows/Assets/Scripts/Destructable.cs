using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public GameObject solidObject;
    public GameObject pieces;

    public Material fadeMaterial;
    public float fadeWaitingTime;

    Collider mainCollider;

    float fadeValue;
    const string FADEVALUENAME = "_FadeValue";

    void Start() 
    {
        fadeValue = 1;
        fadeMaterial.SetFloat(FADEVALUENAME, fadeValue);
        solidObject.SetActive(true);
        pieces.SetActive(false);
        mainCollider = GetComponent<Collider>();
    }

    public void Explode() 
    {   
        if(solidObject != null)
        {
            mainCollider.enabled = false;
            solidObject.SetActive(false);
            Destroy(solidObject);
            pieces.SetActive(true);
            Fade();  
        }
    }

    void Fade()
    {
        StartCoroutine(WaitFadeAndDestroy());
    }


    IEnumerator WaitFadeAndDestroy(){
        yield return new WaitForSeconds(fadeWaitingTime);
        while(fadeMaterial.GetFloat(FADEVALUENAME) > 0)
        {
            fadeValue -= 0.01f;
            fadeMaterial.SetFloat(FADEVALUENAME, fadeValue);
            yield return null;
        }

        fadeValue = 0;
        fadeMaterial.SetFloat(FADEVALUENAME, fadeValue);
        Destroy(gameObject);
    }
}
