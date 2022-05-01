using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct AchievementDescription
{
    public string name, title, description;
    public int threshold, defaults;

    public AchievementDescription(string _name, string _title, string _description, int _threshold, int _defaults = 0)
    {
        name = _name;
        title = _title;
        description = _description;
        threshold = _threshold;
        defaults = _defaults;
    }
}

public class AchievementPanel : MonoBehaviour
{

    public GameObject iconPrefab;
    public List<Sprite> icons;
    public List<Sprite> fadedIcons;
    public GameObject boardManager;
    public Text description;
    public Text progress;

    private AchievementManager achievementsManager;
    const int IANO = 14;

    public AchievementDescription[] achievementDescriptions = new AchievementDescription[IANO]
    {
        // Color combinations
        new AchievementDescription("allblacks", "The wildest dreams", "", 1),
        new AchievementDescription("nomatch", "No match for me", "", 1),
        new AchievementDescription("samesame", "Same Same", "", 1),
        new AchievementDescription("blacktaboo", "Black is taboo", "", 1),
        // Milestones
        new AchievementDescription("score10000", "Hiscore", "", 10000),
        new AchievementDescription("deep10", "Hiround", "", 10, 1),
        new AchievementDescription("tall27", "Histack", "", 27, 1),
        // Socialism
        new AchievementDescription("saveallfor1", "Everyone can be saved", "", 50),
        new AchievementDescription("save33for2", "Some have to be sacrificed for the greater good", "", 33),
        new AchievementDescription("save25for3", "Are three rounds too long", "", 25),
        new AchievementDescription("save20for4", "A score for the fourth", "", 20),
        new AchievementDescription("save16for5", "The chosen many", "", 16),
        // Exceptionalism
        new AchievementDescription("max8for1", "The elites", "", 8),
        // Misere
        new AchievementDescription("loseby4", "Misery", "", 4),
    };
    private bool[] completeds = new bool[IANO] { true, true, false, false, false, false, false, false, false, false, false, false, false, false, };
    private List<GameObject> achievementIconGameObjects = new List<GameObject>();

    private bool visible = true;

    // Use this for initialization
    void Start()
    {
        achievementsManager = new AchievementManager(this);
        boardManager.GetComponent<BoardManager>().achievementsManager = achievementsManager;
        for (int i = 0; i < IANO; i++)
        {
            achievementIconGameObjects.Add(Instantiate(iconPrefab, transform));
            achievementIconGameObjects[i].GetComponent<AchievementIcon>().Init
            (
                icons[i], fadedIcons[i], completeds[i], achievementDescriptions[i].name
            );
            RectTransform rct = achievementIconGameObjects[i].GetComponent<RectTransform>();
            rct.anchoredPosition = positionByNo(i);

        }
        Minimize();
    }

    Vector2 positionByNo(int i)
    {
        return new Vector2(10 + 70 * (i % 7), -10 - 70 * (i / 7));
    }

    public void updateAchievement(string achievement, bool achieved = true)
    {
        for (int i = 0; i < IANO; i++)
        {
            if (achievementDescriptions[i].name == achievement)
            {
                completeds[i] = achieved;
            }
        }

    }

    public void setDescription(string _description, string _progress = " ")
    {
        description.text = _description;
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
