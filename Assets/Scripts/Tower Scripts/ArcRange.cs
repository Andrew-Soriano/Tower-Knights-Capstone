using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ArcRange : MonoBehaviour
{
    private float innerRadius = 0f;
    private float outerRadius = 5f;
    private float startAngle = -45f;
    private float endAngle = 45f;

    public int segments = 64;
    
    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;
    private Material _mat;
    private Coroutine currentFade;

    private Mesh mesh;

    private void Awake()
    {
        _mat = GetComponent<MeshRenderer>().material;
        SetAlpha(0f);
    }

    public void SetArc(float range, float angle)
    {
        outerRadius = range;
        startAngle = -angle * 0.5f;
        endAngle = angle * 0.5f;

        Generate();
    }

    public void Generate()
    {
        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ArcMesh";
            GetComponent<MeshFilter>().mesh = mesh;
        }
        else mesh.Clear();

        float arcSpan = endAngle - startAngle;
        float angleStep = arcSpan / segments;

        int vertCount = (segments + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[segments * 6];

        int v = 0;
        for (int i = 0; i <= segments; i++)
        {
            float angle = startAngle + angleStep * i;
            float rad = Mathf.Deg2Rad * angle;

            float z = Mathf.Cos(rad);
            float x = Mathf.Sin(rad);

            vertices[v++] = new Vector3(x * innerRadius, 0f, z * innerRadius);
            vertices[v++] = new Vector3(x * outerRadius, 0f, z * outerRadius);
        }

        int t = 0;
        for (int i = 0; i < segments; i++)
        {
            int i0 = i * 2;
            int i1 = i0 + 1;
            int i2 = i0 + 2;
            int i3 = i0 + 3;

            triangles[t++] = i0;
            triangles[t++] = i1;
            triangles[t++] = i3;

            triangles[t++] = i0;
            triangles[t++] = i3;
            triangles[t++] = i2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    public void FadeIn()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeTo(1f));
    }

    public void FadeOut()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float start = _mat.color.a;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float a = Mathf.Lerp(start, targetAlpha, t);
            SetAlpha(a);
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float a)
    {
        Color c = _mat.color;
        c.a = a;
        _mat.color = c;
    }

    public Coroutine AnimateRangeChange(MonoBehaviour runner, float newRange, float duration = 0.35f)
    {
        return runner.StartCoroutine(AnimateRangeRoutine(newRange, duration));
    }

    private IEnumerator AnimateRangeRoutine(float newRange, float duration)
    {
        float startRadius = outerRadius;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float r = Mathf.Lerp(startRadius, newRange, Mathf.SmoothStep(0f, 1f, t));
            outerRadius = r;
            Generate();
            yield return null;
        }

        outerRadius = newRange;
        Generate();
    }
}