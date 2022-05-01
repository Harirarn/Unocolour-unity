using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public class AchievementData
{
    public Dictionary<AchievementPanel.Names, int> data;
}



public class AchievementManager
{

    private int hiscore;

    private bool samesameTrack,
                 nomatchTrack,
                 blacktabooTrack,
                 max8for1;
    private int allblacks;

    private AchievementData achievements = new AchievementData();
    private string dataFile;

    private AchievementPanel panel;

    public AchievementManager(AchievementPanel _panel)
    {
        panel = _panel;
        LoadData();
        // foreach (AchievementDescription achievement in panel.achievementDescriptions)
        // {
        //     getAchievement(achievement.name, achievement.defaults);
        // }
        // Highs
        hiscore = getAchievement(AchievementPanel.Names.score10000);
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
        allblacks = 0;
    }

    public void LoadData()
    {
        dataFile = Application.dataPath + "/achievements.json";
        //Debug.Log((string)dataFile);
        //Debug.Log(File.Exists(dataFile));
        if (File.Exists(dataFile))
        {
            string filetext = File.ReadAllText(dataFile);
            achievements = JsonConvert.DeserializeObject<AchievementData>(filetext);
            if (achievements.data == null)
            {
                achievements.data = new Dictionary<AchievementPanel.Names, int>();
            }
        }
        else
        {
            achievements.data = new Dictionary<AchievementPanel.Names, int>();
        }
    }

    public void SaveData()
    {
        string filetext = JsonConvert.SerializeObject(achievements);
        File.WriteAllText(dataFile, filetext);
        //Debug.Log("SaveData: " + achievements.data.Keys.Count.ToString());
        //Debug.Log(filetext);
    }

    public int getAchievement(AchievementPanel.Names achievement, int defaultValue = 0)
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

    private bool setAchievement(AchievementPanel.Names achievement, int value = 1)
    {
        if (achievements.data.ContainsKey(achievement) ? achievements.data[achievement] != value : true)
        {
            achievements.data[achievement] = value;
            //SaveData();
            if (panel != null && value > 0)
            {
                panel.updateAchievement(achievement, value);
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
            changed = setAchievement(AchievementPanel.Names.score10000, hiscore) || changed;
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
        if (maxstack > getAchievement(AchievementPanel.Names.tall27))
        {
            changed = setAchievement(AchievementPanel.Names.tall27, maxstack) || changed;
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
            allblacks += 1;
            if (allblacks > getAchievement(AchievementPanel.Names.allblacks))
            {
                changed = setAchievement(AchievementPanel.Names.allblacks, allblacks) || changed;
            }
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

        if (round > getAchievement(AchievementPanel.Names.deep10))
        {
            changed = setAchievement(AchievementPanel.Names.deep10, round) || changed;
        }
        // Tracked achievements
        if (samesameTrack && round > getAchievement(AchievementPanel.Names.samesame))
        {
            changed = setAchievement(AchievementPanel.Names.samesame, round) || changed;
        }
        if (nomatchTrack && round > getAchievement(AchievementPanel.Names.nomatch))
        {
            changed = setAchievement(AchievementPanel.Names.nomatch, round) || changed;
        }
        if (blacktabooTrack && round > getAchievement(AchievementPanel.Names.blacktaboo))
        {
            changed = setAchievement(AchievementPanel.Names.blacktaboo, round) || changed;
        }
        // Socialism achievements
        if (round == 1 && stackno >= getAchievement(AchievementPanel.Names.saveallfor1))
        {
            changed = setAchievement(AchievementPanel.Names.saveallfor1, stackno) || changed;
        }
        if (round == 2 && stackno >= getAchievement(AchievementPanel.Names.save33for2))
        {
            changed = setAchievement(AchievementPanel.Names.save33for2, stackno) || changed;
        }
        if (round == 3 && stackno >= getAchievement(AchievementPanel.Names.save25for3))
        {
            changed = setAchievement(AchievementPanel.Names.save25for3, stackno) || changed;
        }
        if (round == 4 && stackno >= getAchievement(AchievementPanel.Names.save20for4))
        {
            changed = setAchievement(AchievementPanel.Names.save20for4, stackno) || changed;
        }
        if (round == 5 && stackno >= getAchievement(AchievementPanel.Names.save16for5))
        {
            changed = setAchievement(AchievementPanel.Names.save16for5, stackno) || changed;
        }
        if (round == 1 && stackno == 8)
        {
            max8for1 = true;
        }
        if (max8for1 && round > getAchievement(AchievementPanel.Names.max8for1) && stackno >= 1)
        {
            changed = setAchievement(AchievementPanel.Names.max8for1, round) || changed;
        }
        // Miser Achievements
        if (stackno == 0 && (round < getAchievement(AchievementPanel.Names.loseby4) || getAchievement(AchievementPanel.Names.loseby4) == 0))
        {
            changed = setAchievement(AchievementPanel.Names.loseby4, round) || changed;
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
