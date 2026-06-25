using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using BigNumbersNet;

namespace BigNumbersNet.Gui
{
    public partial class Form1 : Form
    {
        private TextBox txtNumA = null!;
        private TextBox txtNumB = null!;
        private ComboBox cbOperator = null!;
        private Button btnCalculate = null!;
        private TextBox txtResult = null!;
        private Label lblStatus = null!;

        public Form1()
        {
            InitializeCustomLayout();
        }

        private void InitializeCustomLayout()
        {
            this.Text = "BigNumbersNet - Interactive Test Drive";
            this.Size = new Size(800, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Labels
            var lblNumA = new Label { Text = "Number A (Supports up to thousands of digits):", Location = new Point(20, 20), Size = new Size(400, 20) };
            var lblOperator = new Label { Text = "Operation:", Location = new Point(20, 160), Size = new Size(100, 20) };
            var lblNumB = new Label { Text = "Number B (Supports up to thousands of digits):", Location = new Point(20, 220), Size = new Size(400, 20) };
            var lblResult = new Label { Text = "Result Output:", Location = new Point(20, 380), Size = new Size(100, 20) };

            // Input Fields
            txtNumA = new TextBox { Location = new Point(20, 45), Size = new Size(740, 100), Multiline = true, ScrollBars = ScrollBars.Vertical, MaxLength=0 };
            txtNumA.Text = "98765432101234567890123456789098765432101234567890";

            cbOperator = new ComboBox { Location = new Point(20, 185), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cbOperator.Items.AddRange(new object[] {
                "+ (Add)",
                "- (Subtract)",
                "* (Multiply)",
                "/ (Decimal Divide)",
                "IntegerDivide",
                "% (Modulo)",
                "Greatest Common Divisor (GCD)",
                "Pow (A^exponent)"
            });
            cbOperator.SelectedIndex = 0;

            txtNumB = new TextBox { Location = new Point(20, 245), Size = new Size(740, 100), Multiline = true, ScrollBars = ScrollBars.Vertical, MaxLength = 0 };
            txtNumB.Text = "12345678901234567890123456789012345678901234567890";

            btnCalculate = new Button { Text = "Calculate", Location = new Point(240, 182), Size = new Size(120, 30), BackColor = Color.LightBlue };
            btnCalculate.Click += BtnCalculate_Click;

            lblStatus = new Label { Text = "Ready", Location = new Point(380, 187), Size = new Size(380, 20), ForeColor = Color.DarkGreen };

            txtResult = new TextBox { Location = new Point(20, 405), Size = new Size(740, 180), Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, BackColor = Color.WhiteSmoke, MaxLength = 0 };

            // Render Controls
            this.Controls.Add(lblNumA);
            this.Controls.Add(txtNumA);
            this.Controls.Add(lblOperator);
            this.Controls.Add(cbOperator);
            this.Controls.Add(btnCalculate);
            this.Controls.Add(lblStatus);
            this.Controls.Add(lblNumB);
            this.Controls.Add(txtNumB);
            this.Controls.Add(lblResult);
            this.Controls.Add(txtResult);
        }

        private void BtnCalculate_Click(object? sender, EventArgs e)
        {
            txtResult.Clear();
            lblStatus.Text = "Processing Calculation...";
            lblStatus.ForeColor = Color.DarkOrange;
            Application.DoEvents();

            try
            {
                if (!BigNumber.TryParse(txtNumA.Text, out var numA))
                {
                    MessageBox.Show("Failed to parse Number A.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!BigNumber.TryParse(txtNumB.Text, out var numB))
                {
                    MessageBox.Show("Failed to parse Number B.", "Format Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BigNumber result = BigNumber.Zero;
                var sw = Stopwatch.StartNew();

                switch (cbOperator.SelectedIndex)
                {
                    case 0:
                        result = numA + numB;
                        break;
                    case 1:
                        result = numA - numB;
                        break;
                    case 2:
                        result = numA * numB;
                        break;
                    case 3:
                        result = numA / numB;
                        break;
                    case 4:
                        result = BigNumber.IntegerDivide(numA, numB);
                        break;
                    case 5:
                        result = numA % numB;
                        break;
                    case 6:
                        result = BigNumber.GreatestCommonDivisor(numA, numB);
                        break;
                    case 7:
                        if (int.TryParse(txtNumB.Text, out int exp))
                        {
                            result = BigNumber.Pow(numA, exp);
                        }
                        else
                        {
                            throw new ArgumentException("For 'Pow', Number B must be a valid 32-bit integer exponent.");
                        }
                        break;
                }

                sw.Stop();
                txtResult.Text = result.ToString();
                lblStatus.Text = $"Completed in {sw.Elapsed.TotalMilliseconds:F2} ms";
                lblStatus.ForeColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error occurred";
                lblStatus.ForeColor = Color.Red;
                txtResult.Text = $"An error occurred during calculation:\r\n{ex.Message}\r\n\r\nStack Trace:\r\n{ex.StackTrace}";
            }
        }
    }
}