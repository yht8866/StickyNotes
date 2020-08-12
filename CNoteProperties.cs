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
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.ComponentModel;
using System.Drawing;

namespace StickyNotes
{
    ///*******************************************************************************
    ///
    /// CLASS NAME      : 付箋紙リストデータクラス
    /// CLASS ID        : CStickyNotesCollection
    ///
    /// FUNCTION        : 
    /// <summary>
    /// 付箋紙のリストを管理する
    /// </summary>
    /// 
    ///*******************************************************************************
    [Serializable, XmlRoot("StickyNotesCollection")]
    public class CStickyNotesCollection
    {
        #region <プロパティ>
        [XmlElement("StickyNote")]
        public List<CNoteProperties> CNoteProperties { get; set; }
        #endregion

        public CStickyNotesCollection()
        {
            CNoteProperties = new List<CNoteProperties>();
        }
    }

    ///*******************************************************************************
    ///
    /// CLASS NAME      : 付箋紙プロパティ
    /// CLASS ID        : CNoteProperties
    ///
    /// FUNCTION        : 
    /// <summary>
    /// XMLシリアライザで使う付箋紙のプロパティを格納する
    /// </summary>
    /// 
    ///*******************************************************************************
    [Serializable]
    public class CNoteProperties
    {
        #region <プロパティ>
        // 付箋紙ID
        [XmlAttribute]
        public Int32 NoteID { get; set; }
        // textboxの内容
        public String Text { get; set; }
        // フォント(シリアライザ対象外)
        [XmlIgnore]
        public Font FontFont { get; set; }
        // フォント
        [XmlElement("Font")]
        public SerializableFont FontString { get; set; }
        // テキスト色
        public String ForeColor { get; set; }
        // 背景色
        public String BackColor { get; set; }
        // フォームとテキストボックスの大きさ(width,height)
        public Size Size { get; set; }
        // フォームの位置(top,left)
        public Point Location { get; set; }
        // 表示／非表示
        public bool Visible { get; set; }
        // 明瞭
        public double Opacity { get; set; }
        #endregion

        #region <変数>
        [XmlIgnore]
        public System.Windows.Forms.Form Owner = null;
        #endregion

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : コンストラクタ
        /// MODULE ID           : CNoteProperties
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
        public CNoteProperties()
        {
            NoteID = 0;
            Text = String.Empty;
            FontFont = new Font("MSゴシック", 14);
            FontString = new SerializableFont(FontFont);
            ForeColor = (Color.Black).ToArgb().ToString();
            BackColor = (Color.AntiqueWhite).ToArgb().ToString();
            Size = new Size(150, 90);
            Location = new Point(0, 0);
            Visible = true;
            Opacity = 1.00;

            Owner = null;
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : デストラクタ
        /// MODULE ID           : CNoteProperties
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
        /// デストラクタ
        /// </summary>
        ///
        ///*******************************************************************************
        ~CNoteProperties()
        {
            this.Clear();
        }

        ///*******************************************************************************
        ///(－)
        /// MODULE NAME         : クリア
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
        /// 初期化
        /// </summary>
        ///
        ///*******************************************************************************
        public void Clear()
        {
            NoteID = 0;
            Text = String.Empty;
            FontFont = new Font("MSゴシック", 14);
            FontString = new SerializableFont(FontFont);
            ForeColor = (Color.Black).ToArgb().ToString();
            BackColor = (Color.AntiqueWhite).ToArgb().ToString();
            Size = new Size(150, 90);
            Location = new Point(0, 0);
            Visible = true;
            Opacity = 1.00;

            Owner = null;
        }
    }

    ///*******************************************************************************
    ///
    /// CLASS NAME      : シリアライズ可能なフォント
    /// CLASS ID        : SerializableFont
    ///
    /// FUNCTION        : 
    /// <summary>
    /// Serialize可能なFontクラスを作る
    /// </summary>
    /// 
    ///*******************************************************************************
    [Serializable]
    public class SerializableFont
    {
        public SerializableFont()
        {
            this.Font = null;
        }

        public SerializableFont(Font font)
        {
            this.Font = font;
        }

        [XmlIgnore]
        public Font Font { get; set; }

        [XmlElement("Font")]
        public string FontString
        {
            get
            {
                if (Font != null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

                    return converter.ConvertToString(this.Font);
                }
                else return null;
            }
            set
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

                this.Font = (Font)converter.ConvertFromString(value);
            }
        }
    }
}
