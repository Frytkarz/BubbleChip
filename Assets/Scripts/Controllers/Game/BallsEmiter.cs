using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

namespace BubbleChip
{
    public class BallsEmiter : MonoBehaviour
    {
        public const ushort PORTRAIT_COLUMNS = 11;
        public const ushort LANDSCAPE_COLUMNS = PORTRAIT_COLUMNS * LANDSCAPE_MULTIPLIER;
        public const ushort LANDSCAPE_MULTIPLIER = 2;
        public const ushort BALLS_TO_EMIT = LANDSCAPE_COLUMNS * 3;
        public const ushort ALL_ROWS_MULTIPLIER = 2;
        public const ushort ADD_BALL_MIN = 4;
        public const ushort ADD_BALL_MAX = 7;

        public SizeAdjuster sizeAdjuster;
        public BallPrefabController ballPrefab;
        public GameController gameController;
        public PlayerBallEmiter playerBallEmiter;
        public List<BallConfiguration> configurations;

        private BallPrefabController[,] balls;
        private List<BallPrefabController> usedBalls;
        private List<PlayerBallPrefabController> usedPlayerBalls;
        //liczebnośc kulek w każdej z konfiguracji
        private Dictionary<BallConfiguration, int> ballConfigurations;
        private int columns, rows, ballsRows;
        private int lastBallEmit, nextBallEmit, emitedPlayerBallsCount;
        private bool firstBallPaddingLeft = true;
        private int comboCount;

        /// <summary>
        /// Rozpoczyna emitowanie kólek
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartEmit()
        {           
            if (Screen.orientation == ScreenOrientation.Portrait ||
                Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                columns = PORTRAIT_COLUMNS;
            else
                columns = LANDSCAPE_COLUMNS;

            rows = BALLS_TO_EMIT/columns;
            //TODO hardcoding
            ballsRows = rows*ALL_ROWS_MULTIPLIER + 1;

            balls = new BallPrefabController[ballsRows, columns];
            usedBalls = new List<BallPrefabController>(ballsRows * columns);
            usedPlayerBalls = new List<PlayerBallPrefabController>(10);

            ballConfigurations = 
                new Dictionary<BallConfiguration, int>(configurations.Count);
            foreach (BallConfiguration ballConfiguration in configurations)
            {
                ballConfigurations.Add(ballConfiguration, 0);
            }

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    BallPrefabController newBall = Instantiate(ballPrefab);
                    newBall.transform.SetParent(transform, false);
                    balls[row, column] = newBall;
                    newBall.Set(new Vector2Int(row, column), GetBallPosition(row, column), 
                        GetRandomBallConfiguration());
                    yield return null;
                }
            }
            nextBallEmit = Random.Range(ADD_BALL_MIN, ADD_BALL_MAX);

            playerBallEmiter.StartEmit();
        }

        public void EmitRow()
        {
            if(emitedPlayerBallsCount - lastBallEmit != nextBallEmit)
                return;

            firstBallPaddingLeft = !firstBallPaddingLeft;
            MoveAllBallsDown();

            lastBallEmit = emitedPlayerBallsCount + nextBallEmit;
            nextBallEmit = Random.Range(ADD_BALL_MIN, ADD_BALL_MAX);
            List<BallPrefabController> ballsToEmit = new List<BallPrefabController>(columns);

            int count = usedBalls.Count;
            for (int i = count - 1; i >= 0 && count - i <= columns; i--)
            {
                usedBalls[i].ReUse();
                ballsToEmit.Add(usedBalls[i]);
                usedBalls.RemoveAt(i);
            }

            count = ballsToEmit.Count;
            for (int i = count; i < columns; i++)
            {
                BallPrefabController newBall = Instantiate(ballPrefab);
                newBall.transform.SetParent(transform, false);
                ballsToEmit.Add(newBall);
            }

            for (int column = 0; column < columns; column++)
            {
                balls[0, column] = ballsToEmit[column];
                ballsToEmit[column].Set(new Vector2Int(0, column), GetBallPosition(0, column),
                    GetNextBallConfiguration());
                ballConfigurations[ballsToEmit[column].ballConfiguration] ++;
            }
        }

