using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
using Networking;

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
    }
}
