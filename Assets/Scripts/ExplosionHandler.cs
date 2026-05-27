using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    [SerializeField] float duration = 0.75f;
    [SerializeField] AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] GameObject vfx;
    float time = 0f;
    float radius = 1f;
    Renderer rend;
    Vector3 baseScale=new Vector3(1, 1, 1);

    void Awake()
    {
        GameObject obj = Instantiate(vfx, transform.position, transform.rotation);
        obj.transform.parent = transform;
    }

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
        if (time < duration * 2 / 3)
        {
            float s = scaleCurve.Evaluate(t) * radius;
            transform.localScale = baseScale*s;
            float h, v;
            Color.RGBToHSV(rend.material.color, out h, out s, out v);
            h = h - Time.deltaTime/3;
            v = v - Time.deltaTime/3;
            Color c = Color.HSVToRGB(h, s, v);
            rend.material.color = c;
        }

        // Es fon a la segona meitat
        else if (rend != null)
        {
            Color c = rend.material.color;
            c.a = 1-(time-duration*2/3)/(duration*2/3);
            rend.material.color = c;
        }

        if (time > duration) Destroy(gameObject);
    }

    public void setRadius(float r) { radius = r; }

    public void setScale(Vector3 s) { baseScale = s; }

    public void setDuration(float d) { duration = d; }

    void OnTriggerEnter(Collider other)
    {
        if (time < duration * 2 / 3)
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
}