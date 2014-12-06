using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Globalization;
using LiveSplit.TimeFormatters;
using LiveSplit.Model;
using LiveSplit.Model.Comparisons;

namespace LiveSplit.UI.Components
{
    public partial class SpeechSettings : UserControl
    {
        public String Split { get; set; }
        public String SplitAheadGaining { get; set; }
        public String SplitAheadLosing { get; set; }
        public String SplitBehindGaining { get; set; }
        public String SplitBehindLosing { get; set; }
        public String BestSegment { get; set; }
        public String UndoSplit { get; set; }
        public String SkipSplit { get; set; }
        public String PersonalBest { get; set; }
        public String NotAPersonalBest { get; set; }
        public String Reset { get; set; }
        public String Pause { get; set; }
        public String Resume { get; set; }
        public String StartTimer { get; set; }

        public SpeechSettings()
        {
            InitializeComponent();

            Split =
            SplitAheadGaining =
            SplitAheadLosing =
            SplitBehindGaining =
            SplitBehindLosing =
            BestSegment =
            UndoSplit =
            SkipSplit =
            PersonalBest =
            NotAPersonalBest =
            Reset = 
            Pause =
            Resume =
            StartTimer = "";

            txtSplitPath.DataBindings.Add("Text", this, "Split");
            txtSplitAheadGaining.DataBindings.Add("Text", this, "SplitAheadGaining");
            txtSplitAheadLosing.DataBindings.Add("Text", this, "SplitAheadLosing");
            txtSplitBehindGaining.DataBindings.Add("Text", this, "SplitBehindGaining");
            txtSplitBehindLosing.DataBindings.Add("Text", this, "SplitBehindLosing");
            txtBestSegment.DataBindings.Add("Text", this, "BestSegment");
            txtUndo.DataBindings.Add("Text", this, "UndoSplit");
            txtSkip.DataBindings.Add("Text", this, "SkipSplit");
            txtPersonalBest.DataBindings.Add("Text", this, "PersonalBest");
            txtNotAPersonalBest.DataBindings.Add("Text", this, "NotAPersonalBest");
            txtReset.DataBindings.Add("Text", this, "Reset");
            txtPause.DataBindings.Add("Text", this, "Pause");
            txtResume.DataBindings.Add("Text", this, "Resume");
            txtStartTimer.DataBindings.Add("Text", this, "StartTimer");
        }

        void DeltaSettings_Load(object sender, EventArgs e)
        {
        }


        private T ParseEnum<T>(XmlElement element)
        {
            return (T)Enum.Parse(typeof(T), element.InnerText);
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version;
            if (element["Version"] != null)
                version = Version.Parse(element["Version"].InnerText);
            else
                version = new Version(1, 0, 0, 0);
            Split = element["Split"].InnerText;
            SplitAheadGaining = element["SplitAheadGaining"].InnerText;
            SplitAheadLosing = element["SplitAheadLosing"].InnerText;
            SplitBehindGaining = element["SplitBehindGaining"].InnerText;
            SplitBehindLosing = element["SplitBehindLosing"].InnerText;
            BestSegment = element["BestSegment"].InnerText;
            UndoSplit = element["UndoSplit"].InnerText;
            SkipSplit = element["SkipSplit"].InnerText;
            PersonalBest = element["PersonalBest"].InnerText;
            NotAPersonalBest = element["NotAPersonalBest"].InnerText;
            Reset = element["Reset"].InnerText;
            Pause = element["Pause"].InnerText;
            Resume = element["Resume"].InnerText;
            StartTimer = element["StartTimer"].InnerText;
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            parent.AppendChild(ToElement(document, "Version", "1.4"));
            parent.AppendChild(ToElement(document, "Split", Split));
            parent.AppendChild(ToElement(document, "SplitAheadGaining", SplitAheadGaining));
            parent.AppendChild(ToElement(document, "SplitAheadLosing", SplitAheadLosing));
            parent.AppendChild(ToElement(document, "SplitBehindGaining", SplitBehindGaining));
            parent.AppendChild(ToElement(document, "SplitBehindLosing", SplitBehindLosing));
            parent.AppendChild(ToElement(document, "BestSegment", BestSegment));
            parent.AppendChild(ToElement(document, "UndoSplit", UndoSplit));
            parent.AppendChild(ToElement(document, "SkipSplit", SkipSplit));
            parent.AppendChild(ToElement(document, "PersonalBest", PersonalBest));
            parent.AppendChild(ToElement(document, "NotAPersonalBest", NotAPersonalBest));
            parent.AppendChild(ToElement(document, "Reset", Reset));
            parent.AppendChild(ToElement(document, "Pause", Pause));
            parent.AppendChild(ToElement(document, "Resume", Resume));
            parent.AppendChild(ToElement(document, "StartTimer", StartTimer));
            return parent;
        }

