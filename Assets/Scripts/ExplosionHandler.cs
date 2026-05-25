using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    [SerializeField] float duration = 0.5f;
    [SerializeField] AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    float time = 0f;
    float radius = 1f;
    Renderer rend;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        transform.localScale = Vector3.zero;
        AudioManager.instance?.PlayExplosion();  
    }

    void Update()
    {
        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);

        // Creix ràpid i s'estabilitza
        float s = scaleCurve.Evaluate(t) * radius;
        transform.localScale = new Vector3(s, s, s);

        // Es fon a la segona meitat
        if (rend != null)
        {
            Color c = rend.material.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            rend.material.color = c;
        }

        if (time > duration) Destroy(gameObject);
    }

    public void setRadius(float r) { radius = r; }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.GetComponent<PlayerHandler>().takeDamage(1, Direction.NONE);
        else
        {
            IEnemy ie = other.GetComponent<IEnemy>() ?? other.GetComponentInParent<IEnemy>();
            if (ie != null) ie.takeDamage(Direction.NONE);
        }
    }
}