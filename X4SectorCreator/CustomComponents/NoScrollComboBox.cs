namespace X4SectorCreator.CustomComponents
{
    internal class NoScrollComboBox : ComboBox
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Ignore wheel changes to avoid accidental filter switching while scrolling.
        }
    }
}
