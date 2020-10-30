using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 
/// </summary>
public class ClickHandler : MonoBehaviour, IPointerClickHandler
{
	[System.Serializable]
	public class PointerEvents : UnityEvent<PointerEventData> { }

	/// <summary>
	/// 
	/// </summary>
	[SerializeField]
	private UnityEvent unityEvents = null;

	/// <summary>
	/// 
	/// </summary>
	[SerializeField]
	private PointerEvents pointerEvents = null;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="eventData"></param>
	public void OnPointerClick(PointerEventData eventData)
	{
		unityEvents.Invoke();
		pointerEvents.Invoke(eventData);
	}

	public void Test()
	{
		Debug.Log("Test method. No params");
	}

	public void Test(int i1)
	{
		Debug.LogFormat("Test method. int param:{0}", i1);
	}

	public void Test(float i1)
	{
		Debug.LogFormat("Test method. int param:{0}", i1);
	}

	public void Test(string s1)
	{
		Debug.LogFormat("Test method. string param:{0}", s1);
	}

	public void Test(GameObject go)
	{
		Debug.LogFormat("Test method. GameObject param:{0}", go);
	}

	public void Test(PointerEventData eventData)
	{
		Debug.LogFormat("Test method. PointerEventData param: {0}", eventData);
	}

	// THE METHODS BELOW ARE NOT AVAILABLE TO CHOOSE
	public void Test(string s1, string s2)
	{
		Debug.LogFormat("Test method. string param1:{0} - string param2:{1}", s1, s2);
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
}