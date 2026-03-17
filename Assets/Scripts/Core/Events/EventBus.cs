using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public interface IEvent { }

public static class EventBus<T> where T : IEvent
{
	static readonly HashSet<IEventBinding<T>> bindings = new HashSet<IEventBinding<T>>();

	public static void Register(EventBinding<T> binding) => bindings.Add(binding);
	public static void Deregister(EventBinding<T> binding) => bindings.Remove(binding);

	public static void Raise(T @event)
	{
		var snapshot = new HashSet<IEventBinding<T>>(bindings);

		foreach (var binding in snapshot)
		{
			if (bindings.Contains(binding))
			{
				binding.OnEvent.Invoke(@event);
				binding.OnEventNoArgs.Invoke();
			}
		}
	}

	static void Clear()
	{
#if EVENT_BUS_DEBUG
		Debug.Log($"Clearing {typeof(T).Name} bindings");
#endif
		bindings.Clear();
	}
}

public static class EventBusUtil
{
	public static IReadOnlyList<Type> EventTypes { get; set; }
	public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
	public static PlayModeStateChange PlayModeState { get; set; }

	[InitializeOnLoadMethod]
	public static void InitializeEditor()
	{
		EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	static void OnPlayModeStateChanged(PlayModeStateChange state)
	{
		PlayModeState = state;
		if (state == PlayModeStateChange.ExitingPlayMode)
		{
			ClearAllBuses();
		}
	}
#endif

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Initialize()
	{
		EventTypes = PredefinedAssemblyUtil.GetTypes(typeof(IEvent));
		EventBusTypes = InitializeAllBuses();
	}

	static List<Type> InitializeAllBuses()
	{
		List<Type> eventBusTypes = new List<Type>();

		var typedef = typeof(EventBus<>);
		foreach (var eventType in EventTypes)
		{
			var busType = typedef.MakeGenericType(eventType);
			eventBusTypes.Add(busType);
		}

		return eventBusTypes;
	}

	public static void ClearAllBuses()
	{
		for (int i = 0; i < EventBusTypes.Count; ++i)
		{
			var busType = EventBusTypes[i];
			var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
			clearMethod?.Invoke(null, null);
		}
	}
}

public static class PredefinedAssemblyUtil
{
	enum AssemblyType
	{
		AssemblyCSharp,
		AssemblyCSharpEditor,
		AssemblyCSharpEditorFirstPass,
		AssemblyCSharpFirstPass
	}

	static AssemblyType? GetAssemblyType(string assemblyName) => assemblyName switch
	{
		"Assembly-CSharp" => AssemblyType.AssemblyCSharp,
		"Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
		"Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
		"Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
		_ => null
	};

	static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
	{
		if (assemblyTypes == null) 
			return;

		for (int i = 0; i < assemblyTypes.Length; ++i)
		{
			Type type = assemblyTypes[i];
			if (type != interfaceType && interfaceType.IsAssignableFrom(type))
				results.Add(type);
		}
	}

	public static List<Type> GetTypes(Type interfaceType)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

		Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
		List<Type> types = new List<Type>();
		for (int i = 0; i < assemblies.Length; ++i)
		{
			AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);
			if (assemblyType != null)
				assemblyTypes.Add((AssemblyType)assemblyType, assemblies[i].GetTypes());
		}

		assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
		AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types);

		assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
		AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types);

		return types;
	}
}