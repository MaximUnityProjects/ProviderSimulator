using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {

    [SerializeField] GameObject[] disabledObjects = new GameObject[0];
    [SerializeField] GameObject[] enabledObjects = new GameObject[0];
    [Header("Pause")]
    [SerializeField] bool pauseSwitch;
    [SerializeField] bool setPause;
    [SerializeField] bool pauseValue;
    
    ButtonSet buttonSet;

    private void Start() {
        buttonSet = new ButtonSet {
            disabledObjects = disabledObjects,
            enabledObjects = enabledObjects
        };
        GetComponent<Button>().onClick.AddListener(() => { OnButtonClick(); });
    }


    private void OnButtonClick() {
        if (pauseSwitch) {
            Game.SetPause(!Game.Pause);
        }
        else if (setPause) {
            Game.SetPause(pauseValue);
        }


        buttonSet.Enable();
        buttonSet.Disable();
    }
}





public class ButtonSet {
    public GameObject[] disabledObjects;
    public GameObject[] enabledObjects;

    public void Enable() {
        foreach (GameObject obj in enabledObjects) {
            obj.SetActive(true);
        }
    }

    public void Disable() {
        foreach (GameObject obj in disabledObjects) {
            obj.SetActive(false);
        }
    }
}
