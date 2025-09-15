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


        /// <summary>
        /// Close要求イベント
        /// </summary>
        /// <remarks> <see cref="lscyane.Wpf.Services.DialogService.GetDialogViewWithPreProcess"/> で設定 </remarks>
        public event System.Action? RequestClose;


        /// <summary>
        /// Close要求
        /// </summary>
        protected virtual void OnRequestClose()
        {
            this.RequestClose?.Invoke();
        }
    }
}