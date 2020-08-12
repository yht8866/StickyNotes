//********************************************************************************
//
//  SYSTEM          : なし(共用ライブラリ)
//  UNIT            : 付箋紙機能
//
//********************************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : 付箋紙フォームクラス
    /// CLASS ID        : FormStickyNote
    ///
    /// FUNCTION        : 
    /// <summary>
    /// 付箋紙のクラス定義
    /// </summary>
    /// 
    ///*******************************************************************************
    public partial class FormStickyNote : Form
    {
        // ===========================================
        // 変数
        // ===========================================
        #region <変数>
        // フォーム作成時のコントロール
        public TextBox tb = null;
        private bool moving = false;
        private bool resizing = false;
        private static FontDialog fontDlg = new FontDialog();
        private static ColorDialog colorDlg = new ColorDialog();
        // 付箋紙カウンタ
        public static Int32 noteCnt = 0;
        // 各インスタンスのプロパティを格納する
        public CNoteProperties ThisNoteProperties = new CNoteProperties();
        // フォーム作成時のプロパティを格納するオブジェクト
        public static CNoteProperties SetNewProperties = new CNoteProperties();

        #endregion

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : StickyNote
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ///
        ///*******************************************************************************
        /// デフォルトコンストラクタ
        public FormStickyNote() { }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : StickyNote
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// コンストラクタ
        /// </summary>
        ///
        ///*******************************************************************************
        public FormStickyNote(Int16 actionFlg)
        {
            InitializeComponent();

            //this.SetStyle(ControlStyles.ResizeRedraw, true);

            // 変数を初期化
            InitializeVariables();

            this.SuspendLayout();

            if (0 != StickyNote.Current_NoteProperties.NoteID)
            {
                // クリックした付箋紙のプロパティをコピー
                SetNewProperties.BackColor = StickyNote.Current_NoteProperties.BackColor;
                SetNewProperties.ForeColor = StickyNote.Current_NoteProperties.ForeColor;
                SetNewProperties.FontFont = StickyNote.Current_NoteProperties.FontFont;
                SetNewProperties.Text = StickyNote.Current_NoteProperties.Text;
                SetNewProperties.Size = StickyNote.Current_NoteProperties.Size;
                SetNewProperties.Visible = StickyNote.Current_NoteProperties.Visible;
                SetNewProperties.Opacity = StickyNote.Current_NoteProperties.Opacity;
            }
            else
            {
                // 処理無し
            }

            // すべてのケースで増加する
            noteCnt++;

            // 座標調整
            Int32 posX = StickyNote.m_GripPoint.X + StickyNote.Current_NoteProperties.Location.X;
            Int32 posY = StickyNote.m_GripPoint.Y + StickyNote.Current_NoteProperties.Location.Y;

            // 新規作成
            if ( 0 == actionFlg )
            {
                // 設定用のオブジェクトをクリア
                SetNewProperties.Clear();
                SetNewProperties.NoteID = noteCnt;
                SetNewProperties.Location = new Point(posX, posY);
                SetNewProperties.Size = new Size(150, 90);
            }
            // コピー
            else if ( 1 == actionFlg )
            {
                SetNewProperties.NoteID = noteCnt;
                SetNewProperties.Location = new Point(posX, posY);
            }
            // ロード
            else if (2 == actionFlg)
            {
                SetNewProperties.NoteID = noteCnt;
                // 読み込んだ座標を設定する
                SetNewProperties.Location = StickyNote.Current_NoteProperties.Location;
            }

            // Textboxコントロールを作成
            tb = new TextBox()
            {
                Name = "Textbox" + SetNewProperties.NoteID.ToString(),
                Size = SetNewProperties.Size,
                Font = SetNewProperties.FontFont,
                Text = SetNewProperties.Text,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(Convert.ToInt32(SetNewProperties.BackColor)),
                ForeColor = Color.FromArgb(Convert.ToInt32(SetNewProperties.ForeColor)),
                Visible = SetNewProperties.Visible,
                Parent = this,      // textboxの親にフォームコンテーナを設定
                Multiline = true,
                Dock = DockStyle.Fill,
                ContextMenu = new ContextMenu(),
                Enabled = true,
            };

            tb.MouseDown += tb_MouseDown;
            tb.MouseMove += tb_MouseMove;
            tb.MouseUp += tb_MouseUp;

            // 変化時のイベントハンドラ
            tb.TextChanged += tb_TextChanged;
            tb.FontChanged += tb_FontChanged;
            tb.BackColorChanged += tb_BackColorChanged;
            tb.ForeColorChanged += tb_ForeColorChanged;

            // ========================================
            // フォーム自体のプロパティ
            // ========================================
            // テキストボックスをフォームに追加
            this.Controls.Add(tb);

            //// ========================================
            //// フォーム自体のプロパティ
            //// ========================================
            // 呼び出し元のフォームをownerに設定
            this.Owner = StickyNote.m_OwnerForm;
            this.Size = SetNewProperties.Size;
            this.Name = "StickyNote" + SetNewProperties.NoteID.ToString();
            this.Opacity = SetNewProperties.Opacity;

            // フォームを閉じたときのイベントハンドラー
            this.FormClosed += FormStickyNote_FormClosed;
            this.SizeChanged += FormStickyNote_SizeChanged;
            this.LocationChanged += FormStickyNote_LocationChanged;
            this.VisibleChanged += FormStickyNote_VisibleChanged;

            this.ResumeLayout(false);

            if (true == SetNewProperties.Visible)
            {
                Show(StickyNote.m_OwnerForm);
            }

            OnLoad();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 変数初期化
        /// MODULE ID           : InitializeVariables
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 変数を初期化する
        /// </summary>
        ///
        ///*******************************************************************************
        private void InitializeVariables()
        {
            // フォーム作成時のコントロール
            tb = null;
            moving = false;
            resizing = false;
            fontDlg = new FontDialog();
            colorDlg = new ColorDialog();
            // 各インスタンスのプロパティを格納する
            ThisNoteProperties = new CNoteProperties();
            // フォーム作成時のプロパティを格納するオブジェクト
            SetNewProperties = new CNoteProperties();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : マウスダウンイベント
        /// MODULE ID           : tb_MouseDown
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// テキストボックスをクリックしたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_MouseDown(Object sender, MouseEventArgs e)
        {
            var textbox = (TextBox)sender;
            var noteform = (FormStickyNote)textbox.Parent;
            var dcpform = (Form)noteform.Owner;
            
            // クリックした座標を変数に格納
            StickyNote.m_GripPoint = e.Location;
            // クリックした付箋紙を親フォームに格納
            StickyNote.m_ParentForm = noteform;

            // ========================================================
            // 開いているダイアログをあらかじめ閉じる
            // ========================================================
            // 付箋紙に左クリックしたとき、メニューが開いていたら閉じる
            if (StickyNote.StickyNoteMenu != null)
            {
                StickyNote.StickyNoteMenu.Dispose();
                StickyNote.StickyNoteMenu = null;
            }

            // 透過性調整ダイアログを閉じる
            if (StickyNote.m_OpacityForm != null)
            {
                StickyNote.m_OpacityForm.Dispose();
                StickyNote.m_OpacityForm = null;
            }
            // ========================================================

            // クリックした付箋紙のプロパティを退避しておく
            SetCurrentFormProperties(textbox);
            
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // マウスクリックの座標を調整
                Int32 posX = noteform.Left + e.Location.X;
                Int32 posY = noteform.Top + e.Location.Y;

                // 付箋紙にクリックしたときにメインフォームにメッセージを返す
                StickyNote.StickyNoteMenu = new FormStickyNoteMenu((FormStickyNote)StickyNote.m_CurrTextbox.Parent, posX, posY);
            }
            // 左クリック
            else
            {
                moving = true;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : マウスムーブイベント
        /// MODULE ID           : tb_MouseMove
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// テキストボックス上にマウスをあてたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_MouseMove(Object sender, EventArgs e)
        {
            var mea = (MouseEventArgs)e;
            var clicktarget = (TextBox)sender;
            var target = (Form)clicktarget.Parent;

            if (moving)
            {
                var x = target.Location.X + mea.X - StickyNote.m_GripPoint.X;
                var y = target.Location.Y + mea.Y - StickyNote.m_GripPoint.Y;
                ((Form)target).Location = new Point(x, y);
                clicktarget.Cursor = Cursors.Arrow;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : マウスアップイベント
        /// MODULE ID           : tb_MouseUp
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// テキストボックスにてマウスアップしたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_MouseUp(Object sender, MouseEventArgs e)
        {
            var target = (TextBox)sender;
            moving = false;
            resizing = false;

            if (resizing == false)
            {
                target.Cursor = Cursors.IBeam;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : テキスト内容変化イベント
        /// MODULE ID           : tb_TextChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// テキストボックスの内容を編集したときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_TextChanged(Object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;

            SetCurrentFormProperties(textBox);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォント変更イベント
        /// MODULE ID           : tb_FontChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォントが変更されたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_FontChanged(Object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;

            SetCurrentFormProperties(textBox);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 背景色変化イベント
        /// MODULE ID           : tb_BackColorChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 背景色を変更したときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_BackColorChanged(Object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;

            SetCurrentFormProperties(textBox);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 表示色変化イベント
        /// MODULE ID           : tb_ForeColorChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 文字の表示色を変更したときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void tb_ForeColorChanged(Object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;

            SetCurrentFormProperties(textBox);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームロードイベント
        /// MODULE ID           : FormStickyNote_Load
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームロードイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormStickyNote_Load(Object sender, EventArgs e)
        {
            try
            {
                Int32 dispWidth = SetNewProperties.Location.X + this.Size.Width;
                Int32 dispHeight = SetNewProperties.Location.Y + this.Size.Height;

                // X座標を調整
                if (dispWidth >= Screen.PrimaryScreen.WorkingArea.Width)
                {
                    // 画面領域を超えないように調整する
                    dispWidth = Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width;
                }
                else
                {
                    dispWidth = SetNewProperties.Location.X;
                }

                // Y座標を調整
                if (dispHeight >= Screen.PrimaryScreen.WorkingArea.Height)
                {
                    // 画面領域を超えないように調整する
                    dispHeight = Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height;
                }
                else
                {
                    dispHeight = SetNewProperties.Location.Y;
                }

                this.DesktopLocation = new Point(dispWidth, dispHeight);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームオンロード
        /// MODULE ID           : OnLoad
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームロード後の処理
        /// </summary>
        ///
        ///*******************************************************************************
        private void OnLoad()
        {
            // クラス変数にプロパティをセット
            SetThisFormProperties();

            SetCurrentFormProperties(tb);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームサイズ変化イベント
        /// MODULE ID           : FormStickyNote_SizeChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームのサイズを変更したときにグローバル変数に格納する
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormStickyNote_SizeChanged(Object sender, EventArgs e)
        {
            var form = (FormStickyNote)sender;
            
            StickyNote.Current_NoteProperties.Size = form.Size;
            SetCurrentFormProperties(form.tb);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォーム移動イベント
        /// MODULE ID           : FormStickyNote_LocationChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームを移動したときにグローバル変数に格納する。
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormStickyNote_LocationChanged(Object sender, EventArgs e)
        {
            var form = (FormStickyNote)sender;

            if (0 != StickyNote.Current_NoteProperties.NoteID)
            {
                StickyNote.Current_NoteProperties.Location = form.DesktopLocation; 
            }
            else
            {
                StickyNote.Current_NoteProperties.Location = new Point(0, 0);
            }

            SetCurrentFormProperties(form.tb);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォーム表示変化イベント
        /// MODULE ID           : FormStickyNote_VisibleChanged
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームのVisibleプロパティが変化したときにグローバル変数に格納する
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormStickyNote_VisibleChanged(Object sender, EventArgs e)
        {
            var form = (FormStickyNote)sender;

            StickyNote.Current_NoteProperties.Visible = form.Visible;

            SetCurrentFormProperties(form.tb);

            StickyNote.UpdateStickyNoteList();
            StickyNote.SaveStickyNotes();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームクローズイベント
        /// MODULE ID           : FormStickyNote_FormClosed
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)イベントパラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// フォームを閉じたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormStickyNote_FormClosed(Object sender, EventArgs e)
        {
            var formToDispose = (FormStickyNote)sender;

            formToDispose.Dispose();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームクローズイベント
        /// MODULE ID           : SetThisFormProperties
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// 
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 自フォームのプロパティクラスオブジェクトに格納する
        /// </summary>
        ///
        ///*******************************************************************************
        private void SetThisFormProperties()
        {
            ThisNoteProperties.NoteID = SetNewProperties.NoteID;
            ThisNoteProperties.Text = this.tb.Text;
            ThisNoteProperties.FontFont = this.tb.Font;
            ThisNoteProperties.ForeColor = (this.tb.ForeColor).ToArgb().ToString();
            ThisNoteProperties.BackColor = (this.tb.BackColor).ToArgb().ToString();
            ThisNoteProperties.Size = this.Size;
            ThisNoteProperties.Location = this.DesktopLocation;
            ThisNoteProperties.Visible = this.Visible;
            ThisNoteProperties.Opacity = this.Opacity;
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 現在フォームプロパティセット
        /// MODULE ID           : SetCurrentFormProperties
        ///
        /// PARAMETER IN        : 
        /// <param name="copyTarget">(in)テキストボックスコントロール</param>
        /// 
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// クリックしたフォームをグローバル変数に格納する
        /// </summary>
        ///
        ///*******************************************************************************
        private void SetCurrentFormProperties(TextBox copyTarget)
        {
            // set current note to the newly-created note
            StickyNote.Current_NoteProperties = null;
            StickyNote.Current_NoteProperties = new CNoteProperties();

            StickyNote.m_CurrTextbox = null;
            StickyNote.m_CurrTextbox = copyTarget;
            StickyNote.m_ParentForm = (FormStickyNote)copyTarget.Parent;
            StickyNote.Current_NoteProperties.NoteID = Int32.Parse(StickyNote.m_ParentForm.Name.Substring(10, StickyNote.m_ParentForm.Name.Length - 10));
            StickyNote.Current_NoteProperties.Location = StickyNote.m_ParentForm.DesktopLocation;
            StickyNote.Current_NoteProperties.BackColor = (copyTarget.BackColor).ToArgb().ToString();
            StickyNote.Current_NoteProperties.ForeColor = (copyTarget.ForeColor).ToArgb().ToString();
            StickyNote.Current_NoteProperties.Size = StickyNote.m_ParentForm.Size;
            StickyNote.Current_NoteProperties.Text = copyTarget.Text;
            StickyNote.Current_NoteProperties.FontFont = copyTarget.Font;
            StickyNote.Current_NoteProperties.Opacity = StickyNote.m_ParentForm.Opacity;
        }
    }
}
