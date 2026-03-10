using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

namespace UIManager
{
	public abstract class GenericPopupBase : UIScreen<GenericPopupBase>
	{
		[SerializeField]
		private TextMeshProUGUI titleLabel;
		[SerializeField]
		private TextMeshProUGUI descriptionLabel;
		
		[SerializeField]
		private Button[] buttons;

		private ButtonData[] buttonData;

		protected void Init(string title, string description, params ButtonData[] buttonData)
		{
			this.buttonData = buttonData;
			if (titleLabel != null) titleLabel.text = title;
			if (descriptionLabel != null) descriptionLabel.text = description;
			if (buttonData.Length != buttons.Length)
			{
				Debug.LogError($"Provided {buttonData.Length} button data elements, for {buttons.Length} buttons in {this.name}!");
			}

			for (int i = 0; i < buttons.Length; i++)
			{
				var textLabel = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
				string text = string.Empty;
				if (i < buttonData.Length)
				{
					text = buttonData[i].buttonText;
				}
				textLabel.text = text;
				//Create temporary variable as i is modified in the loop
				int t = i;
				buttons[i].onClick.RemoveAllListeners();
				buttons[i].onClick.AddListener(() => OnButtonClick(t));
			}
		}

		public void OnButtonClick(int i)
		{
			if (i < buttonData.Length)
			{
				if (buttonData[i].autoPopOnClick)
				{
					canvasManager.Hide<GenericPopupBase>();
				}
				
				buttonData[i].buttonCallback?.Invoke();
			}
		}
	}
}

public class ButtonData
{
	public ButtonData(string buttonText = "Ok", Action buttonCallback = null, bool autoPopOnClick = true)
	{
		this.autoPopOnClick = autoPopOnClick;
		this.buttonText = buttonText;
		this.buttonCallback = buttonCallback;
	}
	
	public bool autoPopOnClick;
	public string buttonText;
	public Action buttonCallback;
}