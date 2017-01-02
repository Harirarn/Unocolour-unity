using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

	public Pile pile;
	public Pile[,] cardpile = new Pile[logix.WIDTH, logix.HEIGHT];
	public IList<Card.Cardcolor> deck = new List<Card.Cardcolor>();
	public IList<logix.pos> sels = new List<logix.pos> ();
	public bool[,] clickables = new bool[logix.WIDTH, logix.HEIGHT];
	public IList<logix.Shape> possibs;
	public logix game;

	public static void Shuffle<T>(IList<T> list) { //Shuffles the items in the List in place.
		int n = list.Count;
		System.Random rnd = new System.Random();
		while (n > 1) {
			int k = rnd.Next(0, n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	// Use this for initialization
	void Start () {
		// Filling the deck with cards and shuffling them.
		for (int i=0; i<25; i++) deck.Add(Card.Cardcolor.ccred);
		for (int i=0; i<25; i++) deck.Add(Card.Cardcolor.ccyellow);
		for (int i=0; i<25; i++) deck.Add(Card.Cardcolor.ccgreen);
		for (int i=0; i<25; i++) deck.Add(Card.Cardcolor.ccblue);
		for (int i=0; i<8; i++) deck.Add(Card.Cardcolor.ccwild);
		Shuffle<Card.Cardcolor> (deck);

		// Creating the card piles.
		for (int i = 0; i < logix.WIDTH; i++) {
			for (int j = 0; j < logix.HEIGHT; j++) {
				cardpile [i, j] = Instantiate(pile, new Vector3 (i, j, -1), Quaternion.identity);
				cardpile [i, j].row = i;
				cardpile [i, j].col = j;
				Card.Cardcolor color = deck [deck.Count-1];
				deck.RemoveAt (deck.Count-1);
				cardpile [i, j].Add_card (color);
			}
		}
		// Starting the game logix engine.
		game.Makeshapes();
		Highlight_clickables ();
	}

	public List<Card.Cardcolor> Colors_thar (IList<logix.pos> piece){
		List<Card.Cardcolor> ret = new List<Card.Cardcolor>();
		foreach(logix.pos pos in piece){
			ret.Add(cardpile[pos.x, pos.y].topcard);
		}
		return ret;
	}

	public void Highlight_clickables(){
		if (sels.Count == 0) {
			// Clearing out any existing data on which card piles are active.
			for (int i = 0; i < logix.WIDTH; i++) {
				for (int j = 0; j < logix.HEIGHT; j++) {
					clickables [i, j] = false;
				}
			}
			possibs = new List<logix.Shape>();

			// Find out which cells belong to any allowed piece.
			foreach (logix.Shape piece in game.blocks) {
				if (game.Colormatch (Colors_thar (piece.list()))) {
					foreach (logix.pos pos in piece.list()) {
						clickables [pos.x, pos.y] = true;
					}
					possibs.Add (piece);
				}
			}
			// Activate those cells and disable others.
			for (int i = 0; i < logix.WIDTH; i++) {
				for (int j = 0; j < logix.HEIGHT; j++) {
					if (clickables [i, j]) {
						cardpile [i, j].activate ();
					} else {
						cardpile [i, j].disable ();
					}
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
