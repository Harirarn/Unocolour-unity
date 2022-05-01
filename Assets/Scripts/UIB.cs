using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIB : MonoBehaviour {

    public Text round;
    public Text hi;
    public Text score;

    public void setRound(int _round)
    {
        round.text = _round.ToString();
    }
    public void setScore(int _score, ref AchievementManager achievements)
    {
        score.text = _score.ToString();
        hi.text = achievements.NewHiscore(_score).ToString();
    }
    public void setHiscore(int _hiscore)
    {
        hi.text = _hiscore.ToString();
    }
}
