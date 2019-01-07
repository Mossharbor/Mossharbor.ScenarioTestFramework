namespace Mossharbor.ScenarioTestFramework
{
#if NET40 || NET45 || NET46 || NET47
    using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using System.Diagnostics;
	using System.Drawing;
	using System.Text;
	using System.Windows.Forms;
	using Microsoft.Win32;

	internal partial class RemoteDebug : Form
	{
#region *** Private Fields *******************************************************

		private DebugAction daFinalAction;
		private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

		private class RegStrings
		{
			public static string RootKey = "Software\\Microsoft\\Windows CE Tools Test\\Remote Debugging";
			public static string BreakAlwaysValue = "BreakAlways";
			public static string AutoContinueValue = "AutoContinue";
		}

#endregion


#region *** Constructor **********************************************************

		public RemoteDebug()
		{
			InitializeComponent();

            bool breakAlways,autoContinue;

            try
            {
                breakAlways = GetBooleanValueFromKey(Registry.CurrentUser, RegStrings.RootKey, RegStrings.BreakAlwaysValue);
                autoContinue = GetBooleanValueFromKey(Registry.CurrentUser, RegStrings.RootKey, RegStrings.AutoContinueValue);
            }
            catch (NullReferenceException)
            {
                SetAsDefaultButton.Enabled = true;
                autoContinue = false;
                breakAlways = true;
            }

			// Set the initial state of the dialog
			if (breakAlways)
				BreakRadio.Checked = true;
			else
				RunRadio.Checked = true;
			AutoContinueCheck.Checked = autoContinue;

			// Set up the timer with 500ms interval
			myTimer.Tick += new System.EventHandler(TimerEventProcessor);

			myTimer.Interval = 500;
			myTimer.Start();

			// Set up tool tips
			HelpToolTip.SetToolTip(Label1, "Press the Cancel button to terminate\r\nthis script instead.");
			HelpToolTip.SetToolTip(RunRadio, "Upon continuing, the script will run\r\ninstead of stopping at a DebugBreak.");
			HelpToolTip.SetToolTip(BreakRadio, "Causes a DebugBreak to be performed\r\nas soon as the script continues. A \r\ndebugger MUST be connected.");
			HelpToolTip.SetToolTip(ContinueButton, "Tells your script to run or break,\r\ndepending upon the selected debug\r\naction.");
			HelpToolTip.SetToolTip(Cancel_Button, "Click here to terminate the script.");
			HelpToolTip.SetToolTip(AutoContinueCheck, "If this option is checked, your script \r\nwill continue immediately and perform \r\nthe selected debug action.");
			HelpToolTip.SetToolTip(SetAsDefaultButton, "Saves the current debug action and\r\nauto-connect setting in your personal\r\npreferences.");
		}

#endregion


#region "*** Private Methods *****************************************************************"

        
		/// <summary>
		/// This funciton gets the int32 value from a registry key and returns it.
		/// </summary>
		/// <param name="rootKey">This is the registry key to get the value from</param>
		/// <param name="subKeyPath">The path to a sub key, which is appended to the root key</param>
		/// <param name="value">This is the string value under the key to get the int32 out of.</param>
		/// <returns>The int32 Value that is returned.</returns>
		/// <exception cref="NullReferenceException">
		/// Thrown if the value in the registry does not exist or contains no data.
		/// </exception>
        public bool GetBooleanValueFromKey(RegistryKey rootKey, string subKeyPath, string value)
        {
            RegistryKey key = rootKey.OpenSubKey(subKeyPath);

            object temp = key.GetValue(value);

            return ((int)temp != 0) ? true : false;
        }

#region "*** Timer Handler *******************************************************************"

		//***********************************
		// This is the method to run when the timer is raised.
		private void TimerEventProcessor(object myObject, EventArgs myEventArgs)
		{
			if (Debugger.IsAttached)
			{
				if (AutoContinueCheck.Checked)
				{
					DoClose();
				}
			}
		}

#endregion


#region "*** Debug Action State **************************************************************"

		//***********************************
		private void BreakRadio_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (BreakRadio.Checked == true)
			{
				daFinalAction = DebugAction.Break;
			}

			SetAsDefaultButton.Enabled = true;
		}

		private void RunRadio_CheckedChanged(System.Object sender, System.EventArgs e)
		{
			if (RunRadio.Checked == true)
			{
				daFinalAction = DebugAction.Run;
			}

			SetAsDefaultButton.Enabled = true;
		}

#endregion


#region "*** Button Handlers *****************************************************************"

		//***********************************
		private void Cancel_Button_Click(System.Object sender, System.EventArgs e)
		{
			DoClose(DebugAction.Terminate);
		}


		//***********************************
		private void ContinueButton_Click(System.Object sender, System.EventArgs e)
		{
			DoClose();
		}


		//***********************************
		private void SetAsDefaultButton_Click(System.Object sender, System.EventArgs e)
		{
            RegistryKey temp = Registry.CurrentUser.CreateSubKey(RegStrings.RootKey);
            temp.SetValue(RegStrings.BreakAlwaysValue, (object)BreakRadio.Checked, RegistryValueKind.DWord);
            temp.SetValue(RegStrings.AutoContinueValue, (object)AutoContinueCheck.Checked, RegistryValueKind.DWord);

			SetAsDefaultButton.Enabled = false;
		}

#endregion


#region "*** Utility Functions ***************************************************************"

		//***********************************
		private void DoClose(DebugAction daFinal)
		{
			daFinalAction = daFinal;
			DoClose();
		}

		private void DoClose()
		{
			myTimer.Stop();
			myTimer.Dispose();

			Close();
		}

#endregion

#endregion


#region "*** Public Properties ***************************************************************"

		public DebugAction FinalAction
		{
			get { return daFinalAction; }
		}

#endregion
	}


	public static class RemoteDebuggerDialog
	{
        public static DebugAction WaitForDebugger()
		{
			RemoteDebug dlg = new RemoteDebug();
			dlg.ShowDialog();

            return dlg.FinalAction;
		}
	}
#endif
}