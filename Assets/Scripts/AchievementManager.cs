using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class AchievementData
{
    public Dictionary<string, int> data;
}



public class AchievementManager
{

    private int hiscore;
    private int hiround;
    private int histack;

    private bool samesameTrack,
                 nomatchTrack,
                 blacktabooTrack,
                 max8for1;

    private AchievementData achievements = new AchievementData();
    private string dataFile;

    private AchievementPanel panel;

    public AchievementManager()
    {
        LoadData();
        // Highs
        hiscore = getAchievement("hiscore");
        hiround = getAchievement("hiround", 1);
        histack = getAchievement("histack", 1);
        // Color combinations
        getAchievement("allblacks");
        getAchievement("samesame");
        getAchievement("nomatch");
        getAchievement("blacktaboo");
        // Milestones
        getAchievement("score10000");
        getAchievement("deep10");
        getAchievement("tall27");
        // Socialism
        getAchievement("saveallfor1");
        getAchievement("save33for2");
        getAchievement("save25for3");
        getAchievement("save20for4");
        getAchievement("save16for5");
        getAchievement("max8for1");
        // Miser
        getAchievement("loseby4");
        //Debug.Log("init: " + achievements.data.Keys.Count.ToString());
        resetTrackers();

    }

    public void setPanel(AchievementPanel _panel)
    {
        panel = _panel;
    }

    public void resetTrackers()
    {
        samesameTrack = true;
        nomatchTrack = true;
        blacktabooTrack = true;
        max8for1 = false;
    }

    public void LoadData()
    {
        dataFile = Application.dataPath + "/achievements.json";
        //Debug.Log(dataFile);
        if (File.Exists(dataFile))
        {
            string filetext = File.ReadAllText(dataFile);
            achievements = JsonConvert.DeserializeObject<AchievementData>(filetext);
            if (achievements.data == null)
            {
                achievements.data = new Dictionary<string, int>();
            }
        }
        else
        {
            achievements.data = new Dictionary<string, int>();
        }
    }

    public void SaveData()
    {
        panel.setDescription("boo");
        string filetext = JsonConvert.SerializeObject(achievements);
        panel.setDescription("hello");
        File.WriteAllText(dataFile, filetext);
        panel.setDescription("bye");
        //Debug.Log("SaveData: " + achievements.data.Keys.Count.ToString());
        //Debug.Log(filetext);
    }

    public int getAchievement(string achievement, int defaultValue = 0)
    {
        if (achievements.data.ContainsKey(achievement))
        {
            return achievements.data[achievement];
        }
        else
        {
            achievements.data[achievement] = defaultValue;
            return defaultValue;
        }
    }

    private bool setAchievement(string achievement, int value = 1)
    {
        if (achievements.data.ContainsKey(achievement) ? achievements.data[achievement] != value : true)
        {
            achievements.data[achievement] = value;
            //SaveData();
            if (panel != null && value > 0)
            {
                panel.updateAchievement(achievement);
            }
            return true;
        }
        return false;
    }

    public int NewHiscore(int newscore)
    {
        bool changed = false;

        if (newscore > hiscore)
        {
            hiscore = newscore;
            changed = setAchievement("hiscore", hiscore) || changed;
        }
        if (newscore >= 10000)
        {
            changed = setAchievement("score10000") || changed;
        }

        if (changed)
        {
            SaveData();
        }

        return hiscore;
    }
    public void PlaceShape(Shape shape, CardColor[] cardColors, int[] stacks)
    {
        bool changed = false;
        int maxstack = stacks.Max<int>();
        if (maxstack > histack)
        {
            histack = maxstack;
            changed = setAchievement("histack", histack) || changed;
        }
        if (maxstack >= 27)
        {
            changed = setAchievement("tall27") || changed;
        }
        // Achievement allblacks
        if
        (
            cardColors[0] == CardColor.wild &&
            cardColors[1] == CardColor.wild &&
            cardColors[2] == CardColor.wild &&
            cardColors[3] == CardColor.wild
        )
        {
            changed = setAchievement("allblacks") || changed;
        }
        // Track samesame
        if
        (
            cardColors[0] != cardColors[1] ||
            cardColors[0] != cardColors[2] ||
            cardColors[0] != cardColors[3]
        )
        {
            samesameTrack = false;
        }
        // Track nomatch
        if
        (
            cardColors[0] == cardColors[1] ||
            cardColors[0] == cardColors[2] ||
            cardColors[0] == cardColors[3] ||
            cardColors[1] == cardColors[2] ||
            cardColors[1] == cardColors[2] ||
            cardColors[2] == cardColors[3]
        )
        {
            nomatchTrack = false;
        }
        // Track blacktaboo
        if
        (
            cardColors[0] == CardColor.wild ||
            cardColors[1] == CardColor.wild ||
            cardColors[2] == CardColor.wild ||
            cardColors[3] == CardColor.wild
        )
        {
            blacktabooTrack = false;
        }

        if (changed)
        {
            SaveData();
        }

    }
    public void EndRound(int round, Dictionary<int, int> stackCounts, int deckNumber)
    {
        bool changed = false;
        int stackno = 0;
        foreach (int key in stackCounts.Keys)
        {
            if (key > 0)
            {
                stackno += stackCounts[key];
            }
        }

        if (round > hiround)
        {
            hiround = round;
            changed = setAchievement("hiround", hiround) || changed;
        }
        if (round >= 10)
        {
            changed = setAchievement("deep10") || changed;
        }
        // Tracked achievements
        if (samesameTrack)
        {
            changed = setAchievement("samesame") || changed;
        }
        if (nomatchTrack)
        {
            changed = setAchievement("nomatch") || changed;
        }
        if (blacktabooTrack && round >= 5)
        {
            changed = setAchievement("blacktaboo") || changed;
        }
        // Socialism achievements
        if (round == 1 && stackno >= 50)
        {
            changed = setAchievement("saveallfor1") || changed;
        }
        if (round == 2 && stackno >= 33)
        {
            changed = setAchievement("save33for2") || changed;
        }
        if (round == 3 && stackno >= 25)
        {
            changed = setAchievement("save25for3") || changed;
        }
        if (round == 4 && stackno >= 20)
        {
            changed = setAchievement("save20for4") || changed;
        }
        if (round == 5 && stackno >= 16)
        {
            changed = setAchievement("save16for5") || changed;
        }
        if (round == 1 && stackno == 8)
        {
            max8for1 = true;
        }
        if (max8for1 && round == 8 && stackno >= 1)
        {
            changed = setAchievement("max8for1") || changed;
        }
        // Miser Achievements
        if (round <= 4 && stackno == 0)
        {
            changed = setAchievement("loseby4") || changed;
        }

        if (changed)
        {
            SaveData();
        }

    }

    void Update()
    {

    }


}
