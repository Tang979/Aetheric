using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialStep
{
    Welcome,
    SummonTower,
    StartWave,
    UpgradeTower,
    Done
}

public class TutorialManager : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static TutorialManager Instance;

    public GameObject highlightArrow;
    public GameObject dialoguePanel;
    public Text dialogueText;

    private TutorialStep currentStep = TutorialStep.Welcome;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
    }

    private void Start()
    {
        StartCoroutine(HandleStep());
        HighlightObject(GameObject.Find("SummonButton"));
    }

    private IEnumerator HandleStep()
    {
        switch (currentStep)
        {
            case TutorialStep.Welcome:
                ShowDialogue("Chào mừng đến với Aetheric!");
                yield return WaitForClick();
                NextStep();
                break;

            case TutorialStep.SummonTower:
                ShowDialogue("Nhấn nút triệu hồi để tạo tháp.");
                HighlightObject(GameObject.Find("SummonButton"));
                // yield return new WaitUntil(() => TowerManager.Instance.HasSummoned);
                RemoveHighlight();
                NextStep();
                break;

                // Tiếp tục cho các bước tiếp theo
        }
    }

    private void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
    }

    private void RemoveHighlight()
    {
        highlightArrow.SetActive(false);
    }

    private void HighlightObject(GameObject target)
    {
        highlightArrow.SetActive(true);
        highlightArrow.transform.position = target.transform.position + Vector3.up * 50;
    }

    private void NextStep()
    {
        currentStep++;
        StartCoroutine(HandleStep());
    }

    private IEnumerator WaitForClick()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
    }
}
