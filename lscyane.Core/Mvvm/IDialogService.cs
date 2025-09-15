using System;

namespace lscyane.Core.Mvvm
{
    /// <summary>
    /// ダイアログサービスのインターフェース
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// ダイアログをモードレスで表示します
        /// </summary>
        /// <param name="vm_type"></param>
        /// <param name="parameter"></param>
        /// <param name="result_action"></param>
        public void Show(Type vm_type, DialogParameters? parameter = null, Action<object>? result_action = null);


        /// <summary>
        /// ダイアログをモーダルで表示します
        /// </summary>
        /// <param name="vm_type"></param>
        /// <param name="parameter"></param>
        /// <param name="result_action"></param>
        public void ShowDialog(Type vm_type, DialogParameters? parameter = null, Action<object>? result_action = null);
        
    }
}