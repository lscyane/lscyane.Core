using System;

namespace lscyane.Core.Mvvm
{
    /// <summary>
    /// ダイアログサービスのインターフェース
    /// </summary>
    public interface IDialogService
    {
        public void Show(Type vm_type, DialogParameters? parameter = null, Action<object>? result_action = null);
    }
}