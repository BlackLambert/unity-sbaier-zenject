using Zenject;
using NUnit.Framework;
using UnityEngine;
using System;

namespace SBaier.Tests
{
	[TestFixture]
	public class PrefabFactoryTest : ZenjectUnitTestFixture
	{
		private const string _testObjectPrefabPath = "Prefabs/Tests/PrefabFactoryTestObject";
		private const string _simpleTestObjectPrefabPath = "Prefabs/Tests/SimpleTestObject";
		private const int _injectedNumber = 23;

		[SetUp]
		public void Install()
		{
			Container.Bind<PrefabFactory>().AsTransient();
			_injectable = new TestInjectable();
			_boundInjectable = new TestInjectable();
			Container.Bind<TestInjectable>().FromInstance(_boundInjectable).AsSingle();
			Container.Bind<PrefabFactoryTestObject>().FromResource(_testObjectPrefabPath).AsTransient();
			Container.Bind<GameObject>().FromResource(_simpleTestObjectPrefabPath).AsTransient();

			Container.Inject(this);
		}

		private TestInjectable _injectable;
		private TestInjectable _boundInjectable;
		[Inject]
		private PrefabFactoryTestObject _testObjectPrefab = null;
		[Inject]
		private GameObject _simpleTestObjectPrefab = null;
		[Inject]
		private PrefabFactory _prefabFactory = null;

		[Test]
		public void Create_WorksForSimplePrefab()
		{
			GameObject result = _prefabFactory.Create(_simpleTestObjectPrefab);
			Assert.IsNotNull(result);
		}

		[Test]
		public void Create_InjectsParameters()
		{
			Assert.IsNull(_testObjectPrefab.Injectable);
			PrefabFactory.Parameter[] parameters =
			{
				new PrefabFactory.Parameter(_injectable, typeof(TestInjectable)),
				new PrefabFactory.Parameter(_injectedNumber, typeof(int))
			};
			PrefabFactoryTestObject result = _prefabFactory.Create(_testObjectPrefab, parameters);
			Assert.IsNotNull(result);
			Assert.AreEqual(_injectedNumber, result.Number);
			Assert.AreEqual(_injectable, result.Injectable);
		}

		[Test]
		public void Create_InjectionFailsOnMissingParameter()
		{
			PrefabFactory.Parameter[] parameters =
			{
				new PrefabFactory.Parameter(_injectable, typeof(TestInjectable))
			};
			Assert.Throws<ZenjectException>(() => _prefabFactory.Create(_testObjectPrefab, parameters));
		}

		[Test]
		public void Create_InjectsContainerParameter()
		{
			Assert.IsNull(_testObjectPrefab.Injectable);
			PrefabFactory.Parameter[] parameters =
			{
				new PrefabFactory.Parameter(_injectedNumber, typeof(int))
			};
			PrefabFactoryTestObject result = _prefabFactory.Create(_testObjectPrefab, parameters, Container);
			Assert.AreEqual(_injectedNumber, result.Number);
			Assert.AreEqual(_boundInjectable, result.Injectable);
		}

		[Test]
		public void Create_FailsOnNullPrefab()
		{
			Assert.Throws<ArgumentNullException>(() => _prefabFactory.Create<GameObject>(null));
		}

		[Test]
		public void Create_FailsOnWrongArgumentType()
		{
			PrefabFactory.Parameter[] parameters =
			{
				new PrefabFactory.Parameter(_injectable, typeof(TestInjectable)),
				new PrefabFactory.Parameter(_injectedNumber, typeof(string))
			};
			Assert.Throws<ArgumentException>(() => _prefabFactory.Create(_testObjectPrefab, parameters));
		}

		[Test]
		public void Create_FailsOnParametersNull()
		{
			Assert.Throws<ArgumentNullException>(() => _prefabFactory.Create(_testObjectPrefab, null, null));
		}

		[Test]
		public void Create_WorksOnEptyParameters()
		{
			GameObject result = _prefabFactory.Create(_simpleTestObjectPrefab, new PrefabFactory.Parameter[0]);
			Assert.IsNotNull(result);
		}
	}
}