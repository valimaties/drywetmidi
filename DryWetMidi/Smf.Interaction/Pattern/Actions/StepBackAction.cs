﻿using System;

namespace Melanchall.DryWetMidi.Smf.Interaction
{
    internal sealed class StepBackAction : StepAction
    {
        #region Constructor

        public StepBackAction(ILength step)
            : base(step)
        {
        }

        #endregion

        #region IPatternAction

        public override PatternActionResult Invoke(long time, PatternContext context)
        {
            var tempoMap = context.TempoMap;

            context.SaveTime(time);

            var convertedTime = TimeConverter.ConvertFrom(((MidiTime)time).Subtract(Step), tempoMap);
            return new PatternActionResult(Math.Max(convertedTime, 0));
        }

        #endregion
    }
}
