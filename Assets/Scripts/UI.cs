using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public TMP_Text speedometer;
    public TMP_Text collectibleCounter;
    public Renderer speedLines;
    private static readonly int Speed = Shader.PropertyToID("_Speed");

    private void Update()
    {
        speedometer.text = $"{PlayerBall.Instance._rigidbody.velocity.magnitude:F0} cm/s";
        collectibleCounter.text =
            $"{StageManager.Instance.maxCollectibles}/{StageManager.Instance.collectibles}";
        speedLines.material.SetFloat(Speed, PlayerBall.Instance._rigidbody.velocity.magnitude);
    }
}
