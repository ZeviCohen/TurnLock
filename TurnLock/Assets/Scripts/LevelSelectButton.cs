using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private Button button;
    public GameObject lvlSelectCanvas;
    public GameObject inLvlCanvas;
    // Start is called before the first frame update
    void Start()
    { 
        button = GetComponent<Button>();
        button.onClick.AddListener(LoadLevelSelect);  
    }

    void LoadLevelSelect() {
        lvlSelectCanvas.SetActive(true);
        inLvlCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
