using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSimulation : MonoBehaviour
{
    public SkillGameSettings gameSettings;
    public SequenceVisualizer visualizer;
    public ControllerOverlay overlay;

    public List<Skill> skills = new();
    public Skill currentSkill;

    public int currentSkillIndex = 0;
    public float inputDelay = 1f;

    private void Start()
    {
        skills = new(gameSettings.allSkillMoves);

        if (skills.Count > 0)
            StartCoroutine(StartSkill());
    }

    IEnumerator StartSkill()
    {
        while (currentSkillIndex < skills.Count)
        {
            overlay.ResetColors();

            currentSkill = skills[currentSkillIndex];
            yield return new WaitForSeconds(1f);

            if (currentSkill.inputSequence == null || currentSkill.inputSequence.Count == 0)
            {
                Debug.LogError("Current skill has no input sequence.");
                currentSkillIndex++;
                continue;
            }

            List<SkillInputHolder> sequence = currentSkill.inputSequence;

            if (visualizer != null)
                visualizer.VisualizeSequence(sequence, -1);
            else
                Debug.LogWarning("Visualizer not assigned!");

            for (int i = 0; i < sequence[0].input.Count; i++)
            {
                SkillInput input = sequence[0].input[i];

                if (input.ToString().StartsWith("L3") || input.ToString().StartsWith("R3"))
                {
                    overlay.ResetStick(input);
                    yield return new WaitForSeconds(0.2f);
                }
                yield return new WaitForSeconds(0.5f);

                PerformInput(input);

                if (visualizer != null)
                    visualizer.VisualizeSequence(sequence, i);

                yield return new WaitForSeconds(inputDelay);

                if (!IsHoldInput(input))
                    overlay.ResetColors(input);
            }

            currentSkillIndex++;
            Debug.Log("Next Skill");
        }
    }

    private bool IsHoldInput(SkillInput input)
    {
        string name = input.ToString();
        return name.StartsWith("Hold_") || name.EndsWith("_Hold");
    }

    private void PerformInput(SkillInput input)
    {
        if (overlay != null)
        {
            overlay.SimulateButton(input);
            overlay.SetStick(input);
        }
        Debug.Log($"Performing input: {input}");
    }
}
