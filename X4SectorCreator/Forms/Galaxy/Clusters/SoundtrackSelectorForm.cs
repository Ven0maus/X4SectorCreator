using System.Data;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using System.ComponentModel;

namespace X4SectorCreator.Forms
{
    public partial class SoundtrackSelectorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ClusterForm ClusterForm { get; set; }

        private readonly List<Soundtrack> _soundtracks = [];

        public SoundtrackSelectorForm()
        {
            InitializeComponent();

            // Init soundtracks in listbox
            _soundtracks.Add(new Soundtrack("None", null)); // Init none selection as well
            foreach (var kvp in SoundtrackCollection.Instance.Soundtracks)
            {
                foreach (var trackName in kvp.Value.OrderBy(a => a))
                {
                    var soundtrack = new Soundtrack(trackName, kvp.Key);
                    _soundtracks.Add(soundtrack);
                }
            }

            // Enable search textbox
            TxtSearch.EnableTextSearch(_soundtracks, a => a.ToString(), ApplyFilter);
            Disposed += SoundtrackSelectorForm_Disposed;

            ApplyFilter(_soundtracks);
        }

        private void ApplyFilter(List<Soundtrack> soundtracks)
        {
            SoundtracksListBox.Items.Clear();
            foreach (var soundtrack in soundtracks)
                SoundtracksListBox.Items.Add(soundtrack);
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            if (SoundtracksListBox.SelectedItem is not Soundtrack soundtrack)
            {
                MessageBox.Show("Please select a soundtrack.");
                return;
            }

            ClusterForm.TxtSoundtrack.Text = soundtrack.Name.Equals("None", StringComparison.OrdinalIgnoreCase) ? string.Empty : soundtrack.Name;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SoundtrackSelectorForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        class Soundtrack(string name, string dlc)
        {
            public string Dlc { get; set; } = dlc;
            public string Name { get; set; } = name;

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Dlc))
                    return Name;
                return $"[{Dlc}]: {Name}";
            }
        }
    }
}
