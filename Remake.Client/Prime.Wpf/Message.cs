namespace Prime.Wpf
{
    public enum Severity
    {
        Success,
        Secondary,
        Info,
        Warning,
        Danger
    }

    public class Message
    {
        public Severity Severity { get; set; }

        /// <summary>
        /// Message's title
        /// </summary>
        public string? Summary { get; set; }

        /// <summary>
        /// Message's description
        /// </summary>
        public string? Detail { get; set; }

        /// <summary>
        /// Time to display message in milliseconds
        /// </summary>
        public int? Life { get; set; }

        public bool? Sticky { get; set; }
    }
}
