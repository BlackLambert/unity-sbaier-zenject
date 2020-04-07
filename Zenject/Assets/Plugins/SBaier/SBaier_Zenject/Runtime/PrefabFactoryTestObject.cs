using UnityEngine;
using Zenject;

namespace SBaier.Tests
{
	public class PrefabFactoryTestObject : MonoBehaviour
	{
		public TestInjectable Injectable { get; private set; }
		public int Number { get; private set; }

		[Inject]
		private void Construct(TestInjectable injectable,
			int number)
		{
			Injectable = injectable;
			Number = number;
		}
	}
}