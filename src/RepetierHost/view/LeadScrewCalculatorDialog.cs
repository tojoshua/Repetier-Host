using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RepetierHost.model;

namespace RepetierHost.view
{
    public partial class LeadScrewCalculatorDialog : Form
    {
        public LeadScrewCalculatorDialog()
        {
            InitializeComponent();
            LSMicroSteps.SelectedIndex = 4;
            LSMotorAngle.SelectedIndex = 0;
            LSScrewPitch.SelectedIndex = 0;
            this.Text = Trans.T("M_LEADSCREW_CALCULATOR");
            labelGearRatio.Text = Trans.T("L_GEAR_RATIO");
            labelLeadscrewPitch.Text = Trans.T("L_LEADSCREW_PITCH");
            labelMicrostepping.Text = Trans.T("L_DRIVER_MICROSTEPPING");
            labelMotorStepAngle.Text = Trans.T("L_MOTOR_STEP_ANGLE");
            labelStepsPerMM.Text = Trans.T("L_STEPS_PER_MM");
            buttonCalculate.Text = Trans.T("B_CALCULATE");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var selectedStepAngle = LSMotorAngle.SelectedIndex;
            var selectedMicrostepValue = LSMicroSteps.SelectedIndex;
            var selectedScrewPitch = LSScrewPitch.SelectedIndex;

            int GearRatio0Val;

            if (!int.TryParse(GearRatio0.Text, out GearRatio0Val))
            {
                LSOutput.Text = Trans.T("L_TOOTH_COUNT_MUST_INT");
                return;
            }

            int GearRatio1Val;
            if (!int.TryParse(GearRatio1.Text, out GearRatio1Val))
            {
                LSOutput.Text = Trans.T("L_TOOTH_COUNT_MUST_INT");
                return;
            }

            double GearRatio = ((double)GearRatio0Val/GearRatio1Val);

            var stepAngles = new List<double>{1.8,0.9,7.5};
            var stepAngle = stepAngles[selectedStepAngle];

            var microStepValues = new List<double>() {1, 0.5, 0.25, 0.125, 0.0625,0.03125};
            var microStep = microStepValues[selectedMicrostepValue];

            var screwPitchValues = new List<double>() { 1.25, 1,1.5,1.75, 1.41111, 1.5875,1.5,2.0,3.0,4.0 };
            var screwPitch = screwPitchValues[selectedScrewPitch];

            var beltCalculator = new LeadScrewCalculator(stepAngle, microStep, screwPitch, GearRatio);


            var result = beltCalculator.Calculate();
                

            LSOutput.Text = result.ToString(CultureInfo.InvariantCulture);
        
        }
    }
}
