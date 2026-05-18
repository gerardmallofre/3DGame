using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems; 

public class MenuManager : MonoBehaviour
{
    [Header("Navegació")]
    public GameObject botoPerDefecte; 

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(botoPerDefecte);
        }
    }

    public void Play()
    {
        StartCoroutine(LoadWithDelay(SceneController.ChangeToGameScene));
    }

    public void Credits()
    {
        StartCoroutine(LoadWithDelay(SceneController.ChangeToCreditsScene));
    }

    public void Exit()
    {
        StartCoroutine(ExitWithDelay());
    }

    private IEnumerator LoadWithDelay(Action funcioACarregar)
    {
        StartCoroutine(AudioManager.instance.FadeOut(AudioManager.instance.musicaFonsSource, 0.5f));
        yield return new WaitForSeconds(0.3f);
        funcioACarregar?.Invoke();
    }

    private IEnumerator ExitWithDelay()
    {
        yield return new WaitForSeconds(0.3f);

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}