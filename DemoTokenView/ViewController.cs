using System;
using FCareer.iOS;
using UIKit;
using CoreGraphics;

namespace DemoTokenView
{
	public partial class ViewController : UIViewController, TokenFieldDelegate
	{
		public TokenField _tokenField;
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			 _tokenField = new TokenField(new CGRect(10, 100, View.Frame.Width - 20, 30));
			_tokenField.Delegate = this;

			_tokenField.BackgroundColor = UIColor.Red;

			View.AddSubview(_tokenField);

			TokenFieldShouldReturn(_tokenField);

		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}


		public void DidAddToken(TokenField tokenField, string title, object representedObject)
		{

		}

		public void DidRemoveToken(TokenField tokenField, string title, object representedObject)
		{

		}

		public bool TokenFieldShouldReturn(TokenField tokenField)
		{
			string rawString = tokenField._textField.Text;
			rawString = rawString.Trim();

			if (rawString.Length > 0)
			{
				_tokenField.AddTokenWithTitle(rawString, rawString);

				_tokenField._textField.Text = "";
				_tokenField._canNotDeletedToken = false;
			}
			return false;
		}
	}
}
