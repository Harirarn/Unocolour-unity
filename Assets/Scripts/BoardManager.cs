using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;




public class BoardManager : MonoBehaviour
{

    public GameObject cardPrefab;
    public GameObject stackPrefab;
    public List<CardColor> deckColors;
    public List<int> deckColorNumbers;
    public GameObject cameraObject;
    public GameObject scoresheet;
    public GameObject UI;
    public AudioClip cardputSound;
    public AudioClip cardtakeSound;
    public AchievementManager achievementsManager;
    private Camera camera;

    readonly static float DistributeTime = 0.05f;
    readonly static int columns = 10;
    readonly static int rows = 5;
    readonly static CellPos lastCell = new CellPos(columns, rows);
    readonly static float halfcardwidth = 0.3f;
    readonly static float halfcardheight = 0.4f;
    readonly static Vector3 cellwidth = new Vector3(0.83f, 0f, 0f);
    readonly static Vector3 cellheight = new Vector3(0f, 1f, 0f);
    readonly static Vector3 cellsize = cellheight + cellwidth;
    readonly static Vector3 firstcellpos = new Vector3(0f, 1f, 0f) - (columns - 1) / 2f * cellwidth - (rows - 1) / 2f * cellheight;
    private AudioSource soundEmitter;
    private bool restartRequest = false, achievementsOpen = false;

    //CardStack[,] cells = new CardStack[columns, rows];
    Dictionary<CellPos, CardStack> cells = new Dictionary<CellPos, CardStack>(lastCell.area);
    CardStack deck;
    CardStack[] holdingStacks = new CardStack[4];
    CellPos[] moveCellPos = new CellPos[4];
    int score;
    bool animating;
    bool stateTransition;
    int round;
    int cardsplayed;
    enum State { start, roundstart, shuffle, shuffleAnim, deal, dealAnim, playStart, play, collect, collectAnim, scoreboard, finalscoreboard, restart, viewAchievements };
    CellPos mousePos;



    private StateMachine<State> state;

    public BoardManager()
    {
        state = new StateMachine<State>
        (
            State.start,
            new Dictionary<State, Func<State>> { { State.start,GameStart }, { State.roundstart,RoundStart },
            { State.shuffle,Shuffle }, { State.shuffleAnim,ShuffleAnim }, { State.deal,Deal },
            { State.dealAnim,DealAnim }, { State.playStart,PlayStart }, { State.play,Play }, { State.collect,Collect },
            { State.collectAnim, CollectAnim }, { State.scoreboard, Scoreboard }, { State.finalscoreboard, FinalScoreboard },
            { State.restart, GameRestart }, {State.viewAchievements, ViewAchievements},}
        );

    }


