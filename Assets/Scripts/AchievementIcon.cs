using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AchievementIcon: MonoBehaviour {

	public string achievement;
    private bool _active;
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
	public Sprite iconSprite;
    public Sprite fadedIconSprite;
    public GameObject icon;

    private Image backImage;
    private Image iconImage;

    public void Init(Sprite iconSprite, Sprite fadedIconSprite, bool active, string achievement)
    {
        this.iconSprite = iconSprite;
        this.fadedIconSprite = fadedIconSprite;
        this._active = active;
        this.achievement = achievement;
    }

    private void SetSpriteActive(bool active)
    {
        iconImage.sprite = active ? iconSprite : fadedIconSprite;
        backImage.color = active ? Color.white : Color.gray;
    }

	// Use this for initialization
	void Start ()
    {
        backImage = GetComponent<Image>();
        iconImage = icon.GetComponent<Image>();
        SetSpriteActive(active);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
