using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;


namespace BubbleChip
{
    public static class SceneUtils
    {
        public const int MENU_SCENE = 0;
        public const int GAME_SCENE = 1;

        public static void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
