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

    public AchievementManager(AchievementPanel _panel)
    {
        panel = _panel;
        LoadData();
        foreach (AchievementDescription achievement in panel.achievementDescriptions)
        {
            getAchievement(achievement.name, achievement.defaults);
        }
        // Highs
        hiscore = getAchievement("score10000");
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
        string filetext = JsonConvert.SerializeObject(achievements);
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
            changed = setAchievement("score10000", hiscore) || changed;
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
        if (maxstack > getAchievement("tall27"))
        {
            changed = setAchievement("tall27", maxstack) || changed;
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

        if (round > getAchievement("deep10"))
        {
            changed = setAchievement("deep10", round) || changed;
        }
        // Tracked achievements
        if (samesameTrack && round > getAchievement("samesame"))
        {
            changed = setAchievement("samesame", round) || changed;
        }
        if (nomatchTrack && round > getAchievement("nomatch"))
        {
            changed = setAchievement("nomatch", round) || changed;
        }
        if (blacktabooTrack && round > getAchievement("blacktaboo"))
        {
            changed = setAchievement("blacktaboo", round) || changed;
        }
        // Socialism achievements
        if (round == 1 && stackno >= getAchievement("saveallfor1"))
        {
            changed = setAchievement("saveallfor1", stackno) || changed;
        }
        if (round == 2 && stackno >= getAchievement("save33for2"))
        {
            changed = setAchievement("save33for2", stackno) || changed;
        }
        if (round == 3 && stackno >= getAchievement("save25for3"))
        {
            changed = setAchievement("save25for3", stackno) || changed;
        }
        if (round == 4 && stackno >= getAchievement("save20for4"))
        {
            changed = setAchievement("save20for4", stackno) || changed;
        }
        if (round == 5 && stackno >= getAchievement("save16for5"))
        {
            changed = setAchievement("save16for5", stackno) || changed;
        }
        if (round == 1 && stackno == 8)
        {
            max8for1 = true;
        }
        if (max8for1 && round > getAchievement("max8for1") && stackno >= 1)
        {
            changed = setAchievement("max8for1", round) || changed;
        }
        // Miser Achievements
        if (stackno == 0 && round < getAchievement("loseby4"))
        {
            changed = setAchievement("loseby4", round) || changed;
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
