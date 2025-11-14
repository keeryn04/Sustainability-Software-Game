using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SlideshowViewer : MonoBehaviour
{
    [SerializeField] private List<SlideData> slideshows = new List<SlideData>();
    [SerializeField] private Image slideImage;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    public SlideData slideshow;

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
        if (slideshow != null && slideshow.slides.Count > 0)
            ShowSlide(0);

        nextButton.onClick.AddListener(NextSlide);
        prevButton.onClick.AddListener(PrevSlide);
    }

    public void SetSlideshow(SustainabilityPillar slideType)
    {
        SlideData newSlideshow = slideshows
            .Find(s => s.slideshowType == slideType);
        slideshow = newSlideshow;
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
        //Can't move to next slide if talking or simulating
        if (currentSlide < slideshow.slides.Count - 1 && CanChangeSlide())
            ShowSlide(currentSlide + 1);
    }

    public void PrevSlide()
    {
        //Can't move to past slide if talking or simulating
        if (currentSlide > 0 && CanChangeSlide())
            ShowSlide(currentSlide - 1);
    }

    private bool CanChangeSlide()
    {
        return !SimulationManager.Instance.isSimulating && !DialogueManager.Instance.isTalking;
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