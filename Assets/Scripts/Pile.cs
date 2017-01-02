using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pile : MonoBehaviour {

	public List<Card> cardpile = new List<Card>();
	public Card card;
	public Card.Cardcolor topcard;

	public int row, col;

	public Pile (int row_, int col_){
		row = row_;
		col = col_;
	}

	public void activate(){
		if (cardpile.Count > 0) {
			cardpile [cardpile.Count - 1].activate ();
		}
	}
	public void disable(){
		if (cardpile.Count > 0) {
			cardpile [cardpile.Count - 1].disable ();
		}
	}
	public void select_card(){
		if (cardpile.Count > 0) {
			cardpile [cardpile.Count - 1].select_card ();
		}
	}


	public void Add_card(Card.Cardcolor color){
		disable ();
		Card newcard = Instantiate (card, new Vector3 ( row*0.8f-3.6f, col-2.0f, -1), Quaternion.identity);
		newcard.Set_color (color);
		cardpile.Add (newcard);
		topcard = color;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
