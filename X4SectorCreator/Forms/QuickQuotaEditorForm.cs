using System.ComponentModel;
using System.Globalization;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms
{
    public partial class QuickQuotaEditorForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FactoriesForm FactoriesForm { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public JobsForm JobsForm { get; set; }

        public QuickQuotaEditorForm()
        {
            InitializeComponent();

            QuotaView.CellBeginEdit += QuotaView_CellBeginEdit;
            QuotaView.CellValidating += QuotaView_CellValidating;
        }

        enum Quota
        {
            Galaxy = 1,
            Cluster = 2,
            Sector = 3
        }

        private static bool IsValidValue(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && int.TryParse(value, CultureInfo.InvariantCulture, out _);
        }

        private bool ValidateIndex(int row, int index, string value)
        {
            // Validate if the given cell is a match with the object
            if (JobsForm != null)
            {
                var job = JobsForm.AllJobs[QuotaView.Rows[row].Cells[0].Value as string];
                switch ((Quota)index)
                {
                    case Quota.Galaxy:
                        if (job.Quota?.Galaxy != null && !string.IsNullOrWhiteSpace(job.Quota?.Galaxy))
                            return IsValidValue(value);
                        break;
                    case Quota.Cluster:
                        if (job.Quota?.Cluster != null && !string.IsNullOrWhiteSpace(job.Quota?.Cluster))
                            return IsValidValue(value);
                        break;
                    case Quota.Sector:
                        if (job.Quota?.Sector != null && !string.IsNullOrWhiteSpace(job.Quota?.Sector))
                            return IsValidValue(value);
                        break;
                }
                return true;
            }
            else if (FactoriesForm != null)
            {
                var factory = FactoriesForm.AllFactories[QuotaView.Rows[row].Cells[0].Value as string];
                switch ((Quota)index)
                {
                    case Quota.Galaxy:
                        if (factory.Quotas?.Quota?.Galaxy != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Galaxy))
                            return IsValidValue(value);
                        break;
                    case Quota.Cluster:
                        if (factory.Quotas?.Quota?.Cluster != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Cluster))
                            return IsValidValue(value);
                        break;
                    case Quota.Sector:
                        if (factory.Quotas?.Quota?.Sector != null && !string.IsNullOrWhiteSpace(factory.Quotas?.Quota?.Sector))
                            return IsValidValue(value);
                        break;
                }
                return true;
            }
            return true;
        }

        private void QuotaView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!ValidateIndex(e.RowIndex, e.ColumnIndex, e.FormattedValue as string))
            {
                MessageBox.Show("You must set a valid non-empty integer value for this quota.");
                e.Cancel = true;
            }
        }

        private void QuotaView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Don't allow editing empty cells
            var column = QuotaView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (column.Value == null)
            {
                var type = QuotaView.Columns[e.ColumnIndex].Name;
                MessageBox.Show($"You cannot set a \"{type}\" quota on this entry as it is not of this type.");
                e.Cancel = true;
            }
        }

        public void Initialize()
        {
            // Setup data grid values
            if (JobsForm != null)
            {
                foreach (var job in JobsForm.AllJobs)
                {
                    QuotaView.Rows.Add(job.Value.Id, job.Value.Quota?.Galaxy, job.Value.Quota?.Cluster, job.Value.Quota?.Sector);
                }
            }
            else if (FactoriesForm != null)
            {
                foreach (var factory in FactoriesForm.AllFactories)
                {
                    QuotaView.Rows.Add(factory.Value.Id, factory.Value.Quotas?.Quota?.Galaxy, factory.Value.Quotas?.Quota?.Cluster, factory.Value.Quotas?.Quota?.Sector);
                }
            }
            else
            {
                throw new Exception("No JobsForm or FactoriesForm set on QuickQuotaEditorForm. Invalid state, report a bug!");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in QuotaView.Rows)
            {
                if (JobsForm != null)
                {
                    var job = JobsForm.AllJobs[row.Cells[0].Value as string];

                    var galaxy = row.Cells[(int)Quota.Galaxy].Value as string;
                    if (!string.IsNullOrWhiteSpace(galaxy))
                        job.Quota.Galaxy = galaxy;

                    var cluster = row.Cells[(int)Quota.Cluster].Value as string;
                    if (!string.IsNullOrWhiteSpace(cluster))
                        job.Quota.Cluster = cluster;

                    var sector = row.Cells[(int)Quota.Sector].Value as string;
                    if (!string.IsNullOrWhiteSpace(sector))
                        job.Quota.Sector = sector;
                }
                else if (FactoriesForm != null)
                {
                    var factory = FactoriesForm.AllFactories[row.Cells[0].Value as string];

                    var galaxy = row.Cells[(int)Quota.Galaxy].Value as string;
                    if (!string.IsNullOrWhiteSpace(galaxy))
                        factory.Quotas.Quota.Galaxy = galaxy;

                    var cluster = row.Cells[(int)Quota.Cluster].Value as string;
                    if (!string.IsNullOrWhiteSpace(cluster))
                        factory.Quotas.Quota.Cluster = cluster;

                    var sector = row.Cells[(int)Quota.Sector].Value as string;
                    if (!string.IsNullOrWhiteSpace(sector))
                        factory.Quotas.Quota.Sector = sector;
                }
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
