using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using ObjCRuntime;

namespace FCareer.iOS
{
	public class TokenButton : UIButton
	{

		public bool _toggled;
		public TokenField _tokenField;
		public object _representedObject;

		public TokenButton(UIButtonType buttonType) : base(buttonType) { }

		public static TokenButton CreateTokenButton(string title, object representedObject, TokenField parentField)
		{
			TokenButton tokenButton = new TokenButton(UIButtonType.Custom);
			tokenButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
			tokenButton.BackgroundColor = UIColor.White;
			tokenButton.SetTitle(title, UIControlState.Normal);
			tokenButton.SizeToFit();

			CGRect frame = tokenButton.Frame;
			frame.Width += 20;
			frame.Height = TokenField.DEFAULT_HEIGHT;
			tokenButton.Frame = frame;

			tokenButton.Layer.MasksToBounds = true;
			tokenButton.Layer.CornerRadius = frame.Height / 2;

			tokenButton._tokenField = parentField;
			tokenButton._representedObject = representedObject;

			return tokenButton;
		}

		public void SetToggled(bool toggled)
		{
			_toggled = toggled;
			if (_toggled)
			{
				BackgroundColor = UIColor.Blue;
				SetTitleColor(UIColor.White, UIControlState.Normal);
			}
			else
			{
				BackgroundColor = UIColor.White;
				SetTitleColor(UIColor.Blue, UIControlState.Normal);
			}
		}
	}
}
