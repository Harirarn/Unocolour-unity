﻿using UnityEngine;
using UnityEngine.UI;

public class AchievementIcon : MonoBehaviour
{

    public AchievementDescription achievement;
    private bool _active, _isAvailable;
    private AchievementPanel panel;
    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            _active = value;
            SetSpriteActive(value);
        }
    }
    public bool isAvailable
    {
        get
        {
            return _isAvailable;
        }
        set
        {
            _isAvailable = value;
            available.SetActive(_isAvailable);
        }
    }
    public Sprite iconSprite;
    public Sprite fadedIconSprite;
    public GameObject icon;
    public GameObject available;

    private Image backImage;
    private Image iconImage;

    public void Init(Sprite iconSprite, Sprite fadedIconSprite, AchievementDescription achievement, AchievementPanel panel)
    {
        this.iconSprite = iconSprite;
        this.fadedIconSprite = fadedIconSprite;
        this.achievement = achievement;
        this._active = achievement.completed;
        this.panel = panel;
        isAvailable = false;
        Start();
    }

    private void SetSpriteActive(bool active)
    {
        iconImage.sprite = active ? iconSprite : fadedIconSprite;
        backImage.color = active ? Color.white : Color.gray;
    }

    public void onClick()
    {
        panel.showDescription(achievement.name);
    }

    // Use this for initialization
    void Start()
    {
        backImage = GetComponent<Image>();
        iconImage = icon.GetComponent<Image>();
        SetSpriteActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        SetSpriteActive(active);
    }
}
