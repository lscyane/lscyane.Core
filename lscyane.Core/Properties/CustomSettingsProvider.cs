using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace lscyane.Core.Properties 
{
    /// <summary>
    /// クロスプラットフォーム対応のカスタム設定プロバイダー <br/>
    /// - Desktop (Windows/Linux/macOS) 共通で利用可能 <br/>
    /// - 保存形式は JSON <br/>
    /// - バージョン番号でフォルダ分けされない（アプリが制御）<br/>
    /// </summary>
    public class CustomSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        // 設定値の保存用キャッシュ
        private Dictionary<string, object>? _settings;


        /// <summary>
        /// 保存ファイル名（拡張子なし）
        /// </summary>
        protected virtual string FileName => "custom";


        /// <summary>
        /// 設定ファイルの場所<para/>
        /// - Windows: %LOCALAPPDATA%\Company\Product\custom.json <br/>
        /// - Linux:   ~/.local/share/Company/Product/custom.json <br/>
        /// - macOS:   ~/Library/Application Support/Company/Product/custom.json <br/>
        /// </summary>
        private string UserConfigPath
        {
            get
            {
                // 実行中のプロセスのメインモジュールからアセンブリを取得
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null)
                    throw new InvalidOperationException("EntryAssembly を取得できません。");

                // OSごとのユーザーデータフォルダを取得
                string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // Assembly属性から会社名と製品名を取得（無ければデフォルト文字列）
                string company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company ?? "DefaultCompany";
                string product = assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? assembly.GetName().Name ?? "App";

                // 保存先ディレクトリを組み立てて作成
                string dir = Path.Combine(root, company, product);
                Directory.CreateDirectory(dir);

                // JSONファイル名を返す
                return Path.Combine(dir, FileName + ".json");
            }
        }


        /// <summary> アプリケーション名（内部的に使われるだけ） </summary>
        public override string ApplicationName
        {
            get => Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
            set { /* 何もしない */ }
        }


        /// <inheritdoc/>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(ApplicationName, config);
        }


        /// <summary>
        ///  設定値の読み取り
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            this.EnsureLoaded();

            var values = new SettingsPropertyValueCollection();

            foreach (SettingsProperty prop in properties)
            {
                object? val = null;

                // JSONに保存されていればそれを使う
                if (this._settings != null && this._settings.TryGetValue(prop.Name, out var stored))
                {
                    val = stored;
                }
                else if (prop.DefaultValue != null)
                {
                    val = prop.DefaultValue;    // 保存されていなければ既定値を使用
                }

                var spv = new SettingsPropertyValue(prop)
                {
                    IsDirty = false,
                    SerializedValue = val
                };
                values.Add(spv);
            }

            return values;
        }


        /// <summary>
        /// 設定値の書き込み
        /// </summary>
        /// <param name="context"></param>
        /// <param name="values"></param>
        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            this.EnsureLoaded();

            foreach (SettingsPropertyValue value in values)
            {
                if (value.Property != null)
                {
                    this._settings![value.Name] = value.SerializedValue!;
                }
            }

            this.Save();
        }


        /// <summary>
        /// 設定のアップグレード（旧バージョンからの移行時に使う）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            // 旧設定からの移行処理を実装する場合はここに書く
        }


        /// <summary>
        /// 設定のリセット（全消去）
        /// </summary>
        /// <param name="context"></param>
        public void Reset(SettingsContext context)
        {
            this._settings?.Clear();
            this.Save();
        }


        /// <summary>
        /// 設定適用（保存処理にマップ）
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        public void ApplySettings(SettingsContext context, SettingsPropertyCollection properties)
        {
            this.Save();
        }


        /// <inheritdoc/>
        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            throw new NotImplementedException();
        }


        /// <summary> 設定ファイルの読み込み（初回アクセス時のみ） </summary>
        private void EnsureLoaded()
        {
            if (this._settings != null) return;

            this._settings = new Dictionary<string, object>();
            if (File.Exists(UserConfigPath))
            {
                foreach (var line in File.ReadAllLines(this.UserConfigPath))
                {
                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        this._settings[parts[0]] = parts[1];
                    }
                }
            }
        }


        /// <summary> 設定ファイルの保存（JSON書き込み） </summary>
        private void Save()
        {
            if (this._settings == null) return;

            var lines = new List<string>();
            foreach (var kv in this._settings!)
            {
                lines.Add($"{kv.Key}={kv.Value}");
            }
            File.WriteAllLines(this.UserConfigPath, lines);
        }
    }
}