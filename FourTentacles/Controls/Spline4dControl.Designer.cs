﻿namespace FourTentacles
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
			this.numLengthSubdivide = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numRoundSubdivide = new System.Windows.Forms.NumericUpDown();
			this.cbLengthSmooth = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numLengthSubdivide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numRoundSubdivide)).BeginInit();
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
			this.cbEditPoints.Click += new System.EventHandler(this.EditPointsClick);
			// 
			// cbEditSegments
			// 
			this.cbEditSegments.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbEditSegments.AutoSize = true;
			this.cbEditSegments.Location = new System.Drawing.Point(74, 3);
			this.cbEditSegments.Name = "cbEditSegments";
			this.cbEditSegments.Size = new System.Drawing.Size(83, 23);
			this.cbEditSegments.TabIndex = 0;
			this.cbEditSegments.Text = "Edit segments";
			this.cbEditSegments.UseVisualStyleBackColor = true;
			this.cbEditSegments.Click += new System.EventHandler(this.EditSegmentsClick);
			// 
			// numLengthSubdivide
			// 
			this.numLengthSubdivide.Location = new System.Drawing.Point(100, 32);
			this.numLengthSubdivide.Name = "numLengthSubdivide";
			this.numLengthSubdivide.Size = new System.Drawing.Size(57, 20);
			this.numLengthSubdivide.TabIndex = 1;
			this.numLengthSubdivide.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numLengthSubdivide.ValueChanged += new System.EventHandler(this.LengthSubdivideValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Length subdivide:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(3, 61);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Round subdivide:";
			// 
			// numRoundSubdivide
			// 
			this.numRoundSubdivide.Location = new System.Drawing.Point(100, 58);
			this.numRoundSubdivide.Name = "numRoundSubdivide";
			this.numRoundSubdivide.Size = new System.Drawing.Size(57, 20);
			this.numRoundSubdivide.TabIndex = 1;
			this.numRoundSubdivide.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numRoundSubdivide.ValueChanged += new System.EventHandler(this.RoundSubdivideValueChanged);
			// 
			// cbLengthSmooth
			// 
			this.cbLengthSmooth.AutoSize = true;
			this.cbLengthSmooth.Location = new System.Drawing.Point(3, 88);
			this.cbLengthSmooth.Name = "cbLengthSmooth";
			this.cbLengthSmooth.Size = new System.Drawing.Size(96, 17);
			this.cbLengthSmooth.TabIndex = 3;
			this.cbLengthSmooth.Text = "Length smooth";
			this.cbLengthSmooth.UseVisualStyleBackColor = true;
			this.cbLengthSmooth.Click += new System.EventHandler(this.cbLengthSmooth_Click);
			// 
			// Spline4dControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.cbLengthSmooth);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numRoundSubdivide);
			this.Controls.Add(this.numLengthSubdivide);
			this.Controls.Add(this.cbEditSegments);
			this.Controls.Add(this.cbEditPoints);
			this.Name = "Spline4dControl";
			this.Size = new System.Drawing.Size(162, 309);
			((System.ComponentModel.ISupportInitialize)(this.numLengthSubdivide)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numRoundSubdivide)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox cbEditPoints;
		private System.Windows.Forms.CheckBox cbEditSegments;
		private System.Windows.Forms.NumericUpDown numLengthSubdivide;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numRoundSubdivide;
		private System.Windows.Forms.CheckBox cbLengthSmooth;

	}
}
