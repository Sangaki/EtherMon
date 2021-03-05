namespace EtherMon.Models
{
    public struct BoolIsSoloScheme
    {
        public string TrueOption { get; set; }
        public string FalseOption { get; set; }

        public BoolIsSoloScheme(string trueOption, string falseOption)
        {
            TrueOption = trueOption;
            FalseOption = falseOption;
        }

        public string GetIsSolo(bool? value)
        {
            if (value == null) return TrueOption;
            return value.Value ? TrueOption : FalseOption;
        }
    }
}