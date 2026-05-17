using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsRoller : MonoBehaviour
{
    [Header("Ajustos dels Crèdits")]
    public float speed = 60f; 
    public float timeToEnd = 30f; 

    private float timer = 0f;

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        timer += Time.deltaTime;

        if (timer >= timeToEnd || Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }

    void ReturnToMenu()
    {
        SceneController.ChangeToMenuScene();
    }
}