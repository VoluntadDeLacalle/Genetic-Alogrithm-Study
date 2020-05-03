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

    void Start()
    {
        currentGeneration.text = $"Current Generation: {GenerationManager.instance.currentGeneration.ToString()}";

        minutes = Mathf.Floor(GenerationManager.instance.popLifeSpan / 60).ToString("00");
        seconds = Mathf.Floor(GenerationManager.instance.popLifeSpan % 60).ToString("00");
        populationTimer.text = $"{minutes}:{seconds}";
    }

    public void UpdateGeneration()
    {
        currentGeneration.text = $"Current Generation: {GenerationManager.instance.currentGeneration.ToString()}";
    }

    public void UpdateTimer()
    {
        minutes = Mathf.Floor(GenerationManager.instance.popLifeSpan / 60).ToString("00");
        seconds = Mathf.Floor(GenerationManager.instance.popLifeSpan % 60).ToString("00");
        populationTimer.text = $"{minutes}:{seconds}";
    }

    public void PostFinalGeneration()
    {
        currentGeneration.text = "";
        populationTimer.text = "";
        populationTimerHeader.text = "";

        finalGeneration.text = $"Simulation Finished!\nGeneration: {GenerationManager.instance.currentGeneration.ToString()}";
        finalPanel.SetActive(true);
    }
}
