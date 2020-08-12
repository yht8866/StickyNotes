//********************************************************************************
//
//  SYSTEM          : DCP
//  UNIT            : ネイティブコードを呼び出す処理
//
//********************************************************************************
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : ネイティブコードを呼び出す処理
    /// CLASS ID        : NativeMethods
    ///
    /// FUNCTION        : 
    /// <summary>
    /// ネイティブコードの定義を宣言するクラスとする。
    /// </summary>
    /// 
    ///*******************************************************************************
    internal static class NativeMethods
    {
        #region <外部参照>

        /// <summary>
        /// 指定された .ini ファイル（初期化ファイル）の指定されたセクション内にある、指定されたキーに関連付けられている文字列を取得する。
        /// </summary>
        /// <param name="lpAppName">(in)セクション名</param>
        /// <param name="lpKeyName">(in)キー名</param>
        /// <param name="lpDefault">(in)既定の文字列</param>
        /// <param name="lpReturnedString">(in)情報が格納されるバッファ</param>
        /// <param name="nSize">(in)情報バッファのサイズ</param>
        /// <param name="lpFileName">(in).iniファイルの名前</param>
        /// <returns>
        /// 関数が成功すると、バッファに格納された文字数が返ります（終端の NULL 文字は含まない）。
        /// </returns>
        [DllImport("KERNEL32.DLL", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal extern static Int32 GetPrivateProfileString(
            [MarshalAs(UnmanagedType.LPStr)] String lpAppName, [MarshalAs(UnmanagedType.LPStr)] String lpKeyName,
            [MarshalAs(UnmanagedType.LPStr)] String lpDefault, StringBuilder lpReturnedString, UInt32 nSize, String lpFileName);

        /// <summary>
        /// 指定された .ini ファイル（初期化ファイル）の指定されたセクション内にある、指定されたキーに関連付けられている整数を取得する。
        /// </summary>
        /// <param name="lpAppName">(in)セクション名</param>
        /// <param name="lpKeyName">(in)キー名</param>
        /// <param name="nDefault">(in)キー名が見つからなかった場合に返すべき値</param>
        /// <param name="lpFileName">(in).ini ファイルの名前</param>
        /// <returns>
        /// 関数が成功すると、指定した .ini ファイルの指定したセクション内にある、指定したキーに関連付けられている文字列に相当する整数が返ります。
        /// 指定したキーが見つからない場合、nDefault パラメータで指定した既定の値が返ります。キーの値が負の場合、0 が返ります。
        /// </returns>
        [DllImport("KERNEL32.DLL", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal extern static Int32 GetPrivateProfileInt(
            [MarshalAs(UnmanagedType.LPStr)] String lpAppName, [MarshalAs(UnmanagedType.LPStr)] String lpKeyName,
            Int32 nDefault, [MarshalAs(UnmanagedType.LPStr)] String lpFileName);

        /// <summary>
        /// 指定された .ini ファイル（初期化ファイル）の、指定されたセクション内に、指定されたキー名とそれに関連付けられた文字列を格納します。
        /// </summary>
        /// <param name="lpAppName">(in)セクション名</param>
        /// <param name="lpKeyName">(in)キー名</param>
        /// <param name="lpString">(in)追加すべき文字列</param>
        /// <param name="lpFileName">(in).iniファイル</param>
        /// <returns>
        /// 関数が文字列を .ini ファイルに格納することに成功すると、0 以外の値が返ります。
        /// </returns>
        [DllImport("KERNEL32.DLL", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal extern static Int32 WritePrivateProfileString(
            [MarshalAs(UnmanagedType.LPStr)] String lpAppName, [MarshalAs(UnmanagedType.LPStr)] String lpKeyName,
            [MarshalAs(UnmanagedType.LPStr)] String lpString, [MarshalAs(UnmanagedType.LPStr)] String lpFileName);

        /// <summary>
        /// bQueNameにて指定されたQueBoxにメッセージをセットする。
        /// </summary>
        /// <param name="bQueName">(in)送信先QueBox名称｛文字列Max64文字｝</param>
        /// <param name="bSendData">(in)送信情報のアドレス</param>
        /// <param name="iSendSize">(in)送信情報サイズ｛Max131072｝</param>
        /// <param name="iTimeOut">(in)タイムアウト値</param>
        /// <returns>
        /// -64  ：Que名称の異常、Que名称文字列の最大数オーバー
        /// -65  ：Que名称の異常、Que名称文字列無し
        /// -67  ：共有メモリ排他制御用ミューテックスオープンエラー
        /// -2   ：共有メモリ排他制御用ミューテックス確保エラー
        /// -3   ：共有メモリの参照エラー
        /// -6   ：QueBoxに情報がPOSTできない、QueBoxBuffer Full
        /// -7   ：QueBoxのsizeよりメッセージが長い
        /// 1    ：正常
        /// </returns>
        [System.Runtime.InteropServices.DllImport("KPEFUNC.dll", CharSet = CharSet.Auto)]
        internal extern static Int32 kf_qsend(
            [MarshalAs(UnmanagedType.LPArray)] Byte[] bQueName, [MarshalAs(UnmanagedType.LPArray)] Byte[] bSendData, Int32 iSendSize, Int32 iTimeOut);

        /// <summary>
        /// bQueNameにて指定されたQueBoxにメッセージを得る。
        /// </summary>
        /// <param name="bQueName">(in)送信先QueBox名称｛文字列Max64文字｝</param>
        /// <param name="bRecvData">(in)取得データセット先アドレス</param>
        /// <param name="iRecvSize">(out)データセット先バッファサイズ｛Max131072｝</param>
        /// <param name="iTimeOut">(in)タイムアウト値</param>
        /// <returns>
        /// -64  ：Que名称の異常、Que名称文字列の最大数オーバー
        /// -65  ：Que名称の異常、Que名称文字列無し
        /// -67  ：共有メモリ排他制御用ミューテックスオープンエラー
        /// -2   ：共有メモリ排他制御用ミューテックス確保エラー
        /// -3   ：共有メモリの参照エラー
        /// -6   ：QueBoxに情報に情報が入っていない
        /// -7   ：QueBoxのsizeよりセット先バッファが小さい
        /// -8   ：QueBoxオープンエラー
        /// -100 ：タイムアウトイベント待ちエラー
        /// -255 ：タイムアウト
        /// 1    ：正常
        /// </returns>
        [System.Runtime.InteropServices.DllImport("KPEFUNC.dll", CharSet = CharSet.Auto)]
        internal extern static Int32 kf_qrecv(
            [MarshalAs(UnmanagedType.LPArray)] Byte[] bQueName, [MarshalAs(UnmanagedType.LPArray)] Byte[] bRecvData, ref Int32 iRecvSize, Int32 iTimeOut);

        /// <summary>
        /// 指定されたウィンドウを作成したスレッドに関連付けられているメッセージキューに、1 つのメッセージをポストします（書き込みます）。
        /// 対応するスレッドがメッセージを処理するのを待たずに制御を返します。
        /// </summary>
        /// <param name="hWnd">(in)ポスト先ウィンドウのハンドル</param>
        /// <param name="Msg">(in)メッセージ</param>
        /// <param name="wParam">(in)メッセージの最初のパラメータ</param>
        /// <param name="lParam">(in)メッセージの２番目のパラメータ</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、 関数を使います。
        /// </returns>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(
            IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// MCI（メディアコントロールインターフェイス）デバイスへ、コマンド文字列を送信する。
        /// </summary>
        /// <param name="lpszCommand">(in)コマンド文字列</param>
        /// <param name="lpszReturnString">(in)情報を受け取るバッファ</param>
        /// <param name="cchReturn">(in)バッファのサイズ</param>
        /// <param name="hwndCallback">(in)コールバックウィンドウのハンドル</param>
        /// <returns>
        /// 関数が成功すると、0 が返ります。関数が失敗すると、0 以外の値が返ります。
        /// </returns>
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        internal static extern Int32 mciSendString(String lpszCommand, StringBuilder lpszReturnString, Int32 cchReturn, IntPtr hwndCallback);

        /// <summary>
        /// 指定された MCI（メディアコントロールインターフェイス）エラーコードに対応する、エラーの内容を説明する文字列を取得します。
        /// </summary>
        /// <param name="fdwError">(in)エラーコード</param>
        /// <param name="lpszErrorText">(in)バッファのポインタ</param>
        /// <param name="cchErrorText">(in)バッファのサイズ</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値（TRUE）が返ります。指定したエラーコードが不明だった場合、0（FALSE）が返ります。
        /// </returns>
        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        internal static extern bool mciGetErrorString(Int32 fdwError, StringBuilder lpszErrorText, Int32 cchErrorText);

        /// <summary>
        /// マウスの移動やマウスボタンのクリックを合成します。
        /// http://msdn.microsoft.com/ja-jp/library/cc410921.aspx
        /// </summary>
        /// <param name="dwFlags">(in)マウスの移動とマウスボタンのクリックのさまざまな動作を指定します。次のフラグのうち、意味のある組み合わせを指定します。</param>
        /// <param name="dx">(in)dwFlags パラメータで MOUSEEVENTF_ABSOLUTE フラグを指定した場合、x 軸でのマウスの絶対座標を指定します。</param>
        /// <param name="dy">(in)dwFlags パラメータで MOUSEEVENTF_ABSOLUTE フラグを指定した場合、y 軸でのマウスの絶対座標を指定します。</param>
        /// <param name="cButtons">(in)dwFlags パラメータで MOUSEEVENTF_WHEEL フラグを指定した場合、ホイールの移動量を指定します。</param>
        /// <param name="dwExtraInfo">(in)マウスイベントに関連付けられた 32 ビットの追加情報を指定します。</param>
        /// <returns>
        /// 戻り値はありません。
        /// </returns>
        [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 cButtons, UIntPtr dwExtraInfo);

        public delegate IntPtr HOOKPROC(Int32 nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// アプリケーション定義のフックプロシージャをフックチェーン内にインストールします。フックプロシージャをインストールすると、特定のイベントタイプを監視できます。
        /// </summary>
        /// <param name="idHook">(in)インストール対象のフックタイプを指定します。</param>
        /// <param name="lpfn">(in)フックプロシージャへのポインタを指定します。</param>
        /// <param name="hInstance">(in)lpfn パラメータが指すフックプロシージャを保持している DLL のハンドルを指定します。</param>
        /// <param name="threadId">(in)フックプロシージャを関連付けるべきスレッドの識別子を指定します。</param>
        /// <returns>関数が成功すると、フックプロシージャのハンドルが返ります。関数が失敗すると、NULL が返ります。</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowsHookEx(Int32 idHook, HOOKPROC lpfn, IntPtr hInstance, Int32 threadId);

        /// <summary>
        /// SetWindowsHookEx 関数を使ってフックチェーン内にインストールされたフックプロシージャを削除します。
        /// </summary>
        /// <param name="hHook">(in)削除対象のフックプロシージャのハンドルを指定します。</param>
        /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。</returns>
        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// 現在のフックチェーン内の次のフックプロシージャに、フック情報を渡します。フックプロシージャは、フック情報を処理する前でも、フック情報を処理した後でも、この関数を呼び出せます。
        /// </summary>
        /// <param name="hHook">(in)現在のフックのハンドルを指定します。</param>
        /// <param name="nCode">(in)現在のフックプロシージャに渡されたフックコードを指定します。</param>
        /// <param name="wParam">(in)現在のフックプロシージャに渡された wParam 値を指定します。</param>
        /// <param name="lParam">(in)現在のフックプロシージャに渡された lParam 値を指定します。</param>
        /// <returns>フックチェーン内の次のフックプロシージャの戻り値が返ります。</returns>
        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hHook, Int32 nCode, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 呼び出し側スレッドのスレッド識別子を取得します。
        /// </summary>
        /// <returns>呼び出し側スレッドのスレッド識別子が返ります。</returns>
        [DllImport("kernel32.dll")]
        internal static extern Int32 GetCurrentThreadId();

        /// <summary>
        /// ダイアログボックス内のコントロールのタイトルまたはテキストを設定します。
        /// </summary>
        /// <param name="hWnd">(in) [入力］コントロールを保持するダイアログボックスのハンドルを指定します。</param>
        /// <param name="nIDDlgItem">(in) ［入力］タイトルまたはテキストを設定したいコントロールの識別子を指定します。</param>
        /// <param name="lpString">(in)［入力］コントロールへコピーしたいテキストを保持する、NULL で終わる文字列へのポインタを指定します。 </param>
        /// <returns>関数が成功すると、0 以外の値が返ります。関数が失敗すると、0 が返ります。</returns>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool SetDlgItemText(IntPtr hWnd, Int32 nIDDlgItem, String lpString);

        #endregion
    }
}
