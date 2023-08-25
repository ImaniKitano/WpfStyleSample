using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageViewer
{
    //! 画像表示ウィンドウクラス
    internal class MainWindowViewModel : BindableBase
    {
        //! コンストラクタ
        public MainWindowViewModel()
        {
            //ShowingImageFile = new FileInfo("G:\\マイドライブ\\PixelPictures\\PXL_20230326_042102206.jpg");

            // ファイル選択ボタンのコマンドを設定する。
            SelectFileCommand = new DelegateCommand(
                () =>
                {
                    // ファイル選択ダイアログを開く。
                    var dlg = new FileSelectorDialog();
                    dlg.ShowDialog();

                    // ファイルが選択されたら、そのファイルを表示する。
                    if (dlg.DialogResult == true)
                    {
                        ShowingImageFile = dlg.SelectedFile;
                    }
                }
                );
        }

        //! ファイル選択コマンド。
        public DelegateCommand SelectFileCommand { get; init; }

        //! 表示中のファイル名を保持するプロパティ。
        public FileInfo? ShowingImageFile
        {
            get => _showingImageFile;
            set
            {
                // 表示するファイルが変わったら、画像のイメージと情報を設定する。
                if (SetProperty(ref _showingImageFile, value) && (value != null))
                {
                    PicImage = new BitmapImage(new Uri(value.FullName));

                    var infoList = new List<InfoPair>();
                    infoList.Add(new InfoPair("Name", value.Name));
                    infoList.Add(new InfoPair("Width", PicImage.PixelWidth));
                    infoList.Add(new InfoPair("Height", PicImage.PixelHeight));
                    ImageInfoList = infoList;
                }
            }
        }
        public FileInfo? _showingImageFile; //!< ShowingImageFileの実体。

        //! 画像イメージ。
        public BitmapImage? PicImage { get => _picImage; set => SetProperty(ref _picImage, value); }
        public BitmapImage? _picImage;  //!< PicImageの実体。

        //! 画像情報のリスト。
        public List<InfoPair>? ImageInfoList { get => _imageInfoList; set => SetProperty(ref _imageInfoList, value); }
        private List<InfoPair>? _imageInfoList; //!< ImageInfoListの実体。

        //! 画像表示方法。trueなら実際のサイズ、falseならウィンドウサイズに合わせて表示する。
        public bool ShowInFullSize { get => _showInFullSize; set => SetProperty(ref _showInFullSize, value); }
        private bool _showInFullSize = false;
    }

    //! 画像情報を保持するクラス。
    public class InfoPair
    {
        //! コンストラクタ。
        /*! @param[in]  情報の名前
            @param[in]  情報の値
        */
        public InfoPair(string name, object? value)
        {
            Name = name;
            Value = value?.ToString();
        }

        //! 情報の名前。
        public string Name { get; init; }

        //! 情報の値。
        public string? Value { get; init; }
    }
}
