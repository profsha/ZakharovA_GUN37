using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        scoreText.text = "Score: 0";
    }

    public void UpdateScore(List<int> rolls)
    {
        int score = 0;
        int rollIndex = 0;

        for (int frame = 0; frame < 10; frame++)
        {
            if (!HasEnoughRolls(rolls, rollIndex, frame))
                break;

            if (IsStrike(rolls, rollIndex))
            {
                if (!HasStrikeBonus(rolls, rollIndex))
                    break;

                score += 10 + StrikeBonus(rolls, rollIndex);
                rollIndex++;
            }
            else if (IsSpare(rolls, rollIndex))
            {
                if (!HasSpareBonus(rolls, rollIndex))
                    break;

                score += 10 + SpareBonus(rolls, rollIndex);
                rollIndex += 2;
            }
            else
            {
                score += SumOfBallsInFrame(rolls, rollIndex);
                rollIndex += 2;
            }
        }

        scoreText.text = "Score: " + score;
    }

    private bool HasEnoughRolls(List<int> rolls, int rollIndex, int frame)
    {
        if (frame < 9)
        {
            if (IsStrike(rolls, rollIndex))
                return rollIndex + 1 < rolls.Count;
            return rollIndex + 1 < rolls.Count;
        }

        if (IsStrike(rolls, rollIndex))
            return rollIndex + 2 < rolls.Count;
        if (IsSpare(rolls, rollIndex))
            return rollIndex + 2 < rolls.Count;
        return rollIndex + 1 < rolls.Count;
    }

    private bool HasStrikeBonus(List<int> rolls, int rollIndex) => rollIndex + 2 < rolls.Count;
    private bool HasSpareBonus(List<int> rolls, int rollIndex) => rollIndex + 2 < rolls.Count;

    private bool IsStrike(List<int> rolls, int rollIndex) => rolls[rollIndex] == 10;
    private bool IsSpare(List<int> rolls, int rollIndex) => rolls[rollIndex] + rolls[rollIndex + 1] == 10;
    private int SumOfBallsInFrame(List<int> rolls, int rollIndex) => rolls[rollIndex] + rolls[rollIndex + 1];
    private int StrikeBonus(List<int> rolls, int rollIndex) => rolls[rollIndex + 1] + rolls[rollIndex + 2];
    private int SpareBonus(List<int> rolls, int rollIndex) => rolls[rollIndex + 2];
}