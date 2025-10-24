using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SlideshowViewer : MonoBehaviour
{
    [SerializeField] private Image slideImage;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private SlideData slideshow;

    [SerializeField] private RectTransform interactiveContainer;
    [SerializeField] private GameObject simulationButtonPrefab;
    [SerializeField] private GameObject hearMoreButtonPrefab;
    [SerializeField] private TextMeshProUGUI hearMoreTextBox;

    [Header("UI Animation")]
    [SerializeField] private float progressFillSpeed = 0.3f;

    private int currentSlide = 0;
    private List<GameObject> activeButtons = new List<GameObject>();

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
        if (slideshow.slides.Count > 0)
            ShowSlide(0);

        nextButton.onClick.AddListener(NextSlide);
        prevButton.onClick.AddListener(PrevSlide);
    }

    void ShowSlide(int index)
    {
        currentSlide = Mathf.Clamp(index, 0, slideshow.slides.Count - 1);
        var slide = slideshow.slides[currentSlide];

        slideImage.sprite = slide.slideImage;
        StartCoroutine(SmoothFill((float)(currentSlide + 1) / slideshow.slides.Count));

        ClearButtons();

        foreach (var element in slide.elements)
        {
            GameObject prefab = element.type == SlideData.InteractiveElement.ElementType.Simulation
                ? simulationButtonPrefab
                : hearMoreButtonPrefab;

            GameObject button = Instantiate(prefab, interactiveContainer);
            Button btn = button.GetComponent<Button>();

            if (element.type == SlideData.InteractiveElement.ElementType.Simulation)
            {
                btn.onClick.AddListener(() =>
                    SimulationManager.Instance.LoadSimulation(element.targetID, btn.gameObject));

            }
            else if (element.type == SlideData.InteractiveElement.ElementType.HearMore)
            {
                btn.onClick.AddListener(() =>
                    HearMoreManager.Instance.LoadHearMore(element.targetID, hearMoreTextBox, btn));
            }

            activeButtons.Add(button);
        }
    }

    void ClearButtons()
    {
        foreach (var btn in activeButtons)
            Destroy(btn);
        activeButtons.Clear();
    }


    public void NextSlide()
    {
        if (currentSlide < slideshow.slides.Count - 1 && !SimulationManager.Instance.isSimulating)
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