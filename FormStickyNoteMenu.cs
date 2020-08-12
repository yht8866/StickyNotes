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
using StickyNotes;
using System.Diagnostics;
using System.Reflection;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : 付箋紙メニュークラス
    /// CLASS ID        : FormStickyNoteMenu
    ///
    /// FUNCTION        : 
    /// <summary>
    /// 付箋紙メニューのクラス
    /// </summary>
    /// 
    ///*******************************************************************************
    public partial class FormStickyNoteMenu : Form
    {
        #region <変数>

        // メニューに表示する項目
        public static String[] menu_Items;
        // 付箋紙右クリック時のメニュー
        public static bool[] menu_ItemsHidden_RClick;
        // メニューボタンクリック時のメニュー
        public static bool[] menu_ItemsHidden_Button;
        // メニュー項目の表示／非表示設定(true=非表示、false=表示)
        public static bool[] menuItemsHidden;
        // 付箋紙オブジェクト
        private static StickyNote stickyNoteObj = new StickyNote();

        // メニューＩＤ
        private enum MENUID : ushort
        {
            CREATENEW = 0,      // 新規作成
            COPY,               // コピー
            EDITFONT,           // フォント編集
            BCOLOR,             // 背景色指定
            OPACITY,            // 透過性
            SEPARATOR1,         // SEPARATOR
            DELETE,             // 削除
            SEPARATOR2,         // SEPARATOR
            HIDE,               // 非表示
            UNHIDE_ALL,         // すべて表示
            HIDE_ALL,           // すべて非表示
            TOTAL_ITEMS,        // メニュー総数
        };

        private Label menuLabel;

        private Color m_LabelBackColor;
        private Color m_LabelForeColor;

        private Int32 dispPosX = 0;
        private Int32 dispPosY = 0;

        #endregion

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : FormStickyNoteMenu
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
        /// デフォルトコンストラクタ
        /// </summary>
        ///
        ///*******************************************************************************
        public FormStickyNoteMenu() { }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : FormStickyNoteMenu
        ///
        /// PARAMETER IN        : 
        /// <param name="posX">(in)X座標</param>
        /// <param name="posY">(in)Y座標</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 呼び出し元の画面の「付箋紙メニュー」ボタンをクリックしたときに使う。
        /// クリックした位置のXとY座標を元にメニューの表示位置を調整する
        /// </summary>
        ///
        ///*******************************************************************************
        public FormStickyNoteMenu(Int32 posX, Int32 posY)
        {
            InitializeComponent();

            // クリアする
            StickyNote.Current_NoteProperties.Clear();

            // メニューの表示位置
            dispPosX = posX;
            dispPosY = posY;

            this.SuspendLayout();

            // オーナーフォーム
            this.Owner = StickyNote.m_OwnerForm;
            
            // 隠す項目を指定
            for (int i = 0; i < menu_Items.Count(); i++)
            {
                menuItemsHidden[i] = menu_ItemsHidden_Button[i];
            }

            // メニュー項目を生成
            MakeMenu();

            this.ResumeLayout(false);
            this.Refresh();

            this.Disposed += MenuForm_Disposed;

            this.Show(StickyNote.m_OwnerForm);
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : FormStickyNoteMenu
        ///
        /// PARAMETER IN        : 
        /// <param name="posX">(in)X座標</param>
        /// <param name="posY">(in)Y座標</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 付箋紙上に右クリックしたときに使う。
        /// クリックした位置のXとY座標を元にメニューの表示位置を調整する
        /// </summary>
        ///
        ///*******************************************************************************
        public FormStickyNoteMenu(FormStickyNote parentForm, Int32 posX, Int32 posY)
        {
            InitializeComponent();

            // メニューの表示位置
            dispPosX = posX;
            dispPosY = posY;

            this.SuspendLayout();

            // オーナーフォーム
            this.Owner = StickyNote.m_OwnerForm;

            // 隠す項目を指定(全表示)
            for (int i = 0; i < menu_Items.Count(); i++)
            {
                menuItemsHidden[i] = menu_ItemsHidden_RClick[i];
            }

            // メニュー項目を生成
            MakeMenu();
            
            this.ResumeLayout(false);
            this.Refresh();

            this.Show(StickyNote.m_OwnerForm);
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニュー生成
        /// MODULE ID           : MakeMenu
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
        /// メニュー項目をフォームに追加する
        /// </summary>
        ///
        ///*******************************************************************************
        private void MakeMenu()
        {
            // メニュー項目の位置
            Int32 itemPosTop = 0, itemPosLeft = 0;

            for (int i = 0; i < (Int32)MENUID.TOTAL_ITEMS; i++)
            {
                // 表示ありの項目のみ処理する
                if (false == menuItemsHidden[i])
                {
                    if (menu_Items[i] == "SEPARATOR")
                    {
                        menuLabel = new Label()
                        {
                            Name = "Label" + (i).ToString(),
                            Location = new Point(itemPosLeft, itemPosTop),
                            Text = String.Empty,
                            TextAlign = ContentAlignment.MiddleLeft,
                            BorderStyle = BorderStyle.Fixed3D,
                            Size = new Size(150, 3),
                        };

                        // Y座標をプラス
                        itemPosTop += 3;
                    }
                    else
                    {
                        menuLabel = new Label()
                        {
                            Name = "Label" + (i).ToString(),
                            Location = new Point(itemPosLeft, itemPosTop),
                            Size = new Size(150, 30),
                            Text = menu_Items[i],
                            TextAlign = ContentAlignment.MiddleLeft,
                            Font = new Font("MSゴシック", 14),
                            BorderStyle = BorderStyle.None,
                        };

                        // 色を退避する
                        m_LabelBackColor = menuLabel.BackColor;
                        m_LabelForeColor = menuLabel.ForeColor;

                        if ((i == (Int32)MENUID.HIDE_ALL) || (i == (Int32)MENUID.UNHIDE_ALL))
                        {
                            if (0 == StickyNote.NoteListMng.Count())
                            {
                                // 付箋紙が一つもない場合、非活性にする
                                menuLabel.Enabled = true;
                            }
                        }

                        // Y座標をプラス
                        itemPosTop += 30;

                        // イベントハンドラを追加
                        menuLabel.MouseHover += menuLabel_MouseHover;
                        menuLabel.MouseLeave += menuLabel_MouseLeave;
                        menuLabel.Click += menuLabel_Click;
                    }

                    // パネルにコントロールを追加
                    this.Controls.Add(menuLabel);

                    menuLabel = null;

                    this.Size = new Size(150, itemPosTop);
                }
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームロードイベント
        /// MODULE ID           : MenuForm_Load
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)パラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// メニューフォームロードのイベントで、表示位置を調整する
        /// </summary>
        ///
        ///*******************************************************************************
        private void MenuForm_Load(Object sender, EventArgs ea)
        {
            try
            {
                Int32 dispWidth = dispPosX + this.Size.Width;
                Int32 dispHeight = dispPosY + this.Size.Height;

                // X座標を調整
                if (dispWidth >= Screen.PrimaryScreen.WorkingArea.Width)
                {
                    // 画面領域を超えないように調整する
                    dispPosX = Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width;
                }

                // Y座標を調整
                if (dispHeight >= Screen.PrimaryScreen.WorkingArea.Height)
                {
                    // 画面領域を超えないように調整する
                    dispPosY = Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height;
                }

                this.DesktopLocation = new Point(dispPosX, dispPosY);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームディスポース
        /// MODULE ID           : MenuForm_Disposed
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)パラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// メニューフォームを閉じたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void MenuForm_Disposed(Object sender, EventArgs ea)
        {
            try
            {
                var form = (Form)sender;
                if (true == form.IsDisposed)
                {
                    StickyNote.StickyNoteMenu = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニューマウスホーバー
        /// MODULE ID           : menuLabel_MouseHover
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)パラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// メニュー項目にマウスを当てたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void menuLabel_MouseHover(Object sender, EventArgs ea)
        {
            Label lb = (Label)sender;

            // ハイライト色
            lb.BackColor = Color.Blue;
            lb.ForeColor = Color.White;
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニューマウスLeave
        /// MODULE ID           : menuLabel_MouseLeave
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)パラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// メニュー項目からマウスフォーカスがなくなったときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void menuLabel_MouseLeave(Object sender, EventArgs ea)
        {
            Label lb = (Label)sender;

            // 元の色に戻す
            lb.BackColor = m_LabelBackColor;
            lb.ForeColor = m_LabelForeColor;
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニューラベルクリックイベント
        /// MODULE ID           : menuLabel_Click
        ///
        /// PARAMETER IN        : 
        /// <param name="sender">(in)オブジェクト</param>
        /// <param name="e">(in)パラメータ</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// メニュー項目をクリックしたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void menuLabel_Click(Object sender, EventArgs ea)
        {
            Label lb = (Label)sender;

            // メニューを閉じる
            this.Dispose();
            StickyNote.StickyNoteMenu = null;

            Int32 labelNum = Convert.ToInt32(lb.Name.Substring(5));

            switch (labelNum)
            {
                // 新規作成
                case (Int32)MENUID.CREATENEW:
                    stickyNoteObj.CreateNew(0);
                    break;
                
                // コピー
                case (Int32)MENUID.COPY:
                    Int32 result = stickyNoteObj.CopyNote();

                    // コピー成功
                    if (0 == result)
                    {
                        StickyNote.Current_NoteProperties.Clear();
                    }
                    break;

                // フォント編集
                case (Int32)MENUID.EDITFONT:
                    // フォントダイアログを開く
                    stickyNoteObj.OpenFontDialog();
                    break;

                // 背景色指定
                case (Int32)MENUID.BCOLOR:
                    // 色指定ダイアログを開く
                    stickyNoteObj.OpenColorDialog();
                    break;

                // 透過性
                case (Int32)MENUID.OPACITY:
                    var mea = (MouseEventArgs)ea;

                    StickyNote.m_GripPoint.X = mea.X + StickyNote.m_ParentForm.Location.X;
                    StickyNote.m_GripPoint.Y = mea.Y + StickyNote.m_ParentForm.Location.Y;

                    stickyNoteObj.AdjustOpacity();
                    break;

                // 削除
                case (Int32)MENUID.DELETE:
                    stickyNoteObj.DeleteNote();
                    break;

                // 非表示
                case (Int32)MENUID.HIDE:
                    stickyNoteObj.HideUnhide(0);
                    break;

                case (Int32)MENUID.HIDE_ALL:
                    if (0 != StickyNote.Current_NoteProperties.NoteID)
                    {
                        StickyNote.Current_NoteProperties.NoteID = 0;
                    }
                    stickyNoteObj.HideUnhide(0);
                    break;

                // 表示
                case (Int32)MENUID.UNHIDE_ALL:
                    if (0 != StickyNote.Current_NoteProperties.NoteID)
                    {
                        StickyNote.Current_NoteProperties.NoteID = 0;
                    }
                    stickyNoteObj.HideUnhide(1);
                    break;

                default:
                    break;
            }
        }
    }
}
