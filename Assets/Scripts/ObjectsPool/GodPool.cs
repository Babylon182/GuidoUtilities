﻿﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectsPool
{
	public class GodPool : MonoBehaviour
	{
		public static GodPool Instance => instance;
		private static GodPool instance;
		
		[SerializeField] private PoolAmount[] initialPoolObjects;
		private Dictionary<string, Stack<PoolObject>> inactivePoolObjects;
		private Dictionary<string, List<PoolObject>> activePoolObjects;
		private Dictionary<string, GameObject> containers;

		private const string CONTAINER = "Container";
		
		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				inactivePoolObjects = new Dictionary<string, Stack<PoolObject>>();
				activePoolObjects = new Dictionary<string, List<PoolObject>>();
				containers = new Dictionary<string, GameObject>();
				CreateInitialPoolObjects();
			}
			else
			{
				Destroy(this);
			}
		}

		public GameObject InstantiatePoolObject(GameObject poolObjectType, Vector3 initialPosition, Quaternion intialRotation)
		{
			if (!inactivePoolObjects.ContainsKey(poolObjectType.name))
			{
				string poolObjectName = poolObjectType.name;
				inactivePoolObjects.Add(poolObjectName, new Stack<PoolObject>());
				activePoolObjects.Add(poolObjectName, new List<PoolObject>());
				
				GameObject newContainer = new GameObject(poolObjectName + CONTAINER);
				newContainer.transform.parent = transform;
				containers.Add(poolObjectName, newContainer);
			}

			GameObject poolObject = GetObjectFromPool(poolObjectType).PoolGameObject;
			poolObject.transform.position = initialPosition;
			poolObject.transform.rotation = intialRotation;

			return poolObject;
		}

		public void ReturnPoolObject(GameObject poolObjectType)
		{
			if (activePoolObjects.ContainsKey(poolObjectType.name))
			{
				List<PoolObject> listOfActiveGameobjects = activePoolObjects[poolObjectType.name];
				
				for (int i = listOfActiveGameobjects.Count - 1; i >= 0; i--)
				{
					if (listOfActiveGameobjects[i].PoolGameObject.Equals(poolObjectType))
					{
						PoolObject poolObject = listOfActiveGameobjects[i];
						poolObject.IsActive = false;
						
						listOfActiveGameobjects.Remove(poolObject);
						inactivePoolObjects[poolObjectType.name].Push(poolObject);
					}
				}
			}
		}

		private PoolObject GetObjectFromPool(GameObject poolObjectType)
		{
			Stack<PoolObject> stackOfInactiveGameobjects = inactivePoolObjects[poolObjectType.name];

			if (stackOfInactiveGameobjects.Count > 0)
			{
				PoolObject poolObject = stackOfInactiveGameobjects.Pop();
				poolObject.IsActive = true;
				
				activePoolObjects[poolObjectType.name].Add(poolObject);	
				return poolObject;
			}
			else
			{
				GameObject newGameObject = Instantiate(poolObjectType);
				newGameObject.name = poolObjectType.name;
				newGameObject.transform.parent = containers[newGameObject.name].transform;
	
				PoolObject newPoolObject = CreateGenericPoolObject(newGameObject);
				newPoolObject.IsActive = true;
				activePoolObjects[poolObjectType.name].Add(newPoolObject);
				
				return newPoolObject;	
			}
		}
		
		private PoolObject CreateGenericPoolObject(GameObject poolObjectType)
		{
			IPoolable poolInterface = poolObjectType.GetComponent<IPoolable>();

			Action onInit = () => { }; 
			onInit += () => poolObjectType.SetActive(true);
			onInit += () => poolInterface?.Init();

			Action onDispose = () => { }; 
			onDispose += () => poolInterface?.Dispose(); 
			onDispose += () => poolObjectType.SetActive(false);

			return new PoolObject(poolObjectType, onInit, onDispose);;
		}

		private void CreateInitialPoolObjects()
		{
			List<string> createdPoolObjectsNames = new List<string>();
			for (int i = initialPoolObjects.Length - 1; i >= 0; i--)
			{
				PoolAmount poolObject = initialPoolObjects[i];
				createdPoolObjectsNames.Add(poolObject.gameObject.name);
				for (int j = poolObject.ammount; j > 0; j--)
				{
					InstantiatePoolObject(poolObject.gameObject, Vector3.zero, Quaternion.identity);
				}
			}

			for (int i = createdPoolObjectsNames.Count - 1; i >= 0; i--)
			{
				string poolObjectName = createdPoolObjectsNames[i];
				List<PoolObject> listOfActiveGameObjects = activePoolObjects[poolObjectName];
				
				for (int j = listOfActiveGameObjects.Count - 1; j >= 0; j--)
				{
					PoolObject poolObject = listOfActiveGameObjects[j];
					poolObject.IsActive = false;
						
					listOfActiveGameObjects.Remove(poolObject);
					inactivePoolObjects[poolObjectName].Push(poolObject);
				}	
			}
				
			initialPoolObjects = null;
		}
	}

	public class PoolObject
	{   
		private bool isActive;
		private Action initCallback;
		private Action disposeCallback;

		public PoolObject(GameObject poolGameObject, Action initCallback, Action disposeCallback)
		{
			this.PoolGameObject = poolGameObject;
			this.initCallback = initCallback;
			this.disposeCallback = disposeCallback;
		}

		public GameObject PoolGameObject { get;}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set 
			{ 
				isActive = value;

				if (isActive) 
					initCallback.Invoke();
				else 
					disposeCallback.Invoke();
			}
		}
	}

	[Serializable]
	public class PoolAmount
	{
		public GameObject gameObject;
		public int ammount;
	}

	public interface IPoolable
	{
		void Init();
		void Dispose();
	}
}