        private void MoveAllBallsDown()
        {
            for (int row = ballsRows - 2; row >= 0; row--)
            {
                for (int column = columns - 1; column >= 0; column--)
                {
                    int newRow = row + 1;
                    if (balls[row, column] == null)
                    {
                        balls[newRow, column] = null;
                        continue;                        
                    }

                    balls[newRow, column] = balls[row, column];
                    balls[row, column] = null;
                    balls[newRow, column].Set(new Vector2Int(newRow, column), GetBallPosition(newRow, column));
                }
            }
        }

        /// <summary>
        /// Zwaraca pozycje piłki dla podanego rzędu i kolumny
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private Vector2 GetBallPosition(int row, int column)
        {
            Vector2 result;
            if ((firstBallPaddingLeft && row % 2 == 0) || (!firstBallPaddingLeft && row % 2 == 1))//zmiana
                result = new Vector2(column * sizeAdjuster.ballDiameter + sizeAdjuster.ballRadius,
                    -(row * sizeAdjuster.rowHeight + sizeAdjuster.ballRadius));
            else
                result = new Vector2(column * sizeAdjuster.ballDiameter + sizeAdjuster.ballDiameter,
                    -(row * sizeAdjuster.rowHeight + sizeAdjuster.ballRadius));
            return result;
        }

        public void ReUse(BallPrefabController ball)
        {
            if(ball.GetType() == typeof(PlayerBallPrefabController))
                usedPlayerBalls.Add(ball as PlayerBallPrefabController);
            else
                usedBalls.Add(ball);
        }

        public PlayerBallPrefabController GetReusedPlayerBall()
        {
            int index = usedPlayerBalls.Count - 1;
            if (index < 0)
                return null;
            PlayerBallPrefabController result = usedPlayerBalls[index];
            usedPlayerBalls.RemoveAt(index);
            result.ReUse();
            return result;
        }

        /// <summary>
        /// Metoda którą wywołuje pierwotnie player ball gdy doleci do kólek i zatrzyma swoją pozycję.
        /// Zwraca true jeżeli koniec gry.
        /// </summary>
        /// <param name="ball"></param>
        public void OnPlayerBallStay(BallPrefabController ball, BallPrefabController collisionBall)
        {
            ballConfigurations[ball.ballConfiguration] ++;
            //dobieranie pozycji dla piłki
            float row = -((ball.rectTransform.anchoredPosition.y - sizeAdjuster.ballRadius) / sizeAdjuster.rowHeight);
            int rowInt = (int) (row - 0.5f);
            //kolizja z sufitem
            if (collisionBall == null)
                rowInt = 0;
            Vector2Int position = null;


            
            float column;
            if ((firstBallPaddingLeft && rowInt % 2 == 0) || (!firstBallPaddingLeft && rowInt % 2 == 1))//zmiana

                column = (ball.rectTransform.anchoredPosition.x - sizeAdjuster.ballRadius)/
                    sizeAdjuster.ballDiameter;
            else
                column = (ball.rectTransform.anchoredPosition.x - sizeAdjuster.ballDiameter) /
                    sizeAdjuster.ballDiameter;
            int columnInt = (int) (column + 0.5f);
            if (columnInt < 0)
                columnInt = 0;
            else if (columnInt >= columns)
                columnInt = columns - 1;

            position = new Vector2Int(rowInt, columnInt);
            if (!IsPositionExistAndEmpty(position))
            {
                if (collisionBall != null)
                {
                    float diff = int.MaxValue;
                    List<Vector2Int> possiblePositions = new List<Vector2Int>(6);
                    GetNeighborsPositions(possiblePositions, collisionBall);
                    possiblePositions = possiblePositions.FindAll(IsPositionExistAndEmpty);
                    foreach (Vector2Int p in possiblePositions)
                    {
                        float rowDiff = Math.Abs(p.x - row);
                        if (rowDiff < diff)
                        {
                            position = p;
                            diff = rowDiff;
                        }
                    }

                    if ((firstBallPaddingLeft && position.x%2 == 0) ||
                        (!firstBallPaddingLeft && position.x%2 == 1)) //zmiana

                        column = (ball.rectTransform.anchoredPosition.x - sizeAdjuster.ballRadius)/
                                 sizeAdjuster.ballDiameter;
                    else
                        column = (ball.rectTransform.anchoredPosition.x - sizeAdjuster.ballDiameter)/
                                 sizeAdjuster.ballDiameter;

                    foreach (Vector2Int p in possiblePositions)
                    {
                        float sumdiff = Math.Abs(p.x - row + p.y - column);
                        if (sumdiff < diff)
                        {
                            position = p;
                            diff = sumdiff;
                        }
                    }
                }
                else
                {
                    //TODO dla kolizji z sufitem, ale chyba wszystko zawsze działa
                }
            }

            //spróbuj ustawić pozycje
            if (TrySetBallPosition(ball, position))
            {
                //punktuj!
                Score(ball);
            }
            else
            {
                //co prawda nie wchodzi tu raczej ale jakby co to:
                ball.DestroyBall(this, 0);
                playerBallEmiter.NextBall();
                Debug.LogError("Could not change ball position for row = " + row + "(" + rowInt +
                    ") and column = " + column + "(" +columnInt + ")");            
            }
        }

