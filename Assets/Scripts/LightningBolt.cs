using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public Transform startPoint;
    public Transform target;
    public int segments = 12;
    public float jaggedness = 0.5f;
    public float lifetime = 0.2f;
    public float fadeDuration = 0.5f;

    private LineRenderer lr;
    private float timer;
    private bool fading = false;
    private Color initialColor;

    public void Initialize(Transform start, Transform target, int segments = 12, float jaggedness = 0.5f, float lifetime = 0.2f, float fadeDuration = 0.5f)
    {
        this.startPoint = start;
        this.target = target;
        this.segments = segments;
        this.jaggedness = jaggedness;
        this.lifetime = lifetime;
        this.fadeDuration = fadeDuration;

        lr = GetComponent<LineRenderer>();
        lr.positionCount = segments;
        lr.widthMultiplier = 0.5f;
        initialColor = lr.material.color;

        DrawLightning();
        timer = lifetime;
    }

    private void DrawLightning()
    {
        if (startPoint == null || target == null) return;

        Vector3 step = (target.position - startPoint.position) / (segments - 1);

        for (int i = 0; i < segments; i++)
        {
            Vector3 pos = startPoint.position + step * i;
            if (i != 0 && i != segments - 1)
                pos += Random.insideUnitSphere * jaggedness;

            lr.SetPosition(i, pos);
        }
    }

    void Update()
    {
        if (startPoint == null || target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!fading)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                fading = true;
                timer = fadeDuration;
            }
        }
        else
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Clamp01(timer / fadeDuration);
            lr.material.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);

            if (alpha <= 0f)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}