        private Color ParseColor(XmlElement colorElement)
        {
            return Color.FromArgb(Int32.Parse(colorElement.InnerText, NumberStyles.HexNumber));
        }

        private XmlElement ToElement(XmlDocument document, Color color, string name)
        {
            var element = document.CreateElement(name);
            element.InnerText = color.ToArgb().ToString("X8");
            return element;
        }

        private XmlElement ToElement<T>(XmlDocument document, String name, T value)
        {
            var element = document.CreateElement(name);
            element.InnerText = value.ToString();
            return element;
        }

        protected String BrowseForPath(String path)
        {
            var fileDialog = new OpenFileDialog()
            {
                FileName = path ?? "",
                Filter = "Media Files|*.avi;*.mp3;*.wav;*.mid;*.midi;*.mpeg;*.mpg;*.mp4;*.m4a;*.aac;*.m4v;*.mov;*.wmv;|All Files (*.*)|*.*"
            };
            var result = fileDialog.ShowDialog();
            if (result == DialogResult.OK)
                path = fileDialog.FileName;
            return path;
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            txtSplitPath.Text = Split = BrowseForPath(Split);
        }

        private void btnAheadGaining_Click(object sender, EventArgs e)
        {
            txtSplitAheadGaining.Text = SplitAheadGaining = BrowseForPath(SplitAheadGaining);
        }

        private void btnAheadLosing_Click(object sender, EventArgs e)
        {
            txtSplitAheadLosing.Text = SplitAheadLosing = BrowseForPath(SplitAheadLosing);
        }

        private void btnBehindGaining_Click(object sender, EventArgs e)
        {
            txtSplitBehindGaining.Text = SplitBehindGaining = BrowseForPath(SplitBehindGaining);
        }

        private void btnBehindLosing_Click(object sender, EventArgs e)
        {
            txtSplitBehindLosing.Text = SplitBehindLosing = BrowseForPath(SplitBehindLosing);
        }

        private void btnBestSegment_Click(object sender, EventArgs e)
        {
            txtBestSegment.Text = BestSegment = BrowseForPath(BestSegment);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            txtUndo.Text = UndoSplit = BrowseForPath(UndoSplit);
        }

        private void btnSkipSplit_Click(object sender, EventArgs e)
        {
            txtSkip.Text = SkipSplit = BrowseForPath(SkipSplit);
        }

        private void btnPersonalBest_Click(object sender, EventArgs e)
        {
            txtPersonalBest.Text = PersonalBest = BrowseForPath(PersonalBest);
        }

        private void btnNotAPersonalBest_Click(object sender, EventArgs e)
        {
            txtNotAPersonalBest.Text = NotAPersonalBest = BrowseForPath(NotAPersonalBest);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtReset.Text = Reset = BrowseForPath(Reset);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            txtPause.Text = Pause = BrowseForPath(Pause);
        }


        private void btnResume_Click(object sender, EventArgs e)
        {
            txtResume.Text = Resume = BrowseForPath(Resume);
        }

        private void btnStartTimer_Click(object sender, EventArgs e)
        {
            txtStartTimer.Text = StartTimer = BrowseForPath(StartTimer);
        }
    }
}
