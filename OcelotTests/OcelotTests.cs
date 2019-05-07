using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ocelot;

namespace OcelotTests
{
    [TestClass]
    public class OcelotTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var process = Storage.LoadProcess();

            Stanza start = process.GetStanza("start");
            Assert.IsInstanceOfType(start, typeof(QuestionStanza));
            Assert.AreEqual("start", start.ID);
            Assert.AreEqual(2, start.Next.Length);

            Assert.AreEqual("Ask the customer if they have a tea bag", process.GetPhrase(0).Internal);
        }
    }
}
