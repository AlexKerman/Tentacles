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
			this.numLengthSubdivide = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numRoundSubdivide = new System.Windows.Forms.NumericUpDown();
			this.cbLengthSmooth = new System.Windows.Forms.CheckBox();
			this.groupJointType = new System.Windows.Forms.GroupBox();
			this.rbPointSymmetric = new System.Windows.Forms.RadioButton();
			this.rbPointSmooth = new System.Windows.Forms.RadioButton();
			this.rbPointCusp = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numLengthSubdivide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numRoundSubdivide)).BeginInit();
			this.groupJointType.SuspendLayout();
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
			// groupJointType
			// 
			this.groupJointType.Controls.Add(this.rbPointSymmetric);
			this.groupJointType.Controls.Add(this.rbPointSmooth);
			this.groupJointType.Controls.Add(this.rbPointCusp);
			this.groupJointType.Enabled = false;
			this.groupJointType.Location = new System.Drawing.Point(3, 111);
			this.groupJointType.Name = "groupJointType";
			this.groupJointType.Size = new System.Drawing.Size(154, 51);
			this.groupJointType.TabIndex = 4;
			this.groupJointType.TabStop = false;
			this.groupJointType.Text = "Joint type";
			// 
			// rbPointSymmetric
			// 
			this.rbPointSymmetric.Appearance = System.Windows.Forms.Appearance.Button;
			this.rbPointSymmetric.AutoSize = true;
			this.rbPointSymmetric.Location = new System.Drawing.Point(107, 20);
			this.rbPointSymmetric.Name = "rbPointSymmetric";
			this.rbPointSymmetric.Size = new System.Drawing.Size(37, 23);
			this.rbPointSymmetric.TabIndex = 0;
			this.rbPointSymmetric.TabStop = true;
			this.rbPointSymmetric.Text = "Sym";
			this.rbPointSymmetric.UseVisualStyleBackColor = true;
			// 
			// rbPointSmooth
			// 
			this.rbPointSmooth.Appearance = System.Windows.Forms.Appearance.Button;
			this.rbPointSmooth.AutoSize = true;
			this.rbPointSmooth.Location = new System.Drawing.Point(49, 20);
			this.rbPointSmooth.Name = "rbPointSmooth";
			this.rbPointSmooth.Size = new System.Drawing.Size(53, 23);
			this.rbPointSmooth.TabIndex = 0;
			this.rbPointSmooth.TabStop = true;
			this.rbPointSmooth.Text = "Smooth";
			this.rbPointSmooth.UseVisualStyleBackColor = true;
			// 
			// rbPointCusp
			// 
			this.rbPointCusp.Appearance = System.Windows.Forms.Appearance.Button;
			this.rbPointCusp.AutoSize = true;
			this.rbPointCusp.Location = new System.Drawing.Point(6, 20);
			this.rbPointCusp.Name = "rbPointCusp";
			this.rbPointCusp.Size = new System.Drawing.Size(41, 23);
			this.rbPointCusp.TabIndex = 0;
			this.rbPointCusp.TabStop = true;
			this.rbPointCusp.Text = "Cusp";
			this.rbPointCusp.UseVisualStyleBackColor = true;
			// 
			// Spline4dControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupJointType);
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
			this.groupJointType.ResumeLayout(false);
			this.groupJointType.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupJointType;
		private System.Windows.Forms.RadioButton rbPointSymmetric;
		private System.Windows.Forms.RadioButton rbPointSmooth;
		private System.Windows.Forms.RadioButton rbPointCusp;

	}
}
