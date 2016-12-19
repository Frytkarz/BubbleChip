using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

namespace BubbleChip
{
    public class GameController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const int BONUS_NUMBER = 50;

        public SizeAdjuster sizeAdjuster;
        public PlayerBallEmiter playerBallEmiter;
        public BallsEmiter ballsEmiter;
        public SettingsPanelController settings;
        public Text txtPoints;
        public BonusController bonus;
        public AudioSource aSoWin, aSoLose;

        public bool isTouched { get; private set; }

        private int points = 0;

        // Use this for initialization
        public IEnumerator Start () {
            Debug.Log("Starting game");
            yield return null;
            sizeAdjuster.Adjust();
            yield return StartCoroutine(ballsEmiter.StartEmit());
        }

//        public void ScoreOne()
//        {
//            txtPoints.text = (++points).ToString();
//        }
//
//        public void Score(int scoredBalls)
//        {
//            points += scoredBalls;
//            txtPoints.text = points.ToString();
//        }

        public void ScoreCombo(int comboCount)
        {
            if (comboCount >= 2)
            {
                this.bonus.Set("COMBO x" + comboCount);
            }            
        }

        public int Score(int comboCount, int ballNumber)
        {
            points += comboCount;
            txtPoints.text = points.ToString();

            if (ballNumber%10 == 0)
            {
                int bonus = ballNumber/10;
                bonus *= BONUS_NUMBER;
                this.bonus.Set("BONUS +" + bonus);
                points += bonus;
            }

            return comboCount;
        }

        public void Win()
        {
            Debug.Log("Winner!");
            aSoWin.Play();
            Dialog.Get().Set("Congratulation, you won! What do you want to do now?", 
                () =>{SceneManager.LoadScene(SceneUtils.MENU_SCENE);}
                , SceneUtils.ReloadScene, "Back to menu", "Play again");
        }

        public void Loose()
        {
            Debug.Log("Looser");
            aSoLose.Play();
            Dialog.Get().Set("Too bad, you loose! What do you want to do now?", 
                () =>{SceneManager.LoadScene(SceneUtils.MENU_SCENE);}
                , SceneUtils.ReloadScene, "Back to menu", "Play again");
        }

        public void OnMenuClick()
        {
            Dialog.Get().Set("Do you really want to leave game?", 
                () => { SceneManager.LoadScene(SceneUtils.MENU_SCENE); }, null, "Yes");
        }

        public void OnSettingsClick()
        {
            settings.gameObject.SetActive(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isTouched = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isTouched = false;
        }
    }
}
