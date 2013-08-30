using System;

namespace RepetierHost.model
{
    public class BeltCalculator
    {
        public BeltCalculator(double stepAngle, double microStep, double beltPitch, int toothCount)
        {
            StepAngle = stepAngle;
            MicroStep = microStep;
            BeltPitch = beltPitch;
            ToothCount = toothCount;
        }

        private double StepAngle { get;  set; }
        private double MicroStep { get; set; }
        private double BeltPitch { get; set; }
        private int ToothCount { get; set; }

        public double Calculate()
        {
            if (StepAngle == 0 || MicroStep == 0 || BeltPitch == 0 || ToothCount == 0)
                throw new InvalidOperationException();

            var result = ((360 / StepAngle) * (1 / MicroStep)) / (BeltPitch * ToothCount);
            return Math.Round(result, 4);
        }
    }

    public class LeadScrewCalculator
    {
        public LeadScrewCalculator(double stepAngle, double microStep, double leadScrewPitch, double gearRatio)
            {
                StepAngle = stepAngle;
                MicroStep = microStep;
                LeadScrewPitch = leadScrewPitch;
                GearRatio = gearRatio;
            }

            private double StepAngle { get;  set; }
            private double MicroStep { get; set; }
            private double LeadScrewPitch { get; set; }
            private double GearRatio { get; set; }

            public double Calculate()
            {
                if (StepAngle == 0 || MicroStep == 0 || LeadScrewPitch == 0 || GearRatio == 0)
                    throw new InvalidOperationException();

                var result = (((360 / StepAngle) * (1 / MicroStep)) / LeadScrewPitch)/ GearRatio;
                return Math.Round(result, 4);
            }
    }

}