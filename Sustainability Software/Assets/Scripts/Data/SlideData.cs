using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSlideshow", menuName = "Customs/Slideshow")]
public class SlideData : ScriptableObject
{
    public SustainabilityPillar slideshowType;
    [Serializable]
    public class Slide
    {
        public Sprite slideImage;
        public List<InteractiveElement> elements;
    }

    [Serializable]
    public class InteractiveElement
    {
        public enum ElementType { Simulation, HearMore }
        public ElementType type;
        public string targetID;
    }

    public List<Slide> slides;
}