using System.Collections;
using System.Collections.Generic;



public class logix{
	public const int WIDTH = 10;
	public const int HEIGHT = 5;

	public struct pos{
		public int x, y;
		public pos(int x, int y){
			this.x = x; this.y = y;
		}
		public static bool operator ==(pos first, pos second){
			return first.x == second.x && first.y == second.y;
		}
		public static bool operator !=(pos first, pos second){
			return !(first == second);
		}
//		public override bool Equals(object o){
//			return this == (pos) o;
//		}
	}

	public struct Shape{
		public pos a, b, c, d;
		public Shape(int ax, int ay, int bx, int byy, int cx, int cy, int dx, int dy){
			this.a = new pos(ax, ay);
			this.b = new pos(bx, byy);
			this.c = new pos(cx, cy);
			this.d = new pos(dx, dy);
		}
		public void offset(int x, int y){
			a.x += x; b.x += x; c.x += x; d.x += x;
			a.y += y; b.y += y; c.y += y; d.y += y;
		}
		public Shape copy(){
			return new Shape (this.a.x, this.a.y, this.b.x, this.b.y, this.c.x, this.c.y, this.d.x, this.d.y);
		}
		public List<pos> list(){
			List<pos> ret = new List<pos>{ new pos (a.x, a.y), new pos (b.x, b.y), new pos (c.x, c.y), new pos (d.x, d.y) };
			return ret;
		}
	}

	public IList<Shape> blocks = new List<Shape>();

	public static int max4(int a, int b, int c, int d){
		int m = a;
		if (b > m)
			m = b;
		if (c > m)
			m = c;
		if (d > m)
			m = d;
		return m;
	}
	public void Makeshapes(){
		// This sets all possible shapes in the stage. They are stored in `blocks`.
		Shape[] shapes = {
			new Shape (0, 0, 0, 1, 1, 0, 1, 1),
//           ##
//           ##
			new Shape (0, 0, 0, 1, 0, 2, 1, 1),
			new Shape (0, 0, 1, 0, 2, 0, 1, 1),
			new Shape (1, 0, 0, 1, 1, 1, 1, 2),
			new Shape (1, 0, 0, 1, 1, 1, 2, 1),
//           #   ###  #   #
//           ##   #  ##  ###
//           #        #
			new Shape (0, 0, 1, 0, 1, 1, 2, 1),
			new Shape (1, 0, 2, 0, 0, 1, 1, 1),
			new Shape (1, 0, 0, 1, 1, 1, 0, 2),
			new Shape (0, 0, 0, 1, 1, 1, 1, 2),
//           ##   ##  #  # 
//            ## ##  ##  ##
//                   #    #
			new Shape (0, 0, 1, 0, 1, 1, 1, 2),
			new Shape (2, 0, 0, 1, 1, 1, 2, 1),
			new Shape (0, 0, 0, 1, 0, 2, 1, 2),
			new Shape (0, 0, 1, 0, 2, 0, 0, 1),
//           ##    # #   ###
//            #  ### #   # 
//            #      ##    
			new Shape (0, 0, 1, 0, 0, 1, 0, 2),
			new Shape (0, 0, 1, 0, 2, 0, 2, 1),
			new Shape (1, 0, 1, 1, 0, 2, 1, 2),
			new Shape (0, 0, 0, 1, 1, 1, 2, 1),
//           ##  ###  #  #  
//           #     #  #  ###
//           #       ##   
			new Shape (0, 0, 0, 1, 0, 2, 0, 3),
			new Shape (0, 0, 1, 0, 2, 0, 3, 0)
		};
		for (int s=0; s<19; s++){
			int maxi = max4 (shapes [s].a.x, shapes [s].b.x, shapes [s].c.x, shapes [s].d.x);
			int maxj = max4 (shapes [s].a.y, shapes [s].b.y, shapes [s].c.y, shapes [s].d.y);
			for (int i = 0; i < WIDTH - maxi; i++) {
				for (int j = 0; j < HEIGHT - maxj; j++) {
					Shape tshape = shapes [s].copy ();
					tshape.offset (i, j);
					blocks.Add (tshape);
				}
			}
		}
	}

	public static bool Colormatch(IList<Card.Cardcolor> colors){
		// Returns true if the first four colours in the list are in a legit combination.
		bool allred = true, allyellow = true, allgreen = true, allblue = true, allfour;
		int blacks = 0, red = 0, yellow = 0, green = 0, blue = 0;
		for (int i = 0; i < 4; i++) {
			switch (colors [i]) {
			case Card.Cardcolor.ccempty:
				return false;
			case Card.Cardcolor.ccred:
				allyellow = allgreen = allblue = false;
				red = 1;
				break;
			case Card.Cardcolor.ccyellow:
				allred = allgreen = allblue = false;
				yellow = 1;
				break;
			case Card.Cardcolor.ccgreen:
				allyellow = allred = allblue = false;
				green = 1;
				break;
			case Card.Cardcolor.ccblue:
				allyellow = allgreen = allred = false;
				blue = 1;
				break;
			case Card.Cardcolor.ccwild:
				blacks += 1;
				break;
			}
		}
		allfour = (red + yellow + green + blue + blacks == 4);
		return (allred || allyellow || allgreen || allblue || allfour);
	}

	public static IList<Shape> Prune(IList<Shape> bloc, pos p){
		IList<Shape> ret = new List<Shape>();
		foreach (Shape s in bloc) {
			if (s.a == p || s.b == p || s.c == p || s.d == p) {
				ret.Add (s);
			}
		}
		return ret;
	}
}