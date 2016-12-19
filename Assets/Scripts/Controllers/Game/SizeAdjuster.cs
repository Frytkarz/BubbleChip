using System;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace BubbleChip
{
    public class SizeAdjuster : MonoBehaviour
    {
        private const ushort ZONE_OFFSET = 100;

        public RectTransform trsGamePanel, trsPlayerBallEmiter, trsPlayerBall, trsBallPrefab,
            trsViewBallPrefab, trsGameOver;
        public LayoutElement lelNextPlayerBall;
        public BoxCollider2D colZoneLeft, colZoneTop, colZoneRight;
        public PolygonCollider2D colZoneBottom;

        public float gameZoneWidth { get; private set; }
        public float gameZoneHeight { get; private set; }
        public float ballDiameter { get; private set; }
        public float ballRadius { get; private set; }
        public float rowHeight { get; private set; }

        /// <summary>
        /// Dostosowuje wielkości elementów pod daną rozdzielczośc i orientacje
        /// </summary>
        public void Adjust()
        {
            gameZoneWidth = trsGamePanel.rect.width;
            gameZoneHeight = trsGamePanel.rect.height;

            //brzegi panelu gry
            colZoneLeft.size = colZoneRight.size = 
                new Vector2(ZONE_OFFSET, gameZoneHeight);
            colZoneTop.size =
                new Vector2(gameZoneWidth + ZONE_OFFSET * 2, ZONE_OFFSET);
            float width2 = gameZoneWidth/2;
            colZoneBottom.SetPath(0, new []
            {
                new Vector2(width2, -50), new Vector2(width2, 100),
                new Vector2(0, 0),  new Vector2(-width2, 100),
                new Vector2(-width2, -50)
            });

            //wielkości piłek
            int rows;
            if (Screen.orientation == ScreenOrientation.Portrait ||
                Screen.orientation == ScreenOrientation.PortraitUpsideDown)
            {
                ballDiameter = gameZoneWidth/(BallsEmiter.PORTRAIT_COLUMNS + 0.5f);
                rows = BallsEmiter.BALLS_TO_EMIT/BallsEmiter.PORTRAIT_COLUMNS;

            }

            else
            {
                ballDiameter = gameZoneWidth/(BallsEmiter.LANDSCAPE_COLUMNS + 0.5f);
                rows = BallsEmiter.BALLS_TO_EMIT / BallsEmiter.LANDSCAPE_COLUMNS;
            }

            ballRadius = ballDiameter/2;
            rowHeight = (float)(ballRadius * Math.Sqrt(3));
            trsGameOver.anchoredPosition =
                new Vector2(0, -(ballDiameter + rowHeight * (rows * BallsEmiter.ALL_ROWS_MULTIPLIER - 1)));

            trsPlayerBallEmiter.anchoredPosition =
                new Vector2(trsPlayerBallEmiter.anchoredPosition.x, ballDiameter);
            trsBallPrefab.sizeDelta = trsPlayerBall.sizeDelta = 
                new Vector2(ballDiameter, ballDiameter);
            trsPlayerBall.GetComponent<CircleCollider2D>().radius =
                trsViewBallPrefab.GetComponent<CircleCollider2D>().radius = ballRadius;
            trsBallPrefab.GetComponent<CircleCollider2D>().radius = ballRadius * 0.6f;

            lelNextPlayerBall.preferredHeight = lelNextPlayerBall.preferredWidth = ballDiameter;
        }
    }
}