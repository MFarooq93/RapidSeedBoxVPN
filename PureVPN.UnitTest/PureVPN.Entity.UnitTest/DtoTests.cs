using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.UnitTest;
using PureVPN.Entity.Models.DTO;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class DtoTests : UnitTestBase
    {
        [TestMethod()]
        public void DtoBaseNetwork_Code_AreEqual()
        {
            BaseNetwork baseNetwork = new BaseNetwork();
            int code = 99;
            baseNetwork.header = new Header
            {
                response_code = code
            };
            Assert.AreEqual(code, baseNetwork.header.response_code);
        }

        [TestMethod()]
        public void DtoBaseNetwork_Message_AreEqual()
        {
            BaseNetwork baseNetwork = new BaseNetwork();
            string message = "message";
            baseNetwork.header = new Header
            {
                message = message
            };
            Assert.AreEqual(message, baseNetwork.header.message);
        }

        [TestMethod()]
        public void DtoBaseNetwork_Version_AreEqual()
        {
            BaseNetwork baseNetwork = new BaseNetwork();
            string version = "v1";
            baseNetwork.header = new Header
            {
                version = version
            };
            Assert.AreEqual(version, baseNetwork.header.version);
        }

        [TestMethod()]
        public void DtoLoginReply_LoginReplyBody_uuid_AreEqual()
        {
            LoginReply loginReply = new LoginReply();
            string uuid = "1";
            loginReply.body = new LoginReplyBody
            {
                uuid = uuid
            };
            Assert.AreEqual(uuid, loginReply.body.uuid);
        }

        [TestMethod()]
        public void DtoLoginReply_LoginReplyBody_client_id_AreEqual()
        {
            LoginReply loginReply = new LoginReply();
            string client_id = "1";
            loginReply.body = new LoginReplyBody
            {
                client_id = client_id
            };
            Assert.AreEqual(client_id, loginReply.body.client_id);
        }

        [TestMethod()]
        public void DtoLoginReply_LoginReplyBody_token_AreEqual()
        {
            LoginReply loginReply = new LoginReply();
            string token = "3";
            loginReply.body = new LoginReplyBody
            {
                token = token
            };
            Assert.AreEqual(token, loginReply.body.token);
        }

    }
}