        /// <summary>
        /// Próbuje przypisać pozycje dla piłki, zwraca true jeżeli się powiedzie
        /// </summary>
        /// <param name="ball"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool TrySetBallPosition(BallPrefabController ball, Vector2Int position)
        {
            if (IsPositionExistAndEmpty(position))
            {
                balls[position.x, position.y] = ball;
                ball.Set(position, GetBallPosition(position.x, position.y));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sprawdza czy dla przypisanej do player ball pozycji następuje zbicie.
        /// Jeżeli tak to zbija i punktuje.
        /// </summary>
        /// <param name="ball"></param>
        /// <returns></returns>
        private void Score(BallPrefabController ball)
        {

            List<BallPrefabController> scoredBalls = new List<BallPrefabController>
            {
                ball
            };
            List<BallPrefabController> neighbors = new List<BallPrefabController>();

            //sprawdz takie same piłki
            AddSameBalls(scoredBalls, neighbors, ball);

            //jeżeli są conajmniej 3
            if (scoredBalls.Count < 3)
            {
                EmitRow();
                if(CheckLose())
                {
                    gameController.Loose();
                }
                else
                {
                    comboCount = 0;
                    emitedPlayerBallsCount++;
                    playerBallEmiter.NextBall();
                }
                return;
            }

            comboCount++;
            //usuń referencje
            foreach (BallPrefabController b in scoredBalls)
            {
                RemoveBall(b);
            }

            //samotne wyspy piłek
            List<Island> aloneIslands = new List<Island>();
            GetIslandBalls(neighbors, aloneIslands);
            //usun referencje
            foreach (Island island in aloneIslands)
            {
                foreach (BallPrefabController b in island.balls)
                {
                    RemoveBall(b);
                }
            }
            //zniszcz
            StartCoroutine(DestroyBalls(scoredBalls, aloneIslands));
        }

        /// <summary>
        /// Dodaje do przekaznej listy sąsiadów od ball którzy mają ten sam BallConfiguration co ball
        /// </summary>
        /// <param name="sameBalls"></param>
        /// <param name="neighbors"></param>
        /// <param name="ball"></param>

        private void AddSameBalls(List<BallPrefabController> sameBalls, 
            List<BallPrefabController> neighbors, BallPrefabController ball)
        {           
            List<Vector2Int> positions = new List<Vector2Int>(6);
            GetNeighborsPositions(positions, ball);

            List<BallPrefabController> newBalls = new List<BallPrefabController>(6);
            foreach (Vector2Int p in positions)
            {
                if (p.x < 0 || p.x > ballsRows - 1 ||
                    p.y < 0 || p.y > columns - 1)
                    continue;

                BallPrefabController b = balls[p.x, p.y];
                if(b == null)
                    continue;

                if (b.ballConfiguration.Equals(ball.ballConfiguration))
                {
                    if(!sameBalls.Contains(b))
                        newBalls.Add(b);
                }
                else
                {
                    if (!neighbors.Contains(b))
                        neighbors.Add(b);
                }

//                if (b != null && 
//                    b.ballConfiguration.Equals(ball.ballConfiguration) &&
//                    !sameBalls.Contains(b))
//                    newBalls.Add(b);
            }

            sameBalls.AddRange(newBalls);

            foreach (BallPrefabController newBall in newBalls)
            {
                AddSameBalls(sameBalls, neighbors, newBall);
            }
        }

        /// <summary>
        /// Usuwa piłkę z logiki
        /// </summary>
        /// <param name="ball"></param>
        private void RemoveBall(BallPrefabController ball)
        {
            balls[ball.position.x, ball.position.y] = null;
            ballConfigurations[ball.ballConfiguration] --;
            if (ballConfigurations[ball.ballConfiguration] < 1)
                ballConfigurations.Remove(ball.ballConfiguration);
        }

        /// <summary>
        /// Niszczy kolejno piłki z podanej listy
        /// </summary>
        /// <param name="balls"></param>
        /// <returns></returns>
        private IEnumerator DestroyBalls(List<BallPrefabController> balls, List<Island> islands)
        {
            gameController.ScoreCombo(comboCount);
            int i = 0;
            for (; i < balls.Count; i++)
            {
                balls[i].DestroyBall(this, gameController.Score(comboCount, i + 1));
                yield return new WaitForSeconds(0.1f);
            }

            foreach (Island island in islands)
            {
                foreach (BallPrefabController ball in island.balls)
                {
                    ball.DestroyBall(this, gameController.Score(comboCount, i++));
                    yield return new WaitForSeconds(0.1f);
                }
            }

            foreach (BallPrefabController ball in this.balls)
            {
                if (ball != null)
                {
                    EmitRow();
                    if(CheckLose())
                    {
                        gameController.Loose();
                    }
                    else
                    {
                        emitedPlayerBallsCount ++;
                        playerBallEmiter.NextBall();
                    }   
                    yield break;                                     
                }
            }

            //brak kul
            gameController.Win();
        }

        /// <summary>
        /// Zwraca losową konfiguracje
        /// </summary>
        /// <returns></returns>
        private BallConfiguration GetRandomBallConfiguration()
        {
            BallConfiguration ballConfiguration = configurations.Random(); //BallConfiguration.RandomConfiguration;
            ballConfigurations[ballConfiguration] ++;
            return ballConfiguration;
        }

        /// <summary>
        /// Zwraca losową konfiguracje z obecnych jeszcze w logice kolorów
        /// </summary>
        /// <returns></returns>
        public BallConfiguration GetNextBallConfiguration()
        {
            return ballConfigurations.Keys.ElementAt(Random.Range(0, ballConfigurations.Count));
        }

        /// <summary>
        /// Sprawdza czy konfiguracja jest poprawna, tzn. czy istnieje jeszcze w logice obiekt o takiej samej
        /// </summary>
        /// <param name="ballConfiguration"></param>
        /// <returns></returns>
        public bool CheckNextBallConfiguration(BallConfiguration ballConfiguration)
        {
            return ballConfigurations.ContainsKey(ballConfiguration);
//            if (!ballConfigurations.ContainsKey(ballConfiguration))
//                return false;
//            if (ballConfigurations[ballConfiguration] > 1)
//                return true;
//            ballConfigurations.Remove(ballConfiguration);
//            return false;
        }

        private void GetNeighborsPositions(List<Vector2Int> positions, BallPrefabController ball)
        {
            positions.Add(new Vector2Int(ball.position.x, ball.position.y - 1));
            positions.Add(new Vector2Int(ball.position.x, ball.position.y + 1));
            positions.Add(new Vector2Int(ball.position.x + 1, ball.position.y));
            positions.Add(new Vector2Int(ball.position.x - 1, ball.position.y));

            if ((firstBallPaddingLeft && ball.position.x % 2 == 0) ||
                (!firstBallPaddingLeft && ball.position.x % 2 == 1)) //zmiana

            {
                positions.Add(new Vector2Int(ball.position.x + 1, ball.position.y - 1));
                positions.Add(new Vector2Int(ball.position.x - 1, ball.position.y - 1));
            }
            else
            {
                positions.Add(new Vector2Int(ball.position.x + 1, ball.position.y + 1));
                positions.Add(new Vector2Int(ball.position.x - 1, ball.position.y + 1));
            }
        }

        private List<BallPrefabController> GetExistingNeighbors(BallPrefabController ball)
        {
            List<Vector2Int> positions = new List<Vector2Int>(6);
            GetNeighborsPositions(positions, ball);
            positions = positions.FindAll(IsPositionExistAndFull);
            List<BallPrefabController> result = new List<BallPrefabController>(positions.Count);
            foreach (Vector2Int p in positions)
            {
                result.Add(balls[p.x, p.y]);
            }
            return result;
        }

        private bool IsPositionExistAndFull(Vector2Int position)
        {
            return (IsPositionExist(position) &&
                    balls[position.x, position.y] != null);
        }

        private bool IsPositionExist(Vector2Int position)
        {
            return (position.x >= 0 && position.x < ballsRows &&
                    position.y >= 0 && position.y < columns);
        }

        private bool IsPositionExistAndEmpty(Vector2Int position)
        {
            return (IsPositionExist(position) &&
                    balls[position.x, position.y] == null);
        }

        /// <summary>
        /// Wyszukuje wyspy kulek do skasowania
        /// </summary>
        /// <param name="islandBalls"></param>
        /// <param name="potentialIslands"></param>
        private void GetIslandBalls(List<BallPrefabController> potentialIslands, 
            List<Island> islands)
        {

            foreach (BallPrefabController ball in potentialIslands)
            {
                GetIslandBalls(islands, ball);
            }

            islands.RemoveAll(island => !island.aloneIsland);
        }

        private void GetIslandBalls(List<Island> islands, BallPrefabController ball)
        {
            Island island = islands.FirstOrDefault(i => i.balls.Contains(ball));

            if (island == null)
            {
                island = new Island();
                island.balls.Add(ball);
                islands.Add(island);

                if (ball.position.x == 0)
                    island.aloneIsland = false;
            }

            List<BallPrefabController> neighbors = GetExistingNeighbors(ball);
            foreach (BallPrefabController neighbor in neighbors)
            {
                if (island.balls.Contains(neighbor))
                    continue;
                island.balls.Add(neighbor);
                if (neighbor.position.x == 0)
                    island.aloneIsland = false;
                GetIslandBalls(islands, neighbor);
            }
        }

        private bool CheckLose()
        {
            for (int column = 0; column < columns; column++)
            {
                if (balls[ballsRows - 1, column] != null)
                    return true;
            }
            return false;
        }

        private class Island
        {
            public readonly List<BallPrefabController> balls = new List<BallPrefabController>();
            public bool aloneIsland = true;
        }

        private void LogBalls()
        {
            StringBuilder stringBuilder = new StringBuilder();;
            for (int row = 0; row <= ballsRows - 2; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    stringBuilder.Append(balls[row, column] == null ? "x" : "o");
                }
                stringBuilder.Append("\n");
            }
            Debug.Log(stringBuilder.ToString());
        }
    }
}