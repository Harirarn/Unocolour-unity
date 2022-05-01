using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AchievementDescription
{
    public AchievementPanel.Names name;
    public string title, _description;
    public int threshold, defaults, level;
    public bool misere, completed;

    public AchievementDescription(AchievementPanel.Names _name, string _title, string _description, int _threshold, int _defaults = 0, bool _misere = false)
    {
        name = _name;
        title = _title;
        this._description = _description;
        threshold = _threshold;
        level = defaults = _defaults;
        misere = _misere;
        completed = false;
    }

    public void updateLevel(int _level = -1)
    {
        if (_level != -1)
        {
            level = _level;
        }
        completed = (
            (level >= threshold && !misere) ||
            (level != 0 && level <= threshold && misere)
           );
    }
}

public class AchievementPanel : MonoBehaviour
{

    public GameObject iconPrefab;
    public List<Sprite> icons;
    public List<Sprite> fadedIcons;
    public GameObject boardManager;
    public Text title, description, progress;

    private AchievementManager achievementsManager;
    const int IANO = 14;

    public enum Names { allblacks = 0, nomatch, samesame, blacktaboo, score10000, deep10, tall27, saveallfor1, save33for2, save25for3, save20for4, save16for5, max8for1, loseby4, };
    public AchievementDescription[] achievementDescriptions = new AchievementDescription[IANO]
    {
        // Color combinations
        new AchievementDescription(Names.allblacks, "The wildest dreams", "Play four wilds in a single move.", 1),
        new AchievementDescription(Names.nomatch, "No match for me", "Only play on different colours per move for first two rounds.", 2),
        new AchievementDescription(Names.samesame, "Same Same", "Only play on same colour per moves for the first round.", 1),
        new AchievementDescription(Names.blacktaboo, "Black is taboo", "Avoid wilds for 5 rounds", 5),
        // Milestones
        new AchievementDescription(Names.score10000, "Hiscore", "Get a high score of 10000.", 10000),
        new AchievementDescription(Names.deep10, "Hiround", "Reach round 10.", 10, 1),
        new AchievementDescription(Names.tall27, "Histack", "Have a stack with 27 cards at the end of a round.", 27, 1),
        // Socialism
        new AchievementDescription(Names.saveallfor1, "Everyone can be saved", "Have all stack be alive after round 1.", 50),
        new AchievementDescription(Names.save33for2, "Some have to be sacrificed for the greater good", "Have 33 stack be alive after round 2.", 33),
        new AchievementDescription(Names.save25for3, "Are three rounds too long", "Have 25 stack be alive after round 3.", 25),
        new AchievementDescription(Names.save20for4, "A score for the fourth", "Have 20 stack be alive after round 4.", 20),
        new AchievementDescription(Names.save16for5, "The chosen many", "Have 16 stack be alive after round 5.", 16),
        // Exceptionalism
        new AchievementDescription(Names.max8for1, "The elites", "Only save 8 stacks in round 1. Then survive past round 8.", 8),
        // Misere
        new AchievementDescription(Names.loseby4, "Misery", "Have no stacks at the end of round 4.", 4, 0, true),
    };
    private bool[] completeds = new bool[IANO] { true, true, false, false, false, false, false, false, false, false, false, false, false, false, };
    private List<GameObject> achievementIconGameObjects = new List<GameObject>();

    private bool visible = true;

    // Use this for initialization
    void Start()
    {
        achievementsManager = new AchievementManager(this);
        boardManager.GetComponent<BoardManager>().achievementsManager = achievementsManager;
        foreach (Names name in System.Enum.GetValues(typeof(Names)))
        {
            int i = (int)name;
            updateAchievement(name, achievementsManager.getAchievement(name));
            achievementIconGameObjects.Add(Instantiate(iconPrefab, transform));
            achievementIconGameObjects[i].GetComponent<AchievementIcon>().Init
            (
                icons[i], fadedIcons[i], achievementDescriptions[i], this
            );
            RectTransform rct = achievementIconGameObjects[i].GetComponent<RectTransform>();
            rct.anchoredPosition = positionByNo(i);
        }
        clearDescription();
        Minimize();
    }

    Vector2 positionByNo(int i)
    {
        return new Vector2(10 + 70 * (i % 7), -10 - 70 * (i / 7));
    }

    public void updateAchievement(Names achievement, int levelAchieved)
    {
        achievementDescriptions[(int)achievement].updateLevel(levelAchieved);
    }

    public void showDescription(Names achievement)
    {
        AchievementDescription achievementDescription = achievementDescriptions[(int)achievement];
        title.text = achievementDescription.title;
        description.text = achievementDescription._description;
        progress.text = "Progress: " + achievementDescription.level.ToString() + "/" + achievementDescription.threshold.ToString();
    }

    public void clearDescription()
    {
        title.text = description.text = progress.text = "";
    }

    public void Minimize()
    {
        visible = !visible;
        gameObject.SetActive(visible);
        boardManager.GetComponent<BoardManager>().OpenAchievements(visible);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
