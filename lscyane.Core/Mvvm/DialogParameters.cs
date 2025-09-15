using System;
using System.Collections.Generic;
using System.Linq;

namespace lscyane.Core.Mvvm
{
    /// <summary>
    /// ダイアログ呼び出し元とダイアログのViewModel間でパラメータを受け渡すためのクラス
    /// </summary>
    public class DialogParameters
    {
        /// <summary>
        /// パラメータのエントリ一覧
        /// </summary>
        private readonly List<KeyValuePair<string, object?>> _entries = new List<KeyValuePair<string, object?>>();


        /// <summary>
        /// エントリにパラメータを追加する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, object? value)
        {
            _entries.Add(new KeyValuePair<string, object?>(key, value));
        }


        /// <summary>
        /// 指定したキーのパラメータを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public T GetValue<T>(string key)
        {
            var entry = _entries.FirstOrDefault(e => e.Key == key);
            if (entry.Equals(default(KeyValuePair<string, object>)))
            {
                throw new KeyNotFoundException($"Key '{key}' not found in dialog parameters.");
            }
            if (entry.Value is T value)
            {
                return value;
            }
            else
            {
                throw new InvalidCastException($"Value for key '{key}' cannot be cast to type '{typeof(T)}'.");
            }
        }
    }
}