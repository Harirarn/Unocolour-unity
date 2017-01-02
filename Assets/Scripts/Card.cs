using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Card : MonoBehaviour {

	public enum Cardcolor{ccred, ccyellow, ccgreen, ccblue, ccwild, ccempty}

	public List<Sprite> card_sprite;
	public List<Sprite> card_hover_sprite;
	public List<Sprite> card_inactive_sprite;

	public Cardcolor color;
	public int active;
	public bool hover;

	public int posx, posy, tilt;

	public SpriteRenderer card_renderer;

	public void Set_color(Cardcolor color){
		card_renderer = GetComponent<SpriteRenderer> ();
		this.color = color;
		UpdateCard();
	}

	public void activate(){
		active = 1;
		UpdateCard ();
	}
	public void disable(){
		active = 0;
		UpdateCard ();
	}
	public void select_card(){
		active = 2;
		UpdateCard ();
	}

	public void UpdateCard(){
		switch (active) {
		case 0:
			card_renderer.sprite = card_inactive_sprite [(int) color];
			break;
		case 1:
			if (hover) {
				card_renderer.sprite = card_hover_sprite [(int)color];
			} else {
				card_renderer.sprite = card_sprite [(int)color];
			}
			break;
		default:
			card_renderer.sprite = card_hover_sprite [(int)color];
			break;
		}
	}

	// Use this for initialization
	void Start () {
		card_renderer = GetComponent<SpriteRenderer> ();
		active = 0;
		hover = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
