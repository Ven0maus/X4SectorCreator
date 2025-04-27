using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.General
{
    public partial class TemplateGroupsForm : Form
    {
        public enum GroupsFor
        {
            Factories,
            Jobs
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupsFor TemplateGroupsFor { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Action UpdateMethod { get; set; }

        private readonly Dictionary<string, List<Job>> _jobTemplateGroups = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<Factory>> _factoryTemplateGroups = new(StringComparer.OrdinalIgnoreCase);
        private readonly LazyEvaluated<TemplateContentForm> _templateContentForm = new(() => new TemplateContentForm(), a => !a.IsDisposed);

        private readonly List<object> _currentSelection = [];
        private readonly static HashSet<string> _defaultGroups = new(StringComparer.OrdinalIgnoreCase)
        {
            "Vanilla", "Deadair_Scripts"
        };

        public TemplateGroupsForm()
        {
            InitializeComponent();

            TxtSearch.EnableTextSearch(() => _currentSelection, a => a.ToString(), ApplyCurrentFilter);
            Disposed += TemplateGroupsForm_Disposed;
        }

        private void TemplateGroupsForm_Disposed(object sender, EventArgs e)
        {
            TxtSearch.DisableTextSearch();
        }

        private void ApplyCurrentFilter(List<object> items)
        {
            TemplatesInGroupListBox.Items.Clear();

            if (items != null)
            {
                foreach (var item in items.OrderBy(a => a.ToString()))
                    TemplatesInGroupListBox.Items.Add(item);
            }
        }

        private void InitializeTemplateJobs()
        {
            TemplateGroupsListBox.Items.Clear();

            var files = new List<string>();
            var baseDirectory = TemplateGroupsFor == GroupsFor.Factories ?
                        Constants.DataPaths.TemplateFactoriesDirectoryPath :
                        Constants.DataPaths.TemplateJobsDirectoryPath;

            var fileName = TemplateGroupsFor == GroupsFor.Factories ? "god.xml" : "jobs.xml";
            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                var filePath = Path.Combine(directory, fileName);
                if (File.Exists(filePath))
                    files.Add(filePath);
            }

            switch (TemplateGroupsFor)
            {
                case GroupsFor.Factories:
                    foreach (var file in files)
                    {
                        var factories = Objects.Factories.DeserializeFactories(File.ReadAllText(file));
                        string groupName = new DirectoryInfo(Path.GetDirectoryName(file)).Name;
                        _factoryTemplateGroups[groupName] = factories.FactoryList;
                        TemplateGroupsListBox.Items.Add(groupName);
                    }
                    break;

                case GroupsFor.Jobs:
                    foreach (var file in files)
                    {
                        var jobs = Jobs.DeserializeJobs(File.ReadAllText(file));
                        string groupName = new DirectoryInfo(Path.GetDirectoryName(file)).Name;
                        _jobTemplateGroups[groupName] = jobs.JobList;
                        TemplateGroupsListBox.Items.Add(groupName);
                    }
                    break;
            }
        }

        private void BtnCreateNewGroup_Click(object sender, EventArgs e)
        {
            const string lblGroupName = "Group Name:";
            Dictionary<string, string> data = MultiInputDialog.Show("Create New Group",
                (lblGroupName, null, null)
            );
            if (data == null || data.Count == 0)
                return;

            string groupName = data[lblGroupName];
            if (string.IsNullOrWhiteSpace(groupName))
            {
                _ = MessageBox.Show($"Group name cannot be empty.");
                return;
            }

            // Validate path characters
            if (!IsValidFolderName(groupName))
            {
                _ = MessageBox.Show($"This is an invalid group name, special characters are not allowed.");
                return;
            }

            // Check if name already exists
            if (ValidateGroupName(TemplateGroupsFor == GroupsFor.Jobs ?
                _jobTemplateGroups : _factoryTemplateGroups, groupName))
            {
                _ = MessageBox.Show($"A group with the name \"{groupName}\" already exists.");
                return;
            }

            CreateGroup(groupName);
            TemplateGroupsListBox.SelectedItem = groupName;
        }

        private static bool IsValidFolderName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars(); // use FileName chars for folders
            return !string.IsNullOrWhiteSpace(name) && !name.Any(c => invalidChars.Contains(c));
        }

        private void BtnDeleteGroup_Click(object sender, EventArgs e)
        {
            if (TemplateGroupsListBox.SelectedItem is string group &&
                !string.IsNullOrWhiteSpace(group))
            {
                if (_defaultGroups.Contains(group))
                {
                    return;
                }

                RemoveGroup(group);
            }
        }

        public void AddOrUpdateTemplate(string groupName, object original, object @new)
        {
            if (_defaultGroups.Contains(groupName))
            {
                return;
            }

            switch (TemplateGroupsFor)
            {
                case GroupsFor.Jobs:
                    var job = (Job)@new;
                    if (_jobTemplateGroups.TryGetValue(groupName, out var jobs))
                    {
                        if (original != null)
                        {
                            var originJob = (Job)original;
                            jobs.Remove(originJob);
                            TemplatesInGroupListBox.Items.Remove(originJob);
                        }

                        jobs.Add(job);
                        TemplatesInGroupListBox.Items.Add(job);
                    }
                    break;

                case GroupsFor.Factories:
                    var factory = (Factory)@new;
                    if (_factoryTemplateGroups.TryGetValue(groupName, out var factories))
                    {
                        if (original != null)
                        {
                            var originFactory = (Factory)original;
                            factories.Remove(originFactory);
                            TemplatesInGroupListBox.Items.Remove(originFactory);
                        }

                        factories.Add(factory);
                        TemplatesInGroupListBox.Items.Add(factory);
                    }
                    break;
            }
        }

        private void BtnAddTemplate_Click(object sender, EventArgs e)
        {
            if (TemplateGroupsListBox.SelectedItem is string group && !string.IsNullOrWhiteSpace(group))
            {
                if (_defaultGroups.Contains(group))
                {
                    return;
                }

                _templateContentForm.Value.CurrentTemplateGroup = group;
                _templateContentForm.Value.TemplateGroupsForm = this;
                _templateContentForm.Value.Job = null;
                _templateContentForm.Value.Factory = null;
                _templateContentForm.Value.LoadContent(this, null);
                _templateContentForm.Value.Show();
            }
            else
            {
                _ = MessageBox.Show("You must select a template group first.");
            }
        }

        private void BtnDeleteTemplate_Click(object sender, EventArgs e)
        {
            if (TemplateGroupsListBox.SelectedItem is string group && !string.IsNullOrWhiteSpace(group))
            {
                if (_defaultGroups.Contains(group))
                {
                    return;
                }

                if (TemplatesInGroupListBox.SelectedItem is Factory factory)
                {
                    if (_factoryTemplateGroups.TryGetValue(group, out var factories))
                    {
                        factories.Remove(factory);
                        RemoveSelectedIndex(TemplatesInGroupListBox);
                    }
                }
                else if (TemplatesInGroupListBox.SelectedItem is Job job)
                {
                    if (_jobTemplateGroups.TryGetValue(group, out var jobs))
                    {
                        jobs.Remove(job);
                        RemoveSelectedIndex(TemplatesInGroupListBox);
                    }
                }
            }
        }

        private static bool ValidateGroupName(IDictionary data, string key)
        {
            return data.Contains(key);
        }

        private static void RemoveSelectedIndex(ListBox listBox)
        {
            int index = listBox.Items.IndexOf(listBox.SelectedItem);
            listBox.Items.Remove(listBox.SelectedItem);

            // Ensure index is within valid range
            index--;
            index = Math.Max(0, index);
            listBox.SelectedItem = index >= 0 && listBox.Items.Count > 0 ?
                listBox.Items[index] : null;
        }

        private void CreateGroup(string groupName)
        {
            switch (TemplateGroupsFor)
            {
                case GroupsFor.Jobs:
                    _jobTemplateGroups[groupName] = [];
                    break;
                case GroupsFor.Factories:
                    _factoryTemplateGroups[groupName] = [];
                    break;
            }

            TemplateGroupsListBox.Items.Add(groupName);
        }

        private void RemoveGroup(string groupName)
        {
            if (_defaultGroups.Contains(groupName))
            {
                return;
            }

            switch (TemplateGroupsFor)
            {
                case GroupsFor.Factories:
                    _factoryTemplateGroups.Remove(groupName);
                    break;
                case GroupsFor.Jobs:
                    _jobTemplateGroups.Remove(groupName);
                    break;
            }

            RemoveSelectedIndex(TemplateGroupsListBox);
        }

        private string GetFilePath(string groupName, out string directory)
        {
            var baseDirectory = TemplateGroupsFor == GroupsFor.Factories ?
                Constants.DataPaths.TemplateFactoriesDirectoryPath :
                Constants.DataPaths.TemplateJobsDirectoryPath;

            directory = Path.Combine(baseDirectory, groupName);

            return Path.Combine(directory, TemplateGroupsFor == GroupsFor.Factories ?
                "god.xml" : "jobs.xml");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            // Delete all except vanilla
            var baseDirectory = TemplateGroupsFor == GroupsFor.Factories ?
                Constants.DataPaths.TemplateFactoriesDirectoryPath :
                Constants.DataPaths.TemplateJobsDirectoryPath;

            foreach (var directory in Directory.GetDirectories(baseDirectory))
            {
                if (!_defaultGroups.Contains(Path.GetFileName(directory)))
                {
                    Directory.Delete(directory, true);
                }
            }

            // Create new ones
            switch (TemplateGroupsFor)
            {
                case GroupsFor.Factories:
                    foreach (var group in _factoryTemplateGroups)
                    {
                        if (_defaultGroups.Contains(group.Key))
                        {
                            continue;
                        }

                        var filePath = GetFilePath(group.Key, out var directory);
                        Directory.CreateDirectory(directory);

                        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                        XDocument xmlDocument = new(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement("god", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                            group.Value.Select(a => XElement.Parse(a.SerializeFactory()))
                            )
                        );
                        xmlDocument.Save(filePath);
                    }
                    break;

                case GroupsFor.Jobs:
                    foreach (var group in _jobTemplateGroups)
                    {
                        if (_defaultGroups.Contains(group.Key))
                        {
                            continue;
                        }

                        var filePath = GetFilePath(group.Key, out var directory);
                        Directory.CreateDirectory(directory);

                        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                        XDocument xmlDocument = new(
                            new XDeclaration("1.0", "utf-8", null),
                            new XElement("jobs", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                            group.Value.Select(a => XElement.Parse(a.SerializeJob()))
                            )
                        );
                        xmlDocument.Save(filePath);
                    }
                    break;
            }

            // Update view
            UpdateMethod?.Invoke();
            Close();
        }

        private void TemplateGroupsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_changing) return;
            TemplatesInGroupListBox.Items.Clear();
            _currentSelection.Clear();

            if (TemplateGroupsListBox.SelectedIndex == -1) return;
                if (TemplateGroupsListBox.SelectedItem is string group &&
                !string.IsNullOrWhiteSpace(group))
            {
                // Disable functions for vanilla group
                if (_defaultGroups.Contains(group))
                {
                    BtnDeleteGroup.Enabled = false;
                    BtnAddTemplate.Enabled = false;
                    BtnDeleteTemplate.Enabled = false;
                }
                else
                {
                    BtnDeleteGroup.Enabled = true;
                    BtnAddTemplate.Enabled = true;
                    BtnDeleteTemplate.Enabled = true;
                }

                switch (TemplateGroupsFor)
                {
                    case GroupsFor.Jobs:
                        if (_jobTemplateGroups.TryGetValue(group, out var jobList))
                        {
                            foreach (var item in jobList)
                            {
                                TemplatesInGroupListBox.Items.Add(item);
                                _currentSelection.Add(item);
                            }
                        }
                        break;
                    case GroupsFor.Factories:
                        if (_factoryTemplateGroups.TryGetValue(group, out var factoryList))
                        {
                            foreach (var item in factoryList)
                            {
                                TemplatesInGroupListBox.Items.Add(item);
                                _currentSelection.Add(item);
                            }
                        }
                        break;
                }
            }
            TxtSearch.GetTextSearchComponent().ForceCalculate();
        }

        private void TemplatesInGroupListBox_DoubleClick(object sender, EventArgs e)
        {
            if (TemplateGroupsListBox.SelectedItem is string group &&
                !string.IsNullOrWhiteSpace(group))
            {
                if (_defaultGroups.Contains(group))
                {
                    _ = MessageBox.Show("Standard tool templates cannot be modified.");
                    return;
                }

                if (TemplatesInGroupListBox.SelectedItem != null)
                {
                    _templateContentForm.Value.CurrentTemplateGroup = group;
                    _templateContentForm.Value.TemplateGroupsForm = this;

                    if (TemplateGroupsFor == GroupsFor.Jobs)
                    {
                        _templateContentForm.Value.Job = (Job)TemplatesInGroupListBox.SelectedItem;
                        _templateContentForm.Value.Factory = null;
                    }
                    else if (TemplateGroupsFor == GroupsFor.Factories)
                    {
                        _templateContentForm.Value.Factory = (Factory)TemplatesInGroupListBox.SelectedItem;
                        _templateContentForm.Value.Job = null;
                    }

                    _templateContentForm.Value.LoadContent(this, null);
                    _templateContentForm.Value.Show();
                }
            }
        }

        private void TemplateGroupsForm_Load(object sender, EventArgs e)
        {
            InitializeTemplateJobs();
            TemplateGroupsListBox.SelectedItem = "Vanilla";
        }

        private void BtnImportGroup_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "XML files (*.xml)|*.xml";
            var type = TemplateGroupsFor == GroupsFor.Jobs ? "jobs" : "god";
            openFileDialog.Title = $"Select {type} file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                var fileName = Path.GetFileName(filePath);
                if (!fileName.Equals(type + ".xml"))
                {
                    _ = MessageBox.Show($"Invalid file, must be a \"{type + ".xml"}\" file.");
                    return;
                }

                const string lblGroupName = "Group Name:";
                Dictionary<string, string> data = MultiInputDialog.Show("Create New Group",
                    (lblGroupName, null, null)
                );
                if (data == null || data.Count == 0)
                    return;

                string groupName = data[lblGroupName];
                if (string.IsNullOrWhiteSpace(groupName))
                {
                    _ = MessageBox.Show($"Group name cannot be empty.");
                    return;
                }

                // Validate path characters
                if (!IsValidFolderName(groupName))
                {
                    _ = MessageBox.Show($"This is an invalid group name, special characters are not allowed.");
                    return;
                }

                // Check if name already exists
                if (ValidateGroupName(TemplateGroupsFor == GroupsFor.Jobs ?
                    _jobTemplateGroups : _factoryTemplateGroups, groupName))
                {
                    _ = MessageBox.Show($"A group with the name \"{groupName}\" already exists.");
                    return;
                }

                using var sourceStream = openFileDialog.OpenFile();
                using var fs = new StreamReader(sourceStream);
                var content = fs.ReadToEnd();

                if (TemplateGroupsFor == GroupsFor.Jobs)
                {
                    try
                    {
                        var jobs = Jobs.DeserializeJobs(content);
                        CreateGroup(groupName);
                        foreach (var job in jobs.JobList)
                            AddOrUpdateTemplate(groupName, null, job);
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show("Invalid file, cannot read xml: " + ex.Message);
                        return;
                    }
                }
                else if (TemplateGroupsFor == GroupsFor.Factories)
                {
                    try
                    {
                        var factories = Objects.Factories.DeserializeFactories(content);
                        CreateGroup(groupName);
                        foreach (var factory in factories.FactoryList)
                            AddOrUpdateTemplate(groupName, null, factory);
                    }
                    catch (Exception ex)
                    {
                        _ = MessageBox.Show("Invalid file, cannot read xml: " + ex.Message);
                        return;
                    }
                }
            }
        }

        private bool _changing = false;
        private void CmbGroupFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            _changing = true;
            var groups = TemplateGroupsFor == GroupsFor.Jobs ? [.. _jobTemplateGroups.Keys] :
                _factoryTemplateGroups.Keys.ToArray();

            var selected = TemplateGroupsListBox.SelectedItem as string;
            TemplateGroupsListBox.Items.Clear();

            switch (CmbGroupFilter.SelectedItem)
            {
                case "Show All":
                    foreach (var group in groups)
                        TemplateGroupsListBox.Items.Add(group);
                    break;
                case "Show Standard":
                    foreach (var group in groups.Where(_defaultGroups.Contains))
                        TemplateGroupsListBox.Items.Add(group);
                    break;
                case "Show Custom":
                    foreach (var group in groups.Where(a => !_defaultGroups.Contains(a)))
                        TemplateGroupsListBox.Items.Add(group);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(selected) && TemplateGroupsListBox.Items.Contains(selected))
            {
                TemplateGroupsListBox.SelectedItem = selected;
            }
            else
            {
                TemplateGroupsListBox.SelectedIndex = -1;
                TemplatesInGroupListBox.Items.Clear();
                _currentSelection.Clear();
                TxtSearch.GetTextSearchComponent().ForceCalculate();
            }
            _changing = false;
        }
    }
}
