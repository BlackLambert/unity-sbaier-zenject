using Zenject;
using UnityEngine;
using System;

namespace SBaier
{
	/// <summary>
	/// Most default prefab factory.
	/// Use this factory for basic construction of GameObjects.
	/// A reference to the prefab is necessary.
	/// </summary>
	public class PrefabFactory
	{
		[Inject]
		private DiContainer _container = null;

		/// <summary>
		/// Creates instance of <paramref name="prefab"/>.
		/// Dependencies will be injected.
		/// </summary>
		/// <typeparam name="TPrefab">A <see cref="Component"/></typeparam>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <returns>An instance of <paramref name="prefab"/></returns>
		public TPrefab Create<TPrefab>(TPrefab prefab, DiContainer container = null)
			where TPrefab : UnityEngine.Object
		{
			return createInstance(prefab, new Parameter[0], container == null ? _container : container);
		}


		public TPrefab Create<TPrefab>(TPrefab prefab, Parameter[] objects, DiContainer container = null)
			where TPrefab : UnityEngine.Object
		{
			return createInstance(prefab, objects, container == null ? _container : container);
		}

		private TResult createInstance<TResult>(TResult prefab, Parameter[] parameters,
			DiContainer container)
			where TResult : UnityEngine.Object
		{
			validateInput(prefab, parameters, container);
			DiContainer subContainer = container.CreateSubContainer();
			foreach (Parameter parameter in parameters)
				subContainer.Bind(parameter.Type).To(parameter.Type).FromInstance(parameter.Target).AsSingle();
			if(prefab is UnityEngine.Component)
				return subContainer.InstantiatePrefabForComponent<TResult>(prefab);
			return subContainer.InstantiatePrefab(prefab) as TResult;
		}

		private void validateInput<TResult>(TResult prefab, Parameter[] parameters,
			DiContainer container)
		{
			if (prefab == null)
				throw new ArgumentNullException($"The {nameof(prefab)} parameter mustn't be null");
			if (parameters == null)
				throw new ArgumentNullException($"The {nameof(parameters)} parameter mustn't be null");
			foreach(Parameter parameter in parameters)
			{
				if (!parameter.Target.GetType().IsSubclassOf(parameter.Type) &&
					parameter.Target.GetType() != parameter.Type)
					throw new ArgumentException($"The {nameof(parameter.Type)} ({parameter.Type}) of parameter " +
						$"does not match it's {nameof(parameter.Target)} (Type {parameter.Target.GetType()}).");
			}
		}


		public class Parameter
		{
			public object Target { get; private set; }
			public Type Type { get; private set; }

			public Parameter(object target, Type type)
			{
				Target = target;
				Type = type;
			}

			public Parameter(object target)
			{
				Target = target;
				Type = Target.GetType();
			}
		}
	}
}