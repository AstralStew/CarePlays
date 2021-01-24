using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// </summary>
public class ClickHandler2 : MonoBehaviour, IPointerClickHandler
{
	public enum EnumExample
	{
		FIRST,
		SECOND,
		THIRD
	}

	[System.Serializable]
	public class PointerEvents2 : UnityEvent2<PointerEventData> { }

	/// <summary>
	/// 
	/// </summary>
	[SerializeField]
	[Tooltip("Regular events (This tooltip will appear on Inspector)")]
	private UnityEvent2 unityEvents2 = null;

	/// <summary>
	/// 
	/// </summary>
	[SerializeField]
	[Tooltip("PointerEventData dynamic events (This tooltip will appear on Inspector)")]
	private PointerEvents2 pointerEvents2 = null;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerClick(PointerEventData eventData)
	{
		unityEvents2.Invoke();
		pointerEvents2.Invoke(eventData);
	}

	public void Test()
	{
		Debug.Log("Test method. No params");
	}

	public void Test(string s1, string s2)
	{
		Debug.LogFormat("Test method. string param1:{0} - string param2:{1}", s1, s2);
	}

	public void Test(int i1, int i2)
	{
		Debug.LogFormat("Test method. int param1:{0} - int param2:{1}", i1, i2);
	}

	public void Test(GameObject go1, GameObject go2)
	{
		Debug.LogFormat("Test method. GameObject param1: {0} - GameObject param2: {1}", go1, go2);
	}

	public void Test(int i, string s)
	{
		Debug.LogFormat("Test method. int param1:{0} - string param2:{1}", i, s);
	}

	public void Test(string i, int s)
	{
		Debug.LogFormat("Test method. string param1:{0} - int param2:{1}", i, s);
	}

	public void Test(string s1, string s2, string s3)
	{
		Debug.LogFormat("Test method. string param1:{0} - string param2:{1} - string param3:{2}", s1, s2, s3);
	}

	public void Test(int i1, int i2, int i3)
	{
		Debug.LogFormat("Test method. int param1:{0} - int param2:{1} - int param3:{2}", i1, i2, i3);
	}

	public void Test(EnumExample enumExample1, EnumExample enumExample2, EnumExample enumExample3)
	{
		Debug.LogFormat("Test method. enum param1:{0} - enum param2:{1} - enum param3:{2}", enumExample1, enumExample2, enumExample3);
	}

	public void Test(int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Test method. int: {0} - string: {1} - GameObject: {2} - Enum: {3}", i, s, go, enumExample);
	}

	public void TestLayer([Layer] int layer)
	{
		Debug.LogFormat("TestLayer method. Layer {0}: {1}", layer, LayerMask.LayerToName(layer));
	}

	[Layer("Ignore Raycast")]
	public void TestLayer(int layer1, [Layer(0)] int layer2)
	{
		Debug.LogFormat("TestLayer method. Layer {0}: {1} - Layer {2}: {3}", layer1, LayerMask.LayerToName(layer1), layer2, LayerMask.LayerToName(layer2));
	}

	public void TestColor(Color color)
	{
		Debug.LogFormat("TestColor method. color param:{0}", color);
	}

	public void TestLayerMask(LayerMask layerMask)
	{
		string layers = string.Empty;
		for (int i = 0; i < 32; i++)
		{
			if ((layerMask.value & 1 << i) > 0 && !string.IsNullOrEmpty(LayerMask.LayerToName(i)))
				layers += LayerMask.LayerToName(i) + ", ";
		}
		Debug.LogFormat("TestLayerMask method. Selected layers from layerMask:{0}", layers);
	}

	public void Test(Vector2 v)
	{
		Debug.LogFormat("Test method. Vector2 param:{0}", v);
	}

	public void Test(Vector3 v)
	{
		Debug.LogFormat("Test method. Vector3 param:{0}", v);
	}

	public void Test(Vector4 v)
	{
		Debug.LogFormat("Test method. Vector4 param:{0}", v);
	}

	public void Test(PointerEventData eventData)
	{
		Debug.LogFormat("Test method. Dynamic PointerEventData param:{0}", eventData);
	}

	public void Test(PointerEventData eventData, string s)
	{
		Debug.LogFormat("Test method. string param:{1} - Dynamic PointerEventData param:{0}", eventData, s);
	}

	public void Test(PointerEventData eventData, int i)
	{
		Debug.LogFormat("Test method. int param:{1} - Dynamic PointerEventData param:{0}", eventData, i);
	}

	// THE METHODS BELOW ARE NOT AVAILABLE TO CHOOSE
	public void Test(ArrayList list)
	{
		Debug.LogFormat("Test method. list param:{0}", list);
	}

	public void Test(List<Object> list)
	{
		Debug.LogFormat("Test method. list param:{0}", list);
	}

}