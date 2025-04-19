namespace MiBand3
{
    public class CustomComboBoxItem
    {
        public string Text { get; set; }
        public string Value { get; set; }

        public CustomComboBoxItem()
        {
        }

        public CustomComboBoxItem(string text, string value)
        {
            Text = text;
            Value = value;
        }
    }
}
