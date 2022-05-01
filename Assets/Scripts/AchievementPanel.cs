using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AchievementPanel : MonoBehaviour
{

    public GameObject iconPrefab;
    public List<Sprite> icons;
    public List<Sprite> fadedIcons;
    public GameObject boardManager;
    public Text description;
    public Text progress;

    private AchievementManager achievements;
    const int IANO = 14;
    private string[] names = new string[IANO]
    {
        "allblacks",
        "nomatch",
        "samesame",
        "blacktaboo",
        "score10000",
        "deep10",
        "tall27",
        "saveallfor1",
        "save33for2",
        "save25for3",
        "save20for4",
        "save16for5",
        "max8for1",
        "loseby4",
    };
    private string[] titles = new string[IANO]
    {
        "The wildest dreams",
        "No match for me",
        "Same Same",
        "Black is Taboo",
        "Hiscore",
        "Hiround",
        "Histack",
        "Everyone can be saved",
        "Cant we stay together for a couple of days",
        "Are three rounds too long",
        "Twenty for the fourth",
        "The chosen many",
        "max8for1",
        "Misery",
    };
    private string[] descs = new string[IANO]
    {
        "allblacks",
        "nomatch",
        "samesame",
        "blacktaboo",
        "score10000",
        "deep10",
        "tall27",
        "saveallfor1",
        "save33for2",
        "save25for3",
        "save20for4",
        "save16for5",
        "max8for1",
        "loseby4",
    };
    private bool[] completeds = new bool[IANO] { true, true, false, false, false, false, false, false, false, false, false, false, false, false, };
    private List<GameObject> achievementIconGameObjects = new List<GameObject>();

    private bool visible = true;

    // Use this for initialization
    void Start()
    {
        achievements = new AchievementManager();
        boardManager.GetComponent<BoardManager>().achievements = achievements;
        achievements.setPanel(this);
        for (int i = 0; i < IANO; i++)
        {
            achievementIconGameObjects.Add(Instantiate(iconPrefab, transform));
            achievementIconGameObjects[i].GetComponent<AchievementIcon>().Init
            (
                icons[i], fadedIcons[i], completeds[i], names[i]
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
            if (names[i] == achievement)
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
