﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;
using NUnit.Framework;

namespace DocumentDB.Context.Tests
{
    public abstract class JsonSamplesTests<T> : TestBase<T>
    {
        protected override void PopulateTestData()
        {
            TestData.PopulateWithJsonSamples();
        }

        [SetUp]
        public override void SetUp()
        {
            TestService.Configuration = new DocumentDbConfiguration
            {
                MetadataBuildStrategy = new DocumentDbConfiguration.Metadata
                {
                    PrefetchRows = -1, 
                    UpdateDynamically = false
                }
            };
            base.SetUp();
        }

        [Test]
        public void ValidateMetadata()
        {
            base.RequestAndValidateMetadata();
        }

        [Test]
        public void SchemaTables()
        {
            var schema = base.GetSchema();
            var tableNames = schema.Tables.Select(x => x.ActualName).ToList();

            foreach (var tableName in TestData.JsonSamples)
            {
                Assert.Contains(tableName, tableNames);
            }
        }

        [Test]
        public void SchemaColumnNullability()
        {
            var schema = base.GetSchema();
            base.ValidateColumnNullability(schema);
        }

        [Test]
        public void SchemaColumnNames()
        {
            var schema = base.GetSchema();
            base.ValidatePropertyNames(schema);
        }

        [Test]
        public void Colors()
        {
            if (!TestData.JsonSamples.Contains("Colors"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.Colors.All().First();
            Assert.AreEqual(7, result.colorsArray.Count);
            Assert.AreEqual("red", result.colorsArray[0].colorName);
            Assert.AreEqual("#f00", result.colorsArray[0].hexValue);
            Assert.AreEqual("black", result.colorsArray[6].colorName);
            Assert.AreEqual("#000", result.colorsArray[6].hexValue);
        }

        [Test]
        public void Facebook()
        {
            if (!TestData.JsonSamples.Contains("Facebook"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.Facebook.All().First();
            Assert.AreEqual(2, result.data.Count);
            Assert.AreEqual("Tom Brady", result.data[0].from.name);
            Assert.AreEqual("X12", result.data[0].from.id);
            Assert.AreEqual(2, result.data[0].actions.Count);
            Assert.AreEqual("Comment", result.data[0].actions[0].name);
            Assert.AreEqual("Like", result.data[0].actions[1].name);
        }

        [Test]
        public void Flickr()
        {
            if (!TestData.JsonSamples.Contains("Flickr"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.Flickr.All().First();
            Assert.AreEqual("Talk On Travel Pool", result.title);
            Assert.AreEqual(1, result.items.Count);
            Assert.AreEqual("spain dolphins tenerife canaries lagomera aqualand playadelasamericas junglepark losgigantos loscristines talkontravel", result.items.First().tags);
        }

        [Test]
        public void GoogleMaps()
        {
            if (!TestData.JsonSamples.Contains("GoogleMaps"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.GoogleMaps.All().First();
            Assert.AreEqual(3, result.markers.Count);
            Assert.AreEqual(40.266044, result.markers.First().point.latitude);
            Assert.AreEqual(-74.718479, result.markers.First().point.longitude);
            Assert.AreEqual("Linux users group meets second Wednesday of each month.", result.markers.First().information);
            Assert.AreEqual("", result.markers.First().capacity);
        }

        [Test]
        public void iPhone()
        {
            if (!TestData.JsonSamples.Contains("iPhone"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.iPhone.All().First();
            Assert.AreEqual(18, result.menu.items.Count);
            Assert.AreEqual("Open", result.menu.items.First().id);
            Assert.Null(result.menu.items.First().label);
            Assert.AreEqual("About", result.menu.items.Last().id);
            Assert.AreEqual("About xProgress CVG Viewer...", result.menu.items.Last().label);
        }

        [Test]
        public void Twitter()
        {
            if (!TestData.JsonSamples.Contains("Twitter"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.Twitter.All().First();
            Assert.AreEqual("@twitterapi  http://tinyurl.com/ctrefg", result.results.text);
            Assert.AreEqual("popular", result.results.metadata.result_type);
            Assert.AreEqual(109, result.results.metadata.recent_retweets);
        }

        [Test]
        public void YouTube()
        {
            if (!TestData.JsonSamples.Contains("YouTube"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.YouTube.All().First();
            Assert.AreEqual(@"Talk On Travel Pool", result.title);
            Assert.AreEqual(1, result.items.Count);
            Assert.AreEqual("View from the hotel", result.items.First().title);
        }

        [Test]
        public void Nested()
        {
            if (!TestData.JsonSamples.Contains("Nested"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.Nested.All().First();
            Assert.AreEqual("0001", result.id);
            Assert.AreEqual(4, result.batters.batter.Count);
            Assert.AreEqual("1001", result.batters.batter.First().id);
            Assert.AreEqual(7, result.topping.Count);
            Assert.AreEqual("5001", result.topping.First().id);
        }

        [Test]
        public void ArrayOfNested()
        {
            if (!TestData.JsonSamples.Contains("ArrayOfNested"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.ArrayOfNested.All().First();
            Assert.AreEqual("0001", result.nestedArray.First().id);
            Assert.AreEqual(4, result.nestedArray.First().batters.batter.Count);
            Assert.AreEqual("1001", result.nestedArray.First().batters.batter.First().id);
            Assert.AreEqual(7, result.nestedArray.First().topping.Count);
            Assert.AreEqual("5001", result.nestedArray.First().topping.First().id);
        }

        [Test]
        public void ArrayInArray()
        {
            if (!TestData.JsonSamples.Contains("ArrayInArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.ArrayInArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(1, (result[0] as IDictionary<string, object>).Count(x => !x.Key.StartsWith("x_")));
        }

        [Test]
        public void EmptyArray()
        {
            if (!TestData.JsonSamples.Contains("EmptyArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.EmptyArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(7, result[0].topping.Count);
            Assert.AreEqual("0002", result[1].id);
            Assert.AreEqual(0, result[1].topping.Count);
        }

        [Test]
        public void NullArray_a()
        {
            if (!TestData.JsonSamples.Contains("NullArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.NullArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(2, result[0].arrays.a.Count);
            Assert.AreEqual(1, result[0].arrays.a[0].id);
            Assert.AreEqual(2, result[0].arrays.a[0].nonid);
            Assert.AreEqual("Regular", result[0].arrays.a[0].type);
            Assert.AreEqual(null, result[0].arrays.a[1].id);
            Assert.AreEqual("0002", result[1].id);
            Assert.AreEqual(1, result[1].arrays.a.Count);
            Assert.AreEqual(null, result[1].arrays.a[0].id);
        }

        [Test]
        public void NullArray_b()
        {
            if (!TestData.JsonSamples.Contains("NullArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.NullArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(2, result[0].arrays.b.Count);
            Assert.AreEqual(null, result[0].arrays.b[0].id);
            Assert.AreEqual(1, result[0].arrays.b[1].id);
            Assert.AreEqual(2, result[0].arrays.b[1].nonid);
            Assert.AreEqual("Regular", result[0].arrays.b[1].type);
            Assert.AreEqual("0002", result[1].id);
            Assert.AreEqual(2, result[1].arrays.b.Count);
            Assert.AreEqual(null, result[1].arrays.b[0].id);
            Assert.AreEqual(1, result[1].arrays.b[1].id);
            Assert.AreEqual(2, result[1].arrays.b[1].nonid);
            Assert.AreEqual("Regular", result[1].arrays.b[1].type);
        }

        [Test]
        public void NullArray_c()
        {
            if (!TestData.JsonSamples.Contains("NullArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.NullArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(1, result[0].arrays.c.Count);
            Assert.AreEqual(null, result[0].arrays.c[0].id);
            Assert.AreEqual("0002", result[1].id);
            Assert.AreEqual(2, result[1].arrays.c.Count);
            Assert.AreEqual(null, result[1].arrays.c[0].id);
            Assert.AreEqual(1, result[1].arrays.c[1].id);
            Assert.AreEqual(2, result[1].arrays.c[1].nonid);
            Assert.AreEqual("Regular", result[1].arrays.c[1].type);
        }

        [Test]
        public void NullArray_d()
        {
            if (!TestData.JsonSamples.Contains("NullArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.NullArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(0, result[0].arrays.d.Count);
            Assert.AreEqual("0002", result[1].id);
            Assert.AreEqual(1, result[1].arrays.d.Count);
            var a = result[1].arrays.d[0];
            Assert.AreEqual(1, result[1].arrays.d[0].x_id);
            Assert.AreEqual(2, result[1].arrays.d[0].nonid);
            Assert.AreEqual("Regular", result[1].arrays.d[0].type);
        }

        [Test]
        public void UnresolvedArray()
        {
            if (!TestData.JsonSamples.Contains("UnresolvedArray"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.UnresolvedArray.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(2, result[0].arrays.a.Count);
            Assert.AreEqual(0, result[0].arrays.b.Count);
            Assert.Throws<RuntimeBinderException>(() => { var c = result[0].arrays.c.Count; });
        }

        [Test]
        public void UnresolvedProperty()
        {
            if (!TestData.JsonSamples.Contains("UnresolvedProperty"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.UnresolvedProperty.All().ToList();
            Assert.AreEqual("0001", result[0].id);
            Assert.AreEqual(1, result[0].elements.a.id);
            Assert.AreEqual(null, result[0].elements.b.empty_content);
        }

        [Test]
        public void EmptyProperty()
        {
            if (!TestData.JsonSamples.Contains("EmptyProperty"))
                Assert.Ignore("JSON sample is not enabled");

            var result = ctx.EmptyProperty.All().ToList();
            Assert.AreEqual("0001", result[0].id);
        }
    }

    [TestFixture]
    public class InMemoryServiceJsonSamplesTests : JsonSamplesTests<ProductInMemoryService>
    {
    }

    [TestFixture]
    public class QueryableServiceJsonSamplesTests : JsonSamplesTests<ProductQueryableService>
    {
    }
}
