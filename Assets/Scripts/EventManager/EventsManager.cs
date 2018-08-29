using System;
using System.Collections.Generic;

namespace Events
{
	public static class EventsManager<T>
	{
		private static Dictionary<Type , Action<T>> events = new Dictionary<Type, Action<T>>();

		public static void SubscribeToEvent(Action<T> listener) 
		{
			if (!events.ContainsKey(typeof(T)))
				events.Add(typeof(T), null);
	
			events[typeof(T)] += listener;
		}

		public static void UnsubscribeToEvent(Action<T> listener)
		{
			if (events.ContainsKey(typeof(T)))
				events[typeof(T)] -= listener;
		}

		public static void TriggerEvent(T dataEvent = default(T))
		{
			if (events.ContainsKey(typeof(T)) &&
			    events[typeof(T)] != null)
			{
				events[typeof(T)](dataEvent);
			}
		}

		public static void ClearDictionary()
		{
			events.Clear();
		}

		public static void ResetDictionary()
		{
			events = new Dictionary<Type, Action<T>>();
		}
	}
}