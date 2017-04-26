﻿using NJsonApi.Infrastructure;
using NJsonApi.Serialization.Documents;
using NJsonApi.Serialization.Representations;
using NJsonApi.Serialization.Representations.Resources;
using NJsonApi.Test.Builders;
using NJsonApi.Test.TestControllers;
using System;
using System.Collections.Generic;
using Xunit;

namespace NJsonApi.Test.Serialization.JsonApiTransformerTest
{
    public class TestTopLevelDocument
    {
        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_resourceName()
        {
            // Arrange
            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            Assert.NotNull(result.Data);
            var transformedObject = result.Data as SingleResource;
            Assert.NotNull(transformedObject);
        }

        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_id()
        {
            // Arrange
            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Id, objectToTransform.Value.Id.ToString());
        }

        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_properties()
        {
            // Arrange
            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Attributes["someValue"], objectToTransform.Value.SomeValue);
            Assert.Equal(transformedObject.Attributes["date"], objectToTransform.Value.DateTime);
            Assert.Equal(transformedObject.Attributes.Count, 2);
        }

        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_type()
        {
            // Arrange
            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            var transformedObject = result.Data as SingleResource;
            Assert.Equal(transformedObject.Type, "sampleClasses");
        }

        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_metadata()
        {
            // Arrange
            const string pagingValue = "1";
            const string countValue = "2";

            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            objectToTransform.GetMetaData().Add("Paging", pagingValue);
            objectToTransform.GetMetaData().Add("Count", countValue);
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            var transformedObjectMetadata = result.Meta;
            Assert.Equal(transformedObjectMetadata["Paging"], pagingValue);
            Assert.Equal(transformedObjectMetadata["Count"], countValue);
        }

        [Fact]
        public void Creates_CompondDocument_for_TopLevelDocument_single_not_nested_class_and_propertly_map_links()
        {
            // Arrange
            SimpleLink linkSome = new SimpleLink(new Uri("http://somehost/"));
            SimpleLink linkOther = new SimpleLink(new Uri("http://otherhost/"));

            var context = CreateContext();
            TopLevelDocument<SampleClass> objectToTransform = CreateObjectToTransform();
            objectToTransform.Links.Add("linkSome", linkSome);
            objectToTransform.Links.Add("linkOther", linkOther);
            var transfomer = new JsonApiTransformerBuilder()
                .With(CreateConfiguration())
                .Build();

            // Act
            CompoundDocument result = transfomer.Transform(objectToTransform, context);

            // Assert
            var transformedObjectLinks = result.Links;
            Assert.Same(linkSome, transformedObjectLinks["linkSome"]);
            Assert.Same(linkOther, transformedObjectLinks["linkOther"]);
        }

        private static TopLevelDocument<SampleClass> CreateObjectToTransform()
        {
            var objectToTransform = new SampleClass
            {
                Id = 1,
                SomeValue = "Somevalue text test string",
                DateTime = DateTime.UtcNow,
                NotMappedValue = "Should be not mapped"
            };
            return new TopLevelDocument<SampleClass>(objectToTransform);
        }

        private Context CreateContext()
        {
            return new Context(new Uri("http://fakehost:1234/", UriKind.Absolute));
        }

        private IConfiguration CreateConfiguration()
        {
            var mapping = new ResourceMapping<SampleClass, DummyController>(c => c.Id);
            mapping.ResourceType = "sampleClasses";
            mapping.AddPropertyGetter("someValue", c => c.SomeValue);
            mapping.AddPropertyGetter("date", c => c.DateTime);

            var config = new NJsonApi.Configuration();
            config.AddMapping(mapping);
            return config;
        }

        private class SampleClass
        {
            public int Id { get; set; }
            public string SomeValue { get; set; }
            public DateTime DateTime { get; set; }
            public string NotMappedValue { get; set; }
        }
    }
}