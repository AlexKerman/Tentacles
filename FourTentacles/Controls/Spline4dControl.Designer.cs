namespace FourTentacles
{
	partial class Spline4dControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.cbEditPoints = new System.Windows.Forms.CheckBox();
			this.cbEditSegments = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// cbEditPoints
			// 
			this.cbEditPoints.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbEditPoints.AutoSize = true;
			this.cbEditPoints.Location = new System.Drawing.Point(3, 3);
			this.cbEditPoints.Name = "cbEditPoints";
			this.cbEditPoints.Size = new System.Drawing.Size(66, 23);
			this.cbEditPoints.TabIndex = 0;
			this.cbEditPoints.Text = "Edit points";
			this.cbEditPoints.UseVisualStyleBackColor = true;
			this.cbEditPoints.Click += new System.EventHandler(this.cbEditPoints_Click);
			// 
			// cbEditSegments
			// 
			this.cbEditSegments.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbEditSegments.AutoSize = true;
			this.cbEditSegments.Location = new System.Drawing.Point(75, 3);
			this.cbEditSegments.Name = "cbEditSegments";
			this.cbEditSegments.Size = new System.Drawing.Size(83, 23);
			this.cbEditSegments.TabIndex = 0;
			this.cbEditSegments.Text = "Edit segments";
			this.cbEditSegments.UseVisualStyleBackColor = true;
			this.cbEditSegments.Click += new System.EventHandler(this.cbEditSegments_Click);
			// 
			// Spline4dControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbEditSegments);
			this.Controls.Add(this.cbEditPoints);
			this.Name = "Spline4dControl";
			this.Size = new System.Drawing.Size(162, 150);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbEditPoints;
		private System.Windows.Forms.CheckBox cbEditSegments;

	}
}
