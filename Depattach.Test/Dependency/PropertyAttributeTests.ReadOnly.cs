﻿using System;
using Depattach.Fody;
using FluentAssertions;
using Mono.Cecil;
using NUnit.Framework;

namespace Depattach.Test.Dependency
{
	[TestFixture]
	public class PropertyAttributeTestsReadOnly
	{
		[Test]
		public void ValidateAttributeOnReadOnlyPropertyThrowsExecption()
		{
			ModuleDefinition module = ModuleDefinition.ReadModule("AssemblyDependencyProperty.ReadOnlyProperty.dll");

			ModuleWeaver weavingTask = new ModuleWeaver
			{
				ModuleDefinition = module
			};

			Action action = () => weavingTask.Execute();
			action.ShouldThrow<InvalidOperationException>();
		}
	}
}