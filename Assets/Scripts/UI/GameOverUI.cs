using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private bool gameOver = false;

    public GameObject ltDisplay;
    public LaptimeDisplay ltDisplayscript;

    public Image fadePlane;
    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<LapCounter>().OnRaceEnd += OnGameOver;
    }

    void OnGameOver()
    {
        ltDisplay.SetActive(true);
        ltDisplayscript.DisplayText();
        if(gameOver==false)
        StartCoroutine(Fade (Color.clear, Color.black, 1));
    }
    IEnumerator Fade(Color from, Color to, float time) {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from,to,percent);
            yield return null;
        }
    }
    
}
