using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] bool pauseTurn;

    public static bool Pause { get; private set; }

    [SerializeField] GameObject[] disabledObjects = new GameObject[0];
    static GameObject[] _disabledObjects;


    static Game game;

    private void Start() {
        game = this;
        _disabledObjects = disabledObjects;
        SetPause(pauseTurn);
    }

    private void Update() {
        if (pauseTurn != Pause) {
            SetPause(pauseTurn);
        }


    }



    public static void SetPause(bool active) {
        Pause = game.pauseTurn = active;

        foreach (GameObject obj in _disabledObjects) {
            obj.gameObject.SetActive(!active);
        }

    }




}
