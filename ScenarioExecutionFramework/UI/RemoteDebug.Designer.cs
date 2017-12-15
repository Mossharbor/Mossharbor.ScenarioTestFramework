namespace ScenarioExecutionFramework
{
	partial class RemoteDebug
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.SetAsDefaultButton = new System.Windows.Forms.Button();
			this.AutoContinueCheck = new System.Windows.Forms.CheckBox();
			this.HelpToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.RunRadio = new System.Windows.Forms.RadioButton();
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.BreakRadio = new System.Windows.Forms.RadioButton();
			this.Label1 = new System.Windows.Forms.Label();
			this.ContinueButton = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.GroupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// SetAsDefaultButton
			// 
			this.SetAsDefaultButton.Location = new System.Drawing.Point(4, 169);
			this.SetAsDefaultButton.Name = "SetAsDefaultButton";
			this.SetAsDefaultButton.Size = new System.Drawing.Size(88, 23);
			this.SetAsDefaultButton.TabIndex = 9;
			this.SetAsDefaultButton.Text = "Set as Default";
			this.SetAsDefaultButton.Click += new System.EventHandler(this.SetAsDefaultButton_Click);
			// 
			// AutoContinueCheck
			// 
			this.AutoContinueCheck.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.AutoContinueCheck.Location = new System.Drawing.Point(12, 129);
			this.AutoContinueCheck.Name = "AutoContinueCheck";
			this.AutoContinueCheck.Size = new System.Drawing.Size(304, 24);
			this.AutoContinueCheck.TabIndex = 8;
			this.AutoContinueCheck.Text = "Auto-detect debugger connection";
			this.AutoContinueCheck.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// RunRadio
			// 
			this.RunRadio.Location = new System.Drawing.Point(32, 48);
			this.RunRadio.Name = "RunRadio";
			this.RunRadio.Size = new System.Drawing.Size(272, 16);
			this.RunRadio.TabIndex = 1;
			this.RunRadio.Text = "Run";
			this.RunRadio.CheckedChanged += new System.EventHandler(this.RunRadio_CheckedChanged);
			// 
			// GroupBox1
			// 
			this.GroupBox1.Controls.Add(this.RunRadio);
			this.GroupBox1.Controls.Add(this.BreakRadio);
			this.GroupBox1.Location = new System.Drawing.Point(4, 33);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(312, 80);
			this.GroupBox1.TabIndex = 7;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "When the debugger connects: ";
			// 
			// BreakRadio
			// 
			this.BreakRadio.Location = new System.Drawing.Point(32, 24);
			this.BreakRadio.Name = "BreakRadio";
			this.BreakRadio.Size = new System.Drawing.Size(272, 16);
			this.BreakRadio.TabIndex = 0;
			this.BreakRadio.Text = "Break";
			this.BreakRadio.CheckedChanged += new System.EventHandler(this.BreakRadio_CheckedChanged);
			// 
			// Label1
			// 
			this.Label1.Location = new System.Drawing.Point(12, 9);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(304, 24);
			this.Label1.TabIndex = 4;
			this.Label1.Text = "Waiting for remote debugger connection...";
			// 
			// ContinueButton
			// 
			this.ContinueButton.Location = new System.Drawing.Point(156, 169);
			this.ContinueButton.Name = "ContinueButton";
			this.ContinueButton.Size = new System.Drawing.Size(75, 23);
			this.ContinueButton.TabIndex = 6;
			this.ContinueButton.Text = "OK";
			this.ContinueButton.Click += new System.EventHandler(this.ContinueButton_Click);
			// 
			// Cancel_Button
			// 
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(244, 169);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(72, 23);
			this.Cancel_Button.TabIndex = 5;
			this.Cancel_Button.Text = "&Cancel";
			this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
			// 
			// RemoteDebug
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(326, 200);
			this.Controls.Add(this.AutoContinueCheck);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.ContinueButton);
			this.Controls.Add(this.Cancel_Button);
			this.Controls.Add(this.SetAsDefaultButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RemoteDebug";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Remote Debug";
			this.TopMost = true;
			this.GroupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button SetAsDefaultButton;
		private System.Windows.Forms.CheckBox AutoContinueCheck;
		private System.Windows.Forms.ToolTip HelpToolTip;
		private System.Windows.Forms.RadioButton RunRadio;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.RadioButton BreakRadio;
		private System.Windows.Forms.Label Label1;
		private System.Windows.Forms.Button ContinueButton;
		private System.Windows.Forms.Button Cancel_Button;
	}
}