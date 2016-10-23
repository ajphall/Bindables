﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Depattach.Fody;
using Mono.Cecil;
using NUnit.Framework;

namespace Depattach.Test.Dependency
{
	[TestFixture]
	public class ClassAttributeTests
	{
		private ModuleDefinition _module;
		private MemoryStream _stream;

		[OneTimeSetUp]
		public void Setup()
		{
			_stream = new MemoryStream();
			_module = ModuleDefinition.ReadModule("AssemblyDependencyProperty.dll");

			ModuleWeaver weavingTask = new ModuleWeaver
			{
				ModuleDefinition = _module
			};

			weavingTask.Execute();

			_module.Write(_stream);
			_stream.Seek(0, SeekOrigin.Begin);
		}

		[Test]
		public void ValidateConversionToDependencyPropertyReferenceType()
		{
			Assembly assembly = Assembly.Load(_stream.ToArray());

			Type type = assembly.GetType(nameof(ClassAttribute));

			DependencyProperty referenceProperty = (DependencyProperty)type.GetField($"{nameof(ClassAttribute.Reference)}Property").GetValue(null);

			dynamic instance = Activator.CreateInstance(type);

			instance.Reference = "Test1";
			Assert.AreEqual("Test1", instance.Reference);
			Assert.AreEqual("Test1", instance.GetValue(referenceProperty));

			instance.SetValue(referenceProperty, "Test2");
			Assert.AreEqual("Test2", instance.Reference);
			Assert.AreEqual("Test2", instance.GetValue(referenceProperty));
		}

		[Test]
		public void ValidateConversionToDependencyPropertyValueType()
		{
			Assembly assembly = Assembly.Load(_stream.ToArray());

			Type type = assembly.GetType(nameof(ClassAttribute));

			DependencyProperty valueProperty = (DependencyProperty)type.GetField($"{nameof(ClassAttribute.Value)}Property").GetValue(null);

			dynamic instance = Activator.CreateInstance(type);

			instance.Value = 1;
			Assert.AreEqual(1, instance.Value);
			Assert.AreEqual(1, instance.GetValue(valueProperty));

			instance.SetValue(valueProperty, 2);
			Assert.AreEqual(2, instance.Value);
			Assert.AreEqual(2, instance.GetValue(valueProperty));
		}

		[Test]
		public void ValidateReadonlyPropertiesAreNotTouched()
		{
			Assembly assembly = Assembly.Load(_stream.ToArray());

			Type type = assembly.GetType(nameof(ClassAttribute));
			FieldInfo fieldInfo = type.GetField($"{nameof(ClassAttribute.Readonly)}Property");

			Assert.IsNull(fieldInfo);
		}

		[Test]
		public void ValidateNonAutoPropertiesAreNotTouched()
		{
			Assembly assembly = Assembly.Load(_stream.ToArray());

			Type type = assembly.GetType(nameof(ClassAttribute));
			FieldInfo fieldInfo = type.GetField($"{nameof(ClassAttribute.NonAuto)}Property");

			Assert.IsNull(fieldInfo);
		}

		[OneTimeTearDown]
		public void Cleanup()
		{
			_stream?.Dispose();
		}
	}
}