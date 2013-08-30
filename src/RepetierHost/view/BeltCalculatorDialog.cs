using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class BeltCalculatorDialog : Form
    {
        public BeltCalculatorDialog()
        {
            InitializeComponent();
            MotorMicrosteppingInput.SelectedIndex = 4;
            MotorStepAngleInput.SelectedIndex = 0;
            BeltPitchInput.SelectedIndex = 0;
            labelMotorStepAngle.Text = Trans.T("L_MOTOR_STEP_ANGLE");
            labelStepsPerMM.Text = Trans.T("L_STEPS_PER_MM");
            labelToothCount.Text = Trans.T("L_TOOTH_COUNT");
            BeltCalculatorCalculateButton.Text = Trans.T("B_CALCULATE");
            labelDriverMicrostepping.Text = Trans.T("L_DRIVER_MICROSTEPPING");
            labelBeltPitch.Text = Trans.T("L_BELT_PITCH");
            this.Text = Trans.T("M_BELT_CALCULATOR");
        }

        private void BeltCalculator_Load(object sender, EventArgs e)
        {

        }

        private void BeltCalculatorCalculateButton_Click(object sender, EventArgs e)
        {

            var selectedStepAngle = MotorStepAngleInput.SelectedIndex;
            var selectedMicrostepValue = MotorMicrosteppingInput.SelectedIndex;
            var selectedBeltPitch = BeltPitchInput.SelectedIndex;
            
            int toothCount;

            if (!int.TryParse(ToothCountInput.Text, out toothCount) )
            {
                StepsPerMMOutput.Text ="Tooth Count must be int";
                return;
            }
            

            var stepAngles = new List<double>{1.8,0.9,7.5};
            var stepAngle = stepAngles[selectedStepAngle];

            var microStepValues = new List<double>() {1, 0.5, 0.25, 0.125, 0.0625,0.03125};
            var microStep = microStepValues[selectedMicrostepValue];

            var beltPitchValues = new List<double>() {2, 2.03, 2.5, 3, 5, 5.08};
            var beltPitch = beltPitchValues[selectedBeltPitch];

            var beltCalculator = new BeltCalculator(stepAngle, microStep, beltPitch, toothCount);


            var result = beltCalculator.Calculate();
                

            StepsPerMMOutput.Text = result.ToString(CultureInfo.InvariantCulture);
        }
    }
}
