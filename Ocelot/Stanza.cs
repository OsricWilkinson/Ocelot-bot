namespace Ocelot
{
    public class Stanza
    {

        public string ID { get; set; }
        public string[] Next { get; set; }

    }

    public class InstructionStanza : Stanza
    {
        public long Text { get; set; }

    }

    public class ImportantStanza : InstructionStanza { }

    public class QuestionStanza : InstructionStanza
    {
        public long[] Answers { get; set; }
    }
}