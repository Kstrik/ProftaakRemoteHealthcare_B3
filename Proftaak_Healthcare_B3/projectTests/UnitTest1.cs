using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using Networking.Server;

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
            string encrypted = Cryptography.Encrypt(data, "testpass");
            string decrypted = Cryptography.Decrypt(encrypted, "testpass");

            Assert.AreEqual(data, decrypted);

        }
    }
}
