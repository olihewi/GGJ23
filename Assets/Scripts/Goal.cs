using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        StageManager.Instance.LoadScene(nextScene);
    }
}
