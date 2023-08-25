using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Common
{
    //! INotifyPropertyChangedを実装した基本クラス。
    public abstract class BindableBase : INotifyPropertyChanged
    {
        #region //! @name プロパティ値の変更を通知するためのAPI
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
                // 設定される値が元の値と同じなら、falseを返す。
                result = false;
            }
            else
            {
                // 設定される値が元の値と異なるなら、プロパティ値変更を報告して、trueを返す。
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

        #region //! @name INotifyPropertyChangedの実装
        //! INotifyPropertyChangedが定義しているプロパティ値変更通知イベント。
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion
    }
}
