using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using Networking;
using Networking.HealthCare;
using System.Text;
using System.Diagnostics;

namespace projectTests
{
    [TestClass]
    public class UnitTest1
    {

        //test for encryptor
        [TestMethod]
        public void TestMethod1()
        {
            string data = "This is a test message";
            string encrypted = DataEncryptor.Encrypt(data, "testpass");
            string decrypted = DataEncryptor.Decrypt(encrypted, "testpass");

            Assert.AreEqual(data, decrypted);

        }


        [TestMethod]
        public void TestMessageParser()
        {
            byte[] array = new byte[1] {0x00};

            Message message = new Message(false, Message.MessageType.CHAT_MESSAGE, array);
            byte[] array1 = message.GetBytes();
            byte[] array2 = Message.ParseMessage(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00 }).GetBytes();
            Console.WriteLine(array1.ToString() + "   "  + array2.ToString());
            for (int i = 0; i < array1.Length; i++)
            {
                Assert.AreEqual(array1[i], array2[i]);
                Trace.WriteLine(array1[i] + "   " + array2[i]);

            }

        }
    }
}
