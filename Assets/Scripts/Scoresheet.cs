using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class Scoresheet : MonoBehaviour {


    public Text score;
    public Text prevScore;
    public Text buttonText;
    public List<Text> stacks;
    public List<Text> counts;
    public List<Text> scores;

    public void Button()
    {
        gameObject.SetActive(false);
    }

    public int ShowScore(int round, int prevScore, Dictionary<int, int> stackCounts,bool final)
    {
        int score = prevScore;
        int uniqStackSizes = stackCounts.Keys.Count;
        int usi = 0;
        int otherScore = 0;
        int otherCount = 0;
        int eigthkey = 0;
        gameObject.SetActive(true);
        this.prevScore.text = prevScore.ToString();
        for (int key = 1; usi < uniqStackSizes; key++)
        {
            if (stackCounts.ContainsKey(key))
            {
                int subScore = round * Triangle(key) * stackCounts[key];
                score += subScore;
                if (usi >= 7)
                {
                    eigthkey = key;
                    otherScore += subScore;
                    otherCount += stackCounts[key];
                }
                else
                {
                    stacks[usi].text = key.ToString();
                    counts[usi].text = stackCounts[key].ToString();
                    scores[usi].text = subScore.ToString();
                }
                usi += 1;
            }
        }
        if (usi >= 8)
        {
            stacks[7].text = usi == 8 ? eigthkey.ToString() : "Other";
            counts[7].text = otherCount.ToString();
            scores[7].text = otherScore.ToString();
        }
        while (usi < 8)
        {
            stacks[usi].text = "";
            counts[usi].text = "";
            scores[usi].text = "";
            usi += 1;
        }
        this.score.text = score.ToString();
        buttonText.text = final ? "New Game" : "Continue";
        return score;
    }

    private int Triangle(int n)
    {
        return n * (n + 1) / 2;
    }
}
