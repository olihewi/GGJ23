using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public bool isDefaultGoal = false;
    public string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        StageManager.Instance.LoadScene(nextScene);
    }

    private void Update()
    {
        if (isDefaultGoal && Input.GetButtonDown("Skip"))
        {
            StageManager.Instance.LoadScene(nextScene);
        }
    }
}
