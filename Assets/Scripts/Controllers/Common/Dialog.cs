using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BubbleChip
{
    public static class Dialog
    {
        private const string DIALOG_PREFAB_PATH = "Prefabs\\DialogPrefab";
        private static DialogPrefabController dialogPrefab, lastDialog;

        public static DialogPrefabController Get(Transform parent = null, bool allowReUse = true)
        {
            if (parent == null)
                parent = Object.FindObjectOfType<Canvas>().transform;
            if (!allowReUse || lastDialog == null || !lastDialog.gameObject.scene.Equals(SceneManager.GetActiveScene()))
            {
                lastDialog = Object.Instantiate(GetPrefab());
            }
            if(!parent.Equals(lastDialog.transform.parent))
                lastDialog.transform.SetParent(parent, false);

            if(!lastDialog.gameObject.activeSelf)
                lastDialog.gameObject.SetActive(true);

            return lastDialog;
        }

        private static DialogPrefabController GetPrefab()
        {
            if (dialogPrefab == null)
                dialogPrefab = Resources.Load<DialogPrefabController>(DIALOG_PREFAB_PATH);
            if(dialogPrefab == null)
                throw new ArgumentException("Cannot load DialogPrefab from resources!");
            return dialogPrefab;
        }
    }
}
