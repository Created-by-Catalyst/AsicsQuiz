using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    VideoPlayer videoPlayer;

    [SerializeField]
    VideoPlayer outroVideoPlayer;

    [SerializeField]
    VideoClip[] videos;

    [SerializeField]
    Animator anim;

    [SerializeField]
    TMP_Text loadingText;

    [SerializeField]
    TMP_Text correctAnswersText;

    [SerializeField]
    Image loadingCircle;


    [SerializeField]
    Button loadingContinueButton;


    [SerializeField]
    GameObject[] resultsScreens;

    int currentVideoID = 0;

    int currentQuestion = 1;

    int correctAnswers = 0;

    public void StarQuiz()
    {
        anim.Play("IntroOut");   

        if(PlayerPrefs.GetInt("firstLoad") == 0)
        {
            PlayerPrefs.SetInt("firstLoad", 1);
            PlayerPrefs.SetInt("shoeStock", 50);
        }

        print("shoe stock" + PlayerPrefs.GetInt("shoeStock"));
    }

    void EndQuiz(VideoPlayer vp)
    {
        outroVideoPlayer.clip = videos[4];
        outroVideoPlayer.isLooping = false;
        outroVideoPlayer.Play();
        outroVideoPlayer.loopPointReached -= EndQuiz;
        StartCoroutine(LoadResults());
    }

    IEnumerator LoadResults()
    {
        yield return new WaitForSeconds(5);

        anim.Play("CheckResultsIn");


        yield return new WaitForSeconds(2);

        float loadingProgress = 0;

        float loadingSpeed = 0.2f;

        while(loadingProgress < 1)
        {
            loadingProgress+= Time.deltaTime * loadingSpeed;

            if (loadingProgress > 0.3f && loadingProgress < 0.85f) loadingSpeed = 0.5f;

            if (loadingProgress > 0.85f) loadingSpeed = 0.15f;

            loadingCircle.fillAmount = loadingProgress;
            loadingText.text = $"{(loadingProgress*100):0}%";
            yield return null;
        }


        loadingCircle.fillAmount = 1;
        loadingText.text = "100%";

        loadingContinueButton.interactable = true;
    }

    public void ResetApp()
    {
        SceneManager.LoadScene(0);
    }

    public void CheckResults()
    {

        anim.Play("CheckResultsOut");

        if (correctAnswers == 3)
        {
            if (PlayerPrefs.GetInt("shoeStock") > 0)
            {
                if (Random.Range(0, 7) == 0)
                {
                    resultsScreens[0].SetActive(true);
                    PlayerPrefs.SetInt("shoeStock", PlayerPrefs.GetInt("shoeStock") - 1);
                }
                else
                {
                    resultsScreens[1].SetActive(true);
                }
            }
            else
            {
                resultsScreens[1].SetActive(true);
            }
        }
        else
        {
            correctAnswersText.text = correctAnswers.ToString() + "/3 Correct\nGreat Effort";
            resultsScreens[2].SetActive(true);

        }
    }

    public void AnswerQuestion(int answerValue)
    {
        correctAnswers += answerValue;

        currentQuestion++;

        anim.SetInteger("currentQuestion", currentQuestion);

        if (currentQuestion == 4)
        {

            outroVideoPlayer.loopPointReached += EndQuiz;
        }
    }


    public void PlayVideo(int id)
    {
        videoPlayer.clip = videos[id];
        videoPlayer.Play();
    }

    public void PlayVideoThenLoop(int id)
    {
        videoPlayer.clip = videos[id];
        videoPlayer.Play();

        currentVideoID = id;
        videoPlayer.loopPointReached += onVideoFinished;
    }


    void onVideoFinished(VideoPlayer vp)
    {
        videoPlayer.clip = videos[currentVideoID+1];
        videoPlayer.Play();
        videoPlayer.isLooping = true;

        videoPlayer.loopPointReached -= onVideoFinished;
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
