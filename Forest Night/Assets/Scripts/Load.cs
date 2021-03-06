﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    private float Factor;
    private bool down;
    private float Speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        string scene;
        scene = GameObject.Find("LoadingLevelData").GetComponent<LoadingLevelData>().Scene;
        StartCoroutine(LoadScene(scene));
    }

    // Update is called once per frame
    void Update()
    {
        if (down)
        {
            Factor -= Speed * Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), Factor);
            if (Factor < 0)
            {
                Factor = 0;
                down = false;
            }
        }
        else
        {
            Factor += Speed * Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), Factor);
            if (Factor > 1)
            {
                Factor = 1;
                down = true;
            }
        }
    }

    private IEnumerator LoadScene(string s)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(s,LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
