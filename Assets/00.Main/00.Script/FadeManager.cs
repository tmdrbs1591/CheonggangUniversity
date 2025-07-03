using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager instance;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        instance = this;

        if (fadeImage != null)
            SetAlpha(0f);

        FadeOut();
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(0f, 1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(1f, 0f));
    }

    public void FadeInOut(float waitTime = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInOutCoroutine(waitTime));
    }

    private IEnumerator FadeCoroutine(float from, float to)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, timer / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(to);
    }

    private IEnumerator FadeInOutCoroutine(float waitTime)
    {
        yield return StartCoroutine(FadeCoroutine(0f, 1f)); // Fade In
        yield return new WaitForSeconds(waitTime);          // Hold
        yield return StartCoroutine(FadeCoroutine(1f, 0f)); // Fade Out
    }
}
