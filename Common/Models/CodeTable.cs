namespace Common.Models
{
    public class CodeTable
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public override string ToString() => $"{Code}: {Description}";
    }
}
