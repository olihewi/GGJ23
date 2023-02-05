using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public string stageName;
    public float stageTime = 120.0F;
    [NonSerialized] public int maxCollectibles;
    [NonSerialized] public int collectibles = 0;

    [NonSerialized] public float currentTime;

    private void Awake()
    {
        Instance = this;
        maxCollectibles = FindObjectsOfType<Collectible>().Length;
    }

    private void Start()
    {
        currentTime = stageTime;
        UI.Instance.stageText.text = stageName;
    }

    private void Update()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0.0F)
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Restart")) RestartLevel();
    }

    public void RestartLevel()
    {
        LoadScene(gameObject.scene.name);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // transition in
        SceneManager.LoadScene(sceneName);
        // transition out
        yield break;
    }

    public void Collect(Collectible c)
    {
        collectibles++;
    }
}
