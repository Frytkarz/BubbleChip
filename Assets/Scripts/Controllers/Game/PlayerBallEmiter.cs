using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace BubbleChip
{
    public class PlayerBallEmiter : MonoBehaviour
    {
        private const ushort FORCE_MULTIPLIER = 1;

        //inspektor
        public PlayerBallPrefabController playerBallPrefab;
        public ViewBallPrefabController viewBallPrefab;
        public Image imgNextPlayerBall;
        public BallsEmiter ballsEmiter;

        //prywatne
        private Vector2 touchPosition;
        private bool touched;
        private BallConfiguration nextPlayerBallConfiguration;
        private PlayerBallPrefabController playerBall;
        private bool readyToEmit = false;
        private readonly List<ViewBallPrefabController> usedViewBalls = new List<ViewBallPrefabController>();
        private readonly List<ViewBallPrefabController> emitedViewBalls = new List<ViewBallPrefabController>();
        private Coroutine showViewer;

        public void StartEmit()
        {
            enabled = true;
            readyToEmit = true;
            nextPlayerBallConfiguration = ballsEmiter.GetNextBallConfiguration();
            InstantiateNewPlayerBall();
        }
	
        // Update is called once per frame
        void Update () {
            //jeżeli piłka nie jest gotowa
            if(!readyToEmit)
                return;

            if ( ballsEmiter.gameController.isTouched
//#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
//                Input.touches.Length > 0
//#else
//                Input.GetMouseButton(0)
//#endif
                )
            {
                touchPosition =
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
                    Input.touches[0].position;
#else
                    
                Input.mousePosition;
#endif
                if(!touched)
                    EnableViewer();
                touched = true;
            }
            else if (touched)
            {
                DisableViewer();
                readyToEmit = false;
                touched = false;
                Emit();
            }
        }

        private void InstantiateNewPlayerBall()
        {
            playerBall = ballsEmiter.GetReusedPlayerBall() ?? Instantiate(playerBallPrefab);
            playerBall.transform.SetParent(transform, false);
            if (!ballsEmiter.CheckNextBallConfiguration(nextPlayerBallConfiguration))
                nextPlayerBallConfiguration = ballsEmiter.GetNextBallConfiguration();
            BallConfiguration ballConfiguration = nextPlayerBallConfiguration;
            nextPlayerBallConfiguration = ballsEmiter.GetNextBallConfiguration();
            imgNextPlayerBall.material = nextPlayerBallConfiguration.material;
            playerBall.Set(ballConfiguration, this);
        }

        private void Emit()
        {
            Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
            Vector2 force = new Vector2(
                touchWorldPosition.x - transform.position.x,
                touchWorldPosition.y - transform.position.y)
                .normalized * FORCE_MULTIPLIER;

            playerBall.transform.SetParent(ballsEmiter.transform);
            playerBall.Emit(force);
            touched = false;
        }


        /// <summary>
        /// Wywołuje player ball gdy doleci do kólek
        /// </summary>
        public void OnPlayerBallStay(BallPrefabController collisionBall)
        {
            //przekazanie do obliczenia pozycji w emiterze kólek
            ballsEmiter.OnPlayerBallStay(playerBall, collisionBall);
        }

        public void NextBall()
        {
            InstantiateNewPlayerBall();
            readyToEmit = true;
        }

        public void ReUse(ViewBallPrefabController ball)
        {
                emitedViewBalls.Remove(ball);
                usedViewBalls.Add(ball);
        }

        private void EnableViewer()
        {
            showViewer = StartCoroutine(ShowViewer());
        }

        private void DisableViewer()
        {
            if(showViewer != null)
                StopCoroutine(showViewer);

                foreach (ViewBallPrefabController ball in emitedViewBalls)
                {
                    ball.ReUse();
                }
                emitedViewBalls.Clear();
        }

        private IEnumerator ShowViewer()
        {
            yield return new WaitForSeconds(0.2f);
            while (true)
            {
                ViewBallPrefabController ball;
                    int count = usedViewBalls.Count;

                    if (count > 0)
                    {
                        ball = usedViewBalls[count - 1];
                        usedViewBalls.RemoveAt(count - 1);
                    }
                    else
                    {
                        ball = Instantiate(viewBallPrefab);
                        ball.transform.SetParent(transform, false);
                        ball.Initialize(this);
                    }

                        emitedViewBalls.Add(ball);

                Vector3 touchWorldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
                Vector2 force = new Vector2(
                    touchWorldPosition.x - transform.position.x,
                    touchWorldPosition.y - transform.position.y)
                    .normalized * FORCE_MULTIPLIER;
                ball.Emit(force, playerBall.ballConfiguration);

                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
