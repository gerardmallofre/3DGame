using UnityEngine;

public class CreditsRoller : MonoBehaviour
{
    [Header("Ajustos dels Crèdits")]
    [Tooltip("Velocitat basada en l'escala del Canvas. Un valor entre 50 i 150 sol funcionar bé.")]
    public float speed = 100f;
    public float timeToEnd = 30f;

    private RectTransform rectTransform;
    private Canvas scalerCanvas;
    private float timer = 0f;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        scalerCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        float currentScale = scalerCanvas != null ? scalerCanvas.transform.localScale.y : 1f;

        rectTransform.anchoredPosition += Vector2.up * (speed * currentScale) * Time.deltaTime;

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