    State GameStart()
    {
        round = 0;
        score = 0;
        UI.GetComponent<UIB>().setRound(round);
        UI.GetComponent<UIB>().setScore(score, ref achievementsManager);
        achievementsManager.resetTrackers();
        animating = false;
        return State.roundstart;
    }
    State RoundStart()
    {
        round++;
        UI.GetComponent<UIB>().setRound(round);
        return State.shuffle;
    }
    State Shuffle()
    {
        deck.Shuffle();
        return State.shuffleAnim;
    }
    State ShuffleAnim()
    {
        if (animating)
            return State.shuffleAnim;
        if (round == 1)
            return State.deal;
        return State.playStart;
    }
    State Deal()
    {
        StartCoroutine(DistributeToActiveCells());
        return State.dealAnim;
    }
    State DealAnim()
    {
        if (animating)
            return State.dealAnim;
        return State.playStart;
    }
    State PlayStart()
    {
        cardsplayed = 0;
        board.RecalculateActiveShapes(cellsAsCardColor);
        SyncCardActives();
        if (deck.number < 4 || !board.IsAnyMovePossible())
        {
            return State.collect;
        }
        return State.play;
    }
    State Play()
    {
        if (restartRequest)
        {
            restartRequest = false;
            return State.restart;
        }
        if (achievementsOpen)
        {
            return State.viewAchievements;
        }
        if (Input.GetMouseButtonDown(0) && mousePos != null)
        {
            if (board.CellActive(mousePos, true))
            {
                holdingStacks[cardsplayed].ReceiveCard(deck.SendCard());
                moveCellPos[cardsplayed] = mousePos;
                cardsplayed += 1;
                cells[mousePos].Click();
                //cells[mousePos].ReceiveCard(deck.SendCard());
                board.AddCellToMove(mousePos);
                SyncCardActives();
                soundEmitter.PlayOneShot(cardputSound, 0.4f);
            }
            if (cardsplayed == 4)
            {
                // Achievement tracker stuff.
                int[] stacks = new int[4] { 0, 0, 0, 0 };
                CardColor[] cardColors = new CardColor[4] { CardColor.none, CardColor.none, CardColor.none, CardColor.none, };
                Shape shape = new Shape(moveCellPos);

                for (int i = 0; i < 4; i++)
                {
                    cardColors[i] = cells[moveCellPos[i]].topCardColor;
                    // Moving cards from holding stacks to cells.
                    cells[moveCellPos[i]].ReceiveCard(holdingStacks[i].SendCard());
                    stacks[i] = cells[moveCellPos[i]].number;
                }
                achievementsManager.PlaceShape(shape, cardColors, stacks);

                return State.playStart;
            }
        }
        return State.play;
    }
    State Collect()
    {
        StartCoroutine(CollectFromCells(round));
        return State.collectAnim;
    }
    State CollectAnim()
    {
        if (animating)
            return State.collectAnim;
        return State.scoreboard;
    }
    State Scoreboard()
    {
        Dictionary<int, int> stackCounts = new Dictionary<int, int> { };
        int val = 0;
        foreach (CellPos pos in lastCell.Range())
        {
            int num = cells[pos].number;
            if (num > 0)
            {
                stackCounts.TryGetValue(num, out val);
                stackCounts[num] = val + 1;
            }
        }

        score = scoresheet.GetComponent<Scoresheet>().ShowScore(round, score, stackCounts, deck.number == 108);
        achievementsManager.EndRound(round, stackCounts, deck.number);

        UI.GetComponent<UIB>().setScore(score, ref achievementsManager);

        return State.finalscoreboard;
    }
    State FinalScoreboard()
    {
        if (restartRequest)
        {
            restartRequest = false;
            scoresheet.GetComponent<Scoresheet>().Button();
            return State.restart;
        }
        if (scoresheet.activeSelf)
        {
            return State.finalscoreboard;
        }
        if (deck.number == 108)
        {
            return State.restart;
        }
        return State.roundstart;
    }
    State GameRestart()
    {
        // Collect all cards
        CollectAllCards();
        return State.start;
    }

    State ViewAchievements()
    {
        if (achievementsOpen)
        {
            return State.viewAchievements;
        }
        else
        {
            return State.play;
        }
    }

    public void ExternalRestart()
    {
        restartRequest = true;
    }

    public void OpenAchievements(bool opened)
    {
        achievementsOpen = opened;
    }



    BoardEngine board;


    void InstantiateBoard()
    {
        // Making the deck
        deck = MakeStack(new Vector3(0f, -3f, 0f) - 3 * cellwidth, true, false, true);

        // Making the individual stacks.
        foreach (CellPos i in lastCell.Range())
        {
            cells[i] = MakeStack(firstcellpos + i * cellsize, true, true, true);
        }

        // Initializing the holding cells.
        for (int i = 0; i < 4; i++)
        {
            holdingStacks[i] = MakeStack(new Vector3(0f, -3f, 0f) + i * cellwidth, true, true, false);
        }

        // Making the cards
        for (int i = 0; i < deckColors.Count; i++)
        {
            for (int j = 0; j < deckColorNumbers[i]; j++)
            {
                MakeCard(deckColors[i], deck);
            }
        }


        board = new BoardEngine(new CellPos(columns, rows));

    }

