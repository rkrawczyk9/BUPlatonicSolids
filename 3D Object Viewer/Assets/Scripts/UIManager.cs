using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject controlsText;
    [SerializeField] Text isSolText, isntSolText;
    readonly float fadeDur_isIsntSol = 3;
    float timeSinceShowing_isSol = 999, timeSinceShowing_isntSol = 999;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceShowing_isSol += Time.deltaTime;
        SetTextAlpha(isSolText, Mathf.Clamp01((fadeDur_isIsntSol - timeSinceShowing_isSol)/fadeDur_isIsntSol));

        timeSinceShowing_isntSol += Time.deltaTime;
        SetTextAlpha(isntSolText, Mathf.Clamp01((fadeDur_isIsntSol - timeSinceShowing_isntSol) / fadeDur_isIsntSol));
    }

    public void HideControlsButton()
    {
        controlsText.SetActive(!controlsText.activeSelf);
    }

    public void IsSolution()
    {
        timeSinceShowing_isSol = 0;
    }
    public void IsntSolution()
    {
        timeSinceShowing_isntSol = 0;
    }

    void SetTextAlpha(Text text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }
}
