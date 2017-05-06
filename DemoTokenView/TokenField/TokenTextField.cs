using System;
using UIKit;
using CoreGraphics;

namespace FCareer.iOS
{
	public partial interface TokenTextFieldDelegate
	{
		void TokenTextFieldDeleteBackward(TokenTextField tokenTextField);
	}

	public class TokenTextField : UITextField
	{
		public TokenTextFieldDelegate TokenDelegate { get; set; }
		public TokenTextField(CGRect frame) : base(frame)
		{
		}

		public override void DeleteBackward()
		{
			base.DeleteBackward();
			if (Text == "")
			{
				Console.WriteLine("Call TokenTextFieldDelegate");
				TokenDelegate.TokenTextFieldDeleteBackward(this);
			}
		}
	}
}
