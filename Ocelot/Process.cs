using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocelot
{
    public class Process
    {

        private readonly IDictionary<string, Stanza> flow;
        private readonly IList<Phrase> phrases;

        private void Parse(string path)
        {
            dynamic json = JObject.Parse(File.ReadAllText(path));

            foreach (var s in json["flow"])
            {             
                Stanza next = null;

                switch (s.Value["type"].Value)
                {
                    case "InstructionStanza":
                        next = new InstructionStanza
                        {
                            Text = s.Value["text"].Value
                        };
                        break;
                    case "ImportantStanza":
                        next = new InstructionStanza
                        {
                            Text = s.Value["text"].Value
                        };
                        break;
                    case "QuestionStanza":
                        var qs = new QuestionStanza
                        {
                            Text = s.Value["text"].Value,
                        };
                        qs.Answers = new long[s.Value["answers"].Count];
                        for (var i =0; i < s.Value["answers"].Count; i += 1)
                        {
                            qs.Answers[i] = s.Value["answers"][i];
                        }
                        next = qs; 

                        break;
                    case "EndStanza":
                        // ignored
                        continue;
                    default:
                        Console.WriteLine("Unknown stanza type");
                        continue;
                }
                if (next != null)
                {
                    next.ID = s.Name;
                    next.Next = new string[s.Value["next"].Count];
                    for (var i =0; i < s.Value["next"].Count; i += 1)
                    {
                        next.Next[i] = s.Value["next"][i];
                    }
                    flow[next.ID] = next;
                }
            }

            for (int i = 0; i < json["phrases"].Count; i += 1)
            {
                var p = json["phrases"][i];
                if (p.Type == JTokenType.String) 
                {
                    phrases.Add( new Phrase
                    {
                        Internal = p.Value
                    });
                }
                else
                {
                    phrases.Add(new Phrase
                    {
                        Internal = p[0].Value,
                        Exernal = p[1].Value
                    });
                } 
            }
        } 

        public Process(string Path)
        {
            flow = new Dictionary<string, Stanza>();
            phrases = new List<Phrase>();

            Parse(Path);
        }

        public Stanza GetStanza(string id)
        {
            return flow[id];
        }

        public Phrase GetPhrase(long id)
        {
            return phrases[(int)id];
        }

    }
}
