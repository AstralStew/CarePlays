using UnityEngine;
using UnityEngine.EventSystems;

public class UIHandler : MonoBehaviour
{
	public enum EnumExample
	{
		FIRST,
		SECOND,
		THIRD
	}

	public void ButtonClick()
	{
		Debug.Log("Button clicked. No params");
	}

	public void ButtonClick(string s1, string s2)
	{
		Debug.LogFormat("Button clicked {0} - {1}", s1, s2);
	}

	public void ButtonClick(int i1, int i2, int i3)
	{
		Debug.LogFormat("Button clicked {0} - {1} - {2}", i1, i2, i3);
	}

	public void ButtonClick(int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Button clicked {0} - {1} - {2} - {3}", i, s, go, enumExample);
	}

	public void ButtonClick(PointerEventData eventData)
	{
		Debug.LogFormat("Button clicked {0}", eventData);
	}

	public void ButtonClick(PointerEventData eventData, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Button clicked {0} - {1} - {2} - {3} - {4}", i, s, go, enumExample, eventData);
	}

	public void ToggleChanged(bool toggle)
	{
		Debug.LogFormat("Toggle changed {0}", toggle);
	}

	public void ToggleChanged(bool toggle, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Toggle changed {0} - {1} - {2} - {3} - {4}", toggle, i, s, go, enumExample);
	}

	public void SliderChanged(float slider)
	{
		Debug.LogFormat("Slider changed {0}", slider);
	}

	public void SliderChanged(float slider, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Slider changed {0} - {1} - {2} - {3} - {4}", slider, i, s, go, enumExample);
	}

	public void ScrollbarChanged(float scrollbar)
	{
		Debug.LogFormat("Scrollbar changed {0}", scrollbar);
	}

	public void ScrollbarChanged(float scrollbar, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Scrollbar changed {0} - {1} - {2} - {3} - {4}", scrollbar, i, s, go, enumExample);
	}

	public void DropdownChanged(int index)
	{
		Debug.LogFormat("Dropdown changed {0}", index);
	}

	public void DropdownChanged(int index, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4}", index, i, s, go, enumExample);
	}

	public void DropdownChanged(string label)
	{
		Debug.LogFormat("Dropdown changed {0}", label);
	}

	public void DropdownChanged(string label, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4}", label, i, s, go, enumExample);
	}

	public void DropdownChanged(Sprite sprite)
	{
		Debug.LogFormat("Dropdown changed {0}", sprite);
	}

	public void DropdownChanged(Sprite sprite, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4}", sprite, i, s, go, enumExample);
	}

	public void DropdownChanged(int index, string label)
	{
		Debug.LogFormat("Dropdown changed {0} - {1}", index, label);
	}

	public void DropdownChanged(int index, string label, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4} - {5}", index, label, i, s, go, enumExample);
	}

	public void DropdownChanged(int index, Sprite sprite)
	{
		Debug.LogFormat("Dropdown changed {0} - {1}", index, sprite);
	}

	public void DropdownChanged(int index, Sprite sprite, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4} - {5}", index, sprite, i, s, go, enumExample);
	}

	public void DropdownChanged(string label, Sprite sprite)
	{
		Debug.LogFormat("Dropdown changed {0} - {1}", label, sprite);
	}

	public void DropdownChanged(string label, Sprite sprite, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4} - {5}", label, sprite, i, s, go, enumExample);
	}

	public void DropdownChanged(int index, string label, Sprite sprite)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2}", index, label, sprite);
	}

	public void DropdownChanged(int index, string label, Sprite sprite, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("Dropdown changed {0} - {1} - {2} - {3} - {4} - {5} - {6}", index, label, sprite, i, s, go, enumExample);
	}

	public void InputFieldChanged(string value)
	{
		Debug.LogFormat("InputField changed {0}", value);
	}

	public void InputFieldChanged(string value, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("InputField changed {0} - {1} - {2} - {3} - {4}", value, i, s, go, enumExample);
	}

	public void InputFieldEndEdit(string value)
	{
		Debug.LogFormat("InputField end edit {0}", value);
	}

	public void InputFieldEndEdit(string value, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("InputField end edit {0} - {1} - {2} - {3} - {4}", value, i, s, go, enumExample);
	}

	public void ScrollViewChanged(Vector2 vector2)
	{
		Debug.LogFormat("ScrollView changed {0}", vector2);
	}

	public void ScrollViewChanged(Vector2 vector2, int i, string s, GameObject go, EnumExample enumExample)
	{
		Debug.LogFormat("ScrollView changed {0} - {1} - {2} - {3} - {4}", vector2, i, s, go, enumExample);
	}
}