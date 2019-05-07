namespace Ocelot
{
    public abstract class Stanza
    {
        public abstract string StanzaType { get; }
        public string ID { get; set; }
        public string[] Next { get; set; }
        public bool HasNext { get { return Next != null && Next.Length > 0; } }
    }

    public class InstructionStanza : Stanza
    {
        public override string StanzaType { get { return "Instruction"; } }
        public long Text { get; set; }

    }

    public class ImportantStanza : InstructionStanza {

        public override string StanzaType { get { return "Important"; } }
    }

    public class QuestionStanza : InstructionStanza
    {
        public override string StanzaType { get { return "Question"; } }
        public long[] Answers { get; set; }
    }
}