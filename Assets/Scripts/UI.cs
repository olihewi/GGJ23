using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public TMP_Text speedometer;
    public TMP_Text collectibleCounter;
    public Renderer speedLines;
    public TMP_Text timer;
    public TMP_Text stageText;
    public AudioSource music;
    public GameObject pauseMenu;
    private static readonly int Speed = Shader.PropertyToID("_Speed");

    public AnimationCurve speedCurve;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            music.Play();
            ShowPauseMenu(false);
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }

    private void Update()
    {
        speedometer.text = $"{PlayerBall.Instance._rigidbody.velocity.magnitude:F0} cm/s";
        collectibleCounter.text =
            $"{StageManager.Instance.collectibles}/{StageManager.Instance.maxCollectibles}";
        speedLines.material.SetFloat(Speed, speedCurve.Evaluate(PlayerBall.Instance._rigidbody.velocity.magnitude));
        timer.text = $"{StageManager.Instance.currentTime:F2}s";
        if (Input.GetButtonDown("Pause")) TogglePauseMenu();
    }
    
    public void RestartLevel() => StageManager.Instance.RestartLevel();
    public void ExitGame() => Application.Quit();

    public void ExitToTitleScreen()
    {
        
    }

    public void ShowPauseMenu(bool shown = true)
    {
        pauseMenu.gameObject.SetActive(shown);
        if (shown) music.Pause();
        else music.Play();
        Time.timeScale = shown ? 0.0F : 1.0F;
        Cursor.visible = shown;
        Cursor.lockState = shown ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void TogglePauseMenu() => ShowPauseMenu(!pauseMenu.gameObject.activeSelf);

    public void SkipLevel()
    {
        StageManager.Instance.LoadScene(FindObjectsOfType<Goal>().First(x => x.isDefaultGoal).nextScene);
    }

    public bool Paused => pauseMenu.activeSelf;
}
