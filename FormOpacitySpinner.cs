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
using System.Reflection;
using StickyNotes;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : 透過性調整フォームクラス
    /// CLASS ID        : FormOpacitySpinner
    ///
    /// FUNCTION        : 
    /// <summary>
    /// 透過性調整フォームのクラス
    /// </summary>
    /// 
    ///*******************************************************************************
    public partial class FormOpacitySpinner : Form
    {
        #region <変数>
        // 透過性の旧設定値
        private double old_val = 0;
        #endregion

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : FormOpacitySpinner
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
        public FormOpacitySpinner()
        {
            InitializeComponent();

            this.SuspendLayout();

            this.opacitySpinner.Value = (Decimal)(StickyNote.Current_NoteProperties.Opacity*100);

            // 前回の値を退避
            old_val = (double)this.opacitySpinner.Value;

            this.ResumeLayout(false);

            this.okBtn.MouseDown += okBtn_MouseDown;
            this.cancelBtn.MouseDown += cancelBtn_MouseDown;
            this.Load += FormOpacitySpinner_Load;
            this.Disposed += FormOpacitySpinner_Disposed;

            // Modal
            this.ShowDialog(StickyNote.m_OwnerForm);
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : フォームロードイベント
        /// MODULE ID           : FormOpacitySpinner_Load
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
        /// フォームロードのイベントで、表示位置を調整する
        /// </summary>
        ///
        ///*******************************************************************************
        private void FormOpacitySpinner_Load(Object sender, EventArgs e)
        {
            try
            {
                this.Size = new Size(90, 50);

                Int32 dispWidth = StickyNote.m_ParentForm.Location.X + this.Size.Width;
                Int32 dispHeight = StickyNote.m_ParentForm.Location.Y + this.Size.Height;

                // X座標を調整
                if (dispWidth >= Screen.PrimaryScreen.WorkingArea.Width)
                {
                    // 画面領域を超えないように調整する
                    dispWidth = Screen.PrimaryScreen.WorkingArea.Width - this.Size.Width;
                }
                else
                {
                    dispWidth = StickyNote.m_GripPoint.X;
                }

                // Y座標を調整
                if (dispHeight >= Screen.PrimaryScreen.WorkingArea.Height)
                {
                    // 画面領域を超えないように調整する
                    dispHeight = Screen.PrimaryScreen.WorkingArea.Height - this.Size.Height;
                }
                else
                {
                    dispHeight = StickyNote.m_GripPoint.Y;
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
        /// MODULE NAME         : フォームディスポース
        /// MODULE ID           : FormOpacitySpinner_Disposed
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
        private void FormOpacitySpinner_Disposed(Object sender, EventArgs e)
        {
            try
            {
                var form = (Form)sender;
                if (true == form.IsDisposed)
                {
                    StickyNote.m_OpacityForm = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : ＯＫボタンクリックイベント
        /// MODULE ID           : okBtn_MouseDown
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
        /// フォームの「設定」ボタンをクリックしたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void okBtn_MouseDown(Object sender, MouseEventArgs e)
        {
            try
            {
                double new_val = (double)this.opacitySpinner.Value;

                // 変化あるときのみ保存
                if (new_val != old_val)
                {
                    // 0を設定した場合、非表示扱いとする
                    if (20 > new_val)
                    {
                        // 20より以下を許可しない
                        new_val = 20;
                    }
                    
                    StickyNote.m_ParentForm.Opacity = new_val / 100;
                    StickyNote.Current_NoteProperties.Opacity = new_val / 100;

                    StickyNote.UpdateStickyNoteList();
                    StickyNote.SaveStickyNotes();
                }

                this.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 取消ボタンクリックイベント
        /// MODULE ID           : cancelBtn_MouseDown
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
        /// フォームの「取消」ボタンをクリックしたときのイベント
        /// </summary>
        ///
        ///*******************************************************************************
        private void cancelBtn_MouseDown(Object sender, EventArgs e)
        {
            try
            {
                this.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }
    }
}
