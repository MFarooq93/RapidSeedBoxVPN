using Microsoft.VisualStudio.TestTools.UnitTesting;
using PureVPN.Entity.Models;
using PureVPN.UnitTest;
using System;

namespace PureVPN.Entity.Tests
{
    [TestClass()]
    public class CountryModelTests : UnitTestBase
    {
        [TestMethod()]
        public void CountryModel_IsActive_AreEqual()
        {
            bool IsActive = true;
            CountryModel country = new CountryModel
            {
                IsActive = IsActive
            };
            Assert.AreEqual(IsActive, country.IsActive);
        }

        [TestMethod()]
        public void CountryModel_Id_AreEqual()
        {
            int Id = 6;
            CountryModel country = new CountryModel
            {
                Id = Id
            };
            Assert.AreEqual(Id, country.Id);
        }

        [TestMethod()]
        public void CountryModel_CountrySlug_AreEqual()
        {
            string CountrySlug = "countrySlug";
            CountryModel country = new CountryModel
            {
                CountrySlug = CountrySlug
            };
            Assert.AreEqual(CountrySlug, country.CountrySlug);
        }

        [TestMethod()]
        public void CountryModel_ISOCode_AreEqual()
        {
            string ISOCode = "ISOCode";
            CountryModel country = new CountryModel
            {
                ISOCode = ISOCode
            };
            Assert.AreEqual(ISOCode, country.ISOCode);
        }


        [TestMethod()]
        public void CountryModel_Name_AreEqual()
        {
            string Name = "Name";
            CountryModel country = new CountryModel
            {
                Name = Name
            };
            Assert.AreEqual(Name, country.Name);
        }

        [TestMethod()]
        public void CountryModel_Latency_AreEqual()
        {
            long Latency = 99;
            CountryModel country = new CountryModel
            {
                Latency = Latency
            };
            Assert.AreEqual(Latency, country.Latency);
        }

        [TestMethod()]
        public void CountryModel_DisplayName_AreEqual()
        {
            long Latency = 99;
            string Name = "Name";
            CountryModel country = new CountryModel
            {
                Name = Name,
                Latency = Latency
            };
            Assert.AreEqual(Name + " - (" + Latency + " ms) ", country.DisplayName);
        }

        [TestMethod()]
        public void CountryModel_Latitude_AreEqual()
        {
            double Latitude = 99;
            CountryModel country = new CountryModel
            {
                Latitude = Latitude
            };
            Assert.AreEqual(Latitude, country.Latitude);
        }

        [TestMethod()]
        public void CountryModel_Longitude_AreEqual()
        {
            double Longitude = 99;
            CountryModel country = new CountryModel
            {
                Longitude = Longitude
            };
            Assert.AreEqual(Longitude, country.Longitude);
        }

        [TestMethod()]
        public void CountryModel_IsFavorite_AreEqual()
        {
            bool IsFavorite = true;
            CountryModel country = new CountryModel
            {
                IsFavorite = IsFavorite
            };
            Assert.AreEqual(IsFavorite, country.IsFavorite);
        }

        [TestMethod()]
        public void CountryModel_FavImgSource_AreEqual()
        {
            string FavImgSource = "FavImgSource";
            CountryModel country = new CountryModel
            {
                FavImgSource = FavImgSource
            };
            Assert.AreEqual(FavImgSource, country.FavImgSource);
        }

        [TestMethod()]
        public void CountryModel_DateCreated_AreEqual()
        {
            DateTime DateCreated = DateTime.Now;
            CountryModel country = new CountryModel
            {
                DateCreated = DateCreated
            };
            Assert.AreEqual(DateCreated, country.DateCreated);
        }

        [TestMethod()]
        public void CountryModel_DisplayLatency_AreEqual()
        {
            long Latency = 99;
            CountryModel country = new CountryModel
            {
                Latency = Latency
            };
            Assert.AreEqual(Latency + " ms", country.DisplayLatency);
        }

        [TestMethod()]
        public void CountryModel_DisplayLatency_AreEqualZero()
        {
            long Latency = 0;
            CountryModel country = new CountryModel
            {
                Latency = Latency
            };
            Assert.AreEqual("-", country.DisplayLatency);
        }
    }
}