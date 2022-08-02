using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Service;
using PureVPN.App;
using System;
using PureVPN.UnitTest;

namespace PureVPN.APP.Tests
{
    [TestClass]
    public class FAQsURLTest : UnitTestBase
    {
        private string _FAQ_URLs { get; set; }

        public FAQsURLTest()
        {
            _FAQ_URLs = App.Helper.Constants.UrlSupportTabFAQs;
        }

        [TestMethod]
        public void Get_URL_When_Language_Is_English()
        {
            Service.Helper.Common.AppLanguage = App.Common.CultureInfoCodes.EnglishUS;
            var url = Common.GenerateFaqUrlAndAddParams(_FAQ_URLs);

            Assert.IsTrue(url.Contains("https://purevpn-website-assets.s3.amazonaws.com/app-faqs/windows-faqs/FAQs.html"));
        }

        [TestMethod]
        public void Get_URL_When_Language_Is_French()
        {
            Service.Helper.Common.AppLanguage = App.Common.CultureInfoCodes.FrenchFR;
            var url = Common.GenerateFaqUrlAndAddParams(_FAQ_URLs);

            Assert.IsTrue(url.Contains("https://purevpn-website-assets.s3.amazonaws.com/app-faqs/windows-faqs/FAQs-fr.html"));
        }

        [TestMethod]
        public void Get_URL_When_Language_Is_Germany()
        {
            Service.Helper.Common.AppLanguage = App.Common.CultureInfoCodes.GermanDE;
            var url = Common.GenerateFaqUrlAndAddParams(_FAQ_URLs);

            Assert.IsTrue(url.Contains("https://purevpn-website-assets.s3.amazonaws.com/app-faqs/windows-faqs/FAQs-de.html"));
        }
    }
}
