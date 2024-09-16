using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float LevelLength = 30f;
    public static GameManager Instance;
    public float timer;
    public Text TextNode;
    public Text Tips;
    public GameObject TipBG;
    private Coroutine hideTip;

    public void HandleTimerChange(float delta)
    {
        timer += delta;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        timer = LevelLength;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        TextNode.text = "Time Left: " + ((int)timer).ToString();
        if (timer <= 0)
        {
            SceneManager.LoadScene(3);
        }
    }

    IEnumerator HideTips()
    {
        yield return new WaitForSeconds(5);
        Tips.text = "";
        TipBG.SetActive(false);
    }
    public void HandleShowTips(string tips)
    {
        TipBG.SetActive(true);
        Tips.text = tips;
        if (hideTip != null) StopCoroutine(hideTip);
        StartCoroutine(HideTips());
    }
}
