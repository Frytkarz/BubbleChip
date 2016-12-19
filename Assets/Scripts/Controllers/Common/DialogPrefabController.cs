using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BubbleChip
{
    public class DialogPrefabController : MonoBehaviour
    {
        private const string BTN_OK_CONTENT = "OK";
        private const string BTN_CANCEL_CONTENT = "Cancel";

        public Text txtContent, txtBtnCancel, txtBtnOk;
        public Button btnCancel, btnOk;

        /// <summary>
        /// Ustawia dialog z jednym przycickiem
        /// </summary>
        /// <param name="content">Treść</param>
        /// <param name="onOkClick">Ewentualny onClick</param>
        /// <param name="btnOkContent">Ewentualna nazwa przycisku</param>
        public void Set(string content, UnityAction onOkClick = null, string btnOkContent = BTN_OK_CONTENT)
        {
            btnCancel.gameObject.SetActive(false);
            if (!btnOkContent.Equals(txtBtnOk.text))
                txtBtnOk.text = btnOkContent;
            btnOk.onClick.RemoveAllListeners();
            btnOk.onClick.AddListener(Hide);
            if(onOkClick != null)
                btnOk.onClick.AddListener(onOkClick);
            txtContent.text = content;
        }

        /// <summary>
        /// Ustawia dialog z 2 przyciskami
        /// </summary>
        /// <param name="content">Treść</param>
        /// <param name="onOkClick">Onclick przycisku ok</param>
        /// <param name="onCancelClick">Ewentualny onClick przycisku cancel</param>
        /// <param name="btnOkContent">Ewentualna treśc przycisku ok</param>
        /// <param name="btnCancelContent">Ewentualna treść przycisku cancel</param>
        public void Set(string content, UnityAction onOkClick, UnityAction onCancelClick = null,
            string btnOkContent = BTN_OK_CONTENT,string btnCancelContent = BTN_CANCEL_CONTENT)
        {
            if(!btnCancel.gameObject.activeSelf)
                btnCancel.gameObject.SetActive(true);

            if (!btnOkContent.Equals(txtBtnOk.text))
                txtBtnOk.text = btnOkContent;
            if (!btnCancelContent.Equals(txtBtnCancel.text))
                txtBtnCancel.text = btnCancelContent;

            btnOk.onClick.RemoveAllListeners();
            btnOk.onClick.AddListener(Hide);
            btnOk.onClick.AddListener(onOkClick);

            btnCancel.onClick.RemoveAllListeners();
            btnCancel.onClick.AddListener(Hide);
            if (onCancelClick != null)
                btnCancel.onClick.AddListener(onCancelClick);

            txtContent.text = content;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}