    CardStack MakeStack(Vector3 position, bool active, bool staggered, bool numbered)
    {
        GameObject stackObject = Instantiate(stackPrefab, position, Quaternion.identity, transform) as GameObject;
        CardStack stack = stackObject.GetComponent<CardStack>();
        stack.SetProperties(active, staggered, numbered);
        return stack;
    }

    void MakeCard(CardColor color, CardStack stack)
    {
        GameObject cardObject = Instantiate(cardPrefab, stack.transform.position, Quaternion.identity, transform) as GameObject;
        cardObject.GetComponent<Card>().color = color;
        stack.ReceiveCard(cardObject);
    }

    IEnumerator DistributeToActiveCells()
    {
        animating = true;
        foreach (CellPos i in lastCell.Range(true, false, true))
        {
            cells[i].ReceiveCard(deck.SendCard());
            soundEmitter.PlayOneShot(cardputSound, 0.2f);
            yield return new WaitForSeconds(DistributeTime);
        }
        animating = false;
    }

    IEnumerator CollectFromCells(int roundno)
    {
        animating = true;
        for (int i = 0; i < roundno; i++)
        {
            foreach (CellPos pos in lastCell.Range(true, false, false))
            {
                if (cells[pos].number > 0)
                {
                    deck.ReceiveCard(cells[pos].SendCard());
                    soundEmitter.PlayOneShot(cardtakeSound, 0.2f);
                    yield return new WaitForSeconds(DistributeTime);
                }
            }
        }
        animating = false;
    }

    void CollectAllCards()
    {
        animating = true;
        soundEmitter.PlayOneShot(cardtakeSound, 0.2f);
        // Collect all cards from piles
        foreach (CellPos pos in lastCell.Range(true, false, false))
        {
            while (cells[pos].number > 0)
            {
                deck.ReceiveCard(cells[pos].SendCard());
            }
        }
        // Collect all cards from holding cells
        for (int i = 0; i < 4; i++)
        {
            if (holdingStacks[i].number > 0)
            {
                deck.ReceiveCard(holdingStacks[i].SendCard());
            }
        }
        animating = false;
    }


    void SyncCardActives()
    {
        foreach (CellPos i in lastCell.Range())
        {
            cells[i].active = board.CellActive(i, false);
        }
    }
    Dictionary<CellPos, CardColor> cellsAsCardColor
    {
        get
        {
            Dictionary<CellPos, CardColor> result = new Dictionary<CellPos, CardColor>(lastCell.area);
            foreach (CellPos i in lastCell.Range())
            {
                result[i] = cells[i].topCardColor;
            }
            return result;
        }
    }

    int Triangle(int n)
    {
        return n * (n + 1) / 2;
    }


    CellPos GetMouseCell()
    {
        Vector3 currentPos = camera.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log (currentPos);
        int i, j;
        for (i = 0; i < columns; i++)
        {
            if (Mathf.Abs((currentPos - firstcellpos - i * cellwidth).x) < halfcardwidth)
                break;
        }
        for (j = 0; j < rows; j++)
        {
            //Debug.Log ((currentPos - firstcellpos - j * cellheight).y.ToString ());
            if (Mathf.Abs((currentPos - firstcellpos - j * cellheight).y) < halfcardheight)
                break;
        }

        if (i < columns && j < rows)
        {
            return new CellPos(i, j);
        }
        return null;
    }

    void Start()
    {
        camera = cameraObject.GetComponent<Camera>();
        InstantiateBoard();
        scoresheet.SetActive(false);
        soundEmitter = GetComponent<AudioSource>();
    }

    void Update()
    {

        mousePos = GetMouseCell();

        if (mousePos != null && !animating)
        {
            cells[mousePos].Hover();
        }

        state.Process();

    }

}


public class StateMachine<StateType>
{
    public Dictionary<StateType, Func<StateType>> ProcessMap;
    public StateType state;

    public StateMachine(StateType InitialState, Dictionary<StateType, Func<StateType>> InitialProcessMap)
    {
        state = InitialState;
        ProcessMap = InitialProcessMap;
    }

    public void Process()
    {
        state = ProcessMap[state]();
    }
}