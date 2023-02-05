using System;
using System.Collections;
using System.Collections.Generic;
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
    private static readonly int Speed = Shader.PropertyToID("_Speed");

    public AnimationCurve speedCurve;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            music.Play();
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
    }
}
