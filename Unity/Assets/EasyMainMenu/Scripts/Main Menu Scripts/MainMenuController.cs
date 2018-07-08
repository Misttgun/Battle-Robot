using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using Button = UnityEngine.UI.Button;

public class MainMenuController : MonoBehaviour {

    [SerializeField]
    private Animator anim;
    [SerializeField]
    private Animator envAnim;
    [SerializeField]
    private string newGameSceneName;

    [SerializeField] 
    private Button startButton;
    [SerializeField] 
    private Button[] backButton;
    

    [Header("Options Panel")]
    [SerializeField]
    private GameObject MainOptionsPanel;
    [SerializeField]
    private GameObject StartGameOptionsPanel;
    [SerializeField]
    private GameObject ControlsPanel;
    [SerializeField]
    private GameObject GfxPanel;
    [SerializeField]
    private GameObject LoadGamePanel;


    #region Open Different panels

    private void openOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(true);
        StartGameOptionsPanel.SetActive(false);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");
        envAnim.Play("EnvSlide");
        //play anim env style
        
        
        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");
       
    }

    private void openStartGameOptions()
    {
        //enable respective panel
        MainOptionsPanel.SetActive(false);
        StartGameOptionsPanel.SetActive(true);

        //play anim for opening main options panel
        anim.Play("buttonTweenAnims_on");
        envAnim.Play("EnvSlide");
        
        //play click sfx
        playClickSound();

        //enable BLUR
        //Camera.main.GetComponent<Animator>().Play("BlurOn");
        
    }

    private void openOptions_Game()
    {
        //enable respective panel
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");
        envAnim.Play("EnvSlide");
        
        //play click sfx
        playClickSound();

    }
    private void openOptions_Controls()
    {
        //enable respective panel
        ControlsPanel.SetActive(true);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(false);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }
    private void openOptions_Gfx()
    {
        //enable respective panel
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(true);
        LoadGamePanel.SetActive(false);
        
        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");
        envAnim.Play("GFXMenu");

        //play click sfx
        playClickSound();

    }

    private void openContinue_Load()
    {
        //enable respective panel
        ControlsPanel.SetActive(false);
        GfxPanel.SetActive(false);
        LoadGamePanel.SetActive(true);

        //play anim for opening game options panel
        anim.Play("OptTweenAnim_on");

        //play click sfx
        playClickSound();

    }

    private void newGame()
    {
        if (!string.IsNullOrEmpty(newGameSceneName))
            SceneManager.LoadScene(newGameSceneName);
        else
            Debug.Log("Please write a scene name in the 'newGameSceneName' field of the Main Menu Script and don't forget to " +
                "add that scene in the Build Settings!");
    }
    #endregion

    #region Back Buttons

    public void back_options()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("buttonTweenAnims_off");
        envAnim.Play("EnvSlideB");
        MainOptionsPanel.SetActive(false);
        //disable BLUR
       // Camera.main.GetComponent<Animator>().Play("BlurOff");
        
        //play click sfx
        playClickSound();
    }

    public void back_options_panels()
    {
        //simply play anim for CLOSING main options panel
        anim.Play("OptTweenAnim_off");
        envAnim.Play("EnvSlide");
        ControlsPanel.SetActive(false);
        //play click sfx
        playClickSound();

    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion

    #region Sounds
    public void playHoverClip()
    {
       
    }

    void playClickSound() {

    }


    #endregion
}
