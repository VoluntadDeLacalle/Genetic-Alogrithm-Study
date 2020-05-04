using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public TextMeshProUGUI currentGeneration;
    public TextMeshProUGUI finalGeneration;
    public GameObject finalPanel;

    public TextMeshProUGUI populationTimer;
    public TextMeshProUGUI populationTimerHeader;

    private string minutes = "";
    private string seconds = "";

    /// <summary>
    /// Initializes UI values.
    /// </summary>
    void Start()
    {
        currentGeneration.text = $"Current Generation: {GenerationManager.instance.currentGeneration.ToString()}";

        minutes = Mathf.Floor(GenerationManager.instance.popLifeSpan / 60).ToString("00");
        seconds = Mathf.Floor(GenerationManager.instance.popLifeSpan % 60).ToString("00");
        populationTimer.text = $"{minutes}:{seconds}";
    }

    /// <summary>
    /// Updates the generation display on the top left of the simulation view.
    /// </summary>
    public void UpdateGeneration()
    {
        currentGeneration.text = $"Current Generation: {GenerationManager.instance.currentGeneration.ToString()}";
    }

    /// <summary>
    /// Updates the timer display on the top right of the simulation view.
    /// </summary>
    public void UpdateTimer()
    {
        minutes = Mathf.Floor(GenerationManager.instance.popLifeSpan / 60).ToString("00");
        seconds = Mathf.Floor(GenerationManager.instance.popLifeSpan % 60).ToString("00");
        populationTimer.text = $"{minutes}:{seconds}";
    }

    /// <summary>
    /// Brings up the display of the final generation once the genetic algorithm has reached a solution.
    /// </summary>
    public void PostFinalGeneration()
    {
        currentGeneration.text = "";
        populationTimer.text = "";
        populationTimerHeader.text = "";

        finalGeneration.text = $"Simulation Finished!\nGeneration: {GenerationManager.instance.currentGeneration.ToString()}";
        finalPanel.SetActive(true);
    }
}
