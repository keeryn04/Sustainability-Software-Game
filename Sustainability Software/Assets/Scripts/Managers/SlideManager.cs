using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideshowViewer : MonoBehaviour
{
    [SerializeField] private Image slideImage;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private SlideData slideshow;

    [Header("UI Animation")]
    [SerializeField] private float progressFillSpeed = 0.3f;

    private int currentSlide = 0;

    public static SlideshowViewer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //Destroy duplicate instances
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (slideshow.slides.Length > 0)
            ShowSlide(0);

        nextButton.onClick.AddListener(NextSlide);
        prevButton.onClick.AddListener(PrevSlide);
    }

    void ShowSlide(int index)
    {
        currentSlide = Mathf.Clamp(index, 0, slideshow.slides.Length - 1);
        slideImage.sprite = slideshow.slides[currentSlide];
        StartCoroutine(SmoothFill((float)(currentSlide + 1) / slideshow.slides.Length));
    }

    public void NextSlide()
    {
        if (currentSlide < slideshow.slides.Length - 1 && !SimulationManager.Instance.isSimulating)
            ShowSlide(currentSlide + 1);
    }

    public void PrevSlide()
    {
        if (currentSlide > 0 && !SimulationManager.Instance.isSimulating)
            ShowSlide(currentSlide - 1);
    }

    IEnumerator SmoothFill(float targetValue)
    {
        float startValue = progressBar.value;
        float elapsed = 0f;

        while (elapsed < progressFillSpeed)
        {
            elapsed += Time.deltaTime;
            progressBar.value = Mathf.Lerp(startValue, targetValue, elapsed / progressFillSpeed);
            yield return null;
        }

        progressBar.value = targetValue;
    }
}