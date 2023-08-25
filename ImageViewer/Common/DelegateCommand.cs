using System;
using System.Windows.Input;

namespace Common
{
    //! ICommandを実装したコマンドクラス。
    public class DelegateCommand : ICommand
    {
        #region //! @name コンストラクタと内部変数
        //-------------------------------------------------------------------------------
        //! コンストラクタ
        /*! @param[in]  exec    コマンド実行メソッド
            @param[in]  canExec コマンド実行か否かを返すメソッド
        */
        public DelegateCommand(Action exec) => ExecMethod = exec;

        //! コンストラクタに渡されたコマンド実行メソッドを保持する。
        private readonly Action ExecMethod;
        #endregion

        #region //! @name ICommandの実装
        //! ICommand.CanExecuteChangedの実装。
        public event EventHandler? CanExecuteChanged;

        //! ICommand.CanExecute()の実装。
        public bool CanExecute(object? parameter) => true;

        //! ICommand.Execute()の実装。
        public void Execute(object? parameter) => ExecMethod();
        #endregion
    }
}
