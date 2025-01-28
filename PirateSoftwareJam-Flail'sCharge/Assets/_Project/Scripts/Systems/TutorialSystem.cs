using UnityEngine;

public class TutorialSystem : MonoBehaviour
{

    public static TutorialSystem instance;
    public bool isShowingTutorial = true;

    [SerializeField] CanvasGroup tutorialCanvas;

    private void Awake()
    {
        instance = this;
    }

    public void FinishTutorial()
    {
        isShowingTutorial = false;
        tutorialCanvas.alpha = 0;
        tutorialCanvas.blocksRaycasts = false;
        tutorialCanvas.interactable = false;
    }
}
