using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;

namespace FCareer.iOS
{
	public partial interface TokenFieldDelegate
	{
		void DidAddToken(TokenField tokenField, string title, object representedObject);
		void DidRemoveToken(TokenField tokenField, string title, object representedObject);
		bool TokenFieldShouldReturn(TokenField tokenField);
	}

	public class TokenField : UIView, TokenTextFieldDelegate
	{
		public TokenTextField _textField;
		public List<TokenButton> _tokens;
		public TokenButton _deletedToken;
		public bool _canNotDeletedToken;
		public const float HEIGHT_PADDING = 3;
		public const float WIDTH_PADDING = 3;
		public const float DEFAULT_HEIGHT = 31;

		public TokenFieldDelegate Delegate { get; set; }

		public TokenField(CGRect frame) : base(frame)
		{
			CommonSetup();
		}

		void CommonSetup()
		{
			_tokens = new List<TokenButton>();

			if (Frame.Height < (DEFAULT_HEIGHT + HEIGHT_PADDING * 2))
			{
				CGRect newFrame = Frame;
				newFrame.Height = DEFAULT_HEIGHT + HEIGHT_PADDING * 2;
				Frame = newFrame;
			}

			CGRect frame = Frame;
			frame.Y += HEIGHT_PADDING;
			frame.Height -= HEIGHT_PADDING * 2;

			_textField = new TokenTextField(frame);
			_textField.TokenDelegate = this;
			_textField.BorderStyle = UITextBorderStyle.None;
			_textField.BackgroundColor = UIColor.Clear;
			_textField.VerticalAlignment = UIControlContentVerticalAlignment.Center;

			AddSubview(_textField);

			_textField.ShouldReturn += TextFieldShouldReturn;

			_textField.ShouldBeginEditing = (UITextField textField) =>
			{
				_canNotDeletedToken = false;
				return true;
			};
			_textField.ShouldChangeCharacters = (textField, range, replacementString) =>
			{
				_canNotDeletedToken = true;
				return true;
			};
		}



		bool TextFieldShouldReturn(UITextField textField)
		{
			if (textField == _textField)
			{
				return Delegate.TokenFieldShouldReturn(this);
			}
			return false;
		}

		bool TextFieldEditingDidEnd(UITextField textField, UITextFieldDidEndEditingReason reason, string replacementString)
		{
			if (textField == _textField)
			{
				Delegate.TokenFieldShouldReturn(this);
			}
			return false;
		}

		public void AddTokenWithTitle(string title, object representedObject)
		{
			string aTitle = title.Trim();
			if (aTitle.Length > 0)
			{
				TokenButton tokenButton = TokenWithString(aTitle, representedObject);
				_tokens.Add(tokenButton);
				Delegate.DidAddToken(this, aTitle, representedObject);
				SetNeedsLayout();
			}
		}

		public void RemoveTokenForTitle(TokenButton tokenButtonToRemove)
		{
			if (tokenButtonToRemove != null)
			{
				if (tokenButtonToRemove.IsFirstResponder)
				{
					_textField.BecomeFirstResponder();
				}
				tokenButtonToRemove.RemoveFromSuperview();
				_tokens.Remove(tokenButtonToRemove);

				string tokenName = tokenButtonToRemove.Title(UIControlState.Normal);
				// Delegate
				Delegate.DidRemoveToken(this, tokenName, tokenButtonToRemove._representedObject);
			}

		}
		bool CheckTokenButtonNeedRemove(TokenButton tokenButton, string title)
		{
			bool removeTokenButton = false;
			if ((tokenButton.Title(UIControlState.Normal) == title) && (tokenButton._toggled == true))
			{
				removeTokenButton = true;
			}
			return removeTokenButton;
		}


		TokenButton TokenWithString(string title, object representedObject)
		{
			TokenButton tokenButton = TokenButton.CreateTokenButton(title, representedObject, this);
			CGRect frame = tokenButton.Frame;
			if (frame.Width > Frame.Width)
			{
				frame.Width = Frame.Width - (WIDTH_PADDING * 2);
			}
			tokenButton.Frame = frame;
			tokenButton.TouchUpInside += Toggle;

			return tokenButton;
		}

		void Toggle(object sender, EventArgs ea)
		{
			foreach (TokenButton aTokenButton in _tokens)
			{
				aTokenButton.SetToggled(false);
			}

			TokenButton tokenButton = (TokenButton)sender;
			tokenButton.SetToggled(true);
			_textField.BecomeFirstResponder();
		}

		public override void LayoutSubviews()
		{
			CGRect currentRect = new CGRect();
			List<TokenButton> lastLineTokens = new List<TokenButton>();

			foreach (TokenButton tokenButton in _tokens)
			{
				CGRect frameToken = tokenButton.Frame;
				if (currentRect.X + frameToken.Width > Frame.Width)
				{
					lastLineTokens.Clear();
					currentRect.Location = new CGPoint(WIDTH_PADDING, currentRect.Y + frameToken.Height + HEIGHT_PADDING);
				}
				frameToken.X = currentRect.X;
				frameToken.Y = currentRect.Y + HEIGHT_PADDING;

				tokenButton.Frame = frameToken;

				if (tokenButton.Superview == null)
				{
					AddSubview(tokenButton);
				}
				lastLineTokens.Add(tokenButton);

				currentRect.X += frameToken.Width + WIDTH_PADDING;
				currentRect.Size = frameToken.Size;
			}

			CGRect textFieldFrame = _textField.Frame;

			textFieldFrame.Location = currentRect.Location;

			if ((Frame.Width - textFieldFrame.X) >= 60)
			{
				textFieldFrame.Width = Frame.Width - textFieldFrame.X;
			}
			else
			{
				lastLineTokens.Clear();

				textFieldFrame.Width = Frame.Width - WIDTH_PADDING * 2;
				textFieldFrame.Location = new CGPoint(WIDTH_PADDING * 2, currentRect.Y + currentRect.Height + HEIGHT_PADDING);

			}

			textFieldFrame.Y += HEIGHT_PADDING;
			_textField.Frame = textFieldFrame;

			CGRect thisFrame = Frame;
			thisFrame.Height = textFieldFrame.Y + textFieldFrame.Height + HEIGHT_PADDING;

			nfloat textFieldMidY = textFieldFrame.GetMidY();

			foreach (TokenButton tokenButton in lastLineTokens)
			{
				CGPoint tokenCenter = tokenButton.Center;
				tokenCenter.Y = textFieldMidY;
				tokenButton.Center = tokenCenter;
			}

			Frame = thisFrame;
		}

		public void TokenTextFieldDeleteBackward(TokenTextField tokenTextField)
		{
			if (_canNotDeletedToken)
			{
				_canNotDeletedToken = false;
			}
			else
			{
				bool removeTokenButton = false;
				foreach (TokenButton tokenButton in _tokens)
				{
					if (tokenButton._toggled)
					{
						RemoveTokenForTitle(tokenButton);
						removeTokenButton = true;
						break;
					}
				}

				if (!removeTokenButton)
				{
					// Highlight last token button
					if (_tokens.Count > 0)
					{
						TokenButton tokenButtonHighlight = _tokens[_tokens.Count - 1];
						if (tokenButtonHighlight != null)
						{
							tokenButtonHighlight.SetToggled(true);
						}
					}
				}
			}
		}
	}
}
