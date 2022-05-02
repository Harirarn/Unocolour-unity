using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AchievementDescription
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

    [System.Serializable]
    public enum Names { allblacks = 0, nomatch, samesame, blacktaboo, score10000, deep10, tall27, saveallfor1, save33for2, save25for3, save20for4, save16for5, max8for1, loseby4, };
    public Dictionary<Names, AchievementDescription> achievementDescriptions;
    private Dictionary<Names, GameObject> achievementIconGameObjects = new Dictionary<Names, GameObject>();
    private bool visible = true;
    private Names currentlyShowing;
    private bool ifShowing = false;

    // Use this for initialization
    void Start()
    {
        achievementDescriptions = new Dictionary<Names, AchievementDescription>
        {
            // Color combinations
            [Names.allblacks] = new AchievementDescription(Names.allblacks, "The wildest dreams", "Play four wilds in a single move.", 1),
            [Names.nomatch] = new AchievementDescription(Names.nomatch, "No match for me", "Only play on different colours per move for first two rounds.", 2),
            [Names.samesame] = new AchievementDescription(Names.samesame, "Same Same", "Only play on same colour per move for the first round.", 1),
            [Names.blacktaboo] = new AchievementDescription(Names.blacktaboo, "Black is taboo", "Avoid wilds for 5 rounds", 5),
            // Milestones
            [Names.score10000] = new AchievementDescription(Names.score10000, "Hiscore", "Get a high score of 10000.", 10000),
            [Names.deep10] = new AchievementDescription(Names.deep10, "Hiround", "Reach round 10.", 10, 1),
            [Names.tall27] = new AchievementDescription(Names.tall27, "Histack", "Have a stack with 27 cards tall.", 27, 1),
            // Socialism
            [Names.saveallfor1] = new AchievementDescription(Names.saveallfor1, "Everyone can be saved", "Have all stack be alive after round 1.", 50),
            [Names.save33for2] = new AchievementDescription(Names.save33for2, "Some have to be sacrificed for the greater good", "Have 33 stack be alive after round 2.", 33),
            [Names.save25for3] = new AchievementDescription(Names.save25for3, "Are three rounds too long", "Have 25 stack be alive after round 3.", 25),
            [Names.save20for4] = new AchievementDescription(Names.save20for4, "A score for the fourth", "Have 20 stack be alive after round 4.", 20),
            [Names.save16for5] = new AchievementDescription(Names.save16for5, "The chosen many", "Have 16 stack be alive after round 5.", 16),
            // Exceptionalism
            [Names.max8for1] = new AchievementDescription(Names.max8for1, "The elites", "Only save 8 stacks in round 1. Then survive till round 8.", 8),
            // Misere
            [Names.loseby4] = new AchievementDescription(Names.loseby4, "Misery", "Have no stacks at the end of round 4.", 4, 0, true),
        };
        achievementsManager = new AchievementManager(this);
        boardManager.GetComponent<BoardManager>().achievementsManager = achievementsManager;
        foreach (Names name in System.Enum.GetValues(typeof(Names)))
        {
            int i = (int)name;
            achievementDescriptions[name].updateLevel(achievementsManager.getAchievement(name, achievementDescriptions[name].defaults));
            achievementIconGameObjects[name] = Instantiate(iconPrefab, transform);
            achievementIconGameObjects[name].GetComponent<AchievementIcon>().Init
            (
                icons[i], fadedIcons[i], achievementDescriptions[name], this
            );
            RectTransform rct = achievementIconGameObjects[name].GetComponent<RectTransform>();
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
        achievementDescriptions[achievement].updateLevel(levelAchieved);
        achievementIconGameObjects[achievement].GetComponent<AchievementIcon>().active = achievementDescriptions[achievement].completed;
    }

    public void setAvailable(Names achievement, bool value)
    {
        achievementIconGameObjects[achievement].GetComponent<AchievementIcon>().isAvailable = value;
    }

    public void showDescription(Names achievement)
    {
        AchievementDescription achievementDescription = achievementDescriptions[achievement];
        currentlyShowing = achievement;
        ifShowing = true;
        title.text = achievementDescription.title;
        description.text = achievementDescription._description;
        progress.text = "Progress: " + achievementDescription.level.ToString() + "/" + achievementDescription.threshold.ToString();
    }

    public void clearDescription()
    {
        title.text = description.text = progress.text = "";
        ifShowing = false;
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
        if (ifShowing)
        {
            showDescription(currentlyShowing);
        }

    }
}
