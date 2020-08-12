//********************************************************************************
//
//  SYSTEM          : なし(共用ライブラリ)
//  UNIT            : 付箋紙機能
//
//********************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : 付箋紙クラス
    /// CLASS ID        : StickyNote
    ///
    /// FUNCTION        : 
    /// <summary>
    /// 付箋紙のクラス定義
    /// </summary>
    /// 
    ///*******************************************************************************
    public class StickyNote
    {
        // ===========================================
        // プロパティ
        // ===========================================
        #region <プロパティ>
        // フォームオブジェクト
        public FormStickyNote StickyNoteForm { get; set; }
        
        // クリックした付箋紙のプロパティを退避する
        public static CNoteProperties Current_NoteProperties = new CNoteProperties();

        // 付箋紙管理リスト
        public static List<FormStickyNote> NoteListMng { get; set; }

        // 付箋紙メニュー用
        public static FormStickyNoteMenu StickyNoteMenu { get; set; }

        // 透過性調整ダイアログ
        public static FormOpacitySpinner m_OpacityForm { get; set; }

        // クリックした付箋紙のTextboxオブジェクトを格納
        public static TextBox m_CurrTextbox { get; set; }
        #endregion

        // ===========================================
        // 変数
        // ===========================================
        #region <変数>
        // クリックした付箋紙の親フォーム
        public static FormStickyNote m_ParentForm = null;
        // クリックした座標
        public static Point m_GripPoint = new Point();
        // 呼び出し大元のフォーム(別アプリから呼び出すときはアプリのフォームを渡す)
        public static Form m_OwnerForm = null;
        // データをシリアライズ
        public static XmlSerializer xmlSerializer = null;
        // 設定ファイルのファイル名
        private const String FILE_SETTING = @"\StickyNotes_SETTING.ini";
        // 設定ファイルのフルパス(exeと同じフォルダ)
        private static String FILEPATH = Application.StartupPath + FILE_SETTING;
        // 保存先のファイル名
        private const string FILENAME = @"\StickyNotes.xml";
        // XML保存先のフルパス
        private static String m_SaveXMLPath;
        // 初期化フラグ
        private static bool isInitialized = false;
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
        public StickyNote() { }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 初期化処理
        /// MODULE ID           : Init
        ///
        /// PARAMETER IN        : 
        /// <param name="owner">(in)呼び出し大元のフォーム</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// プロパティ・変数を初期化する
        /// </summary>
        ///
        ///*******************************************************************************
        public void Init(Form owner)
        {
            try
            {
                // ownerをクラス変数に格納する
                m_OwnerForm = owner;

                // 設定ファイル読み込み
                ReadSettingFile();

                if (isInitialized == false)
                {
                    NoteListMng = new List<FormStickyNote>();
                    m_CurrTextbox = new TextBox();
                    LoadStickyNotes();
                }

                // 初期化済みにする
                isInitialized = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
                MessageBox.Show("付箋紙初期化失敗", "エラー", MessageBoxButtons.OK);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 初期化処理
        /// MODULE ID           : Clear
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
        /// メンバ変数・プロパティの初期化処理を行う。
        /// </summary>
        ///
        ///*******************************************************************************
        public void Clear()
        {
            m_OwnerForm = null;
            NoteListMng = null;
            m_CurrTextbox = null;
            Current_NoteProperties.Clear();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニュー表示
        /// MODULE ID           : ShowMenu
        ///
        /// PARAMETER IN        : 
        /// <param name="posX">(in)クリックのX軸</param>
        /// <param name="posY">(in)クリックのY軸</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : なし
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 付箋紙メニューを表示する
        /// </summary>
        ///
        ///*******************************************************************************
        public void ShowMenu(Int32 posX, Int32 posY)
        {
            if (StickyNoteMenu == null)
            {
                // メニューオブジェクトを作成
                StickyNoteMenu = new StickyNotes.FormStickyNoteMenu(posX, posY);
            }
            else
            {
                CloseMenu();
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : メニュー非表示
        /// MODULE ID           : CloseMenu
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : なし
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 付箋紙メニューを閉じる
        /// </summary>
        ///
        ///*******************************************************************************
        public void CloseMenu()
        {
            // メニューオブジェクトを作成
            if (StickyNoteMenu != null)
            {
                StickyNoteMenu.Dispose();
                StickyNoteMenu = null;
            }

            // 透過性調整ダイアログを閉じる
            if (m_OpacityForm != null)
            {
                m_OpacityForm.Dispose();
                m_OpacityForm = null;
            }

            // 画面をクリックすると、選択中付箋紙をクリア
            Current_NoteProperties.Clear();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 新規作成
        /// MODULE ID           : CreateNew
        ///
        /// PARAMETER IN        : 
        /// <param name="actionFlg">(in)動作フラグ(0：新規、1:コピー、2：ロード)</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>-1：失敗、0：成功</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 空の付箋紙を新規作成する
        /// </summary>
        ///
        ///*******************************************************************************
        public Int16 CreateNew(Int16 actionFlg)
        {
            Int16 retVal = -1;

            try
            {   
                StickyNoteForm = new FormStickyNote(actionFlg);

                if (null != StickyNoteForm)
                {
                    this.AddtoNoteListMng(StickyNoteForm);
                    SaveStickyNotes();
                    retVal = 0;
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
                return -1;
            }
            finally
            {
                StickyNoteForm = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コピー
        /// MODULE ID           : CopyNote
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
        /// クリック元の付箋紙のコピーを作成する
        /// </summary>
        ///
        ///*******************************************************************************
        public Int16 CopyNote()
        {
            Int16 retVal = -1;

            try
            {
                if (0 != Current_NoteProperties.NoteID)
                {
                    StickyNoteForm = new FormStickyNote(1);

                    // コピー成功
                    if (null != StickyNoteForm)
                    {
                        this.AddtoNoteListMng(StickyNoteForm);
                        SaveStickyNotes();
                        retVal = 0;
                    }
                }
                else
                {
                    // コピー元が選択されていないので失敗
                }

                return retVal;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
                return -1;
            }
            finally
            {
                Current_NoteProperties.Clear();
                StickyNoteForm = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 付箋紙管理リスト追加
        /// MODULE ID           : AddtoNoteListMng
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
        /// 付箋紙の管理リストに追加する
        /// </summary>
        ///
        ///*******************************************************************************
        public void AddtoNoteListMng(FormStickyNote form)
        {
            // 管理リストに追加
            NoteListMng.Insert(0, form);
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 色指定ダイアログオープン
        /// MODULE ID           : OpenColorDialog
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
        /// ColorDialogを開き、背景色を指定する
        /// </summary>
        ///
        ///*******************************************************************************
        public void OpenColorDialog()
        {
            Int32 dlgResult = 0;
            ColorDialog clrDlg = new ColorDialog();

            try
            {
                // 選択した付箋紙の現背景色を初期表示とする
                clrDlg.Color = Color.FromArgb(Convert.ToInt32(Current_NoteProperties.BackColor));

                dlgResult = (Int32)clrDlg.ShowDialog();

                if (dlgResult == (Int32)DialogResult.OK)
                {
                    m_CurrTextbox.BackColor = clrDlg.Color;
                    Current_NoteProperties.BackColor = clrDlg.Color.ToArgb().ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
            finally
            {
                clrDlg = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 色指定ダイアログオープン
        /// MODULE ID           : OpenColorDialog
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
        /// ColorDialogを開き、背景色を指定する
        /// </summary>
        ///
        ///*******************************************************************************
        public void OpenFontDialog()
        {
            Int32 dlgResult = 0;
            FontDialog fontDlg = new FontDialog();

            try
            {
                fontDlg.ShowColor = true;
                fontDlg.ShowEffects = true;

                // 選択した付箋紙の現背景色を初期表示とする
                fontDlg.Font = Current_NoteProperties.FontFont;
                fontDlg.Color = Color.FromArgb(Convert.ToInt32(Current_NoteProperties.ForeColor));

                dlgResult = (Int32)fontDlg.ShowDialog();

                if (dlgResult == (Int32)DialogResult.OK)
                {
                    m_CurrTextbox.ForeColor = fontDlg.Color;
                    m_CurrTextbox.Font = fontDlg.Font;
                    Current_NoteProperties.ForeColor = fontDlg.Color.ToArgb().ToString();
                    Current_NoteProperties.FontFont = fontDlg.Font;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
            finally
            {
                fontDlg = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 透過性調整
        /// MODULE ID           : AdjustOpacity
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
        /// 付箋紙の透過性を調整できるダイアログを表示する
        /// </summary>
        ///
        ///*******************************************************************************
        public void AdjustOpacity()
        {
            try
            {
                m_OpacityForm = null;
                m_OpacityForm = new FormOpacitySpinner();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 付箋紙削除
        /// MODULE ID           : DeleteNote
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
        /// 選択した付箋紙を削除する
        /// </summary>
        ///
        ///*******************************************************************************
        public void DeleteNote()
        {
            FormStickyNote formToDelete;

            try
            {
                // クリックした付箋紙のFormStickyNoteオブジェクト
                formToDelete = m_ParentForm;

                // 管理リストから削除
                NoteListMng.Remove(formToDelete);

                // 削除後はリストを更新する
                SaveStickyNotes();

                // フォームをクロース
                formToDelete.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
            finally
            {
                formToDelete = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 表示／非表示
        /// MODULE ID           : HideUnhide
        ///
        /// PARAMETER IN        : 
        /// <param name="DispFlg">(in)表示フラグ(0:非表示、1:表示)</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>なし</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 画面上のすべての付箋紙を表示／非表示に切換できる
        /// </summary>
        ///
        ///*******************************************************************************
        public void HideUnhide(Int32 DispFlg)
        {
            try
            {
                // すべての付箋紙に適用する
                if (0 == Current_NoteProperties.NoteID)
                {
                    for (int i = 0; i < NoteListMng.Count; i++)
                    {
                        Form formToHide = NoteListMng[i];

                        // 非表示
                        if (0 == DispFlg)
                        {
                            // 表示ありだったら、非表示にする
                            formToHide.Visible = false;
                        }
                        // 表示
                        else
                        {
                            // 非表示だったら、表示する
                            formToHide.Visible = true;
                        }
                    }
                }
                // クリックした付箋紙のみに適用する
                else
                {
                    for (int i = 0; i < NoteListMng.Count; i++)
                    {
                        Form formToHide = NoteListMng[i];

                        if (formToHide == m_ParentForm)
                        {
                            if (0 == DispFlg)
                            {
                                // 表示ありだったら、非表示にする
                                formToHide.Visible = false;
                            }
                            break;
                        }
                        else
                        {
                            // 処理なし
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 付箋紙管理リスト更新
        /// MODULE ID           : UpdateStickyNoteList
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
        /// 付箋紙のプロパティに変化があったときに管理リストの値を更新する
        /// </summary>
        ///
        ///*******************************************************************************
        public static void UpdateStickyNoteList()
        {
            try
            {
                for (int i = 0; i < NoteListMng.Count; i++)
                {
                    Int32 noteID1 = Current_NoteProperties.NoteID;
                    Int32 noteID2 = NoteListMng[i].ThisNoteProperties.NoteID;

                    if (noteID1 == noteID2)
                    {
                        NoteListMng[i].ThisNoteProperties.Text = Current_NoteProperties.Text;
                        NoteListMng[i].ThisNoteProperties.FontFont = Current_NoteProperties.FontFont;
                        NoteListMng[i].ThisNoteProperties.ForeColor = Current_NoteProperties.ForeColor;
                        NoteListMng[i].ThisNoteProperties.BackColor = Current_NoteProperties.BackColor;
                        NoteListMng[i].ThisNoteProperties.Size = Current_NoteProperties.Size;
                        NoteListMng[i].ThisNoteProperties.Location = Current_NoteProperties.Location;
                        NoteListMng[i].ThisNoteProperties.Visible = Current_NoteProperties.Visible;
                        NoteListMng[i].ThisNoteProperties.Opacity = Current_NoteProperties.Opacity;

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 全付箋紙保存
        /// MODULE ID           : SaveStickyNotes
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
        /// 付箋紙を自動的にXMLファイルに保存する
        /// </summary>
        ///
        ///*******************************************************************************
        public static void SaveStickyNotes()
        {
            TextWriter writer;
            CStickyNotesCollection notesDataObj = new CStickyNotesCollection();

            try
            {
                writer = new StreamWriter(m_SaveXMLPath);

                for (int i = 0; i < NoteListMng.Count; i++)
                {
                    CNoteProperties obj = new CNoteProperties();

                    obj.NoteID = NoteListMng[i].ThisNoteProperties.NoteID;
                    obj.Text = NoteListMng[i].ThisNoteProperties.Text;
                    obj.FontString = new SerializableFont(NoteListMng[i].ThisNoteProperties.FontFont);
                    obj.ForeColor = NoteListMng[i].ThisNoteProperties.ForeColor;
                    obj.BackColor = NoteListMng[i].ThisNoteProperties.BackColor;
                    obj.Size = NoteListMng[i].ThisNoteProperties.Size;
                    obj.Location = NoteListMng[i].ThisNoteProperties.Location;
                    obj.Visible = NoteListMng[i].ThisNoteProperties.Visible;
                    obj.Opacity = NoteListMng[i].ThisNoteProperties.Opacity;

                    // リストに追加する
                    notesDataObj.CNoteProperties.Add(obj);

                    obj = null;
                }

                xmlSerializer = new XmlSerializer(typeof(CStickyNotesCollection));
                
                xmlSerializer.Serialize(writer, notesDataObj);
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
            finally
            {
                // オブジェクトをクリア
                xmlSerializer = null;
                writer = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 付箋紙ロード
        /// MODULE ID           : LoadStickyNotes
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
        /// 呼び出し元の装置を立ち上げたときに、XMLに保存されている付箋紙をロードする
        /// </summary>
        ///
        ///*******************************************************************************
        public void LoadStickyNotes()
        {
            CStickyNotesCollection listObj = new CStickyNotesCollection();
            XmlDocument doc;

            try
            {
                bool IsExist = File.Exists(m_SaveXMLPath);

                if (true == IsExist)
                {
                    xmlSerializer = new XmlSerializer(typeof(CStickyNotesCollection));

                    doc = new XmlDocument();
                    doc.PreserveWhitespace = true;
                    doc.Load(m_SaveXMLPath);

                    using (XmlNodeReader reader = new XmlNodeReader(doc.DocumentElement))
                    {
                        listObj = (CStickyNotesCollection)xmlSerializer.Deserialize(reader);
                    }
                    // trial 1 end

                    Int32 cnt = listObj.CNoteProperties.Count();

                    for (int i = 0; i < cnt; i++)
                    {
                        StickyNote.Current_NoteProperties = new CNoteProperties();

                        // 付箋紙ID
                        StickyNote.Current_NoteProperties.NoteID = i + 1;
                        // テキストボックスの内容
                        StickyNote.Current_NoteProperties.Text = listObj.CNoteProperties[i].Text;
                        // String型のフォント情報
                        StickyNote.Current_NoteProperties.FontString = listObj.CNoteProperties[i].FontString;

                        // フォント：stringからFontに変換
                        System.ComponentModel.TypeConverter tc = System.ComponentModel.TypeDescriptor.GetConverter(typeof(Font));
                        Font convertedFont = (Font)tc.ConvertFromString(StickyNote.Current_NoteProperties.FontString.FontString);

                        // Font型のフォント情報
                        StickyNote.Current_NoteProperties.FontFont = convertedFont;
                        // テキスト表示色
                        StickyNote.Current_NoteProperties.ForeColor = listObj.CNoteProperties[i].ForeColor;
                        // 背景色
                        StickyNote.Current_NoteProperties.BackColor = listObj.CNoteProperties[i].BackColor;
                        // サイズ
                        StickyNote.Current_NoteProperties.Size = listObj.CNoteProperties[i].Size;
                        // x,y座標
                        StickyNote.Current_NoteProperties.Location = listObj.CNoteProperties[i].Location;
                        // 表示／非表示
                        StickyNote.Current_NoteProperties.Visible = listObj.CNoteProperties[i].Visible;
                        // 明瞭
                        StickyNote.Current_NoteProperties.Opacity = listObj.CNoteProperties[i].Opacity;

                        // 上記の情報を使って付箋紙を生成する
                        StickyNoteForm = new FormStickyNote(2);

                        if (null != StickyNoteForm)
                        {
                            this.AddtoNoteListMng(StickyNoteForm);
                            UpdateStickyNoteList();
                            SaveStickyNotes();
                            StickyNote.Current_NoteProperties.Clear();
                        }
                    }
                }

                listObj = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
            }
            finally
            {
                doc = null;
                xmlSerializer = null;
            }
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : 設定ファイル読み込み
        /// MODULE ID           : ReadSettingFile
        ///
        /// PARAMETER IN        : 
        /// <param>(in)なし</param>
        /// PARAMETER OUT       : 
        /// <param>(out)なし</param>
        ///
        /// RETURN VALUE        : 
        /// <returns>true = 読込成功、false = 読込失敗</returns>
        ///
        /// FUNCTION            : 
        /// <summary>
        /// 設定ファイルの読込処理
        /// </summary>
        ///
        ///*******************************************************************************
        private bool ReadSettingFile()
        {
            bool retBool = false;
            const String SECTIONMENU = "MENU";
            const String SECTIONPATH = "PATH";
            const String SECTIONMENUHIDDEN = "MENU_ITEMSHIDDEN";
            String[] strTemp;
            StringBuilder sbValue = new StringBuilder(1024);
            // 区切り文字
            char[] delimeterChars = { ',' };

            try
            {
                // =================================
                // ＸＭＬファイルの保存先を読込
                // ================================= 
                NativeMethods.GetPrivateProfileString(SECTIONPATH, @"SAVEXMLDIR", @"", sbValue, (UInt32)sbValue.Capacity, FILEPATH);
                m_SaveXMLPath = sbValue.ToString() + FILENAME;
                sbValue.Clear();

                // =================================
                // メニュー項目の読み込み
                // =================================
                NativeMethods.GetPrivateProfileString(SECTIONMENU, @"MENU_ITEMS", @"", sbValue, (UInt32)sbValue.Capacity, FILEPATH);
                FormStickyNoteMenu.menu_Items = sbValue.ToString().Split(delimeterChars);
                sbValue.Clear();

                // =================================
                // メニュー項目表示設定の読み込み
                // (右クリック)
                // =================================
                NativeMethods.GetPrivateProfileString(SECTIONMENUHIDDEN, @"SETTING_1", @"", sbValue, (UInt32)sbValue.Capacity, FILEPATH);
                strTemp = sbValue.ToString().Split(delimeterChars);
                FormStickyNoteMenu.menu_ItemsHidden_RClick = new bool[strTemp.Count()];
                for (int i = 0; i < strTemp.Count(); i++)
                {
                    if ("true" == strTemp[i])
                    {
                        FormStickyNoteMenu.menu_ItemsHidden_RClick[i] = true;
                    }
                    else
                    {
                        FormStickyNoteMenu.menu_ItemsHidden_RClick[i] = false;
                    }
                }
                sbValue.Clear();
                strTemp = null;

                // =================================
                // メニュー項目表示設定の読み込み
                // (メニューボタン)
                // =================================
                NativeMethods.GetPrivateProfileString(SECTIONMENUHIDDEN, @"SETTING_2", @"", sbValue, (UInt32)sbValue.Capacity, FILEPATH);
                strTemp = sbValue.ToString().Split(delimeterChars);
                FormStickyNoteMenu.menu_ItemsHidden_Button = new bool[strTemp.Count()];
                for (int i = 0; i < strTemp.Count(); i++)
                {
                    if ("true" == strTemp[i])
                    {
                        FormStickyNoteMenu.menu_ItemsHidden_Button[i] = true;
                    }
                    else
                    {
                        FormStickyNoteMenu.menu_ItemsHidden_Button[i] = false;
                    }
                }

                FormStickyNoteMenu.menuItemsHidden = new bool[strTemp.Count()];
                sbValue.Clear();
                strTemp = null;

                return retBool;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(MethodBase.GetCurrentMethod() + ": " + ex.Message);
                return false;
            }
            finally
            {
                sbValue = null;
            }
        }
    }
}
