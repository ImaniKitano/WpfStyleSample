using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ImageViewer
{
    //! FileSelectorDialog.xaml の相互作用ロジック
    /*! TreeViewにドライブとフォルダの一覧を、GridViewにファイルの一覧を表示して、
        その中から一つを選ぶためのダイアログの動作を実現する。

    FileSelectorDialogは、Viewだけで構成されている。
    */
    public partial class FileSelectorDialog : Window, INotifyPropertyChanged
    {
        //! コンストラクタ
        public FileSelectorDialog()
        {
            // ファイルソート方法の選択肢を設定する。
            _sortKeyChoices = new List<SortKeyChoice> {
                    new SortKeyChoice("Sort by name",
                            () =>
                            {
                                _fileList.Sort((a, b) => String.Compare(a.Name, b.Name));
                                FileList = new ReadOnlyCollection<FileInfo>(_fileList);
                            }
                        ),
                    new SortKeyChoice("Sort by date",
                        () =>
                            {
                                _fileList.Sort((a, b) => DateTime.Compare(a.LastWriteTime, b.LastWriteTime));
                                FileList = new ReadOnlyCollection<FileInfo>(_fileList);
                            }
                           )
                };

            // ファイルソート方法の初期値を設定する。
            _selectedSortKey = SortKeyChoices[0];

            // OKボタンの動作を設定する。
            OkCommand = new DelegateCommand(
                () =>
                {
                    DialogResult = SelectedFile != null;
                    Close();
                }
                );

            // Cancelボタンの動作を設定する。
            CancelCommand = new DelegateCommand(
                () =>
                {
                    DialogResult = false;
                    Close();
                }
                );

            // ウィンドウを生成する。
            DataContext = this;
            InitializeComponent();

            // ドライブとフォルダのリストにロジカルドライブの一覧を設定する。
            foreach (var drive in Directory.GetLogicalDrives().ToList<string>())
            {
                FolderTreeView.Items.Add(new DriveItem(drive));
            }
        }

        #region //! @name Xamlイベントのハンドラ
        //! ドライブとフォルダのリストで選択されているドライブかフォルダが変わったときのハンドラ。
        private void SelectedFolderChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = (FolderItem)e.NewValue;  // リストのアイテムは、常にFolderItemクラスのオブジェクトである。

            // 選択されたフォルダの名前を表示用プロパティにコピーする。
            SelectedFolder = item.FullPath;

            // 選択されたフォルダにある画像ファイルの一覧を取得する。
            var picFiles = Directory.GetFiles(item.FullPath, "*.jpg").ToList<string>();
            picFiles.AddRange(Directory.GetFiles(item.FullPath, "*.png").ToList<string>());
            picFiles.AddRange(Directory.GetFiles(item.FullPath, "*.bmp").ToList<string>());

            // 見つかった画像ファイルの一覧を作る。。
            _fileList.Clear();
            foreach (var fn in picFiles)
            {
                _fileList.Add(new FileInfo(Path.Combine(item.FullPath, fn)));
            }

            // 指定された方法でソートする。表示用プロパティFileListは、この中で更新される。
            SelectedSortKey.Execute();
        }
        #endregion

        #region //! @name バインド用プロパティ
        //! 選択中のフォルダ名を保持するプロパティ。
        public string SelectedFolder { get => _selectedFolder; set => SetProperty(ref _selectedFolder, value); }
        private string _selectedFolder = string.Empty;  //!< SelectedFoldeプロパティrの実体。

        //! 選択されているファイルの情報を保持するプロパティ。ダイアログ呼び出し元が選択されたファイルを知るために使う。
        public FileInfo? SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    SelectedSortKey.Execute();
                }
            }
        }
        private FileInfo? _selectedFile;    //!< SelectedFileプロパティの実体。

        //! 選択中のフォルダに含まれる画像ファイルの一覧。
        private List<FileInfo> _fileList = new List<FileInfo>();

        //! 選択中のフォルダに含まれる画像ファイルの一覧。Xamからの参照用。
        public ReadOnlyCollection<FileInfo> FileList { get => __fileList; set => SetProperty(ref __fileList, value); }
        private ReadOnlyCollection<FileInfo> __fileList = new ReadOnlyCollection<FileInfo>(new List<FileInfo>());
                        //!< FileListの実体。

        #region //! @name ファイルリスト関連
        //! ファイルソート方法を保持するクラス
        public class SortKeyChoice
        {
            //! コンストラクタ
            /*! @param[in]  sortMethodName  ソート方法の名前
                @param[in]  execute         ソートを実行するためのActionデリゲート
            */
            public SortKeyChoice(string sortMethodName, Action execute)
            {
                SortMethodName = sortMethodName;
                Execute = execute;
            }

            //! コンボボックスの選択肢として表示する文字列を返す。
            public override string ToString() => SortMethodName;

            //! コンストラクタに渡されたソート方法の名前を保持する。
            public string SortMethodName { get; }

            //! コンストラクタに渡されたソート実行方法のActionデリゲートを保持する。
            public Action Execute { get; }
        }

        //! 画像ファイル一覧のソート方法のリスト。
        public List<SortKeyChoice> SortKeyChoices { get => _sortKeyChoices; set => SetProperty(ref _sortKeyChoices, value); }
        private List<SortKeyChoice> _sortKeyChoices;

        //! 選択されているソート方法
        public SortKeyChoice SelectedSortKey
        {
            get => _selectedSortKey;
            set
            {
                if (SetProperty(ref _selectedSortKey, value))
                {
                    value.Execute();
                }
            }
        }
        private SortKeyChoice _selectedSortKey; //!< SelectedSortKeyの実体。
        #endregion

        //! OKボタンのコマンド。
        public DelegateCommand OkCommand { get; init; }

        //! Cancelボタンのコマンド。
        public DelegateCommand CancelCommand { get; init; }
        #endregion

        #region //! @name INotifyPropertyChangedの実装と関連するメソッド
        //! INotifyPropertyChanged.PropertyChangedイベントの実装。
        public event PropertyChangedEventHandler? PropertyChanged;

        //! プロパティの値を実体に設定する。
        /*! @param[in]  value       プロパティに設定する値。
            @param[in]  storage     プロパティ値の実態を格納する変数。
            @param[in]  propName    この引数を設定しないこと。この関数を呼び出したプロパティの名前。
            @return true    プロパティ値が変更された。
            @return false   設定する値とプロパティ値が同じだったので、プロパティ値が変更されなかった。
        */
        public bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propName = null)
        {
            bool result;

            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                result = false;
            }
            else
            {
                storage = value;
                RaisePropertyChanged(propName);

                result = true;
            }
            return result;
        }

        //! PropertyChangedイベントを発行する。
        /*! @param[in]  propName    変更されたプロパティの名前。
        */
        protected void RaisePropertyChanged(string? propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion
    }

    //! TreeViewのアイテムとしてフォルダ情報を保持するクラス。
    public class FolderItem : TreeViewItem
    {
        //! コンストラクタ。
        /*! @param[in]  path    フォルダのフルパス
        */
        public FolderItem(string path)
        {
            // フォルダのフルパスを覚える。
            FullPath = path;

            // アイテムを展開させない。
            IsExpanded = false;

            // TreeViewItemのヘッダは最下位のフォルダ名のみにする。
            Header = Path.GetFileName(FullPath);

            // サブフォルダがあれば、サブフォルダのリストを得るためにかかる時間を節約するために、
            // 展開用のボタンを表示させるための適当な子アイテムを付けておく。
            if (Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly).Length > 0)
            {
                Items.Add("*");
            }
        }

        //! 子アイテムが展開されるときに呼ばれる関数。
        /*! @param[in]  イベント引数。
        */
        protected override void OnExpanded(RoutedEventArgs e)
        {
            // 展開ボタン表示用の子アイテムを削除する。
            Items.Clear();

            // サブフォルダを子アイテムとして追加する。
            foreach (var subfolder in Directory.GetDirectories(FullPath, "*.*", SearchOption.TopDirectoryOnly))
            {
                var info = new FileInfo(subfolder);
                if ((info.Attributes & System.IO.FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    Items.Add(new FolderItem(subfolder) { Style = this.Style });
                }
            }
        }

        //! 対応するフォルダのフルパス。
        public string FullPath { get; init; }
    }

    //! TreeViewのアイテムとしてドライブ情報を保持するクラス。
    public class DriveItem : FolderItem
    {
        //! コンストラクタ。
        /*! @param[in]  drive   ドライブ名

        引数driveがドライブ名だけの場合、FolderItemクラスのロジックではドライブ名が
        Headerに入らないので、このコンストラクタでHeaderにドライブ名を設定する。
        */
        public DriveItem(string drive) : base(drive)
        {
            Header = drive;
        }
    }
}
