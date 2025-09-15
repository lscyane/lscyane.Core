using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace lscyane.Core.Mvvm
{
    /// <summary>
    /// ダイアログのViewModel基底クラス
    /// </summary>
    public abstract class DialogViewModelBase : ObservableObject
    {
        /// <summary>
        /// ウインドウタイトル
        /// </summary>
        public string Title
        {
            get => this._Title;
            set => SetProperty(ref this._Title, value);
        }
        private string _Title = string.Empty;


        /// <summary>
        /// ダイアログ表示前に呼び出される
        /// </summary>
        public abstract void OnDialogPreviewOpen(DialogParameters? parameter);